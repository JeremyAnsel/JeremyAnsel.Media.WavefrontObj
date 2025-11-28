// <copyright file="ObjFileReader9.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

#if NET6_0_OR_GREATER

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

#if NET9_0_OR_GREATER
using SpanSplitEnumerator = System.MemoryExtensions.SpanSplitEnumerator<char>;
#else
using SpanSplitEnumerator = Polyfills.Polyfill.SpanSplitEnumerator<char>;
#endif

namespace JeremyAnsel.Media.WavefrontObj
{
    internal static class ObjFileReader9
    {
        private static void MoveNextSkipEmpty(ref SpanSplitEnumerator values)
        {
            while (values.MoveNext())
            {
                if (values.Current.Start.Value != values.Current.End.Value)
                {
                    return;
                }
            }
        }

        private static ReadOnlySpan<char> GetNextValue(ref ReadOnlySpan<char> currentLine, ref SpanSplitEnumerator values)
        {
            MoveNextSkipEmpty(ref values);
            return currentLine[values.Current];
        }

        private static float FloatParse(ReadOnlySpan<char> s)
        {
            return float.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        private static int IntParse(ReadOnlySpan<char> s)
        {
            return int.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        private static long LongParse(ReadOnlySpan<char> s)
        {
            return long.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        [SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "Reviewed.")]
        public static ObjFile FromStream(Stream? stream, ObjFileReaderSettings settings)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var obj = new ObjFile();
            var context = new ObjFileReaderContext(obj, settings);
            var lineReader = new LineReader9();

            int valueBufferSize = 16;
            Span<char> valueBuffer = stackalloc char[valueBufferSize];

            foreach (var currentLineString in lineReader.Read9(stream))
            {
                ReadOnlySpan<char> currentLine = currentLineString.Buffer.AsSpan()[..currentLineString.Length];

                int valuesCount = 0;

                foreach (Range range in currentLine.SplitAny(LineReader9.LineSeparators))
                {
                    if (currentLine[range].Length == 0)
                    {
                        continue;
                    }

                    valuesCount++;
                }

                SpanSplitEnumerator values = currentLine.SplitAny(LineReader9.LineSeparators);

                MoveNextSkipEmpty(ref values);

                if (values.Current.End.Value - values.Current.Start.Value > valueBufferSize)
                {
                    continue;
                }

                ReadOnlySpan<char> value0 = currentLine[values.Current];
                int value0Length = value0.ToLowerInvariant(valueBuffer);

                //if (value0Length == -1)
                //{
                //    throw new InvalidDataException("the buffer is too small");
                //}

                switch (valueBuffer[..value0Length])
                {
                    case "v":
                        {
                            if (valuesCount < 4)
                            {
                                throw new InvalidDataException("A v statement must specify at least 3 values.");
                            }

                            float x = FloatParse(GetNextValue(ref currentLine, ref values));
                            float y = FloatParse(GetNextValue(ref currentLine, ref values));
                            float z = FloatParse(GetNextValue(ref currentLine, ref values));
                            float w = 1.0f;
                            bool hasColor = false;
                            float r = 0.0f;
                            float g = 0.0f;
                            float b = 0.0f;
                            float a = 1.0f;

                            if (valuesCount == 4 || valuesCount == 5)
                            {
                                if (valuesCount == 5)
                                {
                                    w = FloatParse(GetNextValue(ref currentLine, ref values));
                                }
                            }
                            else if (valuesCount == 7 || valuesCount == 8)
                            {
                                hasColor = true;
                                r = FloatParse(GetNextValue(ref currentLine, ref values));
                                g = FloatParse(GetNextValue(ref currentLine, ref values));
                                b = FloatParse(GetNextValue(ref currentLine, ref values));

                                if (valuesCount == 8)
                                {
                                    a = FloatParse(GetNextValue(ref currentLine, ref values));
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
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A vp statement must specify at least 1 value.");
                            }

                            var v = new ObjVector3();
                            v.X = FloatParse(GetNextValue(ref currentLine, ref values));

                            if (valuesCount == 2)
                            {
                                v.Y = 0.0f;
                                v.Z = 1.0f;
                            }
                            else if (valuesCount == 3)
                            {
                                v.Y = FloatParse(GetNextValue(ref currentLine, ref values));
                                v.Z = 1.0f;
                            }
                            else if (valuesCount == 4)
                            {
                                v.Y = FloatParse(GetNextValue(ref currentLine, ref values));
                                v.Z = FloatParse(GetNextValue(ref currentLine, ref values));
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
                            if (valuesCount < 4)
                            {
                                throw new InvalidDataException("A vn statement must specify 3 values.");
                            }

                            if (valuesCount != 4)
                            {
                                throw new InvalidDataException("A vn statement has too many values.");
                            }

                            var v = new ObjVector3();
                            v.X = FloatParse(GetNextValue(ref currentLine, ref values));
                            v.Y = FloatParse(GetNextValue(ref currentLine, ref values));
                            v.Z = FloatParse(GetNextValue(ref currentLine, ref values));

                            obj.VertexNormals.Add(v);
                            break;
                        }

                    case "vt":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A vt statement must specify at least 1 value.");
                            }

                            var v = new ObjVector3();
                            v.X = FloatParse(GetNextValue(ref currentLine, ref values));

                            if (valuesCount == 2)
                            {
                                v.Y = 0.0f;
                                v.Z = 0.0f;
                            }
                            else if (valuesCount == 3)
                            {
                                v.Y = FloatParse(GetNextValue(ref currentLine, ref values));
                                v.Z = 0.0f;
                            }
                            else if (valuesCount == 4)
                            {
                                v.Y = FloatParse(GetNextValue(ref currentLine, ref values));
                                v.Z = FloatParse(GetNextValue(ref currentLine, ref values));
                            }
                            else
                            {
                                throw new InvalidDataException("A vt statement has too many values.");
                            }

                            obj.TextureVertices.Add(v);
                            break;
                        }

                    case "cstype":
                        ParseFreeFormType(context, ref currentLine, ref values, valuesCount);
                        break;

                    case "deg":
                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A deg statement must specify at least 1 value.");
                        }

                        if (valuesCount == 2)
                        {
                            context.DegreeU = IntParse(GetNextValue(ref currentLine, ref values));
                            context.DegreeV = 0;
                        }
                        else if (valuesCount == 3)
                        {
                            context.DegreeU = IntParse(GetNextValue(ref currentLine, ref values));
                            context.DegreeV = IntParse(GetNextValue(ref currentLine, ref values));
                        }
                        else
                        {
                            throw new InvalidDataException("A deg statement has too many values.");
                        }

                        break;

                    case "bmat":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A bmat statement must specify a direction.");
                            }

                            int d;
                            var value1 = GetNextValue(ref currentLine, ref values);

                            if (value1.Equals("u", StringComparison.OrdinalIgnoreCase))
                            {
                                d = 1;
                            }
                            else if (value1.Equals("v", StringComparison.OrdinalIgnoreCase))
                            {
                                d = 2;
                            }
                            else
                            {
                                throw new InvalidDataException("A bmat statement has an unknown direction.");
                            }

                            int count = (context.DegreeU + 1) * (context.DegreeV + 1);

                            if (valuesCount != count + 2)
                            {
                                throw new InvalidDataException("A bmat statement has too many or too few values.");
                            }

                            var matrix = new float[count];

                            for (int i = 0; i < count; i++)
                            {
                                matrix[i] = FloatParse(GetNextValue(ref currentLine, ref values));
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
                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A step statement must specify at least 1 value.");
                        }

                        if (valuesCount == 2)
                        {
                            context.StepU = FloatParse(GetNextValue(ref currentLine, ref values));
                            context.StepV = 1.0f;
                        }
                        else if (valuesCount == 3)
                        {
                            context.StepU = FloatParse(GetNextValue(ref currentLine, ref values));
                            context.StepV = FloatParse(GetNextValue(ref currentLine, ref values));
                        }
                        else
                        {
                            throw new InvalidDataException("A step statement has too many values.");
                        }

                        break;

                    case "p":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A p statement must specify at least 1 value.");
                            }

                            var point = new ObjPoint();

                            for (int i = 1; i < valuesCount; i++)
                            {
                                point.Vertices.Add(ParseTriplet(obj, GetNextValue(ref currentLine, ref values)));
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
                            if (valuesCount < 3)
                            {
                                throw new InvalidDataException("A l statement must specify at least 2 values.");
                            }

                            var line = new ObjLine();

                            for (int i = 1; i < valuesCount; i++)
                            {
                                line.Vertices.Add(ParseTriplet(obj, GetNextValue(ref currentLine, ref values)));
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
                            if (valuesCount < 4)
                            {
                                throw new InvalidDataException("A f statement must specify at least 3 values.");
                            }

                            var face = new ObjFace();

                            for (int i = 1; i < valuesCount; i++)
                            {
                                face.Vertices.Add(ParseTriplet(obj, GetNextValue(ref currentLine, ref values)));
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
                            if (valuesCount < 5)
                            {
                                throw new InvalidDataException("A curv statement must specify at least 4 values.");
                            }

                            var curve = new ObjCurve();

                            curve.StartParameter = FloatParse(GetNextValue(ref currentLine, ref values));
                            curve.EndParameter = FloatParse(GetNextValue(ref currentLine, ref values));

                            for (int i = 3; i < valuesCount; i++)
                            {
                                int v = IntParse(GetNextValue(ref currentLine, ref values));

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
                            if (valuesCount < 3)
                            {
                                throw new InvalidDataException("A curv2 statement must specify at least 2 values.");
                            }

                            var curve = new ObjCurve2D();

                            for (int i = 1; i < valuesCount; i++)
                            {
                                int vp = IntParse(GetNextValue(ref currentLine, ref values));

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
                            if (valuesCount < 6)
                            {
                                throw new InvalidDataException("A surf statement must specify at least 5 values.");
                            }

                            var surface = new ObjSurface();

                            surface.StartParameterU = FloatParse(GetNextValue(ref currentLine, ref values));
                            surface.EndParameterU = FloatParse(GetNextValue(ref currentLine, ref values));
                            surface.StartParameterV = FloatParse(GetNextValue(ref currentLine, ref values));
                            surface.EndParameterV = FloatParse(GetNextValue(ref currentLine, ref values));

                            for (int i = 5; i < valuesCount; i++)
                            {
                                surface.Vertices.Add(ParseTriplet(obj, GetNextValue(ref currentLine, ref values)));
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
                        {
                            if (context.CurrentFreeFormElement == null)
                            {
                                break;
                            }

                            if (valuesCount < 4)
                            {
                                throw new InvalidDataException("A parm statement must specify at least 3 values.");
                            }

                            List<float> parameters;
                            var value1 = GetNextValue(ref currentLine, ref values);

                            if (value1.Equals("u", StringComparison.OrdinalIgnoreCase))
                            {
                                parameters = context.CurrentFreeFormElement.ParametersU;
                            }
                            else if (value1.Equals("v", StringComparison.OrdinalIgnoreCase))
                            {
                                parameters = context.CurrentFreeFormElement.ParametersV;
                            }
                            else
                            {
                                throw new InvalidDataException("A parm statement has an unknown direction.");
                            }

                            for (int i = 2; i < valuesCount; i++)
                            {
                                parameters.Add(FloatParse(GetNextValue(ref currentLine, ref values)));
                            }

                            break;
                        }

                    case "trim":
                        if (context.CurrentFreeFormElement == null)
                        {
                            break;
                        }

                        ParseCurveIndex(context.CurrentFreeFormElement.OuterTrimmingCurves, obj, value0, ref currentLine, ref values, valuesCount);
                        break;

                    case "hole":
                        if (context.CurrentFreeFormElement == null)
                        {
                            break;
                        }

                        ParseCurveIndex(context.CurrentFreeFormElement.InnerTrimmingCurves, obj, value0, ref currentLine, ref values, valuesCount);
                        break;

                    case "scrv":
                        if (context.CurrentFreeFormElement == null)
                        {
                            break;
                        }

                        ParseCurveIndex(context.CurrentFreeFormElement.SequenceCurves, obj, value0, ref currentLine, ref values, valuesCount);
                        break;

                    case "sp":
                        if (context.CurrentFreeFormElement == null)
                        {
                            break;
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A sp statement must specify at least 1 value.");
                        }

                        for (int i = 1; i < valuesCount; i++)
                        {
                            int vp = IntParse(GetNextValue(ref currentLine, ref values));

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
                        ParseSurfaceConnection(obj, ref currentLine, ref values, valuesCount);
                        break;

                    case "g":
                        ParseGroupName(context, value0, ref currentLine, ref values, valuesCount);
                        break;

                    case "s":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A s statement must specify a value.");
                            }

                            if (valuesCount != 2)
                            {
                                throw new InvalidDataException("A s statement has too many values.");
                            }

                            var value1 = GetNextValue(ref currentLine, ref values);

                            if (value1.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                context.SmoothingGroupNumber = 0;
                            }
                            else
                            {
                                context.SmoothingGroupNumber = LongParse(value1);
                            }

                            break;
                        }

                    case "mg":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A mg statement must specify a value.");
                            }

                            var value1 = GetNextValue(ref currentLine, ref values);

                            if (value1.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                context.MergingGroupNumber = 0;
                            }
                            else
                            {
                                context.MergingGroupNumber = IntParse(value1);
                            }

                            if (context.MergingGroupNumber == 0)
                            {
                                if (valuesCount > 3)
                                {
                                    throw new InvalidDataException("A mg statement has too many values.");
                                }
                            }
                            else
                            {
                                if (valuesCount != 3)
                                {
                                    throw new InvalidDataException("A mg statement has too many or too few values.");
                                }

                                float res = FloatParse(GetNextValue(ref currentLine, ref values));

                                obj.MergingGroupResolutions[context.MergingGroupNumber] = res;
                            }

                            break;
                        }

                    case "o":
                        if (settings.HandleObjectNamesAsGroup)
                        {
                            ParseGroupName(context, value0, ref currentLine, ref values, valuesCount);
                            break;
                        }

                        if (valuesCount == 1)
                        {
                            context.ObjectName = null;
                            break;
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A o statement has too many values.");
                        }

                        context.ObjectName = GetNextValue(ref currentLine, ref values).ToString();
                        break;

                    case "bevel":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A bevel statement must specify a name.");
                            }

                            if (valuesCount != 2)
                            {
                                throw new InvalidDataException("A bevel statement has too many values.");
                            }

                            var value1 = GetNextValue(ref currentLine, ref values);

                            if (value1.Equals("on", StringComparison.OrdinalIgnoreCase))
                            {
                                context.IsBevelInterpolationEnabled = true;
                            }
                            else if (value1.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                context.IsBevelInterpolationEnabled = false;
                            }
                            else
                            {
                                throw new InvalidDataException("A bevel statement must specify on or off.");
                            }

                            break;
                        }

                    case "c_interp":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A c_interp statement must specify a name.");
                            }

                            if (valuesCount != 2)
                            {
                                throw new InvalidDataException("A c_interp statement has too many values.");
                            }

                            var value1 = GetNextValue(ref currentLine, ref values);

                            if (value1.Equals("on", StringComparison.OrdinalIgnoreCase))
                            {
                                context.IsColorInterpolationEnabled = true;
                            }
                            else if (value1.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                context.IsColorInterpolationEnabled = false;
                            }
                            else
                            {
                                throw new InvalidDataException("A c_interp statement must specify on or off.");
                            }

                            break;
                        }

