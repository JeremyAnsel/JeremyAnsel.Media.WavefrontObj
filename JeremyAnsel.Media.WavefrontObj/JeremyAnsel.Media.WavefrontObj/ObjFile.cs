// <copyright file="ObjFile.cs" company="Jérémy Ansel">
// Copyright (c) 2017 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public IList<ObjVertex> Vertices { get; private set; }

        public IList<ObjVector3> ParameterSpaceVertices { get; private set; }

        public IList<ObjVector3> VertexNormals { get; private set; }

        public IList<ObjVector3> TextureVertices { get; private set; }

        public IList<ObjPoint> Points { get; private set; }

        public IList<ObjLine> Lines { get; private set; }

        public IList<ObjFace> Faces { get; private set; }

        public IList<ObjCurve> Curves { get; private set; }

        public IList<ObjCurve2D> Curves2D { get; private set; }

        public IList<ObjSurface> Surfaces { get; private set; }

        public IList<ObjSurfaceConnection> SurfaceConnections { get; private set; }

        public ObjGroup DefaultGroup { get; private set; }

        public IList<ObjGroup> Groups { get; private set; }

        public IDictionary<int, float> MergingGroupResolutions { get; private set; }

        public IList<string> MapLibraries { get; private set; }

        public IList<string> MaterialLibraries { get; private set; }

        public string ShadowObjectFileName { get; set; }

        public string TraceObjectFileName { get; set; }

        public static ObjFile FromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return ObjFileReader.FromStream(stream);
            }
        }

        public static ObjFile FromStream(Stream stream)
        {
            return ObjFileReader.FromStream(stream);
        }

        public void WriteTo(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                ObjFileWriter.Write(this, writer);
            }
        }

        public void WriteTo(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (var writer = new StreamWriter(stream))
            {
                ObjFileWriter.Write(this, writer);
            }
        }
    }
}
