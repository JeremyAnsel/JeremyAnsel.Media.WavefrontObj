// <copyright file="ObjFileReaderTests.cs" company="Jérémy Ansel">
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
    public class ObjFileReaderTests
    {
        [Fact]
        public void Parsing_UnknownStatement_Valid()
        {
            string content = "unknown";

            var obj = ReadObj(content);
        }

        [Fact]
        public void Parsing_Triplet_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("p ///"));
            Assert.Throws<InvalidDataException>(() => ReadObj("p /"));
            Assert.Throws<InvalidDataException>(() => ReadObj("p 0/"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("p 1"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("p -1"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("v 0 0 0\np 1/1"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("v 0 0 0\np 1/-1"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("v 0 0 0\np 1//1"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("v 0 0 0\np 1//-1"));
        }

        [Fact]
        public void Parsing_Triplet1_Valid()
        {
            string content = @"
v 0 0 0
v 0 0 0
p 2
";

            var obj = ReadObj(content);
            var triplet = obj.Points[0].Vertices[0];

            Assert.Equal(2, triplet.Vertex);
            Assert.Equal(0, triplet.Texture);
            Assert.Equal(0, triplet.Normal);
        }

        [Fact]
        public void Parsing_Triplet2_Valid()
        {
            string content = @"
v 0 0 0
v 0 0 0
vt 0
vt 0
vt 0
p 2/3/
";

            var obj = ReadObj(content);
            var triplet = obj.Points[0].Vertices[0];

            Assert.Equal(2, triplet.Vertex);
            Assert.Equal(3, triplet.Texture);
            Assert.Equal(0, triplet.Normal);
        }

        [Fact]
        public void Parsing_Triplet3_Valid()
        {
            string content = @"
v 0 0 0
v 0 0 0
vn 0 0 0
vn 0 0 0
vn 0 0 0
p 2//3
";

            var obj = ReadObj(content);
            var triplet = obj.Points[0].Vertices[0];

            Assert.Equal(2, triplet.Vertex);
            Assert.Equal(0, triplet.Texture);
            Assert.Equal(3, triplet.Normal);
        }

        [Fact]
        public void Parsing_Triplet4_Valid()
        {
            string content = @"
v 0 0 0
v 0 0 0
vt 0
vt 0
vt 0
vn 0 0 0
vn 0 0 0
vn 0 0 0
vn 0 0 0
p 2/3/4
";

            var obj = ReadObj(content);
            var triplet = obj.Points[0].Vertices[0];

            Assert.Equal(2, triplet.Vertex);
            Assert.Equal(3, triplet.Texture);
            Assert.Equal(4, triplet.Normal);
        }

        [Fact]
        public void Vertex_Geometric_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("v"));
            Assert.Throws<InvalidDataException>(() => ReadObj("v 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("v 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("v 0 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("v 0 0 0 0 0 0 0 0"));
        }

        [Fact]
        public void Vertex_Geometric3_Valid()
        {
            string content = @"
v 2.0 3.0 4.0
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Vertices.Count);
            Assert.Equal(new ObjVertex(2.0f, 3.0f, 4.0f, 1.0f), obj.Vertices[0]);
        }

        [Fact]
        public void Vertex_Geometric4_Valid()
        {
            string content = @"
v 2.0 3.0 4.0 5.0
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Vertices.Count);
            Assert.Equal(new ObjVertex(2.0f, 3.0f, 4.0f, 5.0f), obj.Vertices[0]);
        }

        [Fact]
        public void Vertex_Geometric6_Valid()
        {
            string content = @"
v 2.0 3.0 4.0 5.0 6.0 7.0
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Vertices.Count);
            Assert.Equal(new ObjVertex(2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 1.0f), obj.Vertices[0]);
        }

        [Fact]
        public void Vertex_Geometric7_Valid()
        {
            string content = @"
v 2.0 3.0 4.0 5.0 6.0 7.0 8.0
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Vertices.Count);
            Assert.Equal(new ObjVertex(2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f), obj.Vertices[0]);
        }

        [Fact]
        public void Vertex_ParameterSpace_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("vp"));
            Assert.Throws<InvalidDataException>(() => ReadObj("vp 0 0 0 0"));
        }

        [Fact]
        public void Vertex_ParameterSpace1_Valid()
        {
            string content = @"
vp 2.0
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.ParameterSpaceVertices.Count);
            Assert.Equal(new ObjVector3(2.0f, 0.0f, 1.0f), obj.ParameterSpaceVertices[0]);
        }

        [Fact]
        public void Vertex_ParameterSpace2_Valid()
        {
            string content = @"
vp 2.0 3.0
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.ParameterSpaceVertices.Count);
            Assert.Equal(new ObjVector3(2.0f, 3.0f, 1.0f), obj.ParameterSpaceVertices[0]);
        }

        [Fact]
        public void Vertex_ParameterSpace3_Valid()
        {
            string content = @"
vp 2.0 3.0 4.0
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.ParameterSpaceVertices.Count);
            Assert.Equal(new ObjVector3(2.0f, 3.0f, 4.0f), obj.ParameterSpaceVertices[0]);
        }

        [Fact]
        public void Vertex_Normal_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("vn"));
            Assert.Throws<InvalidDataException>(() => ReadObj("vn 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("vn 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("vn 0 0 0 0"));
        }

        [Fact]
        public void Vertex_Normal_Valid()
        {
            string content = @"
vn 2.0 3.0 4.0
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.VertexNormals.Count);
            Assert.Equal(new ObjVector3(2.0f, 3.0f, 4.0f), obj.VertexNormals[0]);
        }

        [Fact]
        public void Vertex_Texture_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("vt"));
            Assert.Throws<InvalidDataException>(() => ReadObj("vt 0 0 0 0"));
        }

        [Fact]
        public void Vertex_Texture1_Valid()
        {
            string content = @"
vt 2.0
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.TextureVertices.Count);
            Assert.Equal(new ObjVector3(2.0f, 0.0f, 0.0f), obj.TextureVertices[0]);
        }

        [Fact]
        public void Vertex_Texture2_Valid()
        {
            string content = @"
vt 2.0 3.0
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.TextureVertices.Count);
            Assert.Equal(new ObjVector3(2.0f, 3.0f, 0.0f), obj.TextureVertices[0]);
        }

        [Fact]
        public void Vertex_Texture3_Valid()
        {
            string content = @"
vt 2.0 3.0 4.0
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.TextureVertices.Count);
            Assert.Equal(new ObjVector3(2.0f, 3.0f, 4.0f), obj.TextureVertices[0]);
        }

        [Fact]
        public void FreeFormAttributes_Type_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("cstype"));
            Assert.Throws<InvalidDataException>(() => ReadObj("cstype 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("cstype 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("cstype rat 0"));
        }

        [Theory]
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
            string content = @"
