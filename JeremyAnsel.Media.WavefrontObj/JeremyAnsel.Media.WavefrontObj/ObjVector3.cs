// <copyright file="ObjVector3.cs" company="Jérémy Ansel">
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
    public struct ObjVector3 : IEquatable<ObjVector3>
    {
        private float x;

        private float y;

        private float z;

        public ObjVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public ObjVector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 1.0f;
        }

        public ObjVector3(float x)
        {
            this.x = x;
            this.y = 0.0f;
            this.z = 1.0f;
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

        public override bool Equals(object obj)
        {
            return obj is ObjVector3 vector && Equals(vector);
        }

        public bool Equals(ObjVector3 other)
        {
            return x == other.x &&
                   y == other.y &&
                   z == other.z;
        }

        public override int GetHashCode()
        {
            var hashCode = 373119288;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ObjVector3 left, ObjVector3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ObjVector3 left, ObjVector3 right)
        {
            return !(left == right);
        }
    }
}
