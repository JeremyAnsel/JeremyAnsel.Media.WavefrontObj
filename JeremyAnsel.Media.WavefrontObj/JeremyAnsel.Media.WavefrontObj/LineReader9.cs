// <copyright file="LineReader9.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

#if NET6_0_OR_GREATER

using System.Text;

namespace JeremyAnsel.Media.WavefrontObj
{
    internal class LineReader9
    {
        public static readonly char[] LineSeparators = new char[] { ' ', '\t' };

        public List<string> HeaderTextLines { get; } = new List<string>();

        private char[] _lineBuffer = new char[256];
        private int _lineBufferPosition = 0;

        private void SetLineBufferLength(int length)
        {
            if (length <= _lineBuffer.Length)
            {
                return;
            }

            int newLength = Math.Max(length, _lineBuffer.Length * 2);
            var newBuffer = new char[newLength];
            Array.Copy(_lineBuffer, newBuffer, _lineBuffer.Length);
            _lineBuffer = newBuffer;
        }

        private void SetLineBufferSlice(ReadOnlySpan<char> line)
        {
            SetLineBufferLength(line.Length);
            line.CopyTo(_lineBuffer);
            _lineBufferPosition = line.Length;
        }

        private void AppendLineBufferSlice(ReadOnlySpan<char> line)
        {
            SetLineBufferLength(_lineBufferPosition + line.Length);
            line.CopyTo(_lineBuffer.AsSpan()[_lineBufferPosition..]);
            _lineBufferPosition += line.Length;
        }

        //private void AddLineBufferChar(char c)
        //{
        //    SetLineBufferLength(_lineBufferPosition + 1);
        //    _lineBuffer[_lineBufferPosition] = c;
        //    _lineBufferPosition++;
        //}

        private char[] _lineReadBuffer = new char[256];
        private int _lineReadBufferPosition = 0;

        private void SetLineReadBufferLength(int length)
        {
            if (length <= _lineReadBuffer.Length)
            {
                return;
            }

            int newLength = Math.Max(length, _lineReadBuffer.Length * 2);
            var newBuffer = new char[newLength];
            Array.Copy(_lineReadBuffer, newBuffer, _lineReadBuffer.Length);
            _lineReadBuffer = newBuffer;
        }

        //private void SetLineReadBufferSlice(ReadOnlySpan<char> line)
        //{
        //    SetLineReadBufferLength(line.Length);
        //    line.CopyTo(_lineReadBuffer);
        //    _lineReadBufferPosition = line.Length;
        //}

        private void AddLineReadBufferChar(char c)
        {
            SetLineReadBufferLength(_lineReadBufferPosition + 1);
            _lineReadBuffer[_lineReadBufferPosition] = c;
            _lineReadBufferPosition++;
        }

        private void ReadLine9(StreamReader reader)
        {
            _lineReadBufferPosition = 0;

            while (true)
            {
                int r = reader.Read();

                if (r == -1)
                {
                    break;
                }

                char c = (char)r;

                if (c == '\r')
                {
                    continue;
                }

                if (c == '\n')
                {
                    break;
                }

                AddLineReadBufferChar(c);
            }
        }

        public IEnumerable<(char[] Buffer, int Length)> Read9(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true);
            _lineBufferPosition = 0;
            bool isMultiLine = false;
            bool readHeaderText = true;

            while (!reader.EndOfStream)
            {
                ReadLine9(reader);

                if (_lineReadBufferPosition == 0 || MemoryExtensions.IsWhiteSpace(_lineReadBuffer.AsSpan()[.._lineReadBufferPosition]))
                {
                    if (readHeaderText)
                    {
                        this.HeaderTextLines.Add(string.Empty);
                    }

                    continue;
                }

                if (isMultiLine)
                {
                    SetLineBufferSlice(_lineBuffer.AsSpan()[..(_lineBufferPosition - 1)]);
                    AppendLineBufferSlice(_lineReadBuffer.AsSpan()[.._lineReadBufferPosition]);
                }
                else
                {
                    SetLineBufferSlice(_lineReadBuffer.AsSpan()[.._lineReadBufferPosition]);
                }

                SetLineBufferSlice(MemoryExtensions.Trim(_lineBuffer.AsSpan()[.._lineBufferPosition]));

                isMultiLine = MemoryExtensions.EndsWith(_lineBuffer.AsSpan()[.._lineBufferPosition], "\\".AsSpan());

                if (isMultiLine)
                {
                    continue;
                }

                int commentIndex = MemoryExtensions.IndexOf(_lineBuffer.AsSpan()[.._lineBufferPosition], '#');

                if (commentIndex == 0)
                {
                    if (readHeaderText)
                    {
                        this.HeaderTextLines.Add(_lineBuffer.AsSpan()[1.._lineBufferPosition].ToString());
                    }

                    continue;
                }

                if (readHeaderText)
                {
                    readHeaderText = false;
                }

                if (commentIndex != -1)
                {
                    SetLineBufferSlice(_lineBuffer.AsSpan()[0..commentIndex]);
                }

                yield return (_lineBuffer, _lineBufferPosition);
                _lineBufferPosition = 0;
            }
        }
    }
}

#endif
