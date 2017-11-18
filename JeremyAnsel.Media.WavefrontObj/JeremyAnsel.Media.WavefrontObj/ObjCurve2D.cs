// <copyright file="ObjCurve2D.cs" company="Jérémy Ansel">
// Copyright (c) 2017 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JeremyAnsel.Media.WavefrontObj
{
    public class ObjCurve2D : ObjFreeFormElement
    {
        public ObjCurve2D()
        {
            this.ParameterSpaceVertices = new List<int>();
        }

        public IList<int> ParameterSpaceVertices { get; private set; }
    }
}
