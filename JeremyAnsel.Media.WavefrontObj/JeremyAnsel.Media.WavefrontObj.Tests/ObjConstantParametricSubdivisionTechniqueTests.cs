// <copyright file="ObjConstantParametricSubdivisionTechniqueTests.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Xunit;

namespace JeremyAnsel.Media.WavefrontObj.Tests;

public class ObjConstantParametricSubdivisionTechniqueTests
{
    [Fact]
    public void New()
    {
        var technique = new ObjConstantParametricSubdivisionTechnique
        {
            ResolutionU = 2.0f,
            ResolutionV = 3.0f
        };

        Assert.Equal(2.0f, technique.ResolutionU);
        Assert.Equal(3.0f, technique.ResolutionV);
    }

    [Fact]
    public void New1()
    {
        var technique = new ObjConstantParametricSubdivisionTechnique(2.0f);

        Assert.Equal(2.0f, technique.ResolutionU);
        Assert.Equal(2.0f, technique.ResolutionV);
    }

    [Fact]
    public void New2()
    {
        var technique = new ObjConstantParametricSubdivisionTechnique(2.0f, 3.0f);

        Assert.Equal(2.0f, technique.ResolutionU);
        Assert.Equal(3.0f, technique.ResolutionV);
    }
}