// <copyright file="ObjCurve2D.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

namespace JeremyAnsel.Media.WavefrontObj;

public class ObjCurve2D : ObjFreeFormElement
{
    public ObjCurve2D()
    {
        this.ParameterSpaceVertices = new List<int>();
    }

    public List<int> ParameterSpaceVertices { get; private set; }
}