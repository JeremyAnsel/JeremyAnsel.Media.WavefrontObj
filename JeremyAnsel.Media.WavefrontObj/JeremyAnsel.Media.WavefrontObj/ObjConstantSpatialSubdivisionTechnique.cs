// <copyright file="ObjConstantSpatialSubdivisionTechnique.cs" company="Jérémy Ansel">
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
    [System.Diagnostics.DebuggerDisplay("ObjConstantSpatialSubdivisionTechnique MaxLen:{MaximumLength}")]
    public class ObjConstantSpatialSubdivisionTechnique : ObjApproximationTechnique
    {
        public ObjConstantSpatialSubdivisionTechnique()
        {
        }

        public ObjConstantSpatialSubdivisionTechnique(float length)
        {
            this.MaximumLength = length;
        }

        public float MaximumLength { get; set; }
    }
}
