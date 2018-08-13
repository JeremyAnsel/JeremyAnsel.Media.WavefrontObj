// <copyright file="ObjFileWriterTests.cs" company="Jérémy Ansel">
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
using System.Threading.Tasks;
using Xunit;

namespace JeremyAnsel.Media.WavefrontObj.Tests
{
    public class ObjFileWriterTests
    {
        [Fact]
        public void Write_New_Valid()
        {
            var obj = new ObjFile();

            string text = WriteObj(obj);
            string expected =
@"";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void ObjectName_Valid()
        {
            var obj = new ObjFile();
            var point1 = new ObjPoint();
            point1.Vertices.Add(new ObjTriplet(1, 0, 0));
            point1.ObjectName = "a";
            obj.Points.Add(point1);
            var point2 = new ObjPoint();
            point2.Vertices.Add(new ObjTriplet(1, 0, 0));
            obj.Points.Add(point2);

            string text = WriteObj(obj);
            string expected =
@"o a
p 1
o
p 1
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void RenderAttributes_LevelOfDetail_Valid()
        {
            var obj = new ObjFile();
            var point = new ObjPoint();
            point.Vertices.Add(new ObjTriplet(1, 0, 0));
            point.LevelOfDetail = 2;
            obj.Points.Add(point);

            string text = WriteObj(obj);
            string expected =
@"lod 2
p 1
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void RenderAttributes_MapLibrary_Valid()
        {
            var obj = new ObjFile();
            obj.MapLibraries.Add("a.a");
            obj.MapLibraries.Add("b.b");

            string text = WriteObj(obj);
            string expected =
@"maplib a.a b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void RenderAttributes_MaterialLibrary_Valid()
        {
            var obj = new ObjFile();
            obj.MaterialLibraries.Add("a.a");
            obj.MaterialLibraries.Add("b.b");

            string text = WriteObj(obj);
            string expected =
@"mtllib a.a b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData(null, "off")]
        [InlineData("a", "a")]
        public void RenderAttributes_UseMap_Valid(string value, string name)
        {
            var obj = new ObjFile();
            var point1 = new ObjPoint();
            point1.Vertices.Add(new ObjTriplet(1, 0, 0));
            point1.MapName = "b";
            obj.Points.Add(point1);
            var point2 = new ObjPoint();
            point2.Vertices.Add(new ObjTriplet(1, 0, 0));
            point2.MapName = value;
            obj.Points.Add(point2);

            string text = WriteObj(obj);
            string expected =
@"usemap b
p 1
usemap " + name + @"
p 1
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData(null, "off")]
        [InlineData("a", "a")]
        public void RenderAttributes_UseMaterial_Valid(string value, string name)
        {
            var obj = new ObjFile();
            var point1 = new ObjPoint();
            point1.Vertices.Add(new ObjTriplet(1, 0, 0));
            point1.MaterialName = "b";
            obj.Points.Add(point1);
            var point2 = new ObjPoint();
            point2.Vertices.Add(new ObjTriplet(1, 0, 0));
            point2.MaterialName = value;
            obj.Points.Add(point2);

            string text = WriteObj(obj);
            string expected =
@"usemtl b
p 1
usemtl " + name + @"
p 1
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData(0, "off")]
        [InlineData(2, "2")]
        public void SmoothingGroup_Valid(int number, string value)
        {
            var obj = new ObjFile();
            var point1 = new ObjPoint();
            point1.Vertices.Add(new ObjTriplet(1, 0, 0));
            point1.SmoothingGroupNumber = 1;
            obj.Points.Add(point1);
            var point2 = new ObjPoint();
            point2.Vertices.Add(new ObjTriplet(1, 0, 0));
            point2.SmoothingGroupNumber = number;
            obj.Points.Add(point2);

            string text = WriteObj(obj);
            string expected =
@"s 1
p 1
s " + value + @"
p 1
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void RenderAttributes_Bevel_Valid()
        {
            var obj = new ObjFile();
            var point1 = new ObjPoint();
            point1.Vertices.Add(new ObjTriplet(1, 0, 0));
            point1.IsBevelInterpolationEnabled = true;
            obj.Points.Add(point1);
            var point2 = new ObjPoint();
            point2.Vertices.Add(new ObjTriplet(1, 0, 0));
            point2.IsBevelInterpolationEnabled = false;
            obj.Points.Add(point2);

            string text = WriteObj(obj);
            string expected =
@"bevel on
p 1
bevel off
p 1
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void RenderAttributes_ColorInterpolation_Valid()
        {
            var obj = new ObjFile();
            var point1 = new ObjPoint();
            point1.Vertices.Add(new ObjTriplet(1, 0, 0));
            point1.IsColorInterpolationEnabled = true;
            obj.Points.Add(point1);
            var point2 = new ObjPoint();
            point2.Vertices.Add(new ObjTriplet(1, 0, 0));
            point2.IsColorInterpolationEnabled = false;
            obj.Points.Add(point2);

            string text = WriteObj(obj);
            string expected =
@"c_interp on
p 1
c_interp off
p 1
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void RenderAttributes_DissolveInterpolation_Valid()
        {
            var obj = new ObjFile();
            var point1 = new ObjPoint();
            point1.Vertices.Add(new ObjTriplet(1, 0, 0));
            point1.IsDissolveInterpolationEnabled = true;
            obj.Points.Add(point1);
            var point2 = new ObjPoint();
            point2.Vertices.Add(new ObjTriplet(1, 0, 0));
            point2.IsDissolveInterpolationEnabled = false;
            obj.Points.Add(point2);

            string text = WriteObj(obj);
            string expected =
@"d_interp on
p 1
d_interp off
p 1
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Vertex_Geometric3_Valid()
        {
            var obj = new ObjFile();
            obj.Vertices.Add(new ObjVertex(2.0f, 3.0f, 4.0f, 1.0f));

            string text = WriteObj(obj);
            string expected =
@"v 2.000000 3.000000 4.000000
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Vertex_Geometric4_Valid()
        {
            var obj = new ObjFile();
            obj.Vertices.Add(new ObjVertex(2.0f, 3.0f, 4.0f, 5.0f));

            string text = WriteObj(obj);
            string expected =
@"v 2.000000 3.000000 4.000000 5.000000
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Vertex_Geometric6_Valid()
        {
            var obj = new ObjFile();
            obj.Vertices.Add(new ObjVertex(2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 1.0f));

            string text = WriteObj(obj);
            string expected =
@"v 2.000000 3.000000 4.000000 5.000000 6.000000 7.000000
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Vertex_Geometric7_Valid()
        {
            var obj = new ObjFile();
            obj.Vertices.Add(new ObjVertex(2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f));

            string text = WriteObj(obj);
            string expected =
@"v 2.000000 3.000000 4.000000 5.000000 6.000000 7.000000 8.000000
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Vertex_ParameterSpace1_Valid()
        {
            var obj = new ObjFile();
            obj.ParameterSpaceVertices.Add(new ObjVector3(2.0f, 0.0f, 1.0f));

            string text = WriteObj(obj);
            string expected =
@"vp 2.000000
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Vertex_ParameterSpace2_Valid()
        {
            var obj = new ObjFile();
            obj.ParameterSpaceVertices.Add(new ObjVector3(2.0f, 3.0f, 1.0f));

            string text = WriteObj(obj);
            string expected =
@"vp 2.000000 3.000000
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Vertex_ParameterSpace3_Valid()
        {
            var obj = new ObjFile();
            obj.ParameterSpaceVertices.Add(new ObjVector3(2.0f, 3.0f, 4.0f));

            string text = WriteObj(obj);
            string expected =
@"vp 2.000000 3.000000 4.000000
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Vertex_Normal_Valid()
        {
            var obj = new ObjFile();
            obj.VertexNormals.Add(new ObjVector3(2.0f, 3.0f, 4.0f));

            string text = WriteObj(obj);
            string expected =
@"vn 2.000000 3.000000 4.000000
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Vertex_Texture1_Valid()
        {
            var obj = new ObjFile();
            obj.TextureVertices.Add(new ObjVector3(2.0f, 0.0f, 0.0f));

            string text = WriteObj(obj);
            string expected =
@"vt 2.000000
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Vertex_Texture2_Valid()
        {
            var obj = new ObjFile();
            obj.TextureVertices.Add(new ObjVector3(2.0f, 3.0f, 0.0f));

            string text = WriteObj(obj);
            string expected =
@"vt 2.000000 3.000000
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Vertex_Texture3_Valid()
        {
            var obj = new ObjFile();
            obj.TextureVertices.Add(new ObjVector3(2.0f, 3.0f, 4.0f));

            string text = WriteObj(obj);
            string expected =
@"vt 2.000000 3.000000 4.000000
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void SurfaceConnection_Valid()
        {
            var obj = new ObjFile();
            obj.SurfaceConnections.Add(new ObjSurfaceConnection
            {
                Surface1 = 2,
                Curve2D1 = new ObjCurveIndex(6.0f, 7.0f, 3),
                Surface2 = 4,
                Curve2D2 = new ObjCurveIndex(8.0f, 9.0f, 5)
            });

            string text = WriteObj(obj);
            string expected =
@"con 2 6.000000 7.000000 3 4 8.000000 9.000000 5
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Group_Equal_Valid()
        {
            var obj = new ObjFile();
            obj.Groups.Add(new ObjGroup("a"));
            var point1 = new ObjPoint();
            point1.Vertices.Add(new ObjTriplet(1, 0, 0));
            obj.Points.Add(point1);
            obj.Groups[0].Points.Add(point1);
            var point2 = new ObjPoint();
            point2.Vertices.Add(new ObjTriplet(2, 0, 0));
            obj.Points.Add(point2);
            obj.Groups[0].Points.Add(point2);

            string text = WriteObj(obj);
            string expected =
@"g a
p 1
p 2
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Group_Multiple_Valid()
        {
            var obj = new ObjFile();
            obj.Groups.Add(new ObjGroup("a"));
            obj.Groups.Add(new ObjGroup("b"));
            var point = new ObjPoint();
            point.Vertices.Add(new ObjTriplet(1, 0, 0));
            obj.Points.Add(point);
            obj.Groups[0].Points.Add(point);
            obj.Groups[1].Points.Add(point);

            string text = WriteObj(obj);
            string expected =
@"g a b
p 1
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Group_Default_Valid()
        {
            var obj = new ObjFile();
            obj.Groups.Add(new ObjGroup("a"));
            var point1 = new ObjPoint();
            point1.Vertices.Add(new ObjTriplet(1, 0, 0));
            obj.Points.Add(point1);
            obj.Groups[0].Points.Add(point1);
            var point2 = new ObjPoint();
            point2.Vertices.Add(new ObjTriplet(2, 0, 0));
            obj.Points.Add(point2);

            string text = WriteObj(obj);
            string expected =
@"g a
p 1
g default
p 2
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void RenderAttributes_ShadowObject_Valid()
        {
            var obj = new ObjFile();
            obj.ShadowObjectFileName = "a.a";

            string text = WriteObj(obj);
            string expected =
@"shadow_obj a.a
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void RenderAttributes_TraceObject_Valid()
        {
            var obj = new ObjFile();
            obj.TraceObjectFileName = "a.a";

            string text = WriteObj(obj);
            string expected =
@"trace_obj a.a
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Element_Point_Valid()
        {
            var obj = new ObjFile();
            obj.Groups.Add(new ObjGroup("b"));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            var point = new ObjPoint();
            point.Vertices.Add(new ObjTriplet(2, 0, 0));
            point.Vertices.Add(new ObjTriplet(3, 0, 0));
            obj.Points.Add(point);
            obj.Groups[0].Points.Add(point);
            point.ObjectName = "a";
            point.LevelOfDetail = 2;
            point.MapName = "c";
            point.MaterialName = "d";
            point.SmoothingGroupNumber = 10;
            point.IsBevelInterpolationEnabled = true;
            point.IsColorInterpolationEnabled = true;
            point.IsDissolveInterpolationEnabled = true;

            string text = WriteObj(obj);
            string expected =
@"v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
g b
o a
lod 2
usemap c
usemtl d
s 10
bevel on
c_interp on
d_interp on
p 2 3
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Element_Line_Valid()
        {
            var obj = new ObjFile();
            obj.Groups.Add(new ObjGroup("b"));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            var line = new ObjLine();
            line.Vertices.Add(new ObjTriplet(2, 0, 0));
            line.Vertices.Add(new ObjTriplet(3, 0, 0));
            line.Vertices.Add(new ObjTriplet(4, 0, 0));
            obj.Lines.Add(line);
            obj.Groups[0].Lines.Add(line);
            line.ObjectName = "a";
            line.LevelOfDetail = 2;
            line.MapName = "c";
            line.MaterialName = "d";
            line.SmoothingGroupNumber = 10;
            line.IsBevelInterpolationEnabled = true;
            line.IsColorInterpolationEnabled = true;
            line.IsDissolveInterpolationEnabled = true;

            string text = WriteObj(obj);
            string expected =
@"v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
g b
o a
lod 2
usemap c
usemtl d
s 10
bevel on
c_interp on
d_interp on
l 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Element_Face_Valid()
        {
            var obj = new ObjFile();
            obj.Groups.Add(new ObjGroup("b"));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            var face = new ObjFace();
            face.Vertices.Add(new ObjTriplet(2, 0, 0));
            face.Vertices.Add(new ObjTriplet(3, 0, 0));
            face.Vertices.Add(new ObjTriplet(4, 0, 0));
            face.Vertices.Add(new ObjTriplet(5, 0, 0));
            face.Vertices.Add(new ObjTriplet(6, 0, 0));
            obj.Faces.Add(face);
            obj.Groups[0].Faces.Add(face);
            face.ObjectName = "a";
            face.LevelOfDetail = 2;
            face.MapName = "c";
            face.MaterialName = "d";
            face.SmoothingGroupNumber = 10;
            face.IsBevelInterpolationEnabled = true;
            face.IsColorInterpolationEnabled = true;
            face.IsDissolveInterpolationEnabled = true;

            string text = WriteObj(obj);
            string expected =
@"v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
g b
o a
lod 2
usemap c
usemtl d
s 10
bevel on
c_interp on
d_interp on
f 2 3 4 5 6
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public static void Element_Curve_Valid()
        {
            var obj = new ObjFile();
            obj.Groups.Add(new ObjGroup("b"));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            obj.Groups[0].Curves.Add(curve);
            curve.ObjectName = "a";
            curve.LevelOfDetail = 2;
            curve.MapName = "c";
            curve.MaterialName = "d";
            curve.MergingGroupNumber = 10;
            obj.MergingGroupResolutions[10] = 1.0f;
            FillFreeFormAttributes(curve);

            string text = WriteObj(obj);
            string expected =
@"v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
g b
o a
lod 2
usemap c
usemtl d
mg 10 1.000000
cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public static void Element_Curve2D_Valid()
        {
            var obj = new ObjFile();
            obj.Groups.Add(new ObjGroup("b"));
            obj.ParameterSpaceVertices.Add(new ObjVector3(0, 0, 0));
            obj.ParameterSpaceVertices.Add(new ObjVector3(0, 0, 0));
            obj.ParameterSpaceVertices.Add(new ObjVector3(0, 0, 0));
            obj.ParameterSpaceVertices.Add(new ObjVector3(0, 0, 0));
            var curve = new ObjCurve2D();
            curve.ParameterSpaceVertices.Add(2);
            curve.ParameterSpaceVertices.Add(3);
            curve.ParameterSpaceVertices.Add(4);
            obj.Curves2D.Add(curve);
            obj.Groups[0].Curves2D.Add(curve);
            curve.ObjectName = "a";
            curve.LevelOfDetail = 2;
            curve.MapName = "c";
            curve.MaterialName = "d";
            curve.MergingGroupNumber = 10;
            obj.MergingGroupResolutions[10] = 1.0f;
            FillFreeFormAttributes(curve);

            string text = WriteObj(obj);
            string expected =
@"vp 0.000000 0.000000 0.000000
vp 0.000000 0.000000 0.000000
vp 0.000000 0.000000 0.000000
vp 0.000000 0.000000 0.000000
g b
o a
lod 2
usemap c
usemtl d
mg 10 1.000000
cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv2 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public static void Element_Surface_Valid()
        {
            var obj = new ObjFile();
            obj.Groups.Add(new ObjGroup("b"));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            obj.Vertices.Add(new ObjVertex(0, 0, 0));
            var surface = new ObjSurface();
            surface.StartParameterU = 5.0f;
            surface.EndParameterU = 6.0f;
            surface.StartParameterV = 7.0f;
            surface.EndParameterV = 8.0f;
            surface.Vertices.Add(new ObjTriplet(2, 0, 0));
            surface.Vertices.Add(new ObjTriplet(3, 0, 0));
            surface.Vertices.Add(new ObjTriplet(4, 0, 0));
            obj.Surfaces.Add(surface);
            obj.Groups[0].Surfaces.Add(surface);
            surface.ObjectName = "a";
            surface.LevelOfDetail = 2;
            surface.MapName = "c";
            surface.MaterialName = "d";
            surface.MergingGroupNumber = 10;
            obj.MergingGroupResolutions[10] = 1.0f;
            FillFreeFormAttributes(surface);

            string text = WriteObj(obj);
            string expected =
@"v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
v 0.000000 0.000000 0.000000
g b
o a
lod 2
usemap c
usemtl d
mg 10 1.000000
cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
surf 5.000000 6.000000 7.000000 8.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData("u", 0)]
        [InlineData("v", 1)]
        public void FreeFormBody_Parameter_Valid(string value, int type)
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            IList<float> parameters = type == 0 ? obj.Curves[0].ParametersU : obj.Curves[0].ParametersV;
            parameters.Add(2.0f);
            parameters.Add(3.0f);
            parameters.Add(4.0f);

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
parm " + value + @" 2.000000 3.000000 4.000000
end
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void FreeFormBody_Trim_Valid()
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            curve.OuterTrimmingCurves.Add(new ObjCurveIndex(4.0f, 5.0f, 2));
            curve.OuterTrimmingCurves.Add(new ObjCurveIndex(6.0f, 7.0f, 3));

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
trim 4.000000 5.000000 2 6.000000 7.000000 3
end
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void FreeFormBody_Hole_Valid()
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            curve.InnerTrimmingCurves.Add(new ObjCurveIndex(4.0f, 5.0f, 2));
            curve.InnerTrimmingCurves.Add(new ObjCurveIndex(6.0f, 7.0f, 3));

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
hole 4.000000 5.000000 2 6.000000 7.000000 3
end
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void FreeFormBody_Sequence_Valid()
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            curve.SequenceCurves.Add(new ObjCurveIndex(4.0f, 5.0f, 2));
            curve.SequenceCurves.Add(new ObjCurveIndex(6.0f, 7.0f, 3));

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
scrv 4.000000 5.000000 2 6.000000 7.000000 3
end
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void FreeFormBody_SpecialPoints_Valid()
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            curve.SpecialPoints.Add(2);
            curve.SpecialPoints.Add(3);

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
sp 2 3
end
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MergingGroup_Valid()
        {
            var obj = new ObjFile();
            var curve1 = new ObjCurve();
            curve1.StartParameter = 5.0f;
            curve1.EndParameter = 6.0f;
            curve1.Vertices.Add(2);
            curve1.Vertices.Add(3);
            curve1.Vertices.Add(4);
            obj.Curves.Add(curve1);
            curve1.MergingGroupNumber = 10;
            obj.MergingGroupResolutions[10] = 1.0f;
            FillFreeFormAttributes(curve1);
            var curve2 = new ObjCurve();
            curve2.StartParameter = 5.0f;
            curve2.EndParameter = 6.0f;
            curve2.Vertices.Add(2);
            curve2.Vertices.Add(3);
            curve2.Vertices.Add(4);
            obj.Curves.Add(curve2);
            curve2.MergingGroupNumber = 0;
            FillFreeFormAttributes(curve2);

            string text = WriteObj(obj);
            string expected =
@"mg 10 1.000000
cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
mg off
cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void FreeFormAttributes_UnknownType_Valid()
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            curve.FreeFormType = ObjFreeFormType.Taylor + 1;
            curve.IsRationalForm = false;

            string text = WriteObj(obj);
            string expected =
@"deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData("bmatrix", false, ObjFreeFormType.BasisMatrix)]
        [InlineData("bmatrix", false, ObjFreeFormType.BasisMatrix)]
        [InlineData("rat bmatrix", true, ObjFreeFormType.BasisMatrix)]
        [InlineData("bezier", false, ObjFreeFormType.Bezier)]
        [InlineData("rat bezier", true, ObjFreeFormType.Bezier)]
        [InlineData("bspline", false, ObjFreeFormType.BSpline)]
        [InlineData("rat bspline", true, ObjFreeFormType.BSpline)]
        [InlineData("cardinal", false, ObjFreeFormType.Cardinal)]
        [InlineData("rat cardinal", true, ObjFreeFormType.Cardinal)]
        [InlineData("taylor", false, ObjFreeFormType.Taylor)]
        [InlineData("rat taylor", true, ObjFreeFormType.Taylor)]
        public void FreeFormAttributes_Type_Valid(string value, bool rational, ObjFreeFormType type)
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            curve.FreeFormType = type;
            curve.IsRationalForm = rational;

            string text = WriteObj(obj);
            string expected =
@"cstype " + value + @"
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData("2", 2, 0)]
        [InlineData("2 3", 2, 3)]
        public void FreeFormAttributes_Degree_Valid(string value, int u, int v)
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            curve.DegreeU = u;
            curve.DegreeV = v;

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg " + value + @"
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void FreeFormAttributes_NullBasisMatrix_Valid()
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            curve.BasicMatrixU = null;
            curve.BasicMatrixV = null;

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void FreeFormAttributes_BasisMatrix_Valid()
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            curve.BasicMatrixU = new float[] { 2, 3, 4, 5 };
            curve.BasicMatrixV = new float[] { 6, 7, 8, 9 };

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
bmat u 2.000000 3.000000 4.000000 5.000000
bmat v 6.000000 7.000000 8.000000 9.000000
step 0.500000 0.500000
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData("2.000000", 2.0f, 1.0f)]
        [InlineData("2.000000 3.000000", 2.0f, 3.0f)]
        public void FreeFormAttributes_Step_Valid(string value, float u, float v)
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            curve.StepU = u;
            curve.StepV = v;

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step " + value + @"
ctech cparm 1.000000
stech cparma 1.000000 2.000000
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void RenderAttributes_ApproximationTechnique_Null_Valid()
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            curve.CurveApproximationTechnique = null;
            curve.SurfaceApproximationTechnique = null;

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData("ctech cparm 2.000000", "stech cparma 1.000000 2.000000", 2.0f, 2.0f, 0)]
        [InlineData("ctech cparm 1.000000", "stech cparma 2.000000 3.000000", 2.0f, 3.0f, 1)]
        [InlineData("ctech cparm 1.000000", "stech cparmb 2.000000", 2.0f, 2.0f, 1)]
        public void RenderAttributes_ApproximationTechnique_Parametric_Valid(string value1, string value2, float u, float v, int type)
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            var technique = new ObjConstantParametricSubdivisionTechnique(u, v);
            switch (type)
            {
                case 0:
                    curve.CurveApproximationTechnique = technique;
                    break;

                case 1:
                    curve.SurfaceApproximationTechnique = technique;
                    break;
            }

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
" + value1 + @"
" + value2 + @"
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData("ctech cspace 2.000000", "stech cparma 1.000000 2.000000", 2.0f, 0)]
        [InlineData("ctech cparm 1.000000", "stech cspace 2.000000", 2.0f, 1)]
        public void RenderAttributes_ApproximationTechnique_Spatial_Valid(string value1, string value2, float length, int type)
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            var technique = new ObjConstantSpatialSubdivisionTechnique(length);
            switch (type)
            {
                case 0:
                    curve.CurveApproximationTechnique = technique;
                    break;

                case 1:
                    curve.SurfaceApproximationTechnique = technique;
                    break;
            }

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
" + value1 + @"
" + value2 + @"
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData("ctech curv 2.000000 3.000000", "stech cparma 1.000000 2.000000", 2.0f, 3.0f, 0)]
        [InlineData("ctech cparm 1.000000", "stech curv 2.000000 3.000000", 2.0f, 3.0f, 1)]
        public void RenderAttributes_ApproximationTechnique_Curvature_Valid(string value1, string value2, float distance, float angle, int type)
        {
            var obj = new ObjFile();
            var curve = new ObjCurve();
            curve.StartParameter = 5.0f;
            curve.EndParameter = 6.0f;
            curve.Vertices.Add(2);
            curve.Vertices.Add(3);
            curve.Vertices.Add(4);
            obj.Curves.Add(curve);
            FillFreeFormAttributes(curve);

            var technique = new ObjCurvatureDependentSubdivisionTechnique(distance, angle);
            switch (type)
            {
                case 0:
                    curve.CurveApproximationTechnique = technique;
                    break;

                case 1:
                    curve.SurfaceApproximationTechnique = technique;
                    break;
            }

            string text = WriteObj(obj);
            string expected =
@"cstype rat bmatrix
deg 1 1
bmat u 0.000000 0.000000 0.000000 0.000000
bmat v 0.000000 0.000000 0.000000 0.000000
step 0.500000 0.500000
" + value1 + @"
" + value2 + @"
curv 5.000000 6.000000 2 3 4
";

            AssertExtensions.TextEqual(expected, text);
        }

        private static void FillFreeFormAttributes(ObjFreeFormElement element)
        {
            element.FreeFormType = ObjFreeFormType.BasisMatrix;
            element.IsRationalForm = true;
            element.DegreeU = 1;
            element.DegreeV = 1;
            element.BasicMatrixU = new float[4] { 0, 0, 0, 0 };
            element.BasicMatrixV = new float[4] { 0, 0, 0, 0 };
            element.StepU = 0.5f;
            element.StepV = 0.5f;
            element.CurveApproximationTechnique = new ObjConstantParametricSubdivisionTechnique(1.0f);
            element.SurfaceApproximationTechnique = new ObjConstantParametricSubdivisionTechnique(1.0f, 2.0f);
        }

        private static string WriteObj(ObjFile obj)
        {
            byte[] buffer;

            using (var stream = new MemoryStream())
            {
                obj.WriteTo(stream);

                buffer = stream.ToArray();
            }

            string text;

            using (var stream = new MemoryStream(buffer, false))
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }
    }
}
