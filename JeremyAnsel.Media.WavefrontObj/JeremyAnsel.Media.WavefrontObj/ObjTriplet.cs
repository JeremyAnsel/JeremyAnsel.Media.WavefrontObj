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
    [System.Diagnostics.DebuggerDisplay("Vertex:{Vertex} Texture:{Texture} Normal:{Normal}")]
    [Equatable]
    public partial struct ObjTriplet
    {
        public ObjTriplet(int vertexIndex, int textureIndex, int normalIndex)
        {
            Vertex = vertexIndex;
            Texture = textureIndex;
            Normal = normalIndex;
        }

        public int Vertex { get; set; }

        public int Texture { get; set; }

        public int Normal { get; set; }

        public readonly override string ToString()
        {
            if (Normal == 0)
            {
                if (Texture == 0)
                {
                    return Vertex.ToString(CultureInfo.InvariantCulture);
                }

                return string.Concat(
                    Vertex.ToString(CultureInfo.InvariantCulture),
                    "/",
                    Texture.ToString(CultureInfo.InvariantCulture));
            }

            if (Texture == 0)
            {
                return string.Concat(
                    Vertex.ToString(CultureInfo.InvariantCulture),
                    "//",
                    Normal.ToString(CultureInfo.InvariantCulture));
            }

            return string.Concat(
                Vertex.ToString(CultureInfo.InvariantCulture),
                "/",
                Texture.ToString(CultureInfo.InvariantCulture),
                "/",
                Normal.ToString(CultureInfo.InvariantCulture));
        }
    }
}
