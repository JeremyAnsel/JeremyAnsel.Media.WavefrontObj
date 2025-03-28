// <copyright file="LineReader.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System.Text;

namespace JeremyAnsel.Media.WavefrontObj
{
    internal class LineReader
    {
        public static readonly char[] LineSeparators = new char[] { ' ', '\t' };

        public List<string> HeaderTextLines { get; } = new List<string>();

        public IEnumerable<string> Read(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
            {
                string line = string.Empty;
                bool isMultiLine = false;
                bool readHeaderText = true;

                while (!reader.EndOfStream)
                {
                    string? currentLine = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(currentLine))
                    {
                        if (readHeaderText)
                        {
                            this.HeaderTextLines.Add(string.Empty);
                        }

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

                    isMultiLine = line.EndsWith("\\", StringComparison.Ordinal);

                    if (isMultiLine)
                    {
                        continue;
                    }

                    int commentIndex = line.IndexOf('#');

                    if (commentIndex == 0)
                    {
                        if (readHeaderText)
                        {
                            this.HeaderTextLines.Add(line.Substring(1));
                        }

                        continue;
                    }

                    if (readHeaderText)
                    {
                        readHeaderText = false;
                    }

                    if (commentIndex != -1)
                    {
                        line = line.Substring(0, commentIndex);
                    }

                    yield return line;
                }
            }
        }
    }
}
