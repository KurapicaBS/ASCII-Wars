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
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace ImageConverter {
  class Program {

    static double[,] palette = {
      {0, 0, 0},
      { 0, 0, 0x80 },
      { 0, 0x80, 0 },
      { 0, 0x80, 0x80 },
      { 0x80, 0, 0 },
      { 0x80, 0, 0x80 },
      { 0x80, 0x80, 0 },
      { 0xC0, 0xC0, 0xC0 },
      { 0x80, 0x80, 0x80 },
      { 0, 0, 0xFF },
      { 0, 0xFF, 0 },
      { 0, 0xFF, 0xFF },
      { 0xFF, 0, 0 },
      { 0xFF, 0, 0xFF },
      { 0xFF, 0xFF, 0 },
      { 0xFF, 0xFF, 0xFF },
    };

    static void Main(string[] args) {
      if (args.Length != 2) {
        Console.WriteLine("Args: [source image] [destination file]");
        return;
      }
      string sourceImageName = args[0];
      string destinationFileName = args[1];
      Bitmap sourceBitmapTemp = new Bitmap(sourceImageName);
      Bitmap sourceBitmap = new Bitmap(
          (int)(sourceBitmapTemp.Width * 160.0 / 640.0), 
          (int)(sourceBitmapTemp.Height * 50.0 / 480.0), 
          PixelFormat.Format32bppArgb);
      
      using (Graphics g = Graphics.FromImage(sourceBitmap)) {
        g.DrawImage(sourceBitmapTemp, 0, 0, sourceBitmap.Width, sourceBitmap.Height);
      }
      sourceBitmapTemp = null;

      double[, ,] pixels = new double[sourceBitmap.Height, sourceBitmap.Width, 4];
      int[,] paletteIndex = new int[sourceBitmap.Height, sourceBitmap.Width];

      BitmapData bmData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
          ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
      int stride = bmData.Stride;
      System.IntPtr Scan0 = bmData.Scan0;
      unsafe {
        byte* p = (byte*)(void*)Scan0;

        int nOffset = stride - sourceBitmap.Width * 4;

        for (int y = 0; y < sourceBitmap.Height; ++y) {
          for (int x = 0; x < sourceBitmap.Width; ++x) {
            pixels[y, x, 3] = p[3];
            pixels[y, x, 2] = p[0];
            pixels[y, x, 1] = p[1];
            pixels[y, x, 0] = p[2];
            p += 4;
          }
          p += nOffset;
        }
      }

      sourceBitmap.UnlockBits(bmData);

      for (int y = 0; y < sourceBitmap.Height; y++) {
        for (int x = 0; x < sourceBitmap.Width; x++) {
          int bestIndex = 0;
          double lowestError = Double.MaxValue;
          for(int i = 0; i < palette.GetLength(0); i++) {
            double error = 0;
            for(int c = 0; c < 3; c++) {
              double dc = pixels[y, x, c] - palette[i, c];
              error += dc * dc;
            }
            if (error < lowestError) {
              lowestError = error;
              bestIndex = i;
            }
          }
          double redError = (pixels[y, x, 0] - palette[bestIndex, 0]) / 16.0;
          double greenError = (pixels[y, x, 1] - palette[bestIndex, 1]) / 16.0;
          double blueError = (pixels[y, x, 2] - palette[bestIndex, 2]) / 16.0;
          //AddColor(pixels, sourceBitmap.Width, sourceBitmap.Height, x + 1, y, 7 * redError, 7 * greenError, 7 * blueError);
          //AddColor(pixels, sourceBitmap.Width, sourceBitmap.Height, x - 1, y + 1, 3 * redError, 3 * greenError, 3 * blueError);
          //AddColor(pixels, sourceBitmap.Width, sourceBitmap.Height, x, y + 1, 5 * redError, 5 * greenError, 5 * blueError);
          //AddColor(pixels, sourceBitmap.Width, sourceBitmap.Height, x + 1, y + 1, redError, greenError, blueError);
          for (int c = 0; c < 3; c++) {
            if (pixels[y, x, 3] < 255) {
              paletteIndex[y, x] = -1;
            } else {
              paletteIndex[y, x] = bestIndex;
            }
            pixels[y, x, c] = palette[bestIndex, c];            
          }
        }
      }

      BinaryWriter writer = new BinaryWriter(
          new FileStream(destinationFileName, FileMode.Create, FileAccess.Write));
      writer.Write(sourceBitmap.Width);
      writer.Write(sourceBitmap.Height);
      for (int y = 0; y < sourceBitmap.Height; y++) {
        for (int x = 0; x < sourceBitmap.Width; x++) {
          writer.Write(paletteIndex[y, x]);
        }
      }
      writer.Close();
    }

    private static void AddColor(double[,,] pixels, int width, int height, int x, int y, double red, double green, double blue) {
      if (x >= 0 && y >= 0 && x < width && y < height) {
        pixels[y, x, 0] += red;
        pixels[y, x, 1] += green;
        pixels[y, x, 2] += blue;
      }
    }
  }
}

