// <copyright file="ObjPolygonalElement.cs" company="Jérémy Ansel">
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
    public abstract class ObjPolygonalElement : ObjElement
    {
        internal ObjPolygonalElement()
        {
        }

        public int SmoothingGroupNumber { get; set; }

        public bool IsBevelInterpolationEnabled { get; set; }

        public bool IsColorInterpolationEnabled { get; set; }

        public bool IsDissolveInterpolationEnabled { get; set; }
    }
}
