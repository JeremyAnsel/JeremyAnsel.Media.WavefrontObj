// <copyright file="ObjCurvatureDependentSubdivisionTechniqueTests.cs" company="Jérémy Ansel">
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
    public class ObjCurvatureDependentSubdivisionTechniqueTests
    {
        [Fact]
        public void New()
        {
            var technique = new ObjCurvatureDependentSubdivisionTechnique
            {
                MaximumDistance = 2.0f,
                MaximumAngle = 3.0f
            };

            Assert.Equal(2.0f, technique.MaximumDistance);
            Assert.Equal(3.0f, technique.MaximumAngle);
        }

        [Fact]
        public void New2()
        {
            var technique = new ObjCurvatureDependentSubdivisionTechnique(2.0f, 3.0f);

            Assert.Equal(2.0f, technique.MaximumDistance);
            Assert.Equal(3.0f, technique.MaximumAngle);
        }
    }
}