                    case "d_interp":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A d_interp statement must specify a name.");
                            }

                            if (valuesCount != 2)
                            {
                                throw new InvalidDataException("A d_interp statement has too many values.");
                            }

                            var value1 = GetNextValue(ref currentLine, ref values);

                            if (value1.Equals("on", StringComparison.OrdinalIgnoreCase))
                            {
                                context.IsDissolveInterpolationEnabled = true;
                            }
                            else if (value1.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                context.IsDissolveInterpolationEnabled = false;
                            }
                            else
                            {
                                throw new InvalidDataException("A d_interp statement must specify on or off.");
                            }

                            break;
                        }

                    case "lod":
                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A lod statement must specify a value.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A lod statement has too many values.");
                        }

                        context.LevelOfDetail = IntParse(GetNextValue(ref currentLine, ref values));
                        break;

                    case "maplib":
                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A maplib statement must specify a file name.");
                        }

                        for (int i = 1; i < valuesCount; i++)
                        {
                            obj.MapLibraries.Add(GetNextValue(ref currentLine, ref values).ToString());
                        }

                        break;

                    case "mtllib":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A mtllib statement must specify a file name.");
                            }
                            
                            if (settings.KeepWhitespacesOfMtlLibReferences)
                            {
                                obj.MaterialLibraries.Add(new string(currentLine[7..]).Trim());
                            }
                            else
                            {
                                var sb = new StringBuilder();

                                sb.Append(GetNextValue(ref currentLine, ref values));

                                for (int i = 2; i < valuesCount; i++)
                                {
                                    sb.Append(' ');
                                    sb.Append(GetNextValue(ref currentLine, ref values));
                                }

                                obj.MaterialLibraries.Add(sb.ToString());
                            }

                            break;
                        }

                    case "usemap":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A usemap statement must specify a value.");
                            }

                            if (valuesCount != 2)
                            {
                                throw new InvalidDataException("A usemap statement has too many values.");
                            }

                            var value1 = GetNextValue(ref currentLine, ref values);

                            if (value1.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                context.MapName = null;
                            }
                            else
                            {
                                context.MapName = value1.ToString();
                            }

                            break;
                        }

                    case "usemtl":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A usemtl statement must specify a value.");
                            }

                            var value1 = GetNextValue(ref currentLine, ref values);

                            if (value1.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                if (valuesCount != 2)
                                {
                                    throw new InvalidDataException("A usemtl statement has too many values.");
                                }

                                context.MaterialName = null;
                            }
                            else
                            {
                                var sb = new StringBuilder();

                                sb.Append(value1);

                                for (int i = 2; i < valuesCount; i++)
                                {
                                    sb.Append(' ');
                                    sb.Append(GetNextValue(ref currentLine, ref values));
                                }

                                context.MaterialName = sb.ToString();
                            }

                            break;
                        }

                    case "shadow_obj":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A shadow_obj statement must specify a file name.");
                            }

                            var sb = new StringBuilder();

                            sb.Append(GetNextValue(ref currentLine, ref values));

                            for (int i = 2; i < valuesCount; i++)
                            {
                                sb.Append(' ');
                                sb.Append(GetNextValue(ref currentLine, ref values));
                            }

                            obj.ShadowObjectFileName = sb.ToString();
                            break;
                        }

                    case "trace_obj":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A trace_obj statement must specify a file name.");
                            }

                            var sb = new StringBuilder();

                            sb.Append(GetNextValue(ref currentLine, ref values));

                            for (int i = 2; i < valuesCount; i++)
                            {
                                sb.Append(' ');
                                sb.Append(GetNextValue(ref currentLine, ref values));
                            }

                            obj.TraceObjectFileName = sb.ToString();
                            break;
                        }

                    case "ctech":
                        context.CurveApproximationTechnique = ParseApproximationTechnique(value0, ref currentLine, ref values, valuesCount);
                        break;

                    case "stech":
                        context.SurfaceApproximationTechnique = ParseApproximationTechnique(value0, ref currentLine, ref values, valuesCount);
                        break;

                    case "bsp":
                    case "bzp":
                    case "cdc":
                    case "cdp":
                    case "res":
                        throw new NotImplementedException(string.Concat(value0.ToString(), " statement have been replaced by free-form geometry statements."));
                }
            }

            obj.HeaderText = string.Join("\n", lineReader.HeaderTextLines.ToArray());

            return obj;
        }

        [SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "Reviewed.")]
        private static ObjTriplet ParseTriplet(ObjFile obj, ReadOnlySpan<char> value)
        {
            int valuesCount = 0;

            foreach (var _ in value.Split('/'))
            {
                valuesCount++;
            }

            var values = value.Split('/');

            if (valuesCount > 3)
            {
                throw new InvalidDataException("A triplet has too many values.");
            }

            values.MoveNext();
            int v = value[values.Current].Length != 0 ? IntParse(value[values.Current]) : 0;

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

            values.MoveNext();
            int vt = valuesCount > 1 && value[values.Current].Length != 0 ? IntParse(value[values.Current]) : 0;

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

            values.MoveNext();
            int vn = valuesCount > 2 && value[values.Current].Length != 0 ? IntParse(value[values.Current]) : 0;

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

        [SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "Reviewed.")]
        private static ObjCurveIndex ParseCurveIndex(ObjFile obj, ref ReadOnlySpan<char> currentLine, ref SpanSplitEnumerator values)
        {
            float start = FloatParse(GetNextValue(ref currentLine, ref values));
            float end = FloatParse(GetNextValue(ref currentLine, ref values));
            int curve2D = IntParse(GetNextValue(ref currentLine, ref values));

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

        private static void ParseCurveIndex(List<ObjCurveIndex> curves, ObjFile obj, ReadOnlySpan<char> value0, ref ReadOnlySpan<char> currentLine, ref SpanSplitEnumerator values, int valuesCount)
        {
            if (valuesCount < 4)
            {
                throw new InvalidDataException(string.Concat("A ", value0.ToString(), " statement must specify at least 3 value."));
            }

            if ((valuesCount - 1) % 3 != 0)
            {
                throw new InvalidDataException(string.Concat("A ", value0.ToString(), " statement has too many values."));
            }

            for (int i = 1; i < valuesCount; i += 3)
            {
                curves.Add(ParseCurveIndex(obj, ref currentLine, ref values));
            }
        }

        [SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "Reviewed.")]
        private static void ParseFreeFormType(ObjFileReaderContext context, ref ReadOnlySpan<char> currentLine, ref SpanSplitEnumerator values, int valuesCount)
        {
            if (valuesCount < 2)
            {
                throw new InvalidDataException("A cstype statement must specify a value.");
            }

            string type;

            if (valuesCount == 2)
            {
                context.IsRationalForm = false;
                type = GetNextValue(ref currentLine, ref values).ToString();
            }
            else if (valuesCount == 3 && GetNextValue(ref currentLine, ref values).Equals("rat", StringComparison.OrdinalIgnoreCase))
            {
                context.IsRationalForm = true;
                type = GetNextValue(ref currentLine, ref values).ToString();
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

        private static void ParseSurfaceConnection(ObjFile obj, ref ReadOnlySpan<char> currentLine, ref SpanSplitEnumerator values, int valuesCount)
        {
            if (valuesCount < 9)
            {
                throw new InvalidDataException("A con statement must specify 8 values.");
            }

            if (valuesCount != 9)
            {
                throw new InvalidDataException("A con statement has too many values.");
            }

            int surface1 = IntParse(GetNextValue(ref currentLine, ref values));

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

            var curve1 = ParseCurveIndex(obj, ref currentLine, ref values);

            int surface2 = IntParse(GetNextValue(ref currentLine, ref values));

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

            var curve2 = ParseCurveIndex(obj, ref currentLine, ref values);

            var connection = new ObjSurfaceConnection
            {
                Surface1 = surface1,
                Curve2D1 = curve1,
                Surface2 = surface2,
                Curve2D2 = curve2
            };

            obj.SurfaceConnections.Add(connection);
        }

        private static void ParseGroupName(ObjFileReaderContext context, ReadOnlySpan<char> value0, ref ReadOnlySpan<char> currentLine, ref SpanSplitEnumerator values, int valuesCount)
        {
            context.GroupNames.Clear();

            if (context.Settings.OnlyOneGroupNamePerLine)
            {
                var sb = new StringBuilder();

                sb.Append(GetNextValue(ref currentLine, ref values));

                for (int i = 2; i < valuesCount; i++)
                {
                    sb.Append(' ');
                    sb.Append(GetNextValue(ref currentLine, ref values));
                }


                var name = sb.ToString();
                if (!string.Equals(name, "default", StringComparison.OrdinalIgnoreCase))
                {
                    context.GroupNames.Add(name);
                }
            }
            else
            {
                for (int i = 1; i < valuesCount; i++)
                {
                    var name = GetNextValue(ref currentLine, ref values).ToString();

                    if (!string.Equals(name, "default", StringComparison.OrdinalIgnoreCase))
                    {
                        context.GroupNames.Add(name);
                    }
                }
            }

            context.GetCurrentGroups();
        }

        private static ObjApproximationTechnique ParseApproximationTechnique(ReadOnlySpan<char> value0, ref ReadOnlySpan<char> currentLine, ref SpanSplitEnumerator values, int valuesCount)
        {
            ObjApproximationTechnique technique;

            if (valuesCount < 2)
            {
                throw new InvalidDataException(string.Concat("A ", value0.ToString(), " statement must specify a technique."));
            }

            string value1 = GetNextValue(ref currentLine, ref values).ToString().ToLowerInvariant();

            switch (value1)
            {
                case "cparm":
                    {
                        if (valuesCount < 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", value0.ToString(), " cparm statement must specify a value."));
                        }

                        if (valuesCount != 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", value0.ToString(), " cparm statement has too many values."));
                        }

                        float res = FloatParse(GetNextValue(ref currentLine, ref values));
                        technique = new ObjConstantParametricSubdivisionTechnique(res);
                        break;
                    }

                case "cparma":
                    {
                        if (valuesCount < 4)
                        {
                            throw new InvalidDataException(string.Concat("A ", value0.ToString(), " cparma statement must specify a value."));
                        }

                        if (valuesCount != 4)
                        {
                            throw new InvalidDataException(string.Concat("A ", value0.ToString(), " cparma statement has too many values."));
                        }

                        float resU = FloatParse(GetNextValue(ref currentLine, ref values));
                        float resV = FloatParse(GetNextValue(ref currentLine, ref values));
                        technique = new ObjConstantParametricSubdivisionTechnique(resU, resV);
                        break;
                    }

                case "cparmb":
                    {
                        if (valuesCount < 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", value0.ToString(), " cparmb statement must specify a value."));
                        }

                        if (valuesCount != 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", value0.ToString(), " cparmb statement has too many values."));
                        }

                        float resU = FloatParse(GetNextValue(ref currentLine, ref values));
                        technique = new ObjConstantParametricSubdivisionTechnique(resU);
                        break;
                    }

                case "cspace":
                    {
                        if (valuesCount < 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", value0.ToString(), " cspace statement must specify a value."));
                        }

                        if (valuesCount != 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", value0.ToString(), " cspace statement has too many values."));
                        }

                        float length = FloatParse(GetNextValue(ref currentLine, ref values));
                        technique = new ObjConstantSpatialSubdivisionTechnique(length);
                        break;
                    }

                case "curv":
                    {
                        if (valuesCount < 4)
                        {
                            throw new InvalidDataException(string.Concat("A ", value0.ToString(), " curv statement must specify a value."));
                        }

                        if (valuesCount != 4)
                        {
                            throw new InvalidDataException(string.Concat("A ", value0.ToString(), " curv statement has too many values."));
                        }

                        float distance = FloatParse(GetNextValue(ref currentLine, ref values));
                        float angle = FloatParse(GetNextValue(ref currentLine, ref values));
                        technique = new ObjCurvatureDependentSubdivisionTechnique(distance, angle);
                        break;
                    }

                default:
                    throw new InvalidDataException(string.Concat("A ", value0.ToString(), " statement contains an unknown technique."));
            }

            return technique;
        }
    }
}

#endif