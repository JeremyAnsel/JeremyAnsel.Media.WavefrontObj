// <copyright file="ObjVertex.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Equatable.Attributes;

namespace JeremyAnsel.Media.WavefrontObj
{
    [System.Diagnostics.DebuggerDisplay("Vertex Position:{Position} Color:{Color}")]
    [Equatable]
    public partial struct ObjVertex
    {
        public ObjVertex(float x, float y, float z)
        {
            Position = new ObjVector4(x, y, z, 1.0f);
            Color = null;
        }

        public ObjVertex(float x, float y, float z, float w)
        {
            Position = new ObjVector4(x, y, z, w);
            Color = null;
        }

        public ObjVertex(float x, float y, float z, float r, float g, float b)
        {
            Position = new ObjVector4(x, y, z, 1.0f);
            Color = new ObjVector4(r, g, b, 1.0f);
        }

        public ObjVertex(float x, float y, float z, float r, float g, float b, float a)
        {
            Position = new ObjVector4(x, y, z, 1.0f);
            Color = new ObjVector4(r, g, b, a);
        }

        public ObjVector4 Position { get; set; }

        public ObjVector4? Color { get; set; }
    }
}
