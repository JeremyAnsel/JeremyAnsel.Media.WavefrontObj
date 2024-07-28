// <copyright file="ObjCurvatureDependentSubdivisionTechnique.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

namespace JeremyAnsel.Media.WavefrontObj
{
    [System.Diagnostics.DebuggerDisplay("ObjCurvatureDependentSubdivisionTechnique MaxDist:{MaximumDistance} MaxAngle:{MaximumAngle}")]
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
