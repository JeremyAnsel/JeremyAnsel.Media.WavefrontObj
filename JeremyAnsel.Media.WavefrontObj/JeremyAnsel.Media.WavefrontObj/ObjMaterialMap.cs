// <copyright file="ObjMaterialMap.cs" company="Jérémy Ansel">
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
    public class ObjMaterialMap
    {
        public ObjMaterialMap()
        {
            this.IsHorizontalBlendingEnabled = true;
            this.IsVerticalBlendingEnabled = true;
            this.ScalarChannel = ObjMapChannel.Luminance;
            this.ModifierBase = 0.0f;
            this.ModifierGain = 1.0f;
            this.Offset = new ObjVector3(0.0f, 0.0f, 0.0f);
            this.Scale = new ObjVector3(1.0f, 1.0f, 1.0f);
            this.Turbulence = new ObjVector3(0.0f, 0.0f, 0.0f);
        }

        public ObjMaterialMap(string fileName)
            :this()
        {
            this.FileName = fileName;
        }

        public string FileName { get; set; }

        public bool IsHorizontalBlendingEnabled { get; set; }

        public bool IsVerticalBlendingEnabled { get; set; }

        public float BumpMultiplier { get; set; }

        public float Boost { get; set; }

        public bool IsColorCorrectionEnabled { get; set; }

        public bool IsClampingEnabled { get; set; }

        public ObjMapChannel ScalarChannel { get; set; }

        public float ModifierBase { get; set; }

        public float ModifierGain { get; set; }

        public ObjVector3 Offset { get; set; }

        public ObjVector3 Scale { get; set; }

        public ObjVector3 Turbulence { get; set; }

        public int TextureResolution { get; set; }
    }
}
