// <copyright file="ObjVector3Tests.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Xunit;

namespace JeremyAnsel.Media.WavefrontObj.Tests;

public class ObjVector3Tests
{
    [Fact]
    public void New()
    {
        var v = new ObjVector3
        {
            X = 2.0f,
            Y = 3.0f,
            Z = 4.0f
        };

        Assert.Equal(2.0f, v.X);
        Assert.Equal(3.0f, v.Y);
        Assert.Equal(4.0f, v.Z);
    }

    [Fact]
    public void New3()
    {
        var v = new ObjVector3(2.0f, 3.0f, 4.0f);

        Assert.Equal(2.0f, v.X);
        Assert.Equal(3.0f, v.Y);
        Assert.Equal(4.0f, v.Z);
    }

    [Fact]
    public void New2()
    {
        var v = new ObjVector3(2.0f, 3.0f);

        Assert.Equal(2.0f, v.X);
        Assert.Equal(3.0f, v.Y);
        Assert.Equal(1.0f, v.Z);
    }

    [Fact]
    public void New1()
    {
        var v = new ObjVector3(2.0f);

        Assert.Equal(2.0f, v.X);
        Assert.Equal(0.0f, v.Y);
        Assert.Equal(1.0f, v.Z);
    }

    [Fact]
    public void NewNumericsVector3()
    {
        ObjVector3 v = new System.Numerics.Vector3(2.0f, 3.0f, 4.0f);

        Assert.Equal(2.0f, v.X);
        Assert.Equal(3.0f, v.Y);
        Assert.Equal(4.0f, v.Z);
    }

    [Fact]
    public void NewNumericsVector2()
    {
        var v = new ObjVector3(new System.Numerics.Vector2(2.0f, 3.0f), 4.0f);

        Assert.Equal(2.0f, v.X);
        Assert.Equal(3.0f, v.Y);
        Assert.Equal(4.0f, v.Z);
    }

    [Fact]
    public void ToNumericsVector3()
    {
        var v = new ObjVector3(2.0f, 3.0f, 4.0f);
        var v3 = v.ToVector3();

        Assert.Equal(2.0f, v3.X);
        Assert.Equal(3.0f, v3.Y);
        Assert.Equal(4.0f, v3.Z);
    }

    [Fact]
    public void Deconstruct3()
    {
        (float x, float y, float z) = new ObjVector3(2.0f, 3.0f, 4.0f);

        Assert.Equal(2.0f, x);
        Assert.Equal(3.0f, y);
        Assert.Equal(4.0f, z);
    }

    [Fact]
    public void Equality()
    {
        var first = new ObjVector3(2.0f, 3.0f, 4.0f);
        var second = new ObjVector3(2.0f, 3.0f, 4.0f);
        var third = new ObjVector3(20.0f, 30.0f, 40.0f);

        SharpEqualityAssert.EqualityAssert.EqualityMembers(first, second, third);
    }
}