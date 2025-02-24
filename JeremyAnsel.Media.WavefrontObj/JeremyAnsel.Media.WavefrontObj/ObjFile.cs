// <copyright file="ObjFile.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System.Text;

namespace JeremyAnsel.Media.WavefrontObj
{
    public class ObjFile
    {
        public ObjFile()
        {
            this.Vertices = new List<ObjVertex>();
            this.ParameterSpaceVertices = new List<ObjVector3>();
            this.VertexNormals = new List<ObjVector3>();
            this.TextureVertices = new List<ObjVector3>();
            this.Points = new List<ObjPoint>();
            this.Lines = new List<ObjLine>();
            this.Faces = new List<ObjFace>();
            this.Curves = new List<ObjCurve>();
            this.Curves2D = new List<ObjCurve2D>();
            this.Surfaces = new List<ObjSurface>();
            this.SurfaceConnections = new List<ObjSurfaceConnection>();
            this.DefaultGroup = new ObjGroup();
            this.Groups = new List<ObjGroup>();
            this.MergingGroupResolutions = new Dictionary<int, float>();
            this.MapLibraries = new List<string>();
            this.MaterialLibraries = new List<string>();
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
    }
}
