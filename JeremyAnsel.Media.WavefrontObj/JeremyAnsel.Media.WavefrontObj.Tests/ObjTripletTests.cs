// <copyright file="ObjTripletTests.cs" company="Jérémy Ansel">
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
    public class ObjTripletTests
    {
        [Fact]
        public void New()
        {
            var triplet = new ObjTriplet
            {
                Vertex = 2,
                Texture = 3,
                Normal = 4
            };

            Assert.Equal(2, triplet.Vertex);
            Assert.Equal(3, triplet.Texture);
            Assert.Equal(4, triplet.Normal);
        }

        [Fact]
        public void New3()
        {
            var triplet = new ObjTriplet(2, 3, 4);

            Assert.Equal(2, triplet.Vertex);
            Assert.Equal(3, triplet.Texture);
            Assert.Equal(4, triplet.Normal);
        }

        [Fact]
        public void String_V()
        {
            var triplet = new ObjTriplet(2, 0, 0);
            var text = triplet.ToString();

            string expected = "2";

            Assert.Equal(expected, text);
        }

        [Fact]
        public void String_VT()
        {
            var triplet = new ObjTriplet(2, 3, 0);
            var text = triplet.ToString();

            string expected = "2/3";

            Assert.Equal(expected, text);
        }

        [Fact]
        public void String_VN()
        {
            var triplet = new ObjTriplet(2, 0, 4);
            var text = triplet.ToString();

            string expected = "2//4";

            Assert.Equal(expected, text);
        }

        [Fact]
        public void String_VTN()
        {
            var triplet = new ObjTriplet(2, 3, 4);
            var text = triplet.ToString();

            string expected = "2/3/4";

            Assert.Equal(expected, text);
        }
    }
}
