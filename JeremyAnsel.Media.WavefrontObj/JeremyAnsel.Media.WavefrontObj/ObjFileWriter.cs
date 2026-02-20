// <copyright file="ObjFileWriter.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System.Globalization;

namespace JeremyAnsel.Media.WavefrontObj;

internal static class ObjFileWriter
{
    public static void Write(ObjFile obj, StreamWriter stream)
    {
        var context = new ObjFileWriterContext(obj);

        WriteHeaderText(obj, stream);

        WriteShadowObjectFileName(obj, stream);
        WriteTraceObjectFileName(obj, stream);

        WriteMapLibraries(obj, stream);
        WriteMaterialLibraries(obj, stream);

        WriteVertices(obj, stream);
        WriteParameterSpaceVertices(obj, stream);
        WriteVertexNormals(obj, stream);
        WriteTextureVertices(obj, stream);

        WritePoints(obj, stream, context);
        WriteLines(obj, stream, context);
        WriteFaces(obj, stream, context);

        WriteCurves(obj, stream, context);
        WriteCurves2D(obj, stream, context);
        WriteSurfaces(obj, stream, context);

        WriteSurfaceConnection(obj, stream);
    }

    private static void WriteHeaderText(ObjFile obj, StreamWriter stream)
    {
        if (string.IsNullOrEmpty(obj.HeaderText))
        {
            return;
        }

        string[] headerLines = obj.HeaderText!.Split('\n');

        foreach (string line in headerLines)
        {
            stream.Write('#');
            stream.WriteLine(line);
        }
    }

    private static void WriteShadowObjectFileName(ObjFile obj, StreamWriter stream)
    {
        if (!string.IsNullOrEmpty(obj.ShadowObjectFileName))
        {
            stream.Write(string.Format(CultureInfo.InvariantCulture, "shadow_obj {0}\n", obj.ShadowObjectFileName));
        }
    }

    private static void WriteTraceObjectFileName(ObjFile obj, StreamWriter stream)
    {
        if (!string.IsNullOrEmpty(obj.TraceObjectFileName))
        {
            stream.Write(string.Format(CultureInfo.InvariantCulture, "trace_obj {0}\n", obj.TraceObjectFileName));
        }
    }

    private static void WriteMapLibraries(ObjFile obj, StreamWriter stream)
    {
        if (obj.MapLibraries.Count == 0)
        {
            return;
        }

        stream.Write("maplib");

        foreach (string map in obj.MapLibraries)
        {
            stream.Write(" ");
            stream.Write(map);
        }

        stream.WriteLine();
    }

    private static void WriteMaterialLibraries(ObjFile obj, StreamWriter stream)
    {
        if (obj.MaterialLibraries.Count == 0)
        {
            return;
        }

        stream.Write("mtllib");

        foreach (string map in obj.MaterialLibraries)
        {
            stream.Write(" ");
            stream.Write(map);
        }

        stream.WriteLine();
    }

    private static void WriteVertices(ObjFile obj, StreamWriter stream)
    {
        foreach (var vertex in obj.Vertices)
        {
            var position = vertex.Position;

            if (vertex.Color.HasValue)
            {
                var color = vertex.Color.Value;

                if (Math.Abs(color.W - 1.0f) < float.Epsilon)
                {
                    stream.WriteLine(
                        "v {0} {1} {2} {3} {4} {5}",
                        position.X.ToString("F6", CultureInfo.InvariantCulture),
                        position.Y.ToString("F6", CultureInfo.InvariantCulture),
                        position.Z.ToString("F6", CultureInfo.InvariantCulture),
                        color.X.ToString("F6", CultureInfo.InvariantCulture),
                        color.Y.ToString("F6", CultureInfo.InvariantCulture),
                        color.Z.ToString("F6", CultureInfo.InvariantCulture));
                }
                else
                {
                    stream.WriteLine(
                        "v {0} {1} {2} {3} {4} {5} {6}",
                        position.X.ToString("F6", CultureInfo.InvariantCulture),
                        position.Y.ToString("F6", CultureInfo.InvariantCulture),
                        position.Z.ToString("F6", CultureInfo.InvariantCulture),
                        color.X.ToString("F6", CultureInfo.InvariantCulture),
                        color.Y.ToString("F6", CultureInfo.InvariantCulture),
                        color.Z.ToString("F6", CultureInfo.InvariantCulture),
                        color.W.ToString("F6", CultureInfo.InvariantCulture));
                }
            }
            else
            {
                if (Math.Abs(position.W - 1.0f) < float.Epsilon)
                {
                    stream.WriteLine(
                        "v {0} {1} {2}",
                        position.X.ToString("F6", CultureInfo.InvariantCulture),
                        position.Y.ToString("F6", CultureInfo.InvariantCulture),
                        position.Z.ToString("F6", CultureInfo.InvariantCulture));
                }
                else
                {
                    stream.WriteLine(
                        "v {0} {1} {2} {3}",
                        position.X.ToString("F6", CultureInfo.InvariantCulture),
                        position.Y.ToString("F6", CultureInfo.InvariantCulture),
                        position.Z.ToString("F6", CultureInfo.InvariantCulture),
                        position.W.ToString("F6", CultureInfo.InvariantCulture));
                }
            }
        }
    }

