// <copyright file="ObjMaterialTests.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Xunit;

namespace JeremyAnsel.Media.WavefrontObj.Tests;

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