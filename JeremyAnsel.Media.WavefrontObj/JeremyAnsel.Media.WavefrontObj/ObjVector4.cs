// <copyright file="ObjVector4.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Equatable.Attributes;

namespace JeremyAnsel.Media.WavefrontObj;

[System.Diagnostics.DebuggerDisplay("{X} {Y} {Z} {W}")]
[Equatable]
public partial struct ObjVector4
{
    public ObjVector4(System.Numerics.Vector4 v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
        W = v.W;
    }

    public ObjVector4(System.Numerics.Vector3 v, float w = 1.0f)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
        W = w;
    }

    public ObjVector4(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public ObjVector4(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
        W = 1.0f;
    }

    public float X { get; set; }

    public float Y { get; set; }

    public float Z { get; set; }

    public float W { get; set; }

    public static implicit operator ObjVector4(System.Numerics.Vector4 v)
    {
        return new ObjVector4(v);
    }

    public readonly void Deconstruct(out float @x, out float @y, out float @z, out float @w)
    {
        @x = X;
        @y = Y;
        @z = Z;
        @w = W;
    }

    public readonly System.Numerics.Vector4 ToVector4()
    {
        return new System.Numerics.Vector4(X, Y, Z, W);
    }
}