v 0 0 0
v 0 0 0
cstype " + value + @"
curv 0 1 1 2
";

            var obj = ReadObj(content);

            Assert.Equal(rational, obj.Curves[0].IsRationalForm);
            Assert.Equal(type, obj.Curves[0].FreeFormType);
        }

        [Fact]
        public void FreeFormAttributes_Degree_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("deg"));
            Assert.Throws<InvalidDataException>(() => ReadObj("deg 0 0 0"));
        }

        [Theory]
        [InlineData("2", 2, 0)]
        [InlineData("2 3", 2, 3)]
        public void FreeFormAttributes_Degree_Valid(string value, int u, int v)
        {
            string content = @"
v 0 0 0
v 0 0 0
deg " + value + @"
curv 0 1 1 2
";

            var obj = ReadObj(content);

            Assert.Equal(u, obj.Curves[0].DegreeU);
            Assert.Equal(v, obj.Curves[0].DegreeV);
        }

        [Fact]
        public void FreeFormAttributes_BasisMatrix_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("bmat"));
            Assert.Throws<InvalidDataException>(() => ReadObj("bmat 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("deg 1 1\nbmat u 0 0 0 0 0"));
        }

        [Theory]
        [InlineData("u", 0)]
        [InlineData("v", 1)]
        public void FreeFormAttributes_BasisMatrix_Valid(string value, int index)
        {
            string content = @"
v 0 0 0
v 0 0 0
deg 2 3
bmat " + value + @" 2.0 3.0 4.0 5.0 6.0 7.0 8.0 9.0 10.0 11.0 12.0 13.0
curv 0 1 1 2
";

            var obj = ReadObj(content);

            var expected = new float[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

            switch (index)
            {
                case 0:
                    Assert.Equal(expected, obj.Curves[0].BasicMatrixU);
                    break;

                case 1:
                    Assert.Equal(expected, obj.Curves[0].BasicMatrixV);
                    break;
            }
        }

        [Fact]
        public void FreeFormAttributes_Step_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("step"));
            Assert.Throws<InvalidDataException>(() => ReadObj("step 0 0 0"));
        }

        [Theory]
        [InlineData("2.0", 2.0f, 1.0f)]
        [InlineData("2.0 3.0", 2.0f, 3.0f)]
        public void FreeFormAttributes_Step_Valid(string value, float u, float v)
        {
            string content = @"
v 0 0 0
v 0 0 0
step " + value + @"
curv 0 1 1 2
";

            var obj = ReadObj(content);

            Assert.Equal(u, obj.Curves[0].StepU);
            Assert.Equal(v, obj.Curves[0].StepV);
        }

        [Fact]
        public void Element_Point_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("p"));
        }

        [Fact]
        public void Element_Point_Valid()
        {
            string content = @"
s 10
o a
g b
lod 2
usemap c
usemtl d
bevel on
c_interp on
d_interp on
v 0 0 0
v 0 0 0
v 0 0 0
p 2 3
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Points.Count);
            Assert.Equal(2, obj.Points[0].Vertices.Count);
            Assert.Equal(2, obj.Points[0].Vertices[0].Vertex);
            Assert.Equal(3, obj.Points[0].Vertices[1].Vertex);
            Assert.Equal(10, obj.Points[0].SmoothingGroupNumber);
            Assert.Equal("a", obj.Points[0].ObjectName);
            Assert.Same(obj.Points[0], obj.Groups[0].Points[0]);
            Assert.Equal(2, obj.Points[0].LevelOfDetail);
            Assert.Equal("c", obj.Points[0].MapName);
            Assert.Equal("d", obj.Points[0].MaterialName);
            Assert.True(obj.Points[0].IsBevelInterpolationEnabled);
            Assert.True(obj.Points[0].IsColorInterpolationEnabled);
            Assert.True(obj.Points[0].IsDissolveInterpolationEnabled);
        }

        [Fact]
        public void Element_PointDefaultGroup_Valid()
        {
            string content = @"
g default
v 0 0 0
v 0 0 0
v 0 0 0
p 2 3
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Points.Count);
            Assert.Equal(2, obj.Points[0].Vertices.Count);
            Assert.Equal(2, obj.Points[0].Vertices[0].Vertex);
            Assert.Equal(3, obj.Points[0].Vertices[1].Vertex);
            Assert.Same(obj.Points[0], obj.DefaultGroup.Points[0]);
        }

        [Fact]
        public void Element_Line_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("l"));
            Assert.Throws<InvalidDataException>(() => ReadObj("l 0"));
        }

        [Fact]
        public void Element_Line_Valid()
        {
            string content = @"
s 10
o a
g b
lod 2
usemap c
usemtl d
bevel on
c_interp on
d_interp on
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
l 2 3 4
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Lines.Count);
            Assert.Equal(3, obj.Lines[0].Vertices.Count);
            Assert.Equal(2, obj.Lines[0].Vertices[0].Vertex);
            Assert.Equal(3, obj.Lines[0].Vertices[1].Vertex);
            Assert.Equal(4, obj.Lines[0].Vertices[2].Vertex);
            Assert.Equal(10, obj.Lines[0].SmoothingGroupNumber);
            Assert.Equal("a", obj.Lines[0].ObjectName);
            Assert.Same(obj.Lines[0], obj.Groups[0].Lines[0]);
            Assert.Equal(2, obj.Lines[0].LevelOfDetail);
            Assert.Equal("c", obj.Lines[0].MapName);
            Assert.Equal("d", obj.Lines[0].MaterialName);
            Assert.True(obj.Lines[0].IsBevelInterpolationEnabled);
            Assert.True(obj.Lines[0].IsColorInterpolationEnabled);
            Assert.True(obj.Lines[0].IsDissolveInterpolationEnabled);
        }

        [Fact]
        public void Element_LineDefaultGroup_Valid()
        {
            string content = @"
g default
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
l 2 3 4
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Lines.Count);
            Assert.Equal(3, obj.Lines[0].Vertices.Count);
            Assert.Equal(2, obj.Lines[0].Vertices[0].Vertex);
            Assert.Equal(3, obj.Lines[0].Vertices[1].Vertex);
            Assert.Equal(4, obj.Lines[0].Vertices[2].Vertex);
            Assert.Same(obj.Lines[0], obj.DefaultGroup.Lines[0]);
        }

        [Theory]
        [InlineData("f")]
        [InlineData("fo")]
        public void Element_Face_Throws(string statement)
        {
            Assert.Throws<InvalidDataException>(() => ReadObj(statement));
            Assert.Throws<InvalidDataException>(() => ReadObj(statement + " 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(statement + " 0 0"));
        }

        [Theory]
        [InlineData("f")]
        [InlineData("fo")]
        public void Element_Face_Valid(string statement)
        {
            string content = @"
s 10
o a
g b
lod 2
usemap c
usemtl d
bevel on
c_interp on
d_interp on
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
" + statement + @" 2 3 4 5 6
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Faces.Count);
            Assert.Equal(5, obj.Faces[0].Vertices.Count);
            Assert.Equal(2, obj.Faces[0].Vertices[0].Vertex);
            Assert.Equal(3, obj.Faces[0].Vertices[1].Vertex);
            Assert.Equal(4, obj.Faces[0].Vertices[2].Vertex);
            Assert.Equal(5, obj.Faces[0].Vertices[3].Vertex);
            Assert.Equal(6, obj.Faces[0].Vertices[4].Vertex);
            Assert.Equal(10, obj.Faces[0].SmoothingGroupNumber);
            Assert.Equal("a", obj.Faces[0].ObjectName);
            Assert.Same(obj.Faces[0], obj.Groups[0].Faces[0]);
            Assert.Equal(2, obj.Faces[0].LevelOfDetail);
            Assert.Equal("c", obj.Faces[0].MapName);
            Assert.Equal("d", obj.Faces[0].MaterialName);
            Assert.True(obj.Faces[0].IsBevelInterpolationEnabled);
            Assert.True(obj.Faces[0].IsColorInterpolationEnabled);
            Assert.True(obj.Faces[0].IsDissolveInterpolationEnabled);
        }

        [Theory]
        [InlineData("f")]
        [InlineData("fo")]
        public void Element_FaceDefaultGroup_Valid(string statement)
        {
            string content = @"
g default
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
" + statement + @" 2 3 4 5 6
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Faces.Count);
            Assert.Equal(5, obj.Faces[0].Vertices.Count);
            Assert.Equal(2, obj.Faces[0].Vertices[0].Vertex);
            Assert.Equal(3, obj.Faces[0].Vertices[1].Vertex);
            Assert.Equal(4, obj.Faces[0].Vertices[2].Vertex);
            Assert.Equal(5, obj.Faces[0].Vertices[3].Vertex);
            Assert.Equal(6, obj.Faces[0].Vertices[4].Vertex);
            Assert.Same(obj.Faces[0], obj.DefaultGroup.Faces[0]);
        }

        [Fact]
        public void Element_Curve_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("curv"));
            Assert.Throws<InvalidDataException>(() => ReadObj("curv 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("curv 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("curv 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("curv 0 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("curv 0 0 1 1"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("curv 0 0 -1 -1"));
        }

        [Fact]
        public void Element_Curve_Valid()
        {
            string content = @"
mg 10 0
o a
g b
lod 2
usemap c
usemtl d
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
ctech cspace 0
stech cspace 0
curv 5.0 6.0 2 3 4
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Curves.Count);
            Assert.Equal(5.0f, obj.Curves[0].StartParameter);
            Assert.Equal(6.0f, obj.Curves[0].EndParameter);
            Assert.Equal(3, obj.Curves[0].Vertices.Count);
            Assert.Equal(2, obj.Curves[0].Vertices[0]);
            Assert.Equal(3, obj.Curves[0].Vertices[1]);
            Assert.Equal(4, obj.Curves[0].Vertices[2]);
            Assert.Equal(10, obj.Curves[0].MergingGroupNumber);
            Assert.Equal("a", obj.Curves[0].ObjectName);
            Assert.Same(obj.Curves[0], obj.Groups[0].Curves[0]);
            Assert.Equal(2, obj.Curves[0].LevelOfDetail);
            Assert.Equal("c", obj.Curves[0].MapName);
            Assert.Equal("d", obj.Curves[0].MaterialName);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Curves[0].CurveApproximationTechnique);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Curves[0].SurfaceApproximationTechnique);
        }

        [Fact]
        public void Element_CurveDefaultGroup_Valid()
        {
            string content = @"
g default
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
ctech cspace 0
stech cspace 0
curv 5.0 6.0 2 3 4
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Curves.Count);
            Assert.Equal(5.0f, obj.Curves[0].StartParameter);
            Assert.Equal(6.0f, obj.Curves[0].EndParameter);
            Assert.Equal(3, obj.Curves[0].Vertices.Count);
            Assert.Equal(2, obj.Curves[0].Vertices[0]);
            Assert.Equal(3, obj.Curves[0].Vertices[1]);
            Assert.Equal(4, obj.Curves[0].Vertices[2]);
            Assert.Same(obj.Curves[0], obj.DefaultGroup.Curves[0]);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Curves[0].CurveApproximationTechnique);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Curves[0].SurfaceApproximationTechnique);
        }

        [Fact]
        public void Element_Curve2D_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("curv2"));
            Assert.Throws<InvalidDataException>(() => ReadObj("curv2 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("curv2 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("curv2 1 1"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("curv2 -1 -1"));
        }

        [Fact]
        public void Element_Curve2D_Valid()
        {
            string content = @"
mg 10 0
o a
g b
lod 2
usemap c
usemtl d
vp 0 0 0
vp 0 0 0
vp 0 0 0
vp 0 0 0
ctech cspace 0
stech cspace 0
curv2 2 3 4
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Curves2D.Count);
            Assert.Equal(3, obj.Curves2D[0].ParameterSpaceVertices.Count);
            Assert.Equal(2, obj.Curves2D[0].ParameterSpaceVertices[0]);
            Assert.Equal(3, obj.Curves2D[0].ParameterSpaceVertices[1]);
            Assert.Equal(4, obj.Curves2D[0].ParameterSpaceVertices[2]);
            Assert.Equal(10, obj.Curves2D[0].MergingGroupNumber);
            Assert.Equal("a", obj.Curves2D[0].ObjectName);
            Assert.Same(obj.Curves2D[0], obj.Groups[0].Curves2D[0]);
            Assert.Equal(2, obj.Curves2D[0].LevelOfDetail);
            Assert.Equal("c", obj.Curves2D[0].MapName);
            Assert.Equal("d", obj.Curves2D[0].MaterialName);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Curves2D[0].CurveApproximationTechnique);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Curves2D[0].SurfaceApproximationTechnique);
        }

        [Fact]
        public void Element_Curve2DDefaultGroup_Valid()
        {
            string content = @"
g default
vp 0 0 0
vp 0 0 0
vp 0 0 0
vp 0 0 0
ctech cspace 0
stech cspace 0
curv2 2 3 4
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Curves2D.Count);
            Assert.Equal(3, obj.Curves2D[0].ParameterSpaceVertices.Count);
            Assert.Equal(2, obj.Curves2D[0].ParameterSpaceVertices[0]);
            Assert.Equal(3, obj.Curves2D[0].ParameterSpaceVertices[1]);
            Assert.Equal(4, obj.Curves2D[0].ParameterSpaceVertices[2]);
            Assert.Same(obj.Curves2D[0], obj.DefaultGroup.Curves2D[0]);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Curves2D[0].CurveApproximationTechnique);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Curves2D[0].SurfaceApproximationTechnique);
        }

        [Fact]
        public void Element_Surface_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("surf"));
            Assert.Throws<InvalidDataException>(() => ReadObj("surf 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("surf 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("surf 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("surf 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("surf 0 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("surf 0 0 0 0 1"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj("surf 0 0 0 0 -1"));
        }

        [Fact]
        public void Element_Surface_Valid()
        {
            string content = @"
mg 10 0
o a
g b
lod 2
usemap c
usemtl d
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
ctech cspace 0
stech cspace 0
surf 5.0 6.0 7.0 8.0 2 3 4
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Surfaces.Count);
            Assert.Equal(5.0f, obj.Surfaces[0].StartParameterU);
            Assert.Equal(6.0f, obj.Surfaces[0].EndParameterU);
            Assert.Equal(7.0f, obj.Surfaces[0].StartParameterV);
            Assert.Equal(8.0f, obj.Surfaces[0].EndParameterV);
            Assert.Equal(3, obj.Surfaces[0].Vertices.Count);
            Assert.Equal(2, obj.Surfaces[0].Vertices[0].Vertex);
            Assert.Equal(3, obj.Surfaces[0].Vertices[1].Vertex);
            Assert.Equal(4, obj.Surfaces[0].Vertices[2].Vertex);
            Assert.Equal(10, obj.Surfaces[0].MergingGroupNumber);
            Assert.Equal("a", obj.Surfaces[0].ObjectName);
            Assert.Same(obj.Surfaces[0], obj.Groups[0].Surfaces[0]);
            Assert.Equal(2, obj.Surfaces[0].LevelOfDetail);
            Assert.Equal("c", obj.Surfaces[0].MapName);
            Assert.Equal("d", obj.Surfaces[0].MaterialName);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Surfaces[0].CurveApproximationTechnique);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Surfaces[0].SurfaceApproximationTechnique);
        }

        [Fact]
        public void Element_SurfaceDefaultGroup_Valid()
        {
            string content = @"
g default
v 0 0 0
v 0 0 0
v 0 0 0
v 0 0 0
ctech cspace 0
stech cspace 0
surf 5.0 6.0 7.0 8.0 2 3 4
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Surfaces.Count);
            Assert.Equal(5.0f, obj.Surfaces[0].StartParameterU);
            Assert.Equal(6.0f, obj.Surfaces[0].EndParameterU);
            Assert.Equal(7.0f, obj.Surfaces[0].StartParameterV);
            Assert.Equal(8.0f, obj.Surfaces[0].EndParameterV);
            Assert.Equal(3, obj.Surfaces[0].Vertices.Count);
            Assert.Equal(2, obj.Surfaces[0].Vertices[0].Vertex);
            Assert.Equal(3, obj.Surfaces[0].Vertices[1].Vertex);
            Assert.Equal(4, obj.Surfaces[0].Vertices[2].Vertex);
            Assert.Same(obj.Surfaces[0], obj.DefaultGroup.Surfaces[0]);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Surfaces[0].CurveApproximationTechnique);
            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(obj.Surfaces[0].SurfaceApproximationTechnique);
        }

        [Fact]
        public void FreeFormBody_Parameter_Throws()
        {
            ReadObj("parm");

            string data = @"
v 0 0 0
curv 0 0 1 1
";

            Assert.Throws<InvalidDataException>(() => ReadObj(data + "parm"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "parm 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "parm 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "parm 0 0 0"));
        }

        [Theory]
        [InlineData("u", 0)]
        [InlineData("v", 1)]
        public void FreeFormBody_Parameter_Valid(string value, int type)
        {
            string content = @"
v 0 0 0
curv 0 0 1 1
parm " + value + @" 2.0 3.0 4.0
end
";

            var obj = ReadObj(content);
            IList<float> parameters = type == 0 ? obj.Curves[0].ParametersU : obj.Curves[0].ParametersV;

            Assert.Equal(3, parameters.Count);
            Assert.Equal(2.0f, parameters[0]);
            Assert.Equal(3.0f, parameters[1]);
            Assert.Equal(4.0f, parameters[2]);
        }

        [Fact]
        public void FreeFormBody_Trim_Throws()
        {
            ReadObj("trim");

            string data = @"
vp 0 0 0
curv2 1 1
";

            Assert.Throws<InvalidDataException>(() => ReadObj(data + "trim"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "trim 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "trim 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "trim 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "trim 0 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "trim 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "trim 0 0 2"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "trim 0 0 -2"));
        }

        [Fact]
        public void FreeFormBody_Trim_Valid()
        {
            string content = @"
vp 0 0 0
curv2 1 1
curv2 1 1
curv2 1 1
v 0 0 0
curv 0 0 1 1
trim 4.0 5.0 2 6.0 7.0 3
end
";

            var obj = ReadObj(content);

            Assert.Equal(2, obj.Curves[0].OuterTrimmingCurves.Count);
            Assert.Equal(4.0f, obj.Curves[0].OuterTrimmingCurves[0].Start);
            Assert.Equal(5.0f, obj.Curves[0].OuterTrimmingCurves[0].End);
            Assert.Equal(2, obj.Curves[0].OuterTrimmingCurves[0].Curve2D);
            Assert.Equal(6.0f, obj.Curves[0].OuterTrimmingCurves[1].Start);
            Assert.Equal(7.0f, obj.Curves[0].OuterTrimmingCurves[1].End);
            Assert.Equal(3, obj.Curves[0].OuterTrimmingCurves[1].Curve2D);
        }

        [Fact]
        public void FreeFormBody_Hole_Throws()
        {
            ReadObj("hole");

            string data = @"
vp 0 0 0
curv2 1 1
";

            Assert.Throws<InvalidDataException>(() => ReadObj(data + "hole"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "hole 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "hole 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "hole 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "hole 0 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "hole 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "hole 0 0 2"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "hole 0 0 -2"));
        }

        [Fact]
        public void FreeFormBody_Hole_Valid()
        {
            string content = @"
vp 0 0 0
curv2 1 1
curv2 1 1
curv2 1 1
v 0 0 0
curv 0 0 1 1
hole 4.0 5.0 2 6.0 7.0 3
end
";

            var obj = ReadObj(content);

            Assert.Equal(2, obj.Curves[0].InnerTrimmingCurves.Count);
            Assert.Equal(4.0f, obj.Curves[0].InnerTrimmingCurves[0].Start);
            Assert.Equal(5.0f, obj.Curves[0].InnerTrimmingCurves[0].End);
            Assert.Equal(2, obj.Curves[0].InnerTrimmingCurves[0].Curve2D);
            Assert.Equal(6.0f, obj.Curves[0].InnerTrimmingCurves[1].Start);
            Assert.Equal(7.0f, obj.Curves[0].InnerTrimmingCurves[1].End);
            Assert.Equal(3, obj.Curves[0].InnerTrimmingCurves[1].Curve2D);
        }

        [Fact]
        public void FreeFormBody_Sequence_Throws()
        {
            ReadObj("scrv");

            string data = @"
vp 0 0 0
curv2 1 1
";

            Assert.Throws<InvalidDataException>(() => ReadObj(data + "scrv"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "scrv 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "scrv 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "scrv 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "scrv 0 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "scrv 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "scrv 0 0 2"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "scrv 0 0 -2"));
        }

        [Fact]
        public void FreeFormBody_Sequence_Valid()
        {
            string content = @"
vp 0 0 0
curv2 1 1
curv2 1 1
curv2 1 1
v 0 0 0
curv 0 0 1 1
scrv 4.0 5.0 2 6.0 7.0 3
end
";

            var obj = ReadObj(content);

            Assert.Equal(2, obj.Curves[0].SequenceCurves.Count);
            Assert.Equal(4.0f, obj.Curves[0].SequenceCurves[0].Start);
            Assert.Equal(5.0f, obj.Curves[0].SequenceCurves[0].End);
            Assert.Equal(2, obj.Curves[0].SequenceCurves[0].Curve2D);
            Assert.Equal(6.0f, obj.Curves[0].SequenceCurves[1].Start);
            Assert.Equal(7.0f, obj.Curves[0].SequenceCurves[1].End);
            Assert.Equal(3, obj.Curves[0].SequenceCurves[1].Curve2D);
        }

        [Fact]
        public void FreeFormBody_SpecialPoints_Throws()
        {
            ReadObj("sp");

            string data = @"
vp 0 0 0
curv2 1 1
";

            Assert.Throws<InvalidDataException>(() => ReadObj(data + "sp"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "sp 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "sp 2"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "sp -2"));
        }

        [Fact]
        public void FreeFormBody_SpecialPoints_Valid()
        {
            string content = @"
vp 0 0 0
vp 0 0 0
vp 0 0 0
v 0 0 0
curv 0 0 1 1
sp 2 3
end
";

            var obj = ReadObj(content);

            Assert.Equal(2, obj.Curves[0].SpecialPoints.Count);
            Assert.Equal(2, obj.Curves[0].SpecialPoints[0]);
            Assert.Equal(3, obj.Curves[0].SpecialPoints[1]);
        }

        [Fact]
        public void SurfaceConnection_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("con"));
            Assert.Throws<InvalidDataException>(() => ReadObj("con 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("con 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("con 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("con 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("con 0 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("con 0 0 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("con 0 0 0 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("con 0 0 0 0 0 0 0 0 0"));

            string data = @"
v 0 0 0
surf 0 0 0 0 1
vp 0 0 0
curv2 1 1
";

            Assert.Throws<InvalidDataException>(() => ReadObj(data + "con 0 0 0 0 0 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "con 2 0 0 0 0 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "con -2 0 0 0 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "con 1 0 0 0 0 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "con 1 0 0 2 0 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "con 1 0 0 -2 0 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "con 1 0 0 1 0 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "con 1 0 0 1 2 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "con 1 0 0 1 -2 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj(data + "con 1 0 0 1 1 0 0 0"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "con 1 0 0 1 1 0 0 -2"));
            Assert.Throws<IndexOutOfRangeException>(() => ReadObj(data + "con 1 0 0 1 1 0 0 2"));
        }

        [Fact]
        public void SurfaceConnection_Valid()
        {
            string content = @"
v 0 0 0
surf 0 0 0 0 1
surf 0 0 0 0 1
surf 0 0 0 0 1
surf 0 0 0 0 1
vp 0 0 0
curv2 1 1
curv2 1 1
curv2 1 1
curv2 1 1
curv2 1 1
con 2 6.0 7.0 3 4 8.0 9.0 5
";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.SurfaceConnections.Count);
            Assert.Equal(2, obj.SurfaceConnections[0].Surface1);
            Assert.Equal(6.0f, obj.SurfaceConnections[0].Curve2D1.Start);
            Assert.Equal(7.0f, obj.SurfaceConnections[0].Curve2D1.End);
            Assert.Equal(3, obj.SurfaceConnections[0].Curve2D1.Curve2D);
            Assert.Equal(4, obj.SurfaceConnections[0].Surface2);
            Assert.Equal(8.0f, obj.SurfaceConnections[0].Curve2D2.Start);
            Assert.Equal(9.0f, obj.SurfaceConnections[0].Curve2D2.End);
            Assert.Equal(5, obj.SurfaceConnections[0].Curve2D2.Curve2D);
        }

        [Fact]
        public void Group_WithoutName_Valid()
        {
            string content = @"
g
v 0 0 0
p 1
";

            var obj = ReadObj(content);

            Assert.Equal(0, obj.Groups.Count);
        }

        [Fact]
        public void Group_DefaultName_Valid()
        {
            string content = @"
g default
v 0 0 0
p 1
";

            var obj = ReadObj(content);

            Assert.Equal(0, obj.Groups.Count);
        }

        [Fact]
        public void Group_SingleName_Valid()
        {
            string content = "g a";

            var obj = ReadObj(content);

            Assert.Equal(1, obj.Groups.Count);
            Assert.Equal("a", obj.Groups[0].Name);
        }

        [Fact]
        public void Group_MultipleNames_Valid()
        {
            string content = "g a b";

            var obj = ReadObj(content);

            Assert.Equal(2, obj.Groups.Count);
            Assert.Equal("a", obj.Groups[0].Name);
            Assert.Equal("b", obj.Groups[1].Name);
        }

        [Fact]
        public void SmoothingGroup_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("s"));
            Assert.Throws<InvalidDataException>(() => ReadObj("s 0 0"));
        }

        [Theory]
        [InlineData("0", 0)]
        [InlineData("off", 0)]
        [InlineData("2", 2)]
        public void SmoothingGroup_Valid(string value, int number)
        {
            string content = @"
s " + value + @"
v 0 0 0
p 1
";

            var obj = ReadObj(content);

            Assert.Equal(number, obj.Points[0].SmoothingGroupNumber);
        }

        [Fact]
        public void MergingGroup_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("mg"));
            Assert.Throws<InvalidDataException>(() => ReadObj("mg 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("mg 1"));
            Assert.Throws<InvalidDataException>(() => ReadObj("mg 1 0 0"));
        }

        [Theory]
        [InlineData("0", 0, 0.0f)]
        [InlineData("off", 0, 0.0f)]
        [InlineData("2 3.0", 2, 3.0f)]
        public void MergingGroup_Valid(string value, int mg, float res)
        {
            string content = @"
mg " + value + @"
v 0 0 0
v 0 0 0
curv 0 1 1 2
";

            var obj = ReadObj(content);

            Assert.Equal(mg, obj.Curves[0].MergingGroupNumber);

            if (mg != 0)
            {
                Assert.True(obj.MergingGroupResolutions.ContainsKey(mg));
                Assert.Equal(res, obj.MergingGroupResolutions[mg]);
            }
        }

        [Fact]
        public void ObjectName_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("o 0 0"));
        }

        [Fact]
        public void ObjectName_WithoutName_Valid()
        {
            string content = @"
o
v 0 0 0
p 1
";

            var obj = ReadObj(content);

            Assert.Equal(null, obj.Points[0].ObjectName);
        }

        [Fact]
        public void ObjectName_Valid()
        {
            string content = @"
o a
v 0 0 0
p 1
";

            var obj = ReadObj(content);

            Assert.Equal("a", obj.Points[0].ObjectName);
        }

        [Fact]
        public void RenderAttributes_Bevel_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("bevel"));
            Assert.Throws<InvalidDataException>(() => ReadObj("bevel on 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("bevel 0"));
        }

        [Theory]
        [InlineData("on", true)]
        [InlineData("off", false)]
        public void RenderAttributes_Bevel_Valid(string value, bool expected)
        {
            string content = @"
bevel " + value + @"
v 0 0 0
p 1
";

            var obj = ReadObj(content);

            Assert.Equal(expected, obj.Points[0].IsBevelInterpolationEnabled);
        }

        [Fact]
        public void RenderAttributes_ColorInterpolation_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("c_interp"));
            Assert.Throws<InvalidDataException>(() => ReadObj("c_interp on 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("c_interp 0"));
        }

        [Theory]
        [InlineData("on", true)]
        [InlineData("off", false)]
        public void RenderAttributes_ColorInterpolation_Valid(string value, bool expected)
        {
            string content = @"
c_interp " + value + @"
v 0 0 0
p 1
";

            var obj = ReadObj(content);

            Assert.Equal(expected, obj.Points[0].IsColorInterpolationEnabled);
        }

        [Fact]
        public void RenderAttributes_DissolveInterpolation_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("d_interp"));
            Assert.Throws<InvalidDataException>(() => ReadObj("d_interp on 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("d_interp 0"));
        }

        [Theory]
        [InlineData("on", true)]
        [InlineData("off", false)]
        public void RenderAttributes_DissolveInterpolation_Valid(string value, bool expected)
        {
            string content = @"
d_interp " + value + @"
v 0 0 0
p 1
";

            var obj = ReadObj(content);

            Assert.Equal(expected, obj.Points[0].IsDissolveInterpolationEnabled);
        }

        [Fact]
        public void RenderAttributes_LevelOfDetail_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("lod"));
            Assert.Throws<InvalidDataException>(() => ReadObj("lod 0 0"));
        }

        [Fact]
        public void RenderAttributes_LevelOfDetail_Valid()
        {
            string content = @"
lod 2
v 0 0 0
p 1
";

            var obj = ReadObj(content);

            Assert.Equal(2, obj.Points[0].LevelOfDetail);
        }

        [Fact]
        public void RenderAttributes_MapLibrary_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("maplib"));
            Assert.Throws<InvalidDataException>(() => ReadObj("maplib a"));
        }

        [Fact]
        public void RenderAttributes_MapLibrary_Valid()
        {
            string content = "maplib a.a b.b";

            var obj = ReadObj(content);

            Assert.Equal(2, obj.MapLibraries.Count);
            Assert.Equal("a.a", obj.MapLibraries[0]);
            Assert.Equal("b.b", obj.MapLibraries[1]);
        }

        [Fact]
        public void RenderAttributes_MaterialLibrary_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("mtllib"));
            Assert.Throws<InvalidDataException>(() => ReadObj("mtllib a"));
        }

        [Fact]
        public void RenderAttributes_MaterialLibrary_Valid()
        {
            string content = "mtllib a.a b.b";

            var obj = ReadObj(content);

            Assert.Equal(2, obj.MaterialLibraries.Count);
            Assert.Equal("a.a", obj.MaterialLibraries[0]);
            Assert.Equal("b.b", obj.MaterialLibraries[1]);
        }

        [Fact]
        public void RenderAttributes_UseMap_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("usemap"));
            Assert.Throws<InvalidDataException>(() => ReadObj("usemap 0 0"));
        }

        [Theory]
        [InlineData("off", null)]
        [InlineData("a", "a")]
        public void RenderAttributes_UseMap_Valid(string value, string expected)
        {
            string content = @"
usemap " + value + @"
v 0 0 0
p 1
";

            var obj = ReadObj(content);

            Assert.Equal(expected, obj.Points[0].MapName);
        }

        [Fact]
        public void RenderAttributes_UseMaterial_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("usemtl"));
            Assert.Throws<InvalidDataException>(() => ReadObj("usemtl 0 0"));
        }

        [Theory]
        [InlineData("off", null)]
        [InlineData("a", "a")]
        public void RenderAttributes_UseMaterial_Valid(string value, string expected)
        {
            string content = @"
usemtl " + value + @"
v 0 0 0
p 1
";

            var obj = ReadObj(content);

            Assert.Equal(expected, obj.Points[0].MaterialName);
        }

        [Fact]
        public void RenderAttributes_ShadowObject_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("shadow_obj"));
            Assert.Throws<InvalidDataException>(() => ReadObj("shadow_obj a.a 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("shadow_obj a"));
        }

        [Fact]
        public void RenderAttributes_ShadowObject_Valid()
        {
            string content = "shadow_obj a.a";

            var obj = ReadObj(content);

            Assert.Equal("a.a", obj.ShadowObjectFileName);
        }

        [Fact]
        public void RenderAttributes_TraceObject_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("trace_obj"));
            Assert.Throws<InvalidDataException>(() => ReadObj("trace_obj a.a 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("trace_obj a"));
        }

        [Fact]
        public void RenderAttributes_TraceObject_Valid()
        {
            string content = "trace_obj a.a";

            var obj = ReadObj(content);

            Assert.Equal("a.a", obj.TraceObjectFileName);
        }

        [Fact]
        public void RenderAttributes_ApproximationTechnique_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("ctech"));
            Assert.Throws<InvalidDataException>(() => ReadObj("ctech 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech 0"));
        }

        [Fact]
        public void RenderAttributes_ApproximationTechnique_Parametric_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("ctech cparm"));
            Assert.Throws<InvalidDataException>(() => ReadObj("ctech cparm 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech cparma"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech cparma 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech cparma 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech cparmb"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech cparmb 0 0"));
        }

        [Theory]
        [InlineData("ctech cparm 2.0", 2.0f, 2.0f, 0)]
        [InlineData("stech cparma 2.0 3.0", 2.0f, 3.0f, 1)]
        [InlineData("stech cparmb 2.0", 2.0f, 2.0f, 1)]
        public void RenderAttributes_ApproximationTechnique_Parametric_Valid(string value, float u, float v, int type)
        {
            string content = @"
v 0 0 0
v 0 0 0
" + value + @"
curv 0 1 1 2
";

            var obj = ReadObj(content);

            ObjApproximationTechnique technique;

            if (type == 0)
            {
                technique = obj.Curves[0].CurveApproximationTechnique;
            }
            else
            {
                technique = obj.Curves[0].SurfaceApproximationTechnique;

            }

            Assert.IsType<ObjConstantParametricSubdivisionTechnique>(technique);
            Assert.Equal(u, ((ObjConstantParametricSubdivisionTechnique)technique).ResolutionU);
            Assert.Equal(v, ((ObjConstantParametricSubdivisionTechnique)technique).ResolutionV);
        }

        [Fact]
        public void RenderAttributes_ApproximationTechnique_Spatial_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("ctech cspace"));
            Assert.Throws<InvalidDataException>(() => ReadObj("ctech cspace 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech cspace"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech cspace 0 0"));
        }

        [Theory]
        [InlineData("ctech cspace 2.0", 2.0f, 0)]
        [InlineData("stech cspace 2.0", 2.0f, 1)]
        public void RenderAttributes_ApproximationTechnique_Spatial_Valid(string value, float length, int type)
        {
            string content = @"
v 0 0 0
v 0 0 0
" + value + @"
curv 0 1 1 2
";

            var obj = ReadObj(content);

            ObjApproximationTechnique technique;

            if (type == 0)
            {
                technique = obj.Curves[0].CurveApproximationTechnique;
            }
            else
            {
                technique = obj.Curves[0].SurfaceApproximationTechnique;

            }

            Assert.IsType<ObjConstantSpatialSubdivisionTechnique>(technique);
            Assert.Equal(length, ((ObjConstantSpatialSubdivisionTechnique)technique).MaximumLength);
        }

        [Fact]
        public void RenderAttributes_ApproximationTechnique_Curvature_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadObj("ctech curv"));
            Assert.Throws<InvalidDataException>(() => ReadObj("ctech curv 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("ctech curv 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech curv"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech curv 0"));
            Assert.Throws<InvalidDataException>(() => ReadObj("stech curv 0 0 0"));
        }

        [Theory]
        [InlineData("ctech curv 2.0 3.0", 2.0f, 3.0f, 0)]
        [InlineData("stech curv 2.0 3.0", 2.0f, 3.0f, 1)]
        public void RenderAttributes_ApproximationTechnique_Curvature_Valid(string value, float distance, float angle, int type)
        {
            string content = @"
v 0 0 0
v 0 0 0
" + value + @"
curv 0 1 1 2
";

            var obj = ReadObj(content);

            ObjApproximationTechnique technique;

            if (type == 0)
            {
                technique = obj.Curves[0].CurveApproximationTechnique;
            }
            else
            {
                technique = obj.Curves[0].SurfaceApproximationTechnique;

            }

            Assert.IsType<ObjCurvatureDependentSubdivisionTechnique>(technique);
            Assert.Equal(distance, ((ObjCurvatureDependentSubdivisionTechnique)technique).MaximumDistance);
            Assert.Equal(angle, ((ObjCurvatureDependentSubdivisionTechnique)technique).MaximumAngle);
        }

        [Theory]
        [InlineData("bsp")]
        [InlineData("bzp")]
        [InlineData("cdc")]
        [InlineData("cdp")]
        [InlineData("res")]
        public void SupersededStatements_Throws(string statement)
        {
            Assert.Throws<NotImplementedException>(() => ReadObj(statement));
        }

        private static ObjFile ReadObj(string content)
        {
            var buffer = Encoding.UTF8.GetBytes(content);

            using (var stream = new MemoryStream(buffer, false))
            {
                return ObjFile.FromStream(stream);
            }
        }
    }
}
