// <copyright file="ObjFile.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System.Numerics;
using System.Text;

namespace JeremyAnsel.Media.WavefrontObj;

public class ObjFile
{
    public ObjFile()
    {
        Vertices = new List<ObjVertex>();
        ParameterSpaceVertices = new List<ObjVector3>();
        VertexNormals = new List<ObjVector3>();
        TextureVertices = new List<ObjVector3>();
        Points = new List<ObjPoint>();
        Lines = new List<ObjLine>();
        Faces = new List<ObjFace>();
        Curves = new List<ObjCurve>();
        Curves2D = new List<ObjCurve2D>();
        Surfaces = new List<ObjSurface>();
        SurfaceConnections = new List<ObjSurfaceConnection>();
        DefaultGroup = new ObjGroup();
        Groups = new List<ObjGroup>();
        MergingGroupResolutions = new Dictionary<int, float>();
        MapLibraries = new List<string>();
        MaterialLibraries = new List<string>();
    }

    public string? HeaderText { get; set; }

    public List<ObjVertex> Vertices { get; private set; }

    public List<ObjVector3> ParameterSpaceVertices { get; private set; }

    public List<ObjVector3> VertexNormals { get; private set; }

    public List<ObjVector3> TextureVertices { get; private set; }

    public List<ObjPoint> Points { get; private set; }

    public List<ObjLine> Lines { get; private set; }

    public List<ObjFace> Faces { get; private set; }

    public List<ObjCurve> Curves { get; private set; }

    public List<ObjCurve2D> Curves2D { get; private set; }

    public List<ObjSurface> Surfaces { get; private set; }

    public List<ObjSurfaceConnection> SurfaceConnections { get; private set; }

    public ObjGroup DefaultGroup { get; private set; }

    public List<ObjGroup> Groups { get; private set; }

    public Dictionary<int, float> MergingGroupResolutions { get; private set; }

    public List<string> MapLibraries { get; private set; }

    public List<string> MaterialLibraries { get; private set; }

    public string? ShadowObjectFileName { get; set; }

    public string? TraceObjectFileName { get; set; }

    public static ObjFile FromFile(string? path)
    {
        return FromFile(path, ObjFileReaderSettings.Default);
    }

    public static ObjFile FromFile(string? path, ObjFileReaderSettings settings)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            return ObjFileReader.FromStream(stream, settings);
        }
    }

    public static ObjFile FromStream(Stream? stream)
    {
        return FromStream(stream, ObjFileReaderSettings.Default);
    }

    public static ObjFile FromStream(Stream? stream, ObjFileReaderSettings settings)
    {
        return ObjFileReader.FromStream(stream, settings);
    }

    public void WriteTo(string? path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        using (var writer = new StreamWriter(path))
        {
            ObjFileWriter.Write(this, writer);
        }
    }

    public void WriteTo(Stream? stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using (var writer = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, true))
        {
            ObjFileWriter.Write(this, writer);
        }
    }

    public void CalculateFaceNormals()
    {
        var faceAreaDirections = new Vector3[Faces.Count];
        var adjacentFacesMap = Enumerable.Range(0, Vertices.Count).Select(item => new List<int>()).ToArray();

        for (int faceindex = 0; faceindex < Faces.Count; faceindex++)
        {
            var face = Faces[faceindex];

            faceAreaDirections[faceindex] = CalculateFaceAreaDirection(face);

            foreach (var triplet in face.Vertices)
            {
                adjacentFacesMap[triplet.Vertex - 1].Add(faceindex);
            }
        }

        var reuseNormals = new Dictionary<Vector3, int>();

        for (int normalIndex = 0; normalIndex < VertexNormals.Count; ++normalIndex)
        {
            reuseNormals[VertexNormals[normalIndex].ToVector3()] = normalIndex + 1;
        }

        // 3dmax and other software interpret smooth groups as a bit mask.
        Func<ObjPolygonalElement, ObjPolygonalElement, bool> smoothFunc = (l, r) => (l.SmoothingGroupNumber & r.SmoothingGroupNumber) != 0;

        // this should be used if no actual smooth groups are defined, but right now it's not possible to tell.
        if (Faces.All(f => f.SmoothingGroupNumber == 0)) smoothFunc = (l, r) => true;

        for (int faceIndex = 0; faceIndex < Faces.Count; ++faceIndex)
        {
            var face = Faces[faceIndex];

            for (int tripletIndex = 0; tripletIndex < face.Vertices.Count; ++tripletIndex)
            {
                var v = face.Vertices[tripletIndex];
                if (v.Normal > 0) continue;

                var adjacentFaces = adjacentFacesMap[v.Vertex - 1];

                Vector3 normal = faceAreaDirections[faceIndex];

                normal = adjacentFaces
                        .Where(idx => idx != faceIndex && smoothFunc(face, Faces[idx]))
                        .Aggregate(normal, (accum, fi) => accum + faceAreaDirections[fi]);

                if (normal == Vector3.Zero)
                {
                    normal = Vertices[v.Vertex].Position.ToVector3();
                }

                normal = Vector3.Normalize(normal);

                if (float.IsNaN(normal.X) || float.IsNaN(normal.Y) || float.IsNaN(normal.Z)) throw new InvalidOperationException("invalid normals, maybe due degenerated geometry");

                if (!reuseNormals.TryGetValue(normal, out var normalIdx))
                {
                    VertexNormals.Add(normal);
                    normalIdx = VertexNormals.Count;
                    reuseNormals[normal] = normalIdx;
                }

                v.Normal = normalIdx;

                face.Vertices[tripletIndex] = v;
            }
        }
    }

    private Vector3 CalculateFaceAreaDirection(ObjFace face)
    {
        var v0 = Vertices[face.Vertices[0].Vertex - 1].Position.ToVector3();

        Vector3 n = Vector3.Zero;

        for (int k = 2; k < face.Vertices.Count; k++)
        {
            var v1 = Vertices[face.Vertices[k - 1].Vertex - 1].Position.ToVector3();
            var v2 = Vertices[face.Vertices[k].Vertex - 1].Position.ToVector3();

            n += Vector3.Cross(v1 - v0, v2 - v0) / 2;
        }

        return n; // do NOT normalize, may be zero if triangle is degenerated.
    }
}