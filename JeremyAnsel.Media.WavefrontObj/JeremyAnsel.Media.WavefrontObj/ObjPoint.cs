// <copyright file="ObjPoint.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

namespace JeremyAnsel.Media.WavefrontObj
{
    public class ObjPoint : ObjPolygonalElement
    {
        public ObjPoint()
        {
            this.Vertices = new List<ObjTriplet>();
        }

        public List<ObjTriplet> Vertices { get; private set; }
    }
}
