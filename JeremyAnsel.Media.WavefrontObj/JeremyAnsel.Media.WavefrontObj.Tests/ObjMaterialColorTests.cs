// <copyright file="ObjMaterialColorTests.cs" company="Jérémy Ansel">
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
    public class ObjMaterialColorTests
    {
        [Fact]
        public void New()
        {
            var color = new ObjMaterialColor();

            Assert.Equal(1.0f, color.SpectralFactor);
            Assert.False(color.IsSpectral);
            Assert.True(color.IsRGB);
            Assert.False(color.IsXYZ);
        }

        [Fact]
        public void IsSpectral_Valid()
        {
            var color = new ObjMaterialColor();
            color.SpectralFileName = "a.a";
            color.SpectralFactor = 2.0f;

            Assert.Equal(2.0f, color.SpectralFactor);
            Assert.True(color.IsSpectral);
            Assert.False(color.IsRGB);
            Assert.False(color.IsXYZ);
        }

        [Fact]
        public void IsRGB_Valid()
        {
            var color = new ObjMaterialColor();

            Assert.False(color.IsSpectral);
            Assert.True(color.IsRGB);
            Assert.False(color.IsXYZ);
        }

        [Fact]
        public void IsXYZ_Valid()
        {
            var color = new ObjMaterialColor();
            color.UseXYZColorSpace = true;

            Assert.False(color.IsSpectral);
            Assert.False(color.IsRGB);
            Assert.True(color.IsXYZ);
        }
    }
}
