// <copyright file="ObjCurve.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

namespace JeremyAnsel.Media.WavefrontObj;

public class ObjCurve : ObjFreeFormElement
{
    public ObjCurve()
    {
        this.Vertices = new List<int>();
    }

    public float StartParameter { get; set; }

    public float EndParameter { get; set; }

    public List<int> Vertices { get; private set; }
}