// <copyright file="ObjCurvatureDependentSubdivisionTechnique.cs" company="Jérémy Ansel">
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
    public class ObjCurvatureDependentSubdivisionTechnique : ObjApproximationTechnique
    {
        public ObjCurvatureDependentSubdivisionTechnique()
        {
        }

        public ObjCurvatureDependentSubdivisionTechnique(float distance, float angle)
        {
            this.MaximumDistance = distance;
            this.MaximumAngle = angle;
        }

        public float MaximumDistance { get; set; }

        public float MaximumAngle { get; set; }
    }
}
