// <copyright file="ObjCurveIndex.cs" company="Jérémy Ansel">
// Copyright (c) 2017 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace JeremyAnsel.Media.WavefrontObj
{
    public struct ObjCurveIndex
    {
        private float start;

        private float end;

        private int curve2D;

        public ObjCurveIndex(float startParameter, float endParameter, int curve2DIndex)
        {
            this.start = startParameter;
            this.end = endParameter;
            this.curve2D = curve2DIndex;
        }

        public float Start
        {
            get { return this.start; }
            set { this.start = value; }
        }

        public float End
        {
            get { return this.end; }
            set { this.end = value; }
        }

        public int Curve2D
        {
            get { return this.curve2D; }
            set { this.curve2D = value; }
        }

        public override string ToString()
        {
            return string.Concat(
                this.Start.ToString("F6", CultureInfo.InvariantCulture),
                " ",
                this.End.ToString("F6", CultureInfo.InvariantCulture),
                " ",
                this.Curve2D.ToString(CultureInfo.InvariantCulture));
        }
    }
}
