/*
 * Ascii Demon Attack
 * Copyright (C) 2008 Michael Birken
 * 
 * This file is part of Ascii Demon Attack.
 *
 * Ascii Demon Attack is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published 
 * by the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 *
 * Ascii Demon Attack is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using Mischel.ConsoleDotNet;

namespace AsciiWars {
  partial class GameEngine {
    public ConsoleInputEventInfo[] inputEvents = new ConsoleInputEventInfo[16];
    private ConsoleCharInfo[,] hiddenBuffer;
    private ConsoleInputBuffer inputBuffer;
    private bool leftKeyPressed;
    private bool rightKeyPressed;
    private int playerX;
    private int missileX;
    private int missileY;
    private int missileDelay;
    private bool missileFired;
    private Demon[] demons = new Demon[3];
    private Random random = new Random();
    private int[] spritePattern = { 0, 1, 2, 1 };
    private int spriteIndex;
    private int spriteChangeDelay;
    private bool moveDemons;
    private int bulletDelay;
    private List<Bullet> bullets = new List<Bullet>();
    private ShipState shipState = ShipState.Instructions;
    private int shipExplodingStep;
    private int shipExplodingDelay;
    private List<Bird> birds = new List<Bird>();
    private int score;
    private int demonsRemaining = 0;
    private int level = 0;
    private int power = 100;
    private List<Diamond> diamonds = new List<Diamond>();
    private int gameOverDelay;

    public GameEngine(ConsoleCharInfo[,] hiddenBuffer, ConsoleInputBuffer inputBuffer) {
      this.hiddenBuffer = hiddenBuffer;
      this.inputBuffer = inputBuffer;

      playerX = (80 - shipSprite.Width) / 2;
      missileY = 49 - shipSprite.Height;
      for (int i = 0; i < 3; i++) {
        demons[i] = new Demon();
      }
    }

    public int UpdateModel() {

      if (demonsRemaining == 0 && birds.Count == 0 && bullets.Count == 0 && diamonds.Count == 0) {
        demonsRemaining = 10;
        power = Math.Min(100, power + 20);
        level++;
        demons[0].y = 3;
        demons[0].seeker = false;
        demons[0].formDelay = 240;
        demons[0].state = DemonState.Unformed;
        demons[0].demonSpriteType = random.Next(Math.Min(5, level));
        demons[1].y = 12;
        demons[1].seeker = false;
        demons[1].formDelay = 120;
        demons[1].state = DemonState.Unformed;
        demons[1].demonSpriteType = random.Next(Math.Min(5, level));
        demons[2].y = 21;
        demons[2].seeker = true;
        demons[2].formDelay = 0;
        demons[2].state = DemonState.Unformed;
        demons[2].demonSpriteType = random.Next(Math.Min(5, level));
      }

      if (spriteChangeDelay > 0) {
        spriteChangeDelay--;
      } else {
        spriteChangeDelay = 10;
        spriteIndex++;
        if (spriteIndex > 3) {
          spriteIndex = 0;
        }
      }

      for (int i = 0; i < 3; i++) {
        Demon demon = demons[i];
        switch (demon.state) {
          case DemonState.Unformed:
            if (demon.formDelay > 0) {
              demon.formDelay--;
            } else {
              demon.targetX = random.Next(160 - demonSprites[0, 0].Width);
              demon.x = demon.targetX - 160;
              demon.steps = 45;
              demon.dx = (demon.targetX - demon.x) / demon.steps;
              demon.state = DemonState.Forming;
            }
            break;
          case DemonState.Forming:
            if (demon.steps == 0) {
              demon.state = DemonState.Formed;
              demon.demonSpriteType = random.Next(Math.Min(5, level));
            } else {
              demon.x += demon.dx;
              demon.steps--;
            }
            break;
          case DemonState.Exploding:
            if (demon.formDelay > 0) {
              demon.formDelay--;
            } else {
              demon.formDelay = 10;
              demon.steps++;
              if (demon.steps == 4) {
                demonsRemaining--;
                if (demonsRemaining > 2) {
                  demon.state = DemonState.Unformed;
                  demon.formDelay = 120;
                } else {
                  demon.state = DemonState.Done;
                }
              }
            }
            break;
          case DemonState.Formed:
            if (moveDemons) {
              if (demon.steps == 0) {
                demon.steps = random.Next(30, 60);
                int targetX = random.Next(160 - demonSprites[0, 0].Width);
                if (demon.seeker && !missileFired && random.NextDouble() > 0.3) {
                  targetX = playerX - 11;
                }
                demon.dx = (targetX - demon.x) / demon.steps;
              } else {
                demon.x += demon.dx;
                demon.steps--;
              }
              if (demon.bulletDelay == 0) {
                demon.bulletDelay = Math.Max(0, 120 - level * 5) + random.Next(120);
                Bullet bullet = new Bullet();
                bullets.Add(bullet);
                Sprite sprite = demonSprites[demon.demonSpriteType, spritePattern[(i + spriteIndex) & 0x03]];
                bullet.x = random.Next(sprite.Width) + demon.x;
                bullet.y = demon.y + sprite.Height;
                if (level > 2) {
                  double vx = playerX + shipSprite.Width / 2.0 - bullet.x;
                  double vy = 49 - shipSprite.Height / 2.0 - bullet.y;
                  double mag = Math.Sqrt(vx * vx + vy * vy);
                  if (level > 10) {
                    mag /= (level / 10);
                  } 
                  vx /= mag;
                  vy /= mag;
                  bullet.vx = vx;
                  bullet.vy = vy;
                } else {
                  bullet.vy = 1;
                }
              } else {
                demon.bulletDelay--;
              }
            }
            break;
        }
      }

      for (int i = birds.Count - 1; i >= 0; i--) {
        Bird bird = birds[i];
        if (bird.y > 50) {
          birds.RemoveAt(i);
        } else {
          if (bird.exploding) {
            if (bird.explodeDelay > 0) {
              bird.explodeDelay--;
            } else {
              bird.explodeDelay = 5;
              bird.explodeIndex++;
              if (bird.explodeIndex == 3) {
                birds.RemoveAt(i);
              }
            }
          } else if (shipState == ShipState.Alive && SpritesCollide(shipSprite, playerX, 49 - shipSprite.Height,
              birdSprites[spritePattern[spriteIndex]], (int)bird.x, (int)bird.y)) {
            bird.exploding = true;
            bird.explodeIndex = 0;
            bird.explodeDelay = 5;
            HitPlayer();
          } else {
            bird.angle += bird.angleInc;
            bird.y += bird.vy;
            bird.x = (bird.centerX + 40.0 * Math.Sin(bird.angle));
          }
        }
      }

      for (int i = diamonds.Count - 1; i >= 0; i--) {
        Diamond diamond = diamonds[i];
        if (diamond.y > 50) {
          diamonds.RemoveAt(i);
        } else if (SpritesCollide(shipSprite, playerX, 49 - shipSprite.Height,
            diamondSprite, (int)diamond.x, (int)diamond.y)) {
          score += 1000;
          power = Math.Min(100, power + 20);
          diamonds.RemoveAt(i);
        } else {
          diamond.y += 0.1;
        }
      }

      moveDemons = !moveDemons;

      if (shipState == ShipState.Exploding) {
        if (shipExplodingDelay > 0) {
          shipExplodingDelay--;
        } else {
          shipExplodingDelay = 5;
          shipExplodingStep++;
          if (shipExplodingStep == 4) {
            if (power > 0) {
              shipState = ShipState.Alive;
            } else {
              shipState = ShipState.GameOver;
              gameOverDelay = 5 * 120;
            }
          }
        }
      } else if (shipState == ShipState.GameOver) {
        if (gameOverDelay > 0) {
          gameOverDelay--;
        } else {
          for (int y = 0; y < 50; y++) {
            for (int x = 0; x < 80; x++) {
              hiddenBuffer[y, x].AsciiChar = (byte)0xdd;
            }
          }
          shipState = ShipState.Instructions;
        }
      } 

      if (bulletDelay > 0) {
        bulletDelay--;
      } else {
        bulletDelay = 8;
        for (int i = bullets.Count - 1; i >= 0; i--) {
          Bullet bullet = bullets[i];
          bullet.x += bullet.vx;
          bullet.y += bullet.vy;
          if (bullet.x < 0 || bullet.y < 0 || bullet.x > 160 || bullet.y > 50) {
            bullets.RemoveAt(i);
          } else if (shipState == ShipState.Alive 
              && bullet.x >= playerX && bullet.x < playerX + shipSprite.Width
              && bullet.y >= 49 - shipSprite.Height) {
            int x = (int)(bullet.x - playerX);
            int y = (int)(bullet.y - (49 - shipSprite.Height));
            if (y < shipSprite.Height) {
              bullets.RemoveAt(i);
              HitPlayer();
            }
          }
        }
      }

      if (inputBuffer.PeekEvents(inputEvents) > 0) {
        int eventsRead = inputBuffer.ReadEvents(inputEvents);
        for (int i = 0; i < eventsRead; i++) {
          if (inputEvents[i].EventType == ConsoleInputEventType.KeyEvent) {
            switch (inputEvents[i].KeyEvent.VirtualKeyCode) {
              case ConsoleKey.LeftArrow:
                leftKeyPressed = inputEvents[i].KeyEvent.KeyDown;
                break;
              case ConsoleKey.RightArrow:
                rightKeyPressed = inputEvents[i].KeyEvent.KeyDown;
                break;
              case ConsoleKey.Escape:
                Environment.Exit(0);
                break;
              default:
                if (shipState == ShipState.Alive && !missileFired) {
                  missileFired = true;
                  for (int j = 0; j < 3; j++) {
                    Demon demon = demons[j];
                    demon.steps = 0;
                  }
                } else if (shipState == ShipState.Instructions) {
                  ResetGame();
                }
                break;
            }
          }
        }
      }
      if (shipState == ShipState.Alive) {
        if (leftKeyPressed) {
          if (playerX > 0) {
            playerX--;
          }
        } else if (rightKeyPressed) {
          if (playerX < 160 - shipSprite.Width) {
            playerX++;
          }
        }
      }
      if (missileFired) {
        for (int i = 0; i < 3; i++) {
          Demon demon = demons[i];
          if (demon.state == DemonState.Formed) {
            Sprite sprite = demonSprites[demon.demonSpriteType, spritePattern[(i + spriteIndex) & 0x03]];
            if (missileY >= demon.y && missileY < demon.y + sprite.Height
                && missileX >= demon.x && missileX < demon.x + sprite.Width) {
              int x = (int)(missileX - demon.x);
              int y = (int)(missileY - demon.y);
              if (!sprite.transparent[y, x]) {                
                demon.state = DemonState.Exploding;
                demon.steps = 0;
                demon.formDelay = 10;                  
                missileFired = false;
                missileY = 49 - shipSprite.Height;
                missileX = playerX + 6;
                score += 100;

                if (level > 3 && random.NextDouble() < 1 - 1.0 / (level - 2)) {
                  Bird bird = new Bird();
                  bird.x = bird.centerX = demon.x;
                  bird.y = demon.y;
                  bird.angle = 0;
                  bird.angleInc = -0.005 - 0.005 * random.NextDouble();
                  bird.vy = 0.01 + 0.05 * random.NextDouble();
                  birds.Add(bird);
                  bird = new Bird();
                  bird.x = bird.centerX = demon.x + demonSprites[0, 0].Width - birdSprites[0].Width;
                  bird.y = demon.y;
                  bird.angle = 0;
                  bird.angleInc = 0.005 + 0.005 * random.NextDouble();
                  bird.vy = 0.01 + 0.05 * random.NextDouble();
                  birds.Add(bird);
                }

                if (demonsRemaining == 5) {
                  Diamond diamond = new Diamond();
                  diamond.y = demon.y;
                  diamond.x = (demonSprites[0, 0].Width - diamondSprite.Width) / 2.0 + demon.x;
                  diamonds.Add(diamond);
                }

                break;
              }
            }
          }
        }
        for (int i = birds.Count - 1; i >= 0; i--) {
          Bird bird = birds[i];
          if (!bird.exploding) {
            Sprite sprite = birdSprites[spritePattern[spriteIndex]];
            if (missileY >= bird.y && missileY < bird.y + sprite.Height
                  && missileX >= bird.x && missileX < bird.x + sprite.Width) {
              bird.exploding = true;
              bird.explodeIndex = 0;
              bird.explodeDelay = 5;
              missileFired = false;
              missileY = 49 - shipSprite.Height;
              missileX = playerX + 6;
              score += 50;
              break;
            }
          }
        }
        if (missileDelay == 0) {
          missileY--;
          missileDelay = 1;
        } else {
          missileDelay--;
        }
        if (missileY == -10) {
          missileFired = false;
          missileY = 49 - shipSprite.Height;
          missileX = playerX + 6;
        }
      } else {
        missileX = playerX + 6;
      }
      return 0;
    }

    private void HitPlayer() {
      shipState = ShipState.Exploding;
      shipExplodingStep = 0;
      shipExplodingDelay = 5;
      power = Math.Max(power - 10, 0);
    }

    public int RenderFrame() {
      if (shipState == ShipState.Exploding) {
        for (int y = 0; y < 50; y++) {
          for (int x = 0; x < 80; x++) {
            hiddenBuffer[y, x].Foreground = ConsoleColor.White;
            hiddenBuffer[y, x].Background = ConsoleColor.White;
          }
        }
      } else {
        for (int x = 0; x < 80; x++) {
          hiddenBuffer[49, x].Foreground = ConsoleColor.Blue;
          hiddenBuffer[49, x].Background = ConsoleColor.Blue;
        }
        GameEngine.PrintLine(hiddenBuffer, 12, 0, ConsoleColor.White, ConsoleColor.Black,
            string.Format("Score: {0}", score));
        GameEngine.PrintLine(hiddenBuffer, 69, 0, ConsoleColor.White, ConsoleColor.Black,
            string.Format("Power: {0}%", power));
        GameEngine.PrintLine(hiddenBuffer, 55, 0, ConsoleColor.White, ConsoleColor.Black,
            string.Format("Demons: {0}", demonsRemaining));
        GameEngine.PrintLine(hiddenBuffer, 0, 0, ConsoleColor.White, ConsoleColor.Black,
            string.Format("Level: {0}", level));
      }
      foreach (Bullet bullet in bullets) {
        bulletSprite.Draw((int)bullet.x, (int)bullet.y, hiddenBuffer);
      }      
      for (int i = 0; i < 3; i++) {
        Demon demon = demons[i];
        switch(demon.state) {
          case DemonState.Formed:
            demonSprites[demon.demonSpriteType, spritePattern[(i + spriteIndex) & 0x03]].Draw(
              (int)demon.x, demon.y, hiddenBuffer);
            break;
          case DemonState.Forming:
            demonFormSprites[0].Draw((int)demon.x, demon.y, hiddenBuffer);
            demonFormSprites[1].Draw((int)(2 * demon.targetX - demon.x), demon.y, hiddenBuffer);
            break;
          case DemonState.Exploding:
            demonBoomSprites[demon.steps].Draw((int)demon.x, demon.y, hiddenBuffer);
            break;
        }
      }
      foreach (Bird bird in birds) {
        if (bird.exploding) {
          birdBoomSprites[bird.explodeIndex].Draw((int)bird.x, (int)bird.y, hiddenBuffer);
        } else {
          birdSprites[spritePattern[spriteIndex]].Draw((int)bird.x, (int)bird.y, hiddenBuffer);
        }
      }
      foreach (Diamond diamond in diamonds) {
        diamondSprite.Draw((int)diamond.x, (int)diamond.y, hiddenBuffer);
      }
      if (shipState == ShipState.Alive || missileFired) {
        missileSprite.Draw(missileX, missileY, hiddenBuffer);
      }
      if (shipState == ShipState.Alive) {
        shipSprite.Draw(playerX, 49 - shipSprite.Height, hiddenBuffer);         
      } else if (shipState == ShipState.Exploding) {
        shipBoomSprites[shipExplodingStep].Draw(playerX, 49 - shipSprite.Height, hiddenBuffer);
      } else if (shipState == ShipState.GameOver) {
        GameEngine.PrintLine(hiddenBuffer, 32, 24, ConsoleColor.White, ConsoleColor.Black,
            string.Format("G A M E  O V E R", level));
      } else if (shipState == ShipState.Instructions) {
        GameEngine.PrintCentered(hiddenBuffer, 10, ConsoleColor.White, ConsoleColor.Black,
            "A S C I I   D E M O N   A T T A C K");
        GameEngine.PrintCentered(hiddenBuffer, 30, ConsoleColor.White, ConsoleColor.Black,
            "Arrow Keys = Move Ship");
        GameEngine.PrintCentered(hiddenBuffer, 32, ConsoleColor.White, ConsoleColor.Black,
            "All Other Keys = Fire");
        GameEngine.PrintCentered(hiddenBuffer, 34, ConsoleColor.White, ConsoleColor.Black,
            "Alt+Enter = Toggle Full Screen Mode");
        GameEngine.PrintCentered(hiddenBuffer, 36, ConsoleColor.White, ConsoleColor.Black,
            "Esc = Quit");
        GameEngine.PrintCentered(hiddenBuffer, 40, ConsoleColor.White, ConsoleColor.Black,
            "Press any of the keys to the start game.");
        GameEngine.PrintCentered(hiddenBuffer, 48, ConsoleColor.White, ConsoleColor.Black,
            "Copyright (C) 1971 Icy Planet Krybor Consortium, Ltd.");
      }

      return 0;
    }

    private void ResetGame() {
      bullets.Clear();
      shipState = ShipState.Alive;
      birds.Clear();
      score = 0;
      demonsRemaining = 0;
      level = 0;
      power = 100;
      diamonds.Clear();
      playerX = (80 - shipSprite.Width) / 2;
      missileY = 49 - shipSprite.Height;
      for (int y = 0; y < 50; y++) {
        for (int x = 0; x < 80; x++) {
          hiddenBuffer[y, x].AsciiChar = (byte)0xdd;
        }
      }
    }

    private bool SpritesCollide(
        Sprite sprite1, int x1, int y1,
        Sprite sprite2, int x2, int y2) {
      if (RectanglesOverlap(x1, y1, sprite1.Width, sprite1.Height, x2, y2, sprite2.Width, sprite2.Height)) {
        x2 -= x1;
        y2 -= y1;
        for (int y = 0; y < sprite1.Height; y++) {
          for (int x = 0; x < sprite1.Width; x++) {
            if (!sprite1.transparent[y, x]
                && InRectangle(x, y, x2, y2, sprite2.Width, sprite2.Height)
                && !sprite2.transparent[y - y2, x - x2]) {
              return true;
            }
          }
        }
      }
      return false;
    }

    private bool RectanglesOverlap(
        int rect1x, int rect1y, int rect1width, int rect1height,
        int rect2x, int rect2y, int rect2width, int rect2height) {
      return Rect2ContainsRect1(
          rect1x, rect1y, rect1width, rect1height,
          rect2x, rect2y, rect2width, rect2height)
        || Rect2ContainsRect1(
          rect2x, rect2y, rect2width, rect2height,
          rect1x, rect1y, rect1width, rect1height);
    }

    private bool Rect2ContainsRect1(
        int rect1x, int rect1y, int rect1width, int rect1height,
        int rect2x, int rect2y, int rect2width, int rect2height) {
      return InRectangle(rect1x, rect1y, rect2x, rect2y, rect2width, rect2height)
          || InRectangle(rect1x + rect1width, rect1y, rect2x, rect2y, rect2width, rect2height)
          || InRectangle(rect1x, rect1y + rect1height, rect2x, rect2y, rect2width, rect2height)
          || InRectangle(rect1x + rect1width, rect1y + rect1height, rect2x, rect2y, rect2width, rect2height);
    }

    private bool InRectangle(int x, int y, int rectX, int rectY, int width, int height) {
      return x >= rectX && y >= rectY && x < rectX + width && y < rectY + height;
    }

    public static void PrintCentered(ConsoleCharInfo[,] hiddenBuffer, int y,
        ConsoleColor foregroundColor, ConsoleColor backgroundColor,
        string text) {
      PrintLine(hiddenBuffer, (80 - text.Length) / 2, y, foregroundColor, backgroundColor, text);
    }

    public static void PrintLine(
        ConsoleCharInfo[,] hiddenBuffer, int x, int y, ConsoleColor foregroundColor, ConsoleColor backgroundColor, 
        string text) {
      for (int i = 0; i < text.Length; i++) {
        int X = x + i;
        if (X >= 0 && X < 80) {
          hiddenBuffer[y, X].AsciiChar = (byte)text[i];
          hiddenBuffer[y, X].Background = backgroundColor;
          hiddenBuffer[y, X].Foreground = foregroundColor;
        }
      }
    }
  }

  enum DemonState { Unformed, Forming, Formed, Exploding, Done };
  enum ShipState { Alive, Exploding, Forming, GameOver, Instructions };

  class Demon {
    public int demonSpriteType;
    public int y;
    public double x;
    public double targetX;
    public double dx;
    public int steps;
    public bool seeker;
    public int formDelay;
    public DemonState state;
    public int bulletDelay;
  }

  class Bullet {
    public double x;
    public double y;
    public double vx;
    public double vy;
  }

  class Bird {
    public double angle;
    public double angleInc;
    public double centerX;
    public double vy;
    public double y;
    public double x;
    public bool exploding;
    public int explodeDelay;
    public int explodeIndex;
  }

  class Diamond {
    public double x;
    public double y;
  }
}
