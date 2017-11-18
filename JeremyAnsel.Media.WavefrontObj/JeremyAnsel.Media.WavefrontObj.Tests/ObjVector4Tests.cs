// <copyright file="ObjVector4Tests.cs" company="Jérémy Ansel">
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
    public class ObjVector4Tests
    {
        [Fact]
        public void New()
        {
            var v = new ObjVector4
            {
                X = 2.0f,
                Y = 3.0f,
                Z = 4.0f,
                W = 5.0f
            };

            Assert.Equal(2.0f, v.X);
            Assert.Equal(3.0f, v.Y);
            Assert.Equal(4.0f, v.Z);
            Assert.Equal(5.0f, v.W);
        }

        [Fact]
        public void New4()
        {
            var v = new ObjVector4(2.0f, 3.0f, 4.0f, 5.0f);

            Assert.Equal(2.0f, v.X);
            Assert.Equal(3.0f, v.Y);
            Assert.Equal(4.0f, v.Z);
            Assert.Equal(5.0f, v.W);
        }

        [Fact]
        public void New3()
        {
            var v = new ObjVector4(2.0f, 3.0f, 4.0f);

            Assert.Equal(2.0f, v.X);
            Assert.Equal(3.0f, v.Y);
            Assert.Equal(4.0f, v.Z);
            Assert.Equal(1.0f, v.W);
        }
    }
}
