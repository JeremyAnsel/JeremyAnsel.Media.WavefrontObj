// <copyright file="ObjMaterialColor.cs" company="Jérémy Ansel">
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
    public class ObjMaterialColor
    {
        public ObjMaterialColor()
        {
            this.SpectralFactor = 1.0f;
        }

        public ObjMaterialColor(float x, float y, float z)
            : this()
        {
            this.Color = new ObjVector3(x, y, z);
        }

        public ObjMaterialColor(string spectralFileName)
            : this()
        {
            this.SpectralFileName = spectralFileName;
        }

        public ObjMaterialColor(string spectralFileName, float factor)
            : this()
        {
            this.SpectralFileName = spectralFileName;
            this.SpectralFactor = factor;
        }

        public ObjVector3 Color { get; set; }

        public bool UseXYZColorSpace { get; set; }

        public string SpectralFileName { get; set; }

        public float SpectralFactor { get; set; }

        public bool IsSpectral
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.SpectralFileName);
            }
        }

        public bool IsRGB
        {
            get { return !this.IsSpectral && !this.UseXYZColorSpace; }
        }

        public bool IsXYZ
        {
            get { return !this.IsSpectral && this.UseXYZColorSpace; }
        }
    }
}
