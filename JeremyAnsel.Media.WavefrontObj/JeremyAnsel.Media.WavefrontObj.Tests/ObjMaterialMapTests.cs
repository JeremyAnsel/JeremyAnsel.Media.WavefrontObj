// <copyright file="ObjMaterialMapTests.cs" company="Jérémy Ansel">
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
    public class ObjMaterialMapTests
    {
        [Fact]
        public void New()
        {
            var map = new ObjMaterialMap();

            Assert.True(map.IsHorizontalBlendingEnabled);
            Assert.True(map.IsVerticalBlendingEnabled);
            Assert.Equal(ObjMapChannel.Luminance, map.ScalarChannel);
            Assert.Equal(0.0f, map.ModifierBase);
            Assert.Equal(1.0f, map.ModifierGain);
            Assert.Equal(new ObjVector3(0.0f, 0.0f, 0.0f), map.Offset);
            Assert.Equal(new ObjVector3(1.0f, 1.0f, 1.0f), map.Scale);
            Assert.Equal(new ObjVector3(0.0f, 0.0f, 0.0f), map.Turbulence);
        }
    }
}
