// <copyright file="ObjVector4.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Equatable.Attributes;

namespace JeremyAnsel.Media.WavefrontObj
{
    [System.Diagnostics.DebuggerDisplay("{X} {Y} {Z} {W}")]
    [Equatable]
    public partial struct ObjVector4
    {
        private float x;

        private float y;

        private float z;

        private float w;        

        public ObjVector4(System.Numerics.Vector4 v)
        {
            this.x = v.X;
            this.y = v.Y;
            this.z = v.Z;
            this.w = v.W;
        }

        public ObjVector4(System.Numerics.Vector3 v, float w = 1.0f)
        {
            this.x = v.X;
            this.y = v.Y;
            this.z = v.Z;
            this.w = w;
        }

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

        public float W
        {
            readonly get { return this.w; }
            set { this.w = value; }
        }

        public static implicit operator ObjVector4(System.Numerics.Vector4 v)
        {
            return new ObjVector4(v);
        }

        public readonly void Deconstruct(out float @x, out float @y, out float @z, out float @w)
        {
            @x = this.x;
            @y = this.y;
            @z = this.z;
            @w = this.w;
        }

        public readonly System.Numerics.Vector4 ToVector4()
        {
            return new System.Numerics.Vector4(x, y, z, w);
        }
    }
}
