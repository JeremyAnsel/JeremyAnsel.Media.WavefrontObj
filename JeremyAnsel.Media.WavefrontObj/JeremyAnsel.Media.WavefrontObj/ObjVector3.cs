// <copyright file="ObjVector3.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

namespace JeremyAnsel.Media.WavefrontObj
{
    [System.Diagnostics.DebuggerDisplay("{X} {Y} {Z}")]
    public struct ObjVector3 : IEquatable<ObjVector3>
    {
        private float x;

        private float y;

        private float z;        

        public ObjVector3(System.Numerics.Vector3 v)
        {
            this.x = v.X;
            this.y = v.Y;
            this.z = v.Z;
        }

        public ObjVector3(System.Numerics.Vector2 v, float z = 1.0f)
        {
            this.x = v.X;
            this.y = v.Y;
            this.z = z;
        }

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
            readonly get { return this.x; }
            set { this.x = value; }
        }

        public float Y
        {
            readonly get { return this.y; }
            set { this.y = value; }
        }

        public float Z
        {
            readonly get { return this.z; }
            set { this.z = value; }
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ObjVector3 vector && Equals(vector);
        }

        public readonly bool Equals(ObjVector3 other)
        {
            return x == other.x &&
                   y == other.y &&
                   z == other.z;
        }

        public readonly override int GetHashCode()
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

        public static implicit operator ObjVector3(System.Numerics.Vector3 v)
        {
            return new ObjVector3(v);
        }

        public readonly void Deconstruct(out float @x, out float @y, out float @z)
        {
            @x = this.x;
            @y = this.y;
            @z = this.z;            
        }

        public readonly System.Numerics.Vector3 ToVector3()
        {
            return new System.Numerics.Vector3(x, y, z);
        }
    }
}
