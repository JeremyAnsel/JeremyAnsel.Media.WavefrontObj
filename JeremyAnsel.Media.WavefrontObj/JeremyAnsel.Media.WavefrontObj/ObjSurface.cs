// <copyright file="ObjSurface.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

namespace JeremyAnsel.Media.WavefrontObj
{
    public class ObjSurface : ObjFreeFormElement
    {
        public ObjSurface()
        {
            this.Vertices = new List<ObjTriplet>();
        }

        public float StartParameterU { get; set; }

        public float EndParameterU { get; set; }

        public float StartParameterV { get; set; }

        public float EndParameterV { get; set; }

        public List<ObjTriplet> Vertices { get; private set; }
    }
}
