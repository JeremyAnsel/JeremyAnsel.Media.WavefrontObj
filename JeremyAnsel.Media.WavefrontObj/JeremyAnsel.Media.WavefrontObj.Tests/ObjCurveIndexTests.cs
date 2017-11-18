// <copyright file="ObjCurveIndexTests.cs" company="Jérémy Ansel">
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
    public class ObjCurveIndexTests
    {
        [Fact]
        public void New()
        {
            var index = new ObjCurveIndex
            {
                Start = 2.0f,
                End = 3.0f,
                Curve2D = 4
            };

            Assert.Equal(2.0f, index.Start);
            Assert.Equal(3.0f, index.End);
            Assert.Equal(4, index.Curve2D);
        }

        [Fact]
        public void New3()
        {
            var index = new ObjCurveIndex(2.0f, 3.0f, 4);

            Assert.Equal(2.0f, index.Start);
            Assert.Equal(3.0f, index.End);
            Assert.Equal(4, index.Curve2D);
        }

        [Fact]
        public void String()
        {
            var index = new ObjCurveIndex(2.0f, 3.0f, 4);
            var text = index.ToString();

            string expected = "2.000000 3.000000 4";

            Assert.Equal(expected, text);
        }
    }
}
