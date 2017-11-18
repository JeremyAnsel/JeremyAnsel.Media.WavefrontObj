// <copyright file="ObjFileWriterContext.cs" company="Jérémy Ansel">
// Copyright (c) 2017 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace JeremyAnsel.Media.WavefrontObj
{
    internal class ObjFileWriterContext
    {
        private ObjFile obj;

        public ObjFileWriterContext(ObjFile obj)
        {
            this.obj = obj;
            this.GroupNames = string.Empty;
        }

        public string GroupNames { get; private set; }

        public string ObjectName { get; private set; }

        public int LevelOfDetail { get; private set; }

        public string MapName { get; private set; }

        public string MaterialName { get; private set; }

        public int SmoothingGroupNumber { get; private set; }

        public bool IsBevelInterpolationEnabled { get; private set; }

        public bool IsColorInterpolationEnabled { get; private set; }

        public bool IsDissolveInterpolationEnabled { get; private set; }

        public int MergingGroupNumber { get; private set; }

        public void WriteGroupNames<T>(StreamWriter stream, T element, Func<ObjGroup, IList<T>> func)
            where T : ObjElement
        {
            string groupNames = GetGroupNames(element, func);

            if (groupNames != this.GroupNames)
            {
                this.GroupNames = groupNames;

                if (string.IsNullOrEmpty(groupNames))
                {
                    stream.WriteLine("g default");
                }
                else
                {
                    stream.WriteLine("g {0}", groupNames);
                }
            }
        }

        public void WriteAttributesOfElement(StreamWriter stream, ObjElement element)
        {
            if (element.ObjectName != this.ObjectName)
            {
                this.ObjectName = element.ObjectName;

                if (string.IsNullOrEmpty(this.ObjectName))
                {
                    stream.WriteLine("o");
                }
                else
                {
                    stream.WriteLine("o {0}", this.ObjectName);
                }
            }

            if (element.LevelOfDetail != this.LevelOfDetail)
            {
                this.LevelOfDetail = element.LevelOfDetail;

                stream.WriteLine("lod {0}", this.LevelOfDetail);
            }

            if (element.MapName != this.MapName)
            {
                this.MapName = element.MapName;

                if (string.IsNullOrEmpty(this.MapName))
                {
                    stream.WriteLine("usemap off");
                }
                else
                {
                    stream.WriteLine("usemap {0}", this.MapName);
                }
            }

            if (element.MaterialName != this.MaterialName)
            {
                this.MaterialName = element.MaterialName;

                if (string.IsNullOrEmpty(this.MaterialName))
                {
                    stream.WriteLine("usemtl off");
                }
                else
                {
                    stream.WriteLine("usemtl {0}", this.MaterialName);
                }
            }
        }

        public void WriteAttributesOfPolygonalElement(StreamWriter stream, ObjPolygonalElement element)
        {
            if (element.SmoothingGroupNumber != this.SmoothingGroupNumber)
            {
                this.SmoothingGroupNumber = element.SmoothingGroupNumber;

                if (this.SmoothingGroupNumber == 0)
                {
                    stream.WriteLine("s off");
                }
                else
                {
                    stream.WriteLine("s {0}", this.SmoothingGroupNumber);
                }
            }

            if (element.IsBevelInterpolationEnabled != this.IsBevelInterpolationEnabled)
            {
                this.IsBevelInterpolationEnabled = element.IsBevelInterpolationEnabled;

                if (this.IsBevelInterpolationEnabled)
                {
                    stream.WriteLine("bevel on");
                }
                else
                {
                    stream.WriteLine("bevel off");
                }
            }

            if (element.IsColorInterpolationEnabled != this.IsColorInterpolationEnabled)
            {
                this.IsColorInterpolationEnabled = element.IsColorInterpolationEnabled;

                if (this.IsColorInterpolationEnabled)
                {
                    stream.WriteLine("c_interp on");
                }
                else
                {
                    stream.WriteLine("c_interp off");
                }
            }

            if (element.IsDissolveInterpolationEnabled != this.IsDissolveInterpolationEnabled)
            {
                this.IsDissolveInterpolationEnabled = element.IsDissolveInterpolationEnabled;

                if (this.IsDissolveInterpolationEnabled)
                {
                    stream.WriteLine("d_interp on");
                }
                else
                {
                    stream.WriteLine("d_interp off");
                }
            }
        }

        public void WriteAttributesOfFreeFormElement(StreamWriter stream, ObjFreeFormElement element)
        {
            if (element.MergingGroupNumber != this.MergingGroupNumber)
            {
                this.MergingGroupNumber = element.MergingGroupNumber;

                if (this.MergingGroupNumber == 0)
                {
                    stream.WriteLine("mg off");
                }
                else
                {
                    float res = obj.MergingGroupResolutions[this.MergingGroupNumber];
                    stream.WriteLine("mg {0} {1}", this.MergingGroupNumber, res.ToString("F6", CultureInfo.InvariantCulture));
                }
            }

            switch (element.FreeFormType)
            {
                case ObjFreeFormType.BasisMatrix:
                    stream.Write("cstype");

                    if (element.IsRationalForm)
                    {
                        stream.Write(" rat");
                    }

                    stream.WriteLine(" bmatrix");
                    break;

                case ObjFreeFormType.Bezier:
                    stream.Write("cstype");

                    if (element.IsRationalForm)
                    {
                        stream.Write(" rat");
                    }

                    stream.WriteLine(" bezier");
                    break;

                case ObjFreeFormType.BSpline:
                    stream.Write("cstype");

                    if (element.IsRationalForm)
                    {
                        stream.Write(" rat");
                    }

                    stream.WriteLine(" bspline");
                    break;

                case ObjFreeFormType.Cardinal:
                    stream.Write("cstype");

                    if (element.IsRationalForm)
                    {
                        stream.Write(" rat");
                    }

                    stream.WriteLine(" cardinal");
                    break;

                case ObjFreeFormType.Taylor:
                    stream.Write("cstype");

                    if (element.IsRationalForm)
                    {
                        stream.Write(" rat");
                    }

                    stream.WriteLine(" taylor");
                    break;
            }

            if (element.DegreeV == 0)
            {
                stream.WriteLine("deg {0}", element.DegreeU);
            }
            else
            {
                stream.WriteLine("deg {0} {1}", element.DegreeU, element.DegreeV);
            }

            if (element.BasicMatrixU != null)
            {
                stream.Write("bmat u");

                foreach (float value in element.BasicMatrixU)
                {
                    stream.Write(' ');
                    stream.Write(value.ToString("F6", CultureInfo.InvariantCulture));
                }

                stream.WriteLine();
            }

            if (element.BasicMatrixV != null)
            {
                stream.Write("bmat v");

                foreach (float value in element.BasicMatrixV)
                {
                    stream.Write(' ');
                    stream.Write(value.ToString("F6", CultureInfo.InvariantCulture));
                }

                stream.WriteLine();
            }

            if (element.StepV == 1.0f)
            {
                stream.WriteLine(
                    "step {0}",
                    element.StepU.ToString("F6", CultureInfo.InvariantCulture));
            }
            else
            {
                stream.WriteLine(
                    "step {0} {1}",
                    element.StepU.ToString("F6", CultureInfo.InvariantCulture),
                    element.StepV.ToString("F6", CultureInfo.InvariantCulture));
            }

            if (element.CurveApproximationTechnique != null)
            {
                if (element.CurveApproximationTechnique is ObjConstantParametricSubdivisionTechnique)
                {
                    var technique = (ObjConstantParametricSubdivisionTechnique)element.CurveApproximationTechnique;
                    stream.WriteLine("ctech cparm {0}", technique.ResolutionU.ToString("F6", CultureInfo.InvariantCulture));
                }
                else if (element.CurveApproximationTechnique is ObjConstantSpatialSubdivisionTechnique)
                {
                    var technique = (ObjConstantSpatialSubdivisionTechnique)element.CurveApproximationTechnique;
                    stream.WriteLine("ctech cspace {0}", technique.MaximumLength.ToString("F6", CultureInfo.InvariantCulture));
                }
                else if (element.CurveApproximationTechnique is ObjCurvatureDependentSubdivisionTechnique)
                {
                    var technique = (ObjCurvatureDependentSubdivisionTechnique)element.CurveApproximationTechnique;
                    stream.WriteLine(
                        "ctech curv {0} {1}",
                        technique.MaximumDistance.ToString("F6", CultureInfo.InvariantCulture),
                        technique.MaximumAngle.ToString("F6", CultureInfo.InvariantCulture));
                }
            }

            if (element.SurfaceApproximationTechnique != null)
            {
                if (element.SurfaceApproximationTechnique is ObjConstantParametricSubdivisionTechnique)
                {
                    var technique = (ObjConstantParametricSubdivisionTechnique)element.SurfaceApproximationTechnique;

                    if (technique.ResolutionU == technique.ResolutionV)
                    {
                        stream.WriteLine(
                            "stech cparmb {0}",
                            technique.ResolutionU.ToString("F6", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        stream.WriteLine(
                            "stech cparma {0} {1}",
                            technique.ResolutionU.ToString("F6", CultureInfo.InvariantCulture),
                            technique.ResolutionV.ToString("F6", CultureInfo.InvariantCulture));
                    }
                }
                else if (element.SurfaceApproximationTechnique is ObjConstantSpatialSubdivisionTechnique)
                {
                    var technique = (ObjConstantSpatialSubdivisionTechnique)element.SurfaceApproximationTechnique;
                    stream.WriteLine("stech cspace {0}", technique.MaximumLength.ToString("F6", CultureInfo.InvariantCulture));
                }
                else if (element.SurfaceApproximationTechnique is ObjCurvatureDependentSubdivisionTechnique)
                {
                    var technique = (ObjCurvatureDependentSubdivisionTechnique)element.SurfaceApproximationTechnique;
                    stream.WriteLine(
                        "stech curv {0} {1}",
                        technique.MaximumDistance.ToString("F6", CultureInfo.InvariantCulture),
                        technique.MaximumAngle.ToString("F6", CultureInfo.InvariantCulture));
                }
            }
        }

        private string GetGroupNames<T>(T element, Func<ObjGroup, IList<T>> func)
            where T : ObjElement
        {
            var groups = new List<string>();

            foreach (ObjGroup group in obj.Groups)
            {
                IList<T> elements = func(group);

                if (elements.Contains(element))
                {
                    groups.Add(group.Name);
                }
            }

            return string.Join(" ", groups);
        }
    }
}
