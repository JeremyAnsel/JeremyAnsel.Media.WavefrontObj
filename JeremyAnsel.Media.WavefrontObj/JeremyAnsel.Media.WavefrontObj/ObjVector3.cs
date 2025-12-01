// <copyright file="ObjVector3.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Equatable.Attributes;

namespace JeremyAnsel.Media.WavefrontObj;

[System.Diagnostics.DebuggerDisplay("{X} {Y} {Z}")]
[Equatable]
public partial struct ObjVector3
{
    public ObjVector3(System.Numerics.Vector3 v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
    }

    public ObjVector3(System.Numerics.Vector2 v, float z = 1.0f)
    {
        X = v.X;
        Y = v.Y;
        Z = z;
    }

    public ObjVector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public ObjVector3(float x, float y)
    {
        X = x;
        Y = y;
        Z = 1.0f;
    }

    public ObjVector3(float x)
    {
        X = x;
        Y = 0.0f;
        Z = 1.0f;
    }

    public float X { get; set; }

    public float Y { get; set; }

    public float Z { get; set; }

    public static implicit operator ObjVector3(System.Numerics.Vector3 v)
    {
        return new ObjVector3(v);
    }

    public readonly void Deconstruct(out float @x, out float @y, out float @z)
    {
        @x = X;
        @y = Y;
        @z = Z;            
    }

    public readonly System.Numerics.Vector3 ToVector3()
    {
        return new System.Numerics.Vector3(X, Y, Z);
    }
}