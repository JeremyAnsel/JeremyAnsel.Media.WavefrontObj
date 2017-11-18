// <copyright file="ObjVector3Tests.cs" company="Jérémy Ansel">
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
    public class ObjVector3Tests
    {
        [Fact]
        public void New()
        {
            var v = new ObjVector3
            {
                X = 2.0f,
                Y = 3.0f,
                Z = 4.0f
            };

            Assert.Equal(2.0f, v.X);
            Assert.Equal(3.0f, v.Y);
            Assert.Equal(4.0f, v.Z);
        }

        [Fact]
        public void New3()
        {
            var v = new ObjVector3(2.0f, 3.0f, 4.0f);

            Assert.Equal(2.0f, v.X);
            Assert.Equal(3.0f, v.Y);
            Assert.Equal(4.0f, v.Z);
        }

        [Fact]
        public void New2()
        {
            var v = new ObjVector3(2.0f, 3.0f);

            Assert.Equal(2.0f, v.X);
            Assert.Equal(3.0f, v.Y);
            Assert.Equal(1.0f, v.Z);
        }

        [Fact]
        public void New1()
        {
            var v = new ObjVector3(2.0f);

            Assert.Equal(2.0f, v.X);
            Assert.Equal(0.0f, v.Y);
            Assert.Equal(1.0f, v.Z);
        }
    }
}