    private static void WriteParameterSpaceVertices(ObjFile obj, StreamWriter stream)
    {
        foreach (var vertex in obj.ParameterSpaceVertices)
        {
            if (Math.Abs(vertex.Z - 1.0f) < float.Epsilon)
            {
                if (vertex.Y == 0.0f)
                {
                    stream.WriteLine(
                        "vp {0}",
                        vertex.X.ToString("F6", CultureInfo.InvariantCulture));
                }
                else
                {
                    stream.WriteLine(
                        "vp {0} {1}",
                        vertex.X.ToString("F6", CultureInfo.InvariantCulture),
                        vertex.Y.ToString("F6", CultureInfo.InvariantCulture));
                }
            }
            else
            {
                stream.WriteLine(
                    "vp {0} {1} {2}",
                    vertex.X.ToString("F6", CultureInfo.InvariantCulture),
                    vertex.Y.ToString("F6", CultureInfo.InvariantCulture),
                    vertex.Z.ToString("F6", CultureInfo.InvariantCulture));
            }
        }
    }

    private static void WriteVertexNormals(ObjFile obj, StreamWriter stream)
    {
        foreach (var vertex in obj.VertexNormals)
        {
            stream.WriteLine(
                "vn {0} {1} {2}",
                vertex.X.ToString("F6", CultureInfo.InvariantCulture),
                vertex.Y.ToString("F6", CultureInfo.InvariantCulture),
                vertex.Z.ToString("F6", CultureInfo.InvariantCulture));
        }
    }

    private static void WriteTextureVertices(ObjFile obj, StreamWriter stream)
    {
        foreach (var vertex in obj.TextureVertices)
        {
            stream.WriteLine(
                "vt {0} {1} {2}",
                vertex.X.ToString("F6", CultureInfo.InvariantCulture),
                vertex.Y.ToString("F6", CultureInfo.InvariantCulture),
                vertex.Z.ToString("F6", CultureInfo.InvariantCulture));
        }
    }

    private static void WriteSurfaceConnection(ObjFile obj, StreamWriter stream)
    {
        foreach (ObjSurfaceConnection con in obj.SurfaceConnections)
        {
            stream.WriteLine("con {0} {1} {2} {3}",
                con.Surface1.ToString(CultureInfo.InvariantCulture),
                con.Curve2D1.ToString(),
                con.Surface2.ToString(CultureInfo.InvariantCulture),
                con.Curve2D2.ToString());
        }
    }

    private static void WritePoints(ObjFile obj, StreamWriter stream, ObjFileWriterContext context)
    {
        foreach (ObjPoint point in obj.Points)
        {
            context.WriteGroupNames(stream, point, g => g.Points);
            context.WriteAttributesOfElement(stream, point);
            context.WriteAttributesOfPolygonalElement(stream, point);

            stream.Write("p");

            foreach (ObjTriplet vertex in point.Vertices)
            {
                stream.Write(" {0}", vertex);
            }

            stream.WriteLine();
        }
    }

    private static void WriteLines(ObjFile obj, StreamWriter stream, ObjFileWriterContext context)
    {
        foreach (ObjLine line in obj.Lines)
        {
            context.WriteGroupNames(stream, line, g => g.Lines);
            context.WriteAttributesOfElement(stream, line);
            context.WriteAttributesOfPolygonalElement(stream, line);

            stream.Write("l");

            foreach (ObjTriplet vertex in line.Vertices)
            {
                stream.Write(" {0}", vertex);
            }

            stream.WriteLine();
        }
    }

    private static void WriteFaces(ObjFile obj, StreamWriter stream, ObjFileWriterContext context)
    {
        foreach (ObjFace face in obj.Faces)
        {
            context.WriteGroupNames(stream, face, g => g.Faces);
            context.WriteAttributesOfElement(stream, face);
            context.WriteAttributesOfPolygonalElement(stream, face);

            stream.Write("f");

            foreach (ObjTriplet vertex in face.Vertices)
            {
                stream.Write(" {0}", vertex);
            }

            stream.WriteLine();
        }
    }

