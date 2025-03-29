// <copyright file="ObjVector3.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Equatable.Attributes;

namespace JeremyAnsel.Media.WavefrontObj
{
    [System.Diagnostics.DebuggerDisplay("{X} {Y} {Z}")]
    [Equatable]
    public partial struct ObjVector3
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
