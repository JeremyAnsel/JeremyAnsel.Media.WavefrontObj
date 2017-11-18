// <copyright file="ObjFileTests.cs" company="Jérémy Ansel">
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
    public class ObjFileTests
    {
        [Fact]
        public void Read_NullFile_Throws()
        {
            Assert.Throws<ArgumentNullException>("path", () => ObjFile.FromFile(null));
        }

        [Fact]
        public void Read_File_Valid()
        {
            var temp = Path.GetTempFileName();

            try
            {
                File.WriteAllText(temp, "shadow_obj a.a");

                var obj = ObjFile.FromFile(temp);

                Assert.Equal("a.a", obj.ShadowObjectFileName);
            }
            finally
            {
                File.Delete(temp);
            }
        }

        [Fact]
        public void Read_NullStream_Throws()
        {
            Assert.Throws<ArgumentNullException>("stream", () => ObjFile.FromStream(null));
        }

        [Fact]
        public void Read_Stream_Valid()
        {
            using (var stream = new MemoryStream())
            {
                string text = "shadow_obj a.a";
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                stream.Write(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);

                var obj = ObjFile.FromStream(stream);

                Assert.Equal("a.a", obj.ShadowObjectFileName);
            }
        }

        [Fact]
        public void Write_File_Valid()
        {
            var temp = Path.GetTempFileName();

            try
            {
                var obj = new ObjFile();

                obj.ShadowObjectFileName = "a.a";

                obj.WriteTo(temp);

                var text = File.ReadAllText(temp);
                string expected =
@"shadow_obj a.a
";

                AssertExtensions.TextEqual(expected, text);
            }
            finally
            {
                File.Delete(temp);
            }
        }

        [Fact]
        public void Write_NullStream_Throws()
        {
            var obj = new ObjFile();
            Assert.Throws<ArgumentNullException>("stream", () => obj.WriteTo((Stream)null));
        }

        [Fact]
        public void Write_Stream_Valid()
        {
            byte[] buffer;

            using (var stream = new MemoryStream())
            {
                var obj = new ObjFile();
                obj.ShadowObjectFileName = "a.a";

                obj.WriteTo(stream);

                buffer = stream.ToArray();
            }

            string text;

            using (var stream = new MemoryStream(buffer, false))
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            string expected =
@"shadow_obj a.a
";

            AssertExtensions.TextEqual(expected, text);
        }
    }
}
