// <copyright file="ObjMaterialTests.cs" company="Jérémy Ansel">
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
    public class ObjMaterialTests
    {
        [Fact]
        public void New()
        {
            var material = new ObjMaterial();

            Assert.Equal(2, material.IlluminationModel);
            Assert.Equal(1.0f, material.DissolveFactor);
            Assert.Equal(60, material.Sharpness);
            Assert.Equal(1.0f, material.OpticalDensity);
        }
    }
}
