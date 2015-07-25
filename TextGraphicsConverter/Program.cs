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

namespace TextGraphicsConverter {
  public class Program {
    public static void Main(string[] args) {
      string fileName = args[0];
      StreamReader reader = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read));
      Stack<string> strings1 = new Stack<string>();
      Stack<string> strings2 = new Stack<string>();
      StringBuilder sb = new StringBuilder();
      string input = null;
      while ((input = reader.ReadLine()) != null) {
        int index1 = input.IndexOf('|');
        if (index1 >= 0) {
          int index2 = input.IndexOf('|', index1 + 1);
          if (index2 >= 0) {
            string data = input.Substring(index1 + 1, index2 - index1 - 1);
            sb.Length = 0;            
            for (int i = 0; i < data.Length; i++) {
              sb.Append(data[i]).Append(data[i]);
            }
            strings1.Push(string.Format("\"{0}\",", sb.ToString()));
            for (int i = data.Length - 1; i >= 0; i--) {
              sb.Append(data[i]).Append(data[i]);
            }
            strings2.Push(string.Format("\"{0}\",", sb.ToString()));
          }
        }
      }
      reader.Close();
      while (strings1.Count > 0) {
        Console.WriteLine(strings1.Pop());
      }
      while (strings2.Count > 0) {
        Console.WriteLine(strings2.Pop());
      }
    }
  }
}
