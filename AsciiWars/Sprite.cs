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
  class Sprite {
    public int Width;
    public int Height;
    public ConsoleColor[,] image;
    public bool[,] transparent;

    public Sprite(string fileName) : this(Assembly.GetExecutingAssembly()
        .GetManifestResourceStream("AsciiWars.images." + fileName)) {
    }

    public Sprite(Stream stream) {
      BinaryReader reader = new BinaryReader(stream);
      Width = reader.ReadInt32();
      Height = reader.ReadInt32();
      image = new ConsoleColor[Height, Width];
      transparent = new bool[Height, Width];
      for (int y = 0; y < Height; y++) {
        for (int x = 0; x < Width; x++) {
          int palette = reader.ReadInt32();
          if (palette < 0) {
            transparent[y, x] = true;
          } else {
            image[y, x] = (ConsoleColor)palette;
          }
        }
      }
    }

    public Sprite(string[] rows, ConsoleColor color) {
      Width = rows[0].Length;
      Height = rows.Length;
      image = new ConsoleColor[Height, Width];
      transparent = new bool[Height, Width];
      for (int y = 0; y < Height; y++) {
        for (int x = 0; x < Width; x++) {
          if (rows[y][x] == ' ') {
            transparent[y, x] = true;
          } else {
            image[y, x] = color;
            transparent[y, x] = false;
          }
        }
      }
    }

    public void Draw(int x, int y, ConsoleCharInfo[,] hiddenBuffer) {
      for (int i = 0; i < Height; i++) {
        for (int j = 0; j < Width; j++) {
          if (!transparent[i, j]) {
            int r = y + i;
            int c = x + j;
            if (c >= 0 && c < 160 && r >= 0 && r < 50) {
              if ((c & 1) == 0) {
                hiddenBuffer[r, c >> 1].Foreground = image[i, j];
              } else {
                hiddenBuffer[r, c >> 1].Background = image[i, j];
              }
            }
          }
        }
      }
    }
  }
}
