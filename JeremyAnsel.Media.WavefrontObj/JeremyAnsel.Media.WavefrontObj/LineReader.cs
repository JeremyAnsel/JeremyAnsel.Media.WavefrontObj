// <copyright file="LineReader.cs" company="Jérémy Ansel">
// Copyright (c) 2017 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JeremyAnsel.Media.WavefrontObj
{
    internal static class LineReader
    {
        private static readonly char[] lineSeparators = new char[] { ' ', '\t' };

        public static IEnumerable<string[]> Read(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                string line = null;
                bool isMultiLine = false;

                while (!reader.EndOfStream)
                {
                    string currentLine = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(currentLine))
                    {
                        continue;
                    }

                    if (isMultiLine)
                    {
                        line = line.Substring(0, line.Length - 1) + currentLine;
                    }
                    else
                    {
                        line = currentLine;
                    }

                    line = line.Trim();

                    isMultiLine = line.EndsWith("\\");

                    if (isMultiLine)
                    {
                        continue;
                    }

                    int commentIndex = line.IndexOf('#');

                    if (commentIndex == 0)
                    {
                        continue;
                    }

                    if (commentIndex != -1)
                    {
                        line = line.Substring(0, commentIndex);
                    }

                    string[] values = line.Split(lineSeparators, StringSplitOptions.RemoveEmptyEntries);

                    yield return values;
                }
            }
        }
    }
}
