// <copyright file="ObjVector4.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
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
    [System.Diagnostics.DebuggerDisplay("{X} {Y} {Z} {W}")]
    public struct ObjVector4 : IEquatable<ObjVector4>
    {
        private float x;

        private float y;

        private float z;

        private float w;

        public ObjVector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public ObjVector4(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 1.0f;
        }

        public float X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public float Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        public float Z
        {
            get { return this.z; }
            set { this.z = value; }
        }

        public float W
        {
            get { return this.w; }
            set { this.w = value; }
        }

        public override bool Equals(object obj)
        {
            return obj is ObjVector4 vector && Equals(vector);
        }

        public bool Equals(ObjVector4 other)
        {
            return x == other.x &&
                   y == other.y &&
                   z == other.z &&
                   w == other.w;
        }

        public override int GetHashCode()
        {
            var hashCode = -1743314642;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            hashCode = hashCode * -1521134295 + w.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ObjVector4 left, ObjVector4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ObjVector4 left, ObjVector4 right)
        {
            return !(left == right);
        }
    }
}