    private static void WriteCurves(ObjFile obj, StreamWriter stream, ObjFileWriterContext context)
    {
        foreach (ObjCurve curve in obj.Curves)
        {
            context.WriteGroupNames(stream, curve, g => g.Curves);
            context.WriteAttributesOfElement(stream, curve);
            context.WriteAttributesOfFreeFormElement(stream, curve);

            stream.Write("curv {0} {1}",
                curve.StartParameter.ToString("F6", CultureInfo.InvariantCulture),
                curve.EndParameter.ToString("F6", CultureInfo.InvariantCulture));

            foreach (int vertex in curve.Vertices)
            {
                stream.Write(' ');
                stream.Write(vertex);
            }

            stream.WriteLine();

            WriteBodyOfFreeFormElement(stream, curve);
        }
    }

    private static void WriteCurves2D(ObjFile obj, StreamWriter stream, ObjFileWriterContext context)
    {
        foreach (ObjCurve2D curve in obj.Curves2D)
        {
            context.WriteGroupNames(stream, curve, g => g.Curves2D);
            context.WriteAttributesOfElement(stream, curve);
            context.WriteAttributesOfFreeFormElement(stream, curve);

            stream.Write("curv2");

            foreach (int vertex in curve.ParameterSpaceVertices)
            {
                stream.Write(' ');
                stream.Write(vertex);
            }

            stream.WriteLine();

            WriteBodyOfFreeFormElement(stream, curve);
        }
    }

    private static void WriteSurfaces(ObjFile obj, StreamWriter stream, ObjFileWriterContext context)
    {
        foreach (ObjSurface surface in obj.Surfaces)
        {
            context.WriteGroupNames(stream, surface, g => g.Surfaces);
            context.WriteAttributesOfElement(stream, surface);
            context.WriteAttributesOfFreeFormElement(stream, surface);

            stream.Write("surf {0} {1} {2} {3}",
                surface.StartParameterU.ToString("F6", CultureInfo.InvariantCulture),
                surface.EndParameterU.ToString("F6", CultureInfo.InvariantCulture),
                surface.StartParameterV.ToString("F6", CultureInfo.InvariantCulture),
                surface.EndParameterV.ToString("F6", CultureInfo.InvariantCulture));

            foreach (ObjTriplet vertex in surface.Vertices)
            {
                stream.Write(' ');
                stream.Write(vertex);
            }

            stream.WriteLine();

            WriteBodyOfFreeFormElement(stream, surface);
        }
    }

    private static void WriteBodyOfFreeFormElement(StreamWriter stream, ObjFreeFormElement element)
    {
        bool writeEnd = false;

        if (element.ParametersU.Count != 0)
        {
            writeEnd = true;
            stream.Write("parm u");

            foreach (float value in element.ParametersU)
            {
                stream.Write(' ');
                stream.Write(value.ToString("F6", CultureInfo.InvariantCulture));
            }

            stream.WriteLine();
        }

        if (element.ParametersV.Count != 0)
        {
            writeEnd = true;
            stream.Write("parm v");

            foreach (float value in element.ParametersV)
            {
                stream.Write(' ');
                stream.Write(value.ToString("F6", CultureInfo.InvariantCulture));
            }

            stream.WriteLine();
        }

        if (element.OuterTrimmingCurves.Count != 0)
        {
            writeEnd = true;
            stream.Write("trim");

            foreach (ObjCurveIndex index in element.OuterTrimmingCurves)
            {
                stream.Write(' ');
                stream.Write(index);
            }

            stream.WriteLine();
        }

        if (element.InnerTrimmingCurves.Count != 0)
        {
            writeEnd = true;
            stream.Write("hole");

            foreach (ObjCurveIndex index in element.InnerTrimmingCurves)
            {
                stream.Write(' ');
                stream.Write(index);
            }

            stream.WriteLine();
        }

        if (element.SequenceCurves.Count != 0)
        {
            writeEnd = true;
            stream.Write("scrv");

            foreach (ObjCurveIndex index in element.SequenceCurves)
            {
                stream.Write(' ');
                stream.Write(index);
            }

            stream.WriteLine();
        }

        if (element.SpecialPoints.Count != 0)
        {
            writeEnd = true;
            stream.Write("sp");

            foreach (int point in element.SpecialPoints)
            {
                stream.Write(' ');
                stream.Write(point);
            }

            stream.WriteLine();
        }

        if (writeEnd)
        {
            stream.WriteLine("end");
        }
    }
}