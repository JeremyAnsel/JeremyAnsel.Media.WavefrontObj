// <copyright file="ObjFreeFormElement.cs" company="Jérémy Ansel">
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
    public abstract class ObjFreeFormElement : ObjElement
    {
        internal ObjFreeFormElement()
        {
            this.ParametersU = new List<float>();
            this.ParametersV = new List<float>();
            this.OuterTrimmingCurves = new List<ObjCurveIndex>();
            this.InnerTrimmingCurves = new List<ObjCurveIndex>();
            this.SequenceCurves = new List<ObjCurveIndex>();
            this.SpecialPoints = new List<int>();
        }

        public int MergingGroupNumber { get; set; }

        public ObjFreeFormType FreeFormType { get; set; }

        public bool IsRationalForm { get; set; }

        public int DegreeU { get; set; }

        public int DegreeV { get; set; }

        public float[] BasicMatrixU { get; set; }

        public float[] BasicMatrixV { get; set; }

        public float StepU { get; set; }

        public float StepV { get; set; }

        public ObjApproximationTechnique CurveApproximationTechnique { get; set; }

        public ObjApproximationTechnique SurfaceApproximationTechnique { get; set; }

        public IList<float> ParametersU { get; private set; }

        public IList<float> ParametersV { get; private set; }

        public IList<ObjCurveIndex> OuterTrimmingCurves { get; private set; }

        public IList<ObjCurveIndex> InnerTrimmingCurves { get; private set; }

        public IList<ObjCurveIndex> SequenceCurves { get; private set; }

        public IList<int> SpecialPoints { get; private set; }
    }
}
