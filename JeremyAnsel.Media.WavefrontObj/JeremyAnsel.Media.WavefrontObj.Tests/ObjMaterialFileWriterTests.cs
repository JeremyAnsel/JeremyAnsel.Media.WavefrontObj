// <copyright file="ObjMaterialFileWriterTests.cs" company="Jérémy Ansel">
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
    public class ObjMaterialFileWriterTests
    {
        [Fact]
        public void Write_New_Valid()
        {
            var mtl = new ObjMaterialFile();

            string text = WriteMtl(mtl);
            string expected =
@"";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialName_NewName_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Illumination_Model_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.IlluminationModel = 4;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 4
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Illumination_DissolveFactor_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.DissolveFactor = 2.0f;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 2.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Illumination_DissolveFactorHalo_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.DissolveFactor = 2.0f;
            material.IsHaloDissolve = true;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d -halo 2.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Illumination_SpecularExponent_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.SpecularExponent = 2.0f;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 2.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Illumination_Sharpness_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.Sharpness = 100;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 100
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Illumination_OpticalDensity_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.OpticalDensity = 2.0f;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 2.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData("on", true)]
        [InlineData("off", false)]
        public void Texture_AntiAliasing_Valid(string value, bool enabled)
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.IsAntiAliasingEnabled = enabled;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat " + value + @"
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_AmbientRGB_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientColor = new ObjMaterialColor(2.0f, 3.0f, 4.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ka 2.000000 3.000000 4.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_AmbientXYZ_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientColor = new ObjMaterialColor(2.0f, 3.0f, 4.0f) { UseXYZColorSpace = true };

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ka xyz 2.000000 3.000000 4.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_AmbientSpectral_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientColor = new ObjMaterialColor("b.b", 2.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ka spectral b.b 2.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_AmbientSpectral_Optional()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientColor = new ObjMaterialColor("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ka spectral b.b
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_DiffuseRGB_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.DiffuseColor = new ObjMaterialColor(2.0f, 3.0f, 4.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Kd 2.000000 3.000000 4.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_DiffuseXYZ_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.DiffuseColor = new ObjMaterialColor(2.0f, 3.0f, 4.0f) { UseXYZColorSpace = true };

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Kd xyz 2.000000 3.000000 4.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_DiffuseSpectral_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.DiffuseColor = new ObjMaterialColor("b.b", 2.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Kd spectral b.b 2.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_DiffuseSpectral_Optional()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.DiffuseColor = new ObjMaterialColor("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Kd spectral b.b
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_EmissiveRGB_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.EmissiveColor = new ObjMaterialColor(2.0f, 3.0f, 4.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ke 2.000000 3.000000 4.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_EmissiveXYZ_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.EmissiveColor = new ObjMaterialColor(2.0f, 3.0f, 4.0f) { UseXYZColorSpace = true };

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ke xyz 2.000000 3.000000 4.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_EmissiveSpectral_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.EmissiveColor = new ObjMaterialColor("b.b", 2.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ke spectral b.b 2.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_EmissiveSpectral_Optional()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.EmissiveColor = new ObjMaterialColor("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ke spectral b.b
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_SpecularRGB_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.SpecularColor = new ObjMaterialColor(2.0f, 3.0f, 4.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ks 2.000000 3.000000 4.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_SpecularXYZ_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.SpecularColor = new ObjMaterialColor(2.0f, 3.0f, 4.0f) { UseXYZColorSpace = true };

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ks xyz 2.000000 3.000000 4.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_SpecularSpectral_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.SpecularColor = new ObjMaterialColor("b.b", 2.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ks spectral b.b 2.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_SpecularSpectral_Optional()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.SpecularColor = new ObjMaterialColor("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Ks spectral b.b
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_TransmissionRGB_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.TransmissionColor = new ObjMaterialColor(2.0f, 3.0f, 4.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Tf 2.000000 3.000000 4.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_TransmissionXYZ_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.TransmissionColor = new ObjMaterialColor(2.0f, 3.0f, 4.0f) { UseXYZColorSpace = true };

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Tf xyz 2.000000 3.000000 4.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_TransmissionSpectral_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.TransmissionColor = new ObjMaterialColor("b.b", 2.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Tf spectral b.b 2.000000
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MaterialColor_TransmissionSpectral_Optional()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.TransmissionColor = new ObjMaterialColor("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
Tf spectral b.b
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_Ambient_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_Diffuse_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.DiffuseMap = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Kd b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_Emissive_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.EmissiveMap = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ke b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_Specular_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.SpecularMap = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ks b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_SpecularExponent_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.SpecularExponentMap = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ns b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_Dissolve_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.DissolveMap = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_d b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_Decal_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.DecalMap = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
decal b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_Disp_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.DispMap = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
disp b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_Bump_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.BumpMap = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
bump b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_ReflectionSphere_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.ReflectionMap.Sphere = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
refl -type sphere b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_ReflectionCubeTop_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.ReflectionMap.CubeTop = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
refl -type cube_top b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_ReflectionCubeBottom_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.ReflectionMap.CubeBottom = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
refl -type cube_bottom b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_ReflectionCubeFront_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.ReflectionMap.CubeFront = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
refl -type cube_front b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_ReflectionCubeBack_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.ReflectionMap.CubeBack = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
refl -type cube_back b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_ReflectionCubeLeft_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.ReflectionMap.CubeLeft = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
refl -type cube_left b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void Texture_ReflectionCubeRight_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.ReflectionMap.CubeRight = new ObjMaterialMap("b.b");

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
refl -type cube_right b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MapOptions_HorizontalBlending_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.IsHorizontalBlendingEnabled = false;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -blenu off b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MapOptions_VerticalBlending_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.IsVerticalBlendingEnabled = false;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -blenv off b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MapOptions_BumpMultiplier_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.BumpMultiplier = 2.0f;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -bm 2.000000 b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MapOptions_Boost_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.Boost = 2.0f;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -boost 2.000000 b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MapOptions_ColorCorrection_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.IsColorCorrectionEnabled = true;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -cc on b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MapOptions_Clamping_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.IsClampingEnabled = true;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -clamp on b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Theory]
        [InlineData("r", ObjMapChannel.Red)]
        [InlineData("g", ObjMapChannel.Green)]
        [InlineData("b", ObjMapChannel.Blue)]
        [InlineData("m", ObjMapChannel.Matte)]
        [InlineData("z", ObjMapChannel.Depth)]
        [InlineData("l", ObjMapChannel.Depth + 1)]
        public void MapOptions_ScalarChannel_Valid(string value, ObjMapChannel channel)
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.ScalarChannel = channel;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -imfchan " + value + @" b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MapOptions_Modifier_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.ModifierBase = 2.0f;
            material.AmbientMap.ModifierGain = 3.0f;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -mm 2.000000 3.000000 b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MapOptions_Offset3_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.Offset = new ObjVector3(2.0f, 3.0f, 4.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -o 2.000000 3.000000 4.000000 b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MapOptions_Scale3_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.Scale = new ObjVector3(2.0f, 3.0f, 4.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -s 2.000000 3.000000 4.000000 b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MapOptions_Turbulence3_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.Turbulence = new ObjVector3(2.0f, 3.0f, 4.0f);

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -t 2.000000 3.000000 4.000000 b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        [Fact]
        public void MapOptions_TextureResolution_Valid()
        {
            var mtl = new ObjMaterialFile();
            var material = new ObjMaterial("a");
            mtl.Materials.Add(material);
            material.AmbientMap = new ObjMaterialMap("b.b");
            material.AmbientMap.TextureResolution = 2;

            string text = WriteMtl(mtl);
            string expected =
@"newmtl a
illum 2
d 1.000000
Ns 0.000000
sharpness 60
Ni 1.000000
map_aat off
map_Ka -texres 2 b.b
";

            AssertExtensions.TextEqual(expected, text);
        }

        private static string WriteMtl(ObjMaterialFile mtl)
        {
            byte[] buffer;

            using (var stream = new MemoryStream())
            {
                mtl.WriteTo(stream);

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
