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

  delegate int GameDelegate();

  class Program {

    public const int FRAMES_PER_SECOND = 120;
    public long FREQUENCY = Stopwatch.Frequency;
    public long TICKS_PER_FRAME = Stopwatch.Frequency / FRAMES_PER_SECOND;
    public Stopwatch stopwatch = new Stopwatch();
    public long nextFrameStart;
    public Random random = new Random();
    public ConsoleScreenBuffer screenBuffer;
    public ConsoleCharInfo[,] hiddenBuffer;
    public ConsoleInputBuffer inputBuffer;
    public GameDelegate updateModelDelegate;
    public GameDelegate renderFrameDelegate;

    public Program() {
      Console.Title = "Ascii Demon Attack";
      inputBuffer = JConsole.GetInputBuffer();
      screenBuffer = JConsole.GetActiveScreenBuffer();
      screenBuffer.SetWindowSize(80, 50);
      screenBuffer.SetBufferSize(80, 50);
      screenBuffer.CursorVisible = false;
      screenBuffer.Clear();
      hiddenBuffer = new ConsoleCharInfo[50, 80];

      GameEngine gameEngine = new GameEngine(hiddenBuffer, inputBuffer);
      updateModelDelegate = gameEngine.UpdateModel;
      renderFrameDelegate = gameEngine.RenderFrame;

      for (int y = 0; y < 50; y++) {
        for (int x = 0; x < 80; x++) {
          hiddenBuffer[y, x].AsciiChar = (byte)0xdd;
        }
      }

      stopwatch.Start();
      nextFrameStart = stopwatch.ElapsedTicks;
      while (true) {
        do {
          UpdateModel();
          nextFrameStart += TICKS_PER_FRAME;
        } while (nextFrameStart < stopwatch.ElapsedTicks);
        RenderFrame();
        long remainingTicks = nextFrameStart - stopwatch.ElapsedTicks;
        if (remainingTicks > 0) {
          Thread.Sleep((int)(1000 * remainingTicks / FREQUENCY));
        }
      }      
    }

    private void UpdateModel() {
      updateModelDelegate();
    }

    private void RenderFrame() {
      for (int y = 0; y < 50; y++) {
        for (int x = 0; x < 80; x++) {
          hiddenBuffer[y, x].Foreground = ConsoleColor.Black;
          hiddenBuffer[y, x].Background = ConsoleColor.Black;
        }
      }
      renderFrameDelegate();
      screenBuffer.WriteBlock(hiddenBuffer, 0, 0, 0, 0, 79, 49);
    }

    static void Main(string[] args) {
      new Program();
    }
  }
}
