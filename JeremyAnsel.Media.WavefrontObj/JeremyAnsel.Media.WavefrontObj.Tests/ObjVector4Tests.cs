// <copyright file="ObjVector4Tests.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

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

        [Fact]
        public void NewNumericsVector4()
        {
            ObjVector4 v = new System.Numerics.Vector4(2.0f, 3.0f, 4.0f, 5.0f);

            Assert.Equal(2.0f, v.X);
            Assert.Equal(3.0f, v.Y);
            Assert.Equal(4.0f, v.Z);
            Assert.Equal(5.0f, v.W);
        }

        [Fact]
        public void NewNumericsVector3()
        {
            var v = new ObjVector4(new System.Numerics.Vector3(2.0f, 3.0f, 4.0f), 5.0f);

            Assert.Equal(2.0f, v.X);
            Assert.Equal(3.0f, v.Y);
            Assert.Equal(4.0f, v.Z);
            Assert.Equal(5.0f, v.W);
        }

        [Fact]
        public void ToNumericsVector4()
        {
            var v = new ObjVector4(2.0f, 3.0f, 4.0f, 5.0f);
            var v4 = v.ToVector4();

            Assert.Equal(2.0f, v4.X);
            Assert.Equal(3.0f, v4.Y);
            Assert.Equal(4.0f, v4.Z);
            Assert.Equal(5.0f, v4.W);
        }

        [Fact]
        public void Deconstruct4()
        {
            (float x, float y, float z, float w) = new ObjVector4(2.0f, 3.0f, 4.0f, 5.0f);

            Assert.Equal(2.0f, x);
            Assert.Equal(3.0f, y);
            Assert.Equal(4.0f, z);
            Assert.Equal(5.0f, w);
        }

        [Fact]
        public void Equality()
        {
            var first = new ObjVector4(2.0f, 3.0f, 4.0f, 5.0f);
            var second = new ObjVector4(2.0f, 3.0f, 4.0f, 5.0f);
            var third = new ObjVector4(20.0f, 30.0f, 40.0f, 50.0f);

            SharpEqualityAssert.EqualityAssert.EqualityMembers(first, second, third);
        }
    }
}
