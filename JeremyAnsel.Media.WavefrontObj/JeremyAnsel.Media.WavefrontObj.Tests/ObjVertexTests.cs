// <copyright file="ObjVertexTests.cs" company="Jérémy Ansel">
// Copyright (c) 2017 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace JeremyAnsel.Media.WavefrontObj.Tests
{
    public class ObjVertexTests
    {
        [Fact]
        public void New()
        {
            var v = new ObjVertex
            {
                Position = new ObjVector4(2.0f, 3.0f, 4.0f, 5.0f),
                Color = new ObjVector4(6.0f, 7.0f, 8.0f, 9.0f)
            };

            Assert.Equal(new ObjVector4(2.0f, 3.0f, 4.0f, 5.0f), v.Position);
            Assert.Equal(new ObjVector4(6.0f, 7.0f, 8.0f, 9.0f), v.Color);
        }

        [Fact]
        public void New3()
        {
            var v = new ObjVertex(2.0f, 3.0f, 4.0f);

            Assert.Equal(2.0f, v.Position.X);
            Assert.Equal(3.0f, v.Position.Y);
            Assert.Equal(4.0f, v.Position.Z);
            Assert.Equal(1.0f, v.Position.W);
            Assert.Null(v.Color);
        }

        [Fact]
        public void New4()
        {
            var v = new ObjVertex(2.0f, 3.0f, 4.0f, 5.0f);

            Assert.Equal(2.0f, v.Position.X);
            Assert.Equal(3.0f, v.Position.Y);
            Assert.Equal(4.0f, v.Position.Z);
            Assert.Equal(5.0f, v.Position.W);
            Assert.Null(v.Color);
        }

        [Fact]
        public void New6()
        {
            var v = new ObjVertex(2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f);

            Assert.Equal(2.0f, v.Position.X);
            Assert.Equal(3.0f, v.Position.Y);
            Assert.Equal(4.0f, v.Position.Z);
            Assert.Equal(1.0f, v.Position.W);
            Assert.Equal(5.0f, v.Color.Value.X);
            Assert.Equal(6.0f, v.Color.Value.Y);
            Assert.Equal(7.0f, v.Color.Value.Z);
            Assert.Equal(1.0f, v.Color.Value.W);
        }

        [Fact]
        public void New7()
        {
            var v = new ObjVertex(2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f);

            Assert.Equal(2.0f, v.Position.X);
            Assert.Equal(3.0f, v.Position.Y);
            Assert.Equal(4.0f, v.Position.Z);
            Assert.Equal(1.0f, v.Position.W);
            Assert.Equal(5.0f, v.Color.Value.X);
            Assert.Equal(6.0f, v.Color.Value.Y);
            Assert.Equal(7.0f, v.Color.Value.Z);
            Assert.Equal(8.0f, v.Color.Value.W);
        }
    }
}
