// <copyright file="LineReaderTests.cs" company="Jérémy Ansel">
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
using System.Threading.Tasks;
using Xunit;

namespace JeremyAnsel.Media.WavefrontObj.Tests
{
    public class LineReaderTests
    {
        [Fact]
        public void Parsing_SpaceAndTab_Valid()
        {
            string content = "g \ta  b\t\tc";

            var obj = ReadObj(content);

            Assert.Equal(3, obj.Groups.Count);
            Assert.Equal("a", obj.Groups[0].Name);
            Assert.Equal("b", obj.Groups[1].Name);
            Assert.Equal("c", obj.Groups[2].Name);
        }

        [Fact]
        public void Parsing_Comment_Valid()
        {
            string content = "#\ng a #b";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Groups.Count);
            Assert.Equal("a", obj.Groups[0].Name);
        }

        [Fact]
        public void Parsing_MultilineComment_Valid()
        {
            string content = @"
# \
g a";

            var obj = ReadObj(content);

            Assert.Equal(0, obj.Groups.Count);
        }

        [Fact]
        public void Parsing_Multiline_Valid()
        {
            string content = @"
g \

a\
 b";

            var obj = ReadObj(content);

            Assert.Equal(2, obj.Groups.Count);
            Assert.Equal("a", obj.Groups[0].Name);
            Assert.Equal("b", obj.Groups[1].Name);
        }

        private ObjFile ReadObj(string content)
        {
            var buffer = Encoding.UTF8.GetBytes(content);

            using (var stream = new MemoryStream(buffer, false))
            {
                return ObjFile.FromStream(stream);
            }
        }
    }
}
