// <copyright file="ObjCurveIndex.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Equatable.Attributes;
using System.Globalization;

namespace JeremyAnsel.Media.WavefrontObj
{
    [System.Diagnostics.DebuggerDisplay("Curve Index:{Curve2D} Start:{Start} End:{End}")]
    [Equatable]
    public partial struct ObjCurveIndex
    {
        public ObjCurveIndex(float startParameter, float endParameter, int curve2DIndex)
        {
            Start = startParameter;
            End = endParameter;
            Curve2D = curve2DIndex;
        }

        public float Start { get; set; }

        public float End { get; set; }

        public int Curve2D { get; set; }

        public override string ToString() => string.Concat(
            Start.ToString("F6", CultureInfo.InvariantCulture),
            " ",
            End.ToString("F6", CultureInfo.InvariantCulture),
            " ",
            Curve2D.ToString(CultureInfo.InvariantCulture));
    }
}