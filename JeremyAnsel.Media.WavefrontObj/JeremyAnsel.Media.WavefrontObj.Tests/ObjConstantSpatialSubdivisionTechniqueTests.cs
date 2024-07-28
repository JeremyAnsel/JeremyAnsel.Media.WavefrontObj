﻿// <copyright file="ObjConstantSpatialSubdivisionTechniqueTests.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Xunit;

namespace JeremyAnsel.Media.WavefrontObj.Tests
{
    public class ObjConstantSpatialSubdivisionTechniqueTests
    {
        [Fact]
        public void New()
        {
            var technique = new ObjConstantSpatialSubdivisionTechnique
            {
                MaximumLength = 2.0f
            };

            Assert.Equal(2.0f, technique.MaximumLength);
        }

        [Fact]
        public void New1()
        {
            var technique = new ObjConstantSpatialSubdivisionTechnique(2.0f);

            Assert.Equal(2.0f, technique.MaximumLength);
        }
    }
}
