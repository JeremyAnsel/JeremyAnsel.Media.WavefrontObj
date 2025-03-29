// <copyright file="ObjTriplet.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Equatable.Attributes;
using System.Globalization;

namespace JeremyAnsel.Media.WavefrontObj
{
    [System.Diagnostics.DebuggerDisplay("Vertex:{vertex} Texture:{texture} Normal:{normal}")]
    [Equatable]
    public partial struct ObjTriplet
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
            readonly get { return this.vertex; }
            set { this.vertex = value; }
        }

        public int Texture
        {
            readonly get { return this.texture; }
            set { this.texture = value; }
        }

        public int Normal
        {
            readonly get { return this.normal; }
            set { this.normal = value; }
        }

        public readonly override string ToString()
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
