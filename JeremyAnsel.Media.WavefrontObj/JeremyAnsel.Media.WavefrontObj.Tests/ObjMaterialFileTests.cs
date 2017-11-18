// <copyright file="ObjMaterialFileTests.cs" company="Jérémy Ansel">
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
    public class ObjMaterialFileTests
    {
        [Fact]
        public void Read_NullFile_Throws()
        {
            Assert.Throws<ArgumentNullException>("path", () => ObjMaterialFile.FromFile(null));
        }

        [Fact]
        public void Read_File_Valid()
        {
            var temp = Path.GetTempFileName();

            try
            {
                File.WriteAllText(temp, "newmtl a");

                var mtl = ObjMaterialFile.FromFile(temp);

                Assert.Equal(1, mtl.Materials.Count);
                Assert.Equal("a", mtl.Materials[0].Name);
            }
            finally
            {
                File.Delete(temp);
            }
        }

        [Fact]
        public void Read_NullStream_Throws()
        {
            Assert.Throws<ArgumentNullException>("stream", () => ObjMaterialFile.FromStream(null));
        }

        [Fact]
        public void Read_Stream_Valid()
        {
            using (var stream = new MemoryStream())
            {
                string text = "newmtl a";
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                stream.Write(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);

                var mtl = ObjMaterialFile.FromStream(stream);

                Assert.Equal("a", mtl.Materials[0].Name);
            }
        }

        [Fact]
        public void Write_File_Valid()
        {
            var temp = Path.GetTempFileName();

            try
            {
                var mtl = new ObjMaterialFile();
                mtl.Materials.Add(new ObjMaterial("a"));

                mtl.WriteTo(temp);

                var text = File.ReadAllText(temp);
                string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
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
            var mtl = new ObjMaterialFile();
            Assert.Throws<ArgumentNullException>("stream", () => mtl.WriteTo((Stream)null));
        }

        [Fact]
        public void Write_Stream_Valid()
        {
            byte[] buffer;

            using (var stream = new MemoryStream())
            {
                var mtl = new ObjMaterialFile();
                mtl.Materials.Add(new ObjMaterial("a"));

                mtl.WriteTo(stream);

                buffer = stream.ToArray();
            }

            string text;

            using (var stream = new MemoryStream(buffer, false))
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }
    }
}
