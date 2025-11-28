// <copyright file="ObjFace.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

namespace JeremyAnsel.Media.WavefrontObj;

public class ObjFace : ObjPolygonalElement
{
    public ObjFace()
    {
        this.Vertices = new List<ObjTriplet>();
    }

    public List<ObjTriplet> Vertices { get; private set; }
}