// <copyright file="ObjMaterialColorTests.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System.Reflection;
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

        [Fact]
        public void ToDebuggerDisplayString_Spectral_Valid()
        {
            var color = new ObjMaterialColor
            {
                SpectralFactor = 2.0f,
                SpectralFileName = "a.a"
            };

            string s = (string)typeof(ObjMaterialColor).GetTypeInfo().GetDeclaredMethod("ToDebuggerDisplayString")!.Invoke(color, null)!;

            Assert.Equal($"Spectral:a.a Factor:{2.0f}", s);
        }

        [Fact]
        public void ToDebuggerDisplayString_XYZColorSpace_Valid()
        {
            var color = new ObjMaterialColor(2, 3, 4)
            {
                UseXYZColorSpace = true
            };

            string s = (string)typeof(ObjMaterialColor).GetTypeInfo().GetDeclaredMethod("ToDebuggerDisplayString")!.Invoke(color, null)!;

            Assert.Equal($"X:{2.0f} Y:{3.0f} Z:{4.0f}", s);
        }

        [Fact]
        public void ToDebuggerDisplayString_RGBColorSpace_Valid()
        {
            var color = new ObjMaterialColor(2, 3, 4);

            string s = (string)typeof(ObjMaterialColor).GetTypeInfo().GetDeclaredMethod("ToDebuggerDisplayString")!.Invoke(color, null)!;

            Assert.Equal($"R:{2.0f} G:{3.0f} B:{4.0f}", s);
        }
    }
}
