// <copyright file="ObjFileReader.cs" company="Jérémy Ansel">
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
    internal static class ObjFileReader
    {
        public static ObjFile FromStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var obj = new ObjFile();
            var context = new ObjFileReaderContext(obj);

            foreach (var values in LineReader.Read(stream))
            {
                switch (values[0].ToLowerInvariant())
                {
                    case "v":
                        {
                            if (values.Length < 4)
                            {
                                throw new InvalidDataException("A v statement must specify at least 3 values.");
                            }

                            float x = float.Parse(values[1], CultureInfo.InvariantCulture);
                            float y = float.Parse(values[2], CultureInfo.InvariantCulture);
                            float z = float.Parse(values[3], CultureInfo.InvariantCulture);
                            float w = 1.0f;
                            bool hasColor = false;
                            float r = 0.0f;
                            float g = 0.0f;
                            float b = 0.0f;
                            float a = 1.0f;

                            if (values.Length == 4 || values.Length == 5)
                            {
                                if (values.Length == 5)
                                {
                                    w = float.Parse(values[4], CultureInfo.InvariantCulture);
                                }
                            }
                            else if (values.Length == 7 || values.Length == 8)
                            {
                                hasColor = true;
                                r = float.Parse(values[4], CultureInfo.InvariantCulture);
                                g = float.Parse(values[5], CultureInfo.InvariantCulture);
                                b = float.Parse(values[6], CultureInfo.InvariantCulture);

                                if (values.Length == 8)
                                {
                                    a = float.Parse(values[7], CultureInfo.InvariantCulture);
                                }
                            }
                            else
                            {
                                throw new InvalidDataException("A v statement has too many values.");
                            }

                            var v = new ObjVertex();
                            v.Position = new ObjVector4(x, y, z, w);

                            if (hasColor)
                            {
                                v.Color = new ObjVector4(r, g, b, a);
                            }

                            obj.Vertices.Add(v);
                            break;
                        }

                    case "vp":
                        {
                            if (values.Length < 2)
                            {
                                throw new InvalidDataException("A vp statement must specify at least 1 value.");
                            }

                            var v = new ObjVector3();
                            v.X = float.Parse(values[1], CultureInfo.InvariantCulture);

                            if (values.Length == 2)
                            {
                                v.Y = 0.0f;
                                v.Z = 1.0f;
                            }
                            else if (values.Length == 3)
                            {
                                v.Y = float.Parse(values[2], CultureInfo.InvariantCulture);
                                v.Z = 1.0f;
                            }
                            else if (values.Length == 4)
                            {
                                v.Y = float.Parse(values[2], CultureInfo.InvariantCulture);
                                v.Z = float.Parse(values[3], CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                throw new InvalidDataException("A vp statement has too many values.");
                            }

                            obj.ParameterSpaceVertices.Add(v);
                            break;
                        }

                    case "vn":
                        {
                            if (values.Length < 4)
                            {
                                throw new InvalidDataException("A vn statement must specify 3 values.");
                            }

                            if (values.Length != 4)
                            {
                                throw new InvalidDataException("A vn statement has too many values.");
                            }

                            var v = new ObjVector3();
                            v.X = float.Parse(values[1], CultureInfo.InvariantCulture);
                            v.Y = float.Parse(values[2], CultureInfo.InvariantCulture);
                            v.Z = float.Parse(values[3], CultureInfo.InvariantCulture);

                            obj.VertexNormals.Add(v);
                            break;
                        }

                    case "vt":
                        {
                            if (values.Length < 2)
                            {
                                throw new InvalidDataException("A vt statement must specify at least 1 value.");
                            }

                            var v = new ObjVector3();
                            v.X = float.Parse(values[1], CultureInfo.InvariantCulture);

                            if (values.Length == 2)
                            {
                                v.Y = 0.0f;
                                v.Z = 0.0f;
                            }
                            else if (values.Length == 3)
                            {
                                v.Y = float.Parse(values[2], CultureInfo.InvariantCulture);
                                v.Z = 0.0f;
                            }
                            else if (values.Length == 4)
                            {
                                v.Y = float.Parse(values[2], CultureInfo.InvariantCulture);
                                v.Z = float.Parse(values[3], CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                throw new InvalidDataException("A vt statement has too many values.");
                            }

                            obj.TextureVertices.Add(v);
                            break;
                        }

                    case "cstype":
                        ObjFileReader.ParseFreeFormType(context, values);
                        break;

                    case "deg":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A deg statement must specify at least 1 value.");
                        }

                        if (values.Length == 2)
                        {
                            context.DegreeU = int.Parse(values[1]);
                            context.DegreeV = 0;
                        }
                        else if (values.Length == 3)
                        {
                            context.DegreeU = int.Parse(values[1], CultureInfo.InvariantCulture);
                            context.DegreeV = int.Parse(values[2], CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            throw new InvalidDataException("A deg statement has too many values.");
                        }

                        break;

                    case "bmat":
                        {
                            if (values.Length < 2)
                            {
                                throw new InvalidDataException("A bmat statement must specify a direction.");
                            }

                            int d;

                            if (string.Equals(values[1], "u", StringComparison.OrdinalIgnoreCase))
                            {
                                d = 1;
                            }
                            else if (string.Equals(values[1], "v", StringComparison.OrdinalIgnoreCase))
                            {
                                d = 2;
                            }
                            else
                            {
                                throw new InvalidDataException("A bmat statement has an unknown direction.");
                            }

                            int count = (context.DegreeU + 1) * (context.DegreeV + 1);

                            if (values.Length != count + 2)
                            {
                                throw new InvalidDataException("A bmat statement has too many or too few values.");
                            }

                            var matrix = new float[count];

                            for (int i = 0; i < count; i++)
                            {
                                matrix[i] = float.Parse(values[2 + i], CultureInfo.InvariantCulture);
                            }

                            switch (d)
                            {
                                case 1:
                                    context.BasicMatrixU = matrix;
                                    break;

                                case 2:
                                    context.BasicMatrixV = matrix;
                                    break;
                            }

                            break;
                        }

                    case "step":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A step statement must specify at least 1 value.");
                        }

                        if (values.Length == 2)
                        {
                            context.StepU = float.Parse(values[1], CultureInfo.InvariantCulture);
                            context.StepV = 1.0f;
                        }
                        else if (values.Length == 3)
                        {
                            context.StepU = float.Parse(values[1], CultureInfo.InvariantCulture);
                            context.StepV = float.Parse(values[2], CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            throw new InvalidDataException("A step statement has too many values.");
                        }

                        break;

                    case "p":
                        {
                            if (values.Length < 2)
                            {
                                throw new InvalidDataException("A p statement must specify at least 1 value.");
                            }

                            var point = new ObjPoint();

                            for (int i = 1; i < values.Length; i++)
                            {
                                point.Vertices.Add(ObjFileReader.ParseTriplet(obj, values[i]));
                            }

                            context.ApplyAttributesToElement(point);
                            context.ApplyAttributesToPolygonalElement(point);

                            obj.Points.Add(point);

                            foreach (var group in context.GetCurrentGroups())
                            {
                                group.Points.Add(point);
                            }

                            break;
                        }

                    case "l":
                        {
                            if (values.Length < 3)
                            {
                                throw new InvalidDataException("A l statement must specify at least 2 values.");
                            }

                            var line = new ObjLine();

                            for (int i = 1; i < values.Length; i++)
                            {
                                line.Vertices.Add(ObjFileReader.ParseTriplet(obj, values[i]));
                            }

                            context.ApplyAttributesToElement(line);
                            context.ApplyAttributesToPolygonalElement(line);

                            obj.Lines.Add(line);

                            foreach (var group in context.GetCurrentGroups())
                            {
                                group.Lines.Add(line);
                            }

                            break;
                        }

                    case "f":
                    case "fo":
                        {
                            if (values.Length < 4)
                            {
                                throw new InvalidDataException("A f statement must specify at least 3 values.");
                            }

                            var face = new ObjFace();

                            for (int i = 1; i < values.Length; i++)
                            {
                                face.Vertices.Add(ObjFileReader.ParseTriplet(obj, values[i]));
                            }

                            context.ApplyAttributesToElement(face);
                            context.ApplyAttributesToPolygonalElement(face);

                            obj.Faces.Add(face);

                            foreach (var group in context.GetCurrentGroups())
                            {
                                group.Faces.Add(face);
                            }

                            break;
                        }

                    case "curv":
                        {
                            if (values.Length < 5)
                            {
                                throw new InvalidDataException("A curv statement must specify at least 4 values.");
                            }

                            var curve = new ObjCurve();

                            curve.StartParameter = float.Parse(values[1], CultureInfo.InvariantCulture);
                            curve.EndParameter = float.Parse(values[2], CultureInfo.InvariantCulture);

                            for (int i = 3; i < values.Length; i++)
                            {
                                int v = int.Parse(values[i], CultureInfo.InvariantCulture);

                                if (v == 0)
                                {
                                    throw new InvalidDataException("A curv statement contains an invalid vertex index.");
                                }

                                if (v < 0)
                                {
                                    v = obj.Vertices.Count + v + 1;
                                }

                                if (v <= 0 || v > obj.Vertices.Count)
                                {
                                    throw new IndexOutOfRangeException();
                                }

                                curve.Vertices.Add(v);
                            }

                            context.ApplyAttributesToElement(curve);
                            context.ApplyAttributesToFreeFormElement(curve);
                            context.CurrentFreeFormElement = curve;

                            obj.Curves.Add(curve);

                            foreach (var group in context.GetCurrentGroups())
                            {
                                group.Curves.Add(curve);
                            }

                            break;
                        }

                    case "curv2":
                        {
                            if (values.Length < 3)
                            {
                                throw new InvalidDataException("A curv2 statement must specify at least 2 values.");
                            }

                            var curve = new ObjCurve2D();

                            for (int i = 1; i < values.Length; i++)
                            {
                                int vp = int.Parse(values[i], CultureInfo.InvariantCulture);

                                if (vp == 0)
                                {
                                    throw new InvalidDataException("A curv2 statement contains an invalid parameter space vertex index.");
                                }

                                if (vp < 0)
                                {
                                    vp = obj.ParameterSpaceVertices.Count + vp + 1;
                                }

                                if (vp <= 0 || vp > obj.ParameterSpaceVertices.Count)
                                {
                                    throw new IndexOutOfRangeException();
                                }

                                curve.ParameterSpaceVertices.Add(vp);
                            }

                            context.ApplyAttributesToElement(curve);
                            context.ApplyAttributesToFreeFormElement(curve);
                            context.CurrentFreeFormElement = curve;

                            obj.Curves2D.Add(curve);

                            foreach (var group in context.GetCurrentGroups())
                            {
                                group.Curves2D.Add(curve);
                            }

                            break;
                        }

                    case "surf":
                        {
                            if (values.Length < 6)
                            {
                                throw new InvalidDataException("A surf statement must specify at least 5 values.");
                            }

                            var surface = new ObjSurface();

                            surface.StartParameterU = float.Parse(values[1], CultureInfo.InvariantCulture);
                            surface.EndParameterU = float.Parse(values[2], CultureInfo.InvariantCulture);
                            surface.StartParameterV = float.Parse(values[3], CultureInfo.InvariantCulture);
                            surface.EndParameterV = float.Parse(values[4], CultureInfo.InvariantCulture);

                            for (int i = 5; i < values.Length; i++)
                            {
                                surface.Vertices.Add(ObjFileReader.ParseTriplet(obj, values[i]));
                            }

                            context.ApplyAttributesToElement(surface);
                            context.ApplyAttributesToFreeFormElement(surface);
                            context.CurrentFreeFormElement = surface;

                            obj.Surfaces.Add(surface);

                            foreach (var group in context.GetCurrentGroups())
                            {
                                group.Surfaces.Add(surface);
                            }

                            break;
                        }

                    case "parm":
                        if (context.CurrentFreeFormElement == null)
                        {
                            break;
                        }

                        if (values.Length < 4)
                        {
                            throw new InvalidDataException("A parm statement must specify at least 3 values.");
                        }

                        IList<float> parameters;

                        if (string.Equals(values[1], "u", StringComparison.OrdinalIgnoreCase))
                        {
                            parameters = context.CurrentFreeFormElement.ParametersU;
                        }
                        else if (string.Equals(values[1], "v", StringComparison.OrdinalIgnoreCase))
                        {
                            parameters = context.CurrentFreeFormElement.ParametersV;
                        }
                        else
                        {
                            throw new InvalidDataException("A parm statement has an unknown direction.");
                        }

                        for (int i = 2; i < values.Length; i++)
                        {
                            parameters.Add(float.Parse(values[i], CultureInfo.InvariantCulture));
                        }

                        break;

                    case "trim":
                        if (context.CurrentFreeFormElement == null)
                        {
                            break;
                        }

                        ObjFileReader.ParseCurveIndex(context.CurrentFreeFormElement.OuterTrimmingCurves, obj, values);
                        break;

                    case "hole":
                        if (context.CurrentFreeFormElement == null)
                        {
                            break;
                        }

                        ObjFileReader.ParseCurveIndex(context.CurrentFreeFormElement.InnerTrimmingCurves, obj, values);
                        break;

                    case "scrv":
                        if (context.CurrentFreeFormElement == null)
                        {
                            break;
                        }

                        ObjFileReader.ParseCurveIndex(context.CurrentFreeFormElement.SequenceCurves, obj, values);
                        break;

                    case "sp":
                        if (context.CurrentFreeFormElement == null)
                        {
                            break;
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A sp statement must specify at least 1 value.");
                        }

                        for (int i = 1; i < values.Length; i++)
                        {
                            int vp = int.Parse(values[i], CultureInfo.InvariantCulture);

                            if (vp == 0)
                            {
                                throw new InvalidDataException("A sp statement contains an invalid parameter space vertex index.");
                            }

                            if (vp < 0)
                            {
                                vp = obj.ParameterSpaceVertices.Count + vp + 1;
                            }

                            if (vp <= 0 || vp > obj.ParameterSpaceVertices.Count)
                            {
                                throw new IndexOutOfRangeException();
                            }

                            context.CurrentFreeFormElement.SpecialPoints.Add(vp);
                        }

                        break;

                    case "end":
                        context.CurrentFreeFormElement = null;
                        break;

                    case "con":
                        ObjFileReader.ParseSurfaceConnection(obj, values);
                        break;

                    case "g":
                        ObjFileReader.ParseGroupName(values, context);
                        break;

                    case "s":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A s statement must specify a value.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A s statement has too many values.");
                        }

                        if (string.Equals(values[1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            context.SmoothingGroupNumber = 0;
                        }
                        else
                        {
                            context.SmoothingGroupNumber = int.Parse(values[1], CultureInfo.InvariantCulture);
                        }

                        break;

                    case "mg":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A mg statement must specify a value.");
                        }

                        if (string.Equals(values[1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            context.MergingGroupNumber = 0;
                        }
                        else
                        {
                            context.MergingGroupNumber = int.Parse(values[1], CultureInfo.InvariantCulture);
                        }

                        if (context.MergingGroupNumber == 0)
                        {
                            if (values.Length > 3)
                            {
                                throw new InvalidDataException("A mg statement has too many values.");
                            }
                        }
                        else
                        {
                            if (values.Length != 3)
                            {
                                throw new InvalidDataException("A mg statement has too many or too few values.");
                            }

                            float res = float.Parse(values[2], CultureInfo.InvariantCulture);

                            obj.MergingGroupResolutions[context.MergingGroupNumber] = res;
                        }

                        break;

                    case "o":
                        if (values.Length == 1)
                        {
                            context.ObjectName = null;
                            break;
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A o statement has too many values.");
                        }

                        context.ObjectName = values[1];
                        break;

                    case "bevel":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A bevel statement must specify a name.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A bevel statement has too many values.");
                        }

                        if (string.Equals(values[1], "on", StringComparison.OrdinalIgnoreCase))
                        {
                            context.IsBevelInterpolationEnabled = true;
                        }
                        else if (string.Equals(values[1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            context.IsBevelInterpolationEnabled = false;
                        }
                        else
                        {
                            throw new InvalidDataException("A bevel statement must specify on or off.");
                        }

                        break;

                    case "c_interp":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A c_interp statement must specify a name.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A c_interp statement has too many values.");
                        }

                        if (string.Equals(values[1], "on", StringComparison.OrdinalIgnoreCase))
                        {
                            context.IsColorInterpolationEnabled = true;
                        }
                        else if (string.Equals(values[1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            context.IsColorInterpolationEnabled = false;
                        }
                        else
                        {
                            throw new InvalidDataException("A c_interp statement must specify on or off.");
                        }

                        break;

                    case "d_interp":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A d_interp statement must specify a name.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A d_interp statement has too many values.");
                        }

                        if (string.Equals(values[1], "on", StringComparison.OrdinalIgnoreCase))
                        {
                            context.IsDissolveInterpolationEnabled = true;
                        }
                        else if (string.Equals(values[1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            context.IsDissolveInterpolationEnabled = false;
                        }
                        else
                        {
                            throw new InvalidDataException("A d_interp statement must specify on or off.");
                        }

                        break;

                    case "lod":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A lod statement must specify a value.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A lod statement has too many values.");
                        }

                        context.LevelOfDetail = int.Parse(values[1], CultureInfo.InvariantCulture);
                        break;

                    case "maplib":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A maplib statement must specify a file name.");
                        }

                        for (int i = 1; i < values.Length; i++)
                        {
                            if (!Path.HasExtension(values[i]))
                            {
                                throw new InvalidDataException("A file name must have an extension.");
                            }

                            obj.MapLibraries.Add(values[i]);
                        }

                        break;

                    case "mtllib":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A mtllib statement must specify a file name.");
                        }

                        for (int i = 1; i < values.Length; i++)
                        {
                            if (!Path.HasExtension(values[i]))
                            {
                                throw new InvalidDataException("A file name must have an extension.");
                            }

                            obj.MaterialLibraries.Add(values[i]);
                        }

                        break;

                    case "usemap":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A usemap statement must specify a value.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A usemap statement has too many values.");
                        }

                        if (string.Equals(values[1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            context.MapName = null;
                        }
                        else
                        {
                            context.MapName = values[1];
                        }

                        break;

                    case "usemtl":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A usemtl statement must specify a value.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A usemtl statement has too many values.");
                        }

                        if (string.Equals(values[1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            context.MaterialName = null;
                        }
                        else
                        {
                            context.MaterialName = values[1];
                        }

                        break;

                    case "shadow_obj":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A shadow_obj statement must specify a file name.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A shadow_obj statement has too many values.");
                        }

                        if (!Path.HasExtension(values[1]))
                        {
                            throw new InvalidDataException("A file name must have an extension.");
                        }

                        obj.ShadowObjectFileName = values[1];
                        break;

                    case "trace_obj":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A trace_obj statement must specify a file name.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A trace_obj statement has too many values.");
                        }

                        if (!Path.HasExtension(values[1]))
                        {
                            throw new InvalidDataException("A file name must have an extension.");
                        }

                        obj.TraceObjectFileName = values[1];
                        break;

                    case "ctech":
                        context.CurveApproximationTechnique = ObjFileReader.ParseApproximationTechnique(values);
                        break;

                    case "stech":
                        context.SurfaceApproximationTechnique = ObjFileReader.ParseApproximationTechnique(values);
                        break;

                    case "bsp":
                    case "bzp":
                    case "cdc":
                    case "cdp":
                    case "res":
                        throw new NotImplementedException(string.Concat(values[0], " statement have been replaced by free-form geometry statements."));
                }
            }

            return obj;
        }

        private static ObjTriplet ParseTriplet(ObjFile obj, string value)
        {
            var values = value.Split('/');

            if (values.Length > 3)
            {
                throw new InvalidDataException("A triplet has too many values.");
            }

            int v = !string.IsNullOrEmpty(values[0]) ? int.Parse(values[0], CultureInfo.InvariantCulture) : 0;

            if (v == 0)
            {
                throw new InvalidDataException("A triplet must specify a vertex index.");
            }

            if (v < 0)
            {
                v = obj.Vertices.Count + v + 1;
            }

            if (v <= 0 || v > obj.Vertices.Count)
            {
                throw new IndexOutOfRangeException();
            }

            int vt = values.Length > 1 && !string.IsNullOrEmpty(values[1]) ? int.Parse(values[1], CultureInfo.InvariantCulture) : 0;

            if (vt != 0)
            {
                if (vt < 0)
                {
                    vt = obj.TextureVertices.Count + vt + 1;
                }

                if (vt <= 0 || vt > obj.TextureVertices.Count)
                {
                    throw new IndexOutOfRangeException();
                }
            }

            int vn = values.Length > 2 && !string.IsNullOrEmpty(values[2]) ? int.Parse(values[2], CultureInfo.InvariantCulture) : 0;

            if (vn != 0)
            {
                if (vn < 0)
                {
                    vn = obj.VertexNormals.Count + vn + 1;
                }

                if (vn <= 0 || vn > obj.VertexNormals.Count)
                {
                    throw new IndexOutOfRangeException();
                }
            }

            return new ObjTriplet(v, vt, vn);
        }

        private static ObjCurveIndex ParseCurveIndex(ObjFile obj, string[] values, int index)
        {
            float start = float.Parse(values[index], CultureInfo.InvariantCulture);
            float end = float.Parse(values[index + 1], CultureInfo.InvariantCulture);
            int curve2D = int.Parse(values[index + 2], CultureInfo.InvariantCulture);

            if (curve2D == 0)
            {
                throw new InvalidDataException("A curve index must specify an index.");
            }

            if (curve2D < 0)
            {
                curve2D = obj.Curves2D.Count + curve2D + 1;
            }

            if (curve2D <= 0 || curve2D > obj.Curves2D.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return new ObjCurveIndex(start, end, curve2D);
        }

        private static void ParseCurveIndex(IList<ObjCurveIndex> curves, ObjFile obj, string[] values)
        {
            if (values.Length < 4)
            {
                throw new InvalidDataException(string.Concat("A ", values[0], " statement must specify at least 3 value."));
            }

            if ((values.Length - 1) % 3 != 0)
            {
                throw new InvalidDataException(string.Concat("A ", values[0], " statement has too many values."));
            }

            for (int i = 1; i < values.Length; i += 3)
            {
                curves.Add(ObjFileReader.ParseCurveIndex(obj, values, i));
            }
        }

        private static void ParseFreeFormType(ObjFileReaderContext context, string[] values)
        {
            if (values.Length < 2)
            {
                throw new InvalidDataException("A cstype statement must specify a value.");
            }

            string type;

            if (values.Length == 2)
            {
                context.IsRationalForm = false;
                type = values[1];
            }
            else if (values.Length == 3 && string.Equals(values[1], "rat", StringComparison.OrdinalIgnoreCase))
            {
                context.IsRationalForm = true;
                type = values[2];
            }
            else
            {
                throw new InvalidDataException("A cstype statement has too many values.");
            }

            switch (type.ToLowerInvariant())
            {
                case "bmatrix":
                    context.FreeFormType = ObjFreeFormType.BasisMatrix;
                    break;

                case "bezier":
                    context.FreeFormType = ObjFreeFormType.Bezier;
                    break;

                case "bspline":
                    context.FreeFormType = ObjFreeFormType.BSpline;
                    break;

                case "cardinal":
                    context.FreeFormType = ObjFreeFormType.Cardinal;
                    break;

                case "taylor":
                    context.FreeFormType = ObjFreeFormType.Taylor;
                    break;

                default:
                    throw new InvalidDataException("A cstype statement has an unknown type.");
            }
        }

        private static void ParseSurfaceConnection(ObjFile obj, string[] values)
        {
            if (values.Length < 9)
            {
                throw new InvalidDataException("A con statement must specify 8 values.");
            }

            if (values.Length != 9)
            {
                throw new InvalidDataException("A con statement has too many values.");
            }

            int surface1 = int.Parse(values[1], CultureInfo.InvariantCulture);

            if (surface1 == 0)
            {
                throw new InvalidDataException("A con statement must specify a surface index.");
            }

            if (surface1 < 0)
            {
                surface1 = obj.Surfaces.Count + surface1 + 1;
            }

            if (surface1 <= 0 || surface1 > obj.Surfaces.Count)
            {
                throw new IndexOutOfRangeException();
            }

            var curve1 = ObjFileReader.ParseCurveIndex(obj, values, 2);

            int surface2 = int.Parse(values[5], CultureInfo.InvariantCulture);

            if (surface2 == 0)
            {
                throw new InvalidDataException("A con statement must specify a surface index.");
            }

            if (surface2 < 0)
            {
                surface2 = obj.Surfaces.Count + surface2 + 1;
            }

            if (surface2 <= 0 || surface2 > obj.Surfaces.Count)
            {
                throw new IndexOutOfRangeException();
            }

            var curve2 = ObjFileReader.ParseCurveIndex(obj, values, 6);

            var connection = new ObjSurfaceConnection
            {
                Surface1 = surface1,
                Curve2D1 = curve1,
                Surface2 = surface2,
                Curve2D2 = curve2
            };

            obj.SurfaceConnections.Add(connection);
        }

        private static void ParseGroupName(string[] values, ObjFileReaderContext context)
        {
            context.GroupNames.Clear();

            for (int i = 1; i < values.Length; i++)
            {
                var name = values[i];

                if (!string.Equals(name, "default", StringComparison.OrdinalIgnoreCase))
                {
                    context.GroupNames.Add(name);
                }
            }

            context.GetCurrentGroups();
        }

        private static ObjApproximationTechnique ParseApproximationTechnique(string[] values)
        {
            ObjApproximationTechnique technique = null;

            if (values.Length < 2)
            {
                throw new InvalidDataException(string.Concat("A ", values[0], " statement must specify a technique."));
            }

            switch (values[1].ToLowerInvariant())
            {
                case "cparm":
                    {
                        if (values.Length < 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", values[0], " cparm statement must specify a value."));
                        }

                        if (values.Length != 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", values[0], " cparm statement has too many values."));
                        }

                        float res = float.Parse(values[2], CultureInfo.InvariantCulture);
                        technique = new ObjConstantParametricSubdivisionTechnique(res);
                        break;
                    }

                case "cparma":
                    {
                        if (values.Length < 4)
                        {
                            throw new InvalidDataException(string.Concat("A ", values[0], " cparma statement must specify a value."));
                        }

                        if (values.Length != 4)
                        {
                            throw new InvalidDataException(string.Concat("A ", values[0], " cparma statement has too many values."));
                        }

                        float resU = float.Parse(values[2], CultureInfo.InvariantCulture);
                        float resV = float.Parse(values[3], CultureInfo.InvariantCulture);
                        technique = new ObjConstantParametricSubdivisionTechnique(resU, resV);
                        break;
                    }

                case "cparmb":
                    {
                        if (values.Length < 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", values[0], " cparmb statement must specify a value."));
                        }

                        if (values.Length != 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", values[0], " cparmb statement has too many values."));
                        }

                        float resU = float.Parse(values[2], CultureInfo.InvariantCulture);
                        technique = new ObjConstantParametricSubdivisionTechnique(resU);
                        break;
                    }

                case "cspace":
                    {
                        if (values.Length < 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", values[0], " cspace statement must specify a value."));
                        }

                        if (values.Length != 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", values[0], " cspace statement has too many values."));
                        }

                        float length = float.Parse(values[2], CultureInfo.InvariantCulture);
                        technique = new ObjConstantSpatialSubdivisionTechnique(length);
                        break;
                    }

                case "curv":
                    {
                        if (values.Length < 4)
                        {
                            throw new InvalidDataException(string.Concat("A ", values[0], " curv statement must specify a value."));
                        }

                        if (values.Length != 4)
                        {
                            throw new InvalidDataException(string.Concat("A ", values[0], " curv statement has too many values."));
                        }

                        float distance = float.Parse(values[2], CultureInfo.InvariantCulture);
                        float angle = float.Parse(values[3], CultureInfo.InvariantCulture);
                        technique = new ObjCurvatureDependentSubdivisionTechnique(distance, angle);
                        break;
                    }

                default:
                    throw new InvalidDataException(string.Concat("A ", values[0], " statement contains an unknown technique."));
            }

            return technique;
        }
    }
}
