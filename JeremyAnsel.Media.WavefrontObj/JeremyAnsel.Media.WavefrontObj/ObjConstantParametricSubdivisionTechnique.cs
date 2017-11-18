// <copyright file="ObjConstantParametricSubdivisionTechnique.cs" company="Jérémy Ansel">
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
    public class ObjConstantParametricSubdivisionTechnique : ObjApproximationTechnique
    {
        public ObjConstantParametricSubdivisionTechnique()
        {
        }

        public ObjConstantParametricSubdivisionTechnique(float u)
        {
            this.ResolutionU = u;
            this.ResolutionV = u;
        }

        public ObjConstantParametricSubdivisionTechnique(float u, float v)
        {
            this.ResolutionU = u;
            this.ResolutionV = v;
        }

        public float ResolutionU { get; set; }

        public float ResolutionV { get; set; }
    }
}
