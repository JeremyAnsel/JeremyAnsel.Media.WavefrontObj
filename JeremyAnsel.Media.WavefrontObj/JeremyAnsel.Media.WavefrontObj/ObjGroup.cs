// <copyright file="ObjGroup.cs" company="Jérémy Ansel">
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
    public class ObjGroup
    {
        public ObjGroup()
        {
            this.Points = new List<ObjPoint>();
            this.Lines = new List<ObjLine>();
            this.Faces = new List<ObjFace>();
            this.Curves = new List<ObjCurve>();
            this.Curves2D = new List<ObjCurve2D>();
            this.Surfaces = new List<ObjSurface>();
        }

        public ObjGroup(string name)
            : this()
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public IList<ObjPoint> Points { get; private set; }

        public IList<ObjLine> Lines { get; private set; }

        public IList<ObjFace> Faces { get; private set; }

        public IList<ObjCurve> Curves { get; private set; }

        public IList<ObjCurve2D> Curves2D { get; private set; }

        public IList<ObjSurface> Surfaces { get; private set; }
    }
}
