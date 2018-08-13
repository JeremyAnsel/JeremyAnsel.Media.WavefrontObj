// <copyright file="ObjVertex.cs" company="Jérémy Ansel">
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
    public struct ObjVertex
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
    }
}
