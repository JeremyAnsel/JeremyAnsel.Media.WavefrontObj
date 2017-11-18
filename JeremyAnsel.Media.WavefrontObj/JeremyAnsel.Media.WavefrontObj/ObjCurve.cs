// <copyright file="ObjCurve.cs" company="Jérémy Ansel">
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
    public class ObjCurve : ObjFreeFormElement
    {
        public ObjCurve()
        {
            this.Vertices = new List<int>();
        }

        public float StartParameter { get; set; }

        public float EndParameter { get; set; }

        public IList<int> Vertices { get; private set; }
    }
}
