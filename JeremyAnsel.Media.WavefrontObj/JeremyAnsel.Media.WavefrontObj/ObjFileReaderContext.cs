// <copyright file="ObjFileReaderContext.cs" company="Jérémy Ansel">
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
    internal class ObjFileReaderContext
    {
        private ObjFile obj;

        public ObjFileReaderContext(ObjFile obj)
        {
            this.obj = obj;

            this.GroupNames = new List<string>();
        }

        public string ObjectName { get; set; }

        public int LevelOfDetail { get; set; }

        public string MapName { get; set; }

        public string MaterialName { get; set; }

        public int SmoothingGroupNumber { get; set; }

        public bool IsBevelInterpolationEnabled { get; set; }

        public bool IsColorInterpolationEnabled { get; set; }

        public bool IsDissolveInterpolationEnabled { get; set; }

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

        public ObjFreeFormElement CurrentFreeFormElement { get; set; }

        public IList<string> GroupNames { get; private set; }

        public IList<ObjGroup> GetCurrentGroups()
        {
            var groups = new List<ObjGroup>();

            foreach (var name in this.GroupNames)
            {
                var group = obj.Groups.FirstOrDefault(t => string.Equals(t.Name, name));

                if (group == null)
                {
                    group = new ObjGroup(name);
                    obj.Groups.Add(group);
                }

                groups.Add(group);
            }

            if (groups.Count == 0)
            {
                groups.Add(obj.DefaultGroup);
            }

            return groups;
        }

        public void ApplyAttributesToElement(ObjElement element)
        {
            element.ObjectName = this.ObjectName;
            element.LevelOfDetail = this.LevelOfDetail;
            element.MapName = this.MapName;
            element.MaterialName = this.MaterialName;
        }

        public void ApplyAttributesToPolygonalElement(ObjPolygonalElement element)
        {
            element.SmoothingGroupNumber = this.SmoothingGroupNumber;
            element.IsBevelInterpolationEnabled = this.IsBevelInterpolationEnabled;
            element.IsColorInterpolationEnabled = this.IsColorInterpolationEnabled;
            element.IsDissolveInterpolationEnabled = this.IsDissolveInterpolationEnabled;
        }

        public void ApplyAttributesToFreeFormElement(ObjFreeFormElement element)
        {
            element.MergingGroupNumber = this.MergingGroupNumber;
            element.FreeFormType = this.FreeFormType;
            element.IsRationalForm = this.IsRationalForm;
            element.DegreeU = this.DegreeU;
            element.DegreeV = this.DegreeV;
            element.BasicMatrixU = this.BasicMatrixU;
            element.BasicMatrixV = this.BasicMatrixV;
            element.StepU = this.StepU;
            element.StepV = this.StepV;
            element.CurveApproximationTechnique = this.CurveApproximationTechnique;
            element.SurfaceApproximationTechnique = this.SurfaceApproximationTechnique;
        }
    }
}
