// <copyright file="ObjPoint.cs" company="Jérémy Ansel">
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
    public class ObjPoint : ObjPolygonalElement
    {
        public ObjPoint()
        {
            this.Vertices = new List<ObjTriplet>();
        }

        public IList<ObjTriplet> Vertices { get; private set; }
    }
}
