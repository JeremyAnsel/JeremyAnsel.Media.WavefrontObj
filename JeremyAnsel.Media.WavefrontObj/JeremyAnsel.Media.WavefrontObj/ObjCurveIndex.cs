// <copyright file="ObjCurveIndex.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
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
    public struct ObjCurveIndex : IEquatable<ObjCurveIndex>
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

        public override bool Equals(object obj)
        {
            return obj is ObjCurveIndex index && Equals(index);
        }

        public bool Equals(ObjCurveIndex other)
        {
            return start == other.start &&
                   end == other.end &&
                   curve2D == other.curve2D;
        }

        public override int GetHashCode()
        {
            var hashCode = -540012629;
            hashCode = hashCode * -1521134295 + start.GetHashCode();
            hashCode = hashCode * -1521134295 + end.GetHashCode();
            hashCode = hashCode * -1521134295 + curve2D.GetHashCode();
            return hashCode;
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

        public static bool operator ==(ObjCurveIndex left, ObjCurveIndex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ObjCurveIndex left, ObjCurveIndex right)
        {
            return !(left == right);
        }
    }
}
