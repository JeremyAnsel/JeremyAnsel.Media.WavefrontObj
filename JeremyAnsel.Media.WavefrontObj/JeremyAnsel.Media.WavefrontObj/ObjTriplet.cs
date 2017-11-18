// <copyright file="ObjTriplet.cs" company="Jérémy Ansel">
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
    public struct ObjTriplet
    {
        private int vertex;

        private int texture;

        private int normal;

        public ObjTriplet(int vertexIndex, int textureIndex, int normalIndex)
        {
            this.vertex = vertexIndex;
            this.texture = textureIndex;
            this.normal = normalIndex;
        }

        public int Vertex
        {
            get { return this.vertex; }
            set { this.vertex = value; }
        }

        public int Texture
        {
            get { return this.texture; }
            set { this.texture = value; }
        }

        public int Normal
        {
            get { return this.normal; }
            set { this.normal = value; }
        }

        public override string ToString()
        {
            if (this.Normal == 0)
            {
                if (this.Texture == 0)
                {
                    return this.Vertex.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    return string.Concat(
                        this.Vertex.ToString(CultureInfo.InvariantCulture),
                        "/",
                        this.Texture.ToString(CultureInfo.InvariantCulture));
                }
            }
            else
            {
                if (this.Texture == 0)
                {
                    return string.Concat(
                        this.Vertex.ToString(CultureInfo.InvariantCulture),
                        "//",
                        this.Normal.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    return string.Concat(
                        this.Vertex.ToString(CultureInfo.InvariantCulture),
                        "/",
                        this.Texture.ToString(CultureInfo.InvariantCulture),
                        "/",
                        this.Normal.ToString(CultureInfo.InvariantCulture));
                }
            }
        }
    }
}
