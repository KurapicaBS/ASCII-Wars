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

namespace AsciiWars {
  partial class GameEngine {
    Sprite diamondSprite = new Sprite(new string[] {
      "   X X X   ",
      "   X X X   ",
      "  XX X XX  ",
      "XXX  X  XXX",
    }, ConsoleColor.Cyan);
    Sprite shipSprite = new Sprite(new string[] {
      "     X  X     ",
      "    XX  XX    ",
      "  XXXX  XXXX  ",
      " XXXX    XXXX ",
      "XXXX      XXXX",
      "XXXX      XXXX",
    }, ConsoleColor.Magenta);
    Sprite[] shipBoomSprites = {
      new Sprite(new string[] {
        "   X X     X  ",
        "  XXX   XX  XX",
        " X  XX  XXX  X",
        "XX XX     XXXX",
        "XXX       XXX ",
        "X XX       XXX",
      }, ConsoleColor.Magenta),
      new Sprite(new string[] {
        " XX       XXX ",
        " XX X X XXX X ",
        "XX X X  X X XX",
        "X X X    XX X ",
        "X X        XXX",
        "              ",
      }, ConsoleColor.Red),
     new Sprite(new string[] {
        "X X X XX X  X ",
        "X XX    X XX  ",
        " XX X    XX X ",
        "  X      X X X",
        "              ",
        "              ",
      }, ConsoleColor.Magenta),
     new Sprite(new string[] {
        "   X    X  X  ",
        " X  X   X    X",
        "  X        XX ",
        "              ",
        "              ",
        "              ",
      }, ConsoleColor.DarkRed),
    };
    Sprite missileSprite = new Sprite(new string[] {
      "XX",
      "XX",
      "XX",
      "XX",
    }, ConsoleColor.White);
    Sprite bulletSprite = new Sprite(new string[] {
      "XX",
    }, ConsoleColor.Yellow);
    Sprite[] birdSprites = {
      new Sprite(new string[] {
        "      XX        ",
        "  XXXXXXXXXX    ",
        "XXXX      XXXX  ",
        "XX          XX  ",
        "XX          XX  ",
        "  XX      XX    ",
        "                ",
        "                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {
        "                ",
        "                ",
        "  XXXX  XXXX    ",
        "XXXX  XX  XXXX  ",
        "XX          XX  ",
        "XX          XX  ",
        "                ",
        "                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {
        "XX          XX  ",
        "XXXX      XXXX  ",
        "  XXXX  XXXX    ",
        "    XX  XX      ",
        "      XX        ",
        "                ",
        "                ",
        "                ",
      }, ConsoleColor.Green),
    };
    Sprite[] birdBoomSprites = {
      new Sprite(new string[] {
        "                ",
        "    XX          ",
        "      XX  XX    ",
        "        XX      ",
        "  XXXX  XX      ",
        "      XX  XX    ",
        "    XX          ",
        "                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {
        "                ",
        "XX        XX    ",
        "  XX            ",
        "      XX  XX    ",
        "XX  XX          ",
        "      XX    XX  ",
        "          XX    ",
        "  XX            ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {
        "                ",
        "X      X        ",
        "  X          X  ",
        "      X    X    ",
        " X              ",
        "      X        X",
        "          X     ",
        "    X           ",
      }, ConsoleColor.DarkGreen),
    };
    Sprite[] demonFormSprites = { 
      new Sprite(new string[] {
        "                                ",
        "X  X   X    X      X   X        ",
        "                                ",
        "  X        X       X    X  X    ",
        "                                ",
        "     X    X    X         X      ",
        "                                ",
        "X X     X     X   X  X          ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {
        "        X   X    X   X   X  X X ",
        "                                ",
        "   X   X      X      X  X    X  ",
        "                                ",
        "  X     X       X  X    X X   X ",
        "                                ",
        "       X    X     X   X    X   X",
        "                                ",
      }, ConsoleColor.Green),
    };
    Sprite[] demonBoomSprites = {
      new Sprite(new string[] {       
        "        XX     XX  X   X X      ",
        "           X  X                 ",
        "  XX    XXX X XXXXXX    XX      ",
        "      XX   XXX XX XXXXXX    XX  ",
        " X  X   XX  XXX  XXX  XXX       ",
        " XX       XXXX    X   XX      XX",  
        "  X     XX          XX       XX ",
        "                                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {       
        "X    X           X X X    X     ",
        " X     XX  X  X  XXX     XX     ",
        "     X  X   X XX    XX X     X  ",
        "X  X  X  X  XX XX X X   XX  X   ",
        "X       XX  X X  XX   X X     X ",
        " X     X    XX    X   X       X ",  
        "  X                             ",
        "                                ",
      }, ConsoleColor.Yellow),
      new Sprite(new string[] {       
        " X X   XX  X  X  X X        X   ",
        "     X  X X X XX  X XX X  X  X  ",
        "X  X       X X    X X X X   X  X",
        "X       X   X           X     X ",
        " X                    X         ",  
        "                                ",
        "                                ",
        "                                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {       
        "    X   X  X X      XX X  X     ",
        "    X      X X   X  X   XX      ",
        "       X   X   X        X       ",
        "                      X         ",  
        "                                ",
        "                                ",
        "                                ",
        "                                ",
      }, ConsoleColor.DarkGreen),
    };
    Sprite[,] demonSprites = { {
      new Sprite(new string[] {
        "                                ",
        "            XX    XX            ",
        "XXXXXXXX                XXXXXXXX",
        "      XXXX    XXXX    XXXX      ",
        "        XXXXXX    XXXXXX        ",
        "          XXXXXXXXXXXX          ",
        "            XXXXXXXX            ",
        "                                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] { 
        "                                ",
        "          XX        XX          ",
        "                                ",
        "  XXXXXX      XXXX      XXXXXX  ",
        "XXXX    XXXXXX    XXXXXX    XXXX",      
        "            XXXXXXXX            ",
        "          XXXX    XXXX          ",
        "                                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {       
        "        XX            XX        ",
        "              XXXX              ",
        "            XX    XX            ",
        "      XXXXXXXXXXXXXXXXXXXX      ",
        "    XX      XXXXXXXX      XX    ",
        "  XX      XXXX    XXXX      XX  ",  
        "  XX    XXXX        XXXX    XX  ",
        "                                ",
      }, ConsoleColor.Green),
    }, {
      new Sprite(new string[] {
        "        XXXX        XXXX        ",
        "        XXXXXXXXXXXXXXXX        ",
        "      XX  XX        XX  XX      ",
        "    XX    XX        XX    XX    ",
        "    XX      XX    XX      XX    ",
        "    XX        XXXX        XX    ",
        "      XX                XX      ",
        "                                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {
        "                                ",
        "          XXXX    XXXX          ",
        "      XXXXXXXXXXXXXXXXXXXX      ",
        "  XXXX    XX        XX    XXXX  ",
        "XX        XX        XX        XX",
        "XX          XX    XX          XX",
        "  XX                        XX  ",
        "                                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {
        "                                ",
        "                                ",
        "        XXXXXXXXXXXXXXXX        ",
        "      XX  XX        XX  XX      ",
        "    XX    XX        XX    XX    ",
        "  XX      XX        XX      XX  ",
        "                                ",
      }, ConsoleColor.Green),
    }, {
      new Sprite(new string[] {
        "                                ",
        "          XX        XX          ",
        "        XX  XX    XX  XX        ",
        "          XX        XX          ",
        "            XX    XX            ",
        "      XXXXXX  XXXX  XXXXXX      ",
        "    XXXX  XXXX    XXXX  XXXX    ",
        "                                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {
        "                                ",
        "        XX  XX    XX  XX        ",
        "        XX            XX        ",
        "    XX    XX        XX    XX    ",
        "    XXXX    XX    XX    XXXX    ",
        "      XXXXXXXX    XXXXXXXX      ",
        "        XX    XXXX    XX        ",
        "                                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {
        "    XX    XX        XX    XX    ",
        "      XX                XX      ",
        "  XX    XX            XX    XX  ",
        "XXXXXX    XX        XX    XXXXXX",
        "XX  XXXX    XX    XX    XXXX  XX",
        "XX    XXXXXXXXXXXXXXXXXXXX    XX",
        "            XX    XX            ",
        "                                ",
      }, ConsoleColor.Green),
    },{
      new Sprite(new string[] {
        "                                ",
        "  XXXX    XX        XX    XXXX  ",
        "XX    XX                XX    XX",
        "XX      XX            XX      XX",
        "XX        XXXXXXXXXXXX        XX",
        "XX      XXXXXXXXXXXXXXXX      XX",
        "XX    XXXXXXXXXXXXXXXXXXXX    XX",
        "                                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {
        "          XX        XX          ",    
        "  XXXXXX                XXXXXX  ",
        "XX      XX            XX      XX",
        "XX        XXXXXXXXXXXX        XX",
        "XX      XXXX        XXXX      XX",
        "XX    XXXX            XXXX    XX",
        "  XX    XXXXXXXXXXXXXXXX    XX  ",
        "                                ",
      }, ConsoleColor.Green),
      new Sprite(new string[] {
        "    XXXX    XX    XX    XXXX    ",
        "  XX    XX            XX    XX  ",
        "XX        XXXXXXXXXXXX        XX",
        "XX      XXXX        XXXX      XX",
        "XX    XXXX            XXXX    XX",
        "  XX    XXXX        XXXX    XX  ",
        "    XX    XXXXXXXXXXXX    XX    ",
        "                                ",
      }, ConsoleColor.Green),
    }, {
      new Sprite(new string[] {
        "        XX            XX        ",
        "      XX  XX        XX  XX      ",
        "    XX      XXXXXXXX      XX    ",
        "    XX      XXXXXXXX      XX    ",
        "    XX    XX        XX    XX    ",
        "  XX      XX        XX      XX  ",
        "          XX        XX          ",
        "                                ",
      }, ConsoleColor.Green),

      new Sprite(new string[] {
        "      XXXX            XXXX      ",
        "    XX    XXXXXXXXXXXX    XX    ",
        "    XX      XXXXXXXX      XX    ",
        "    XX    XX        XX    XX    ",
        "    XX  XX            XX  XX    ",
        "    XX  XX            XX  XX    ",
        "    XX                    XX    ",
        "                                ",
      }, ConsoleColor.Green),

      new Sprite(new string[] {
        "    XXXXXXXXXXXXXXXXXXXXXXXX    ",
        "  XX      XXXXXXXXXXXX      XX  ",
        "  XX        XX    XX        XX  ",
        "  XX      XX        XX      XX  ",
        "  XX    XX            XX    XX  ",
        "    XX                    XX    ",
        "      XX                XX      ",
        "                                ",
      }, ConsoleColor.Green),
    }};
  }
}
