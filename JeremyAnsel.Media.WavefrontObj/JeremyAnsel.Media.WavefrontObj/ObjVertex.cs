// <copyright file="ObjVertex.cs" company="Jérémy Ansel">
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
    [System.Diagnostics.DebuggerDisplay("Vertex Position:{Position} Color:{Color}")]
    public struct ObjVertex : IEquatable<ObjVertex>
    {
        private ObjVector4 position;

        private ObjVector4? color;

        public ObjVertex(float x, float y, float z)
        {
            this.position = new ObjVector4(x, y, z, 1.0f);
            this.color = null;
        }

        public ObjVertex(float x, float y, float z, float w)
        {
            this.position = new ObjVector4(x, y, z, w);
            this.color = null;
        }

        public ObjVertex(float x, float y, float z, float r, float g, float b)
        {
            this.position = new ObjVector4(x, y, z, 1.0f);
            this.color = new ObjVector4(r, g, b, 1.0f);
        }

        public ObjVertex(float x, float y, float z, float r, float g, float b, float a)
        {
            this.position = new ObjVector4(x, y, z, 1.0f);
            this.color = new ObjVector4(r, g, b, a);
        }

        public ObjVector4 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public ObjVector4? Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        public override bool Equals(object obj)
        {
            return obj is ObjVertex vertex && Equals(vertex);
        }

        public bool Equals(ObjVertex other)
        {
            return position.Equals(other.position) &&
                   EqualityComparer<ObjVector4?>.Default.Equals(color, other.color);
        }

        public override int GetHashCode()
        {
            var hashCode = -2056440846;
            hashCode = hashCode * -1521134295 + EqualityComparer<ObjVector4>.Default.GetHashCode(position);
            hashCode = hashCode * -1521134295 + EqualityComparer<ObjVector4?>.Default.GetHashCode(color);
            return hashCode;
        }

        public static bool operator ==(ObjVertex left, ObjVertex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ObjVertex left, ObjVertex right)
        {
            return !(left == right);
        }
    }
}
