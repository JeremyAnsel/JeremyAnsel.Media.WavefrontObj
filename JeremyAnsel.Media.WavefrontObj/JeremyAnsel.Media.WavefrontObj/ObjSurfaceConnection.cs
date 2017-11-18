// <copyright file="ObjSurfaceConnection.cs" company="Jérémy Ansel">
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
    public class ObjSurfaceConnection
    {
        public int Surface1 { get; set; }

        public ObjCurveIndex Curve2D1 { get; set; }

        public int Surface2 { get; set; }

        public ObjCurveIndex Curve2D2 { get; set; }
    }
}
