// <copyright file="ObjMaterialFileReaderTests.cs" company="Jérémy Ansel">
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
    public class ObjMaterialFileReaderTests
    {
        [Fact]
        public void Parsing_UnknownStatement_Valid()
        {
            string content = "unknown";

            var mtl = ReadMtl(content);
        }

        [Fact]
        public void MaterialName_NoName_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl 0 0"));
        }

        [Fact]
        public void MaterialName_NewName_Valid()
        {
            string content = "newmtl a";

            var mtl = ReadMtl(content);

            Assert.Equal(1, mtl.Materials.Count);
            Assert.Equal("a", mtl.Materials[0].Name);
        }

        [Fact]
        public void MaterialColor_Ambient_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("Ka"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKa"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKa 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKa xyz"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKa xyz 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKa spectral"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKa spectral b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKa 0 0 0 0"));
        }

        [Fact]
        public void MaterialColor_AmbientRGB_Valid()
        {
            string content = @"
newmtl a
Ka 2.0 3.0 4.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].AmbientColor.IsRGB);
            Assert.False(mtl.Materials[0].AmbientColor.IsSpectral);
            Assert.False(mtl.Materials[0].AmbientColor.IsXYZ);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientColor.Color.X);
            Assert.Equal(3.0f, mtl.Materials[0].AmbientColor.Color.Y);
            Assert.Equal(4.0f, mtl.Materials[0].AmbientColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_AmbientRGB_Optional()
        {
            string content = @"
newmtl a
Ka 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].AmbientColor.IsRGB);
            Assert.False(mtl.Materials[0].AmbientColor.IsSpectral);
            Assert.False(mtl.Materials[0].AmbientColor.IsXYZ);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientColor.Color.X);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientColor.Color.Y);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_AmbientXYZ_Valid()
        {
            string content = @"
newmtl a
Ka xyz 2.0 3.0 4.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].AmbientColor.IsXYZ);
            Assert.False(mtl.Materials[0].AmbientColor.IsSpectral);
            Assert.False(mtl.Materials[0].AmbientColor.IsRGB);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientColor.Color.X);
            Assert.Equal(3.0f, mtl.Materials[0].AmbientColor.Color.Y);
            Assert.Equal(4.0f, mtl.Materials[0].AmbientColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_AmbientXYZ_Optional()
        {
            string content = @"
newmtl a
Ka xyz 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].AmbientColor.IsXYZ);
            Assert.False(mtl.Materials[0].AmbientColor.IsSpectral);
            Assert.False(mtl.Materials[0].AmbientColor.IsRGB);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientColor.Color.X);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientColor.Color.Y);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_AmbientSpectral_Valid()
        {
            string content = @"
newmtl a
Ka spectral b.b 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].AmbientColor.IsSpectral);
            Assert.False(mtl.Materials[0].AmbientColor.IsRGB);
            Assert.False(mtl.Materials[0].AmbientColor.IsXYZ);
            Assert.Equal("b.b", mtl.Materials[0].AmbientColor.SpectralFileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientColor.SpectralFactor);
        }

        [Fact]
        public void MaterialColor_AmbientSpectral_Optional()
        {
            string content = @"
newmtl a
Ka spectral b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].AmbientColor.IsSpectral);
            Assert.False(mtl.Materials[0].AmbientColor.IsRGB);
            Assert.False(mtl.Materials[0].AmbientColor.IsXYZ);
            Assert.Equal("b.b", mtl.Materials[0].AmbientColor.SpectralFileName);
            Assert.Equal(1.0f, mtl.Materials[0].AmbientColor.SpectralFactor);
        }

        [Fact]
        public void MaterialColor_Diffuse_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("Kd"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKd"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKd 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKd xyz"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKd xyz 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKd spectral"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKd spectral b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKd 0 0 0 0"));
        }

        [Fact]
        public void MaterialColor_DiffuseRGB_Valid()
        {
            string content = @"
newmtl a
Kd 2.0 3.0 4.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].DiffuseColor.IsRGB);
            Assert.Equal(2.0f, mtl.Materials[0].DiffuseColor.Color.X);
            Assert.Equal(3.0f, mtl.Materials[0].DiffuseColor.Color.Y);
            Assert.Equal(4.0f, mtl.Materials[0].DiffuseColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_DiffuseRGB_Optional()
        {
            string content = @"
newmtl a
Kd 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].DiffuseColor.IsRGB);
            Assert.Equal(2.0f, mtl.Materials[0].DiffuseColor.Color.X);
            Assert.Equal(2.0f, mtl.Materials[0].DiffuseColor.Color.Y);
            Assert.Equal(2.0f, mtl.Materials[0].DiffuseColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_DiffuseXYZ_Valid()
        {
            string content = @"
newmtl a
Kd xyz 2.0 3.0 4.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].DiffuseColor.IsXYZ);
            Assert.Equal(2.0f, mtl.Materials[0].DiffuseColor.Color.X);
            Assert.Equal(3.0f, mtl.Materials[0].DiffuseColor.Color.Y);
            Assert.Equal(4.0f, mtl.Materials[0].DiffuseColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_DiffuseXYZ_Optional()
        {
            string content = @"
newmtl a
Kd xyz 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].DiffuseColor.IsXYZ);
            Assert.Equal(2.0f, mtl.Materials[0].DiffuseColor.Color.X);
            Assert.Equal(2.0f, mtl.Materials[0].DiffuseColor.Color.Y);
            Assert.Equal(2.0f, mtl.Materials[0].DiffuseColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_DiffuseSpectral_Valid()
        {
            string content = @"
newmtl a
Kd spectral b.b 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].DiffuseColor.IsSpectral);
            Assert.Equal("b.b", mtl.Materials[0].DiffuseColor.SpectralFileName);
            Assert.Equal(2.0f, mtl.Materials[0].DiffuseColor.SpectralFactor);
        }

        [Fact]
        public void MaterialColor_DiffuseSpectral_Optional()
        {
            string content = @"
newmtl a
Kd spectral b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].DiffuseColor.IsSpectral);
            Assert.Equal("b.b", mtl.Materials[0].DiffuseColor.SpectralFileName);
            Assert.Equal(1.0f, mtl.Materials[0].DiffuseColor.SpectralFactor);
        }

        [Fact]
        public void MaterialColor_Emissive_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("Ke"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKe"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKe 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKe xyz"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKe xyz 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKe spectral"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKe spectral b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKe 0 0 0 0"));
        }

        [Fact]
        public void MaterialColor_EmissiveRGB_Valid()
        {
            string content = @"
newmtl a
Ke 2.0 3.0 4.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].EmissiveColor.IsRGB);
            Assert.Equal(2.0f, mtl.Materials[0].EmissiveColor.Color.X);
            Assert.Equal(3.0f, mtl.Materials[0].EmissiveColor.Color.Y);
            Assert.Equal(4.0f, mtl.Materials[0].EmissiveColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_EmissiveRGB_Optional()
        {
            string content = @"
newmtl a
Ke 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].EmissiveColor.IsRGB);
            Assert.Equal(2.0f, mtl.Materials[0].EmissiveColor.Color.X);
            Assert.Equal(2.0f, mtl.Materials[0].EmissiveColor.Color.Y);
            Assert.Equal(2.0f, mtl.Materials[0].EmissiveColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_EmissiveXYZ_Valid()
        {
            string content = @"
newmtl a
Ke xyz 2.0 3.0 4.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].EmissiveColor.IsXYZ);
            Assert.Equal(2.0f, mtl.Materials[0].EmissiveColor.Color.X);
            Assert.Equal(3.0f, mtl.Materials[0].EmissiveColor.Color.Y);
            Assert.Equal(4.0f, mtl.Materials[0].EmissiveColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_EmissiveXYZ_Optional()
        {
            string content = @"
newmtl a
Ke xyz 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].EmissiveColor.IsXYZ);
            Assert.Equal(2.0f, mtl.Materials[0].EmissiveColor.Color.X);
            Assert.Equal(2.0f, mtl.Materials[0].EmissiveColor.Color.Y);
            Assert.Equal(2.0f, mtl.Materials[0].EmissiveColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_EmissiveSpectral_Valid()
        {
            string content = @"
newmtl a
Ke spectral b.b 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].EmissiveColor.IsSpectral);
            Assert.Equal("b.b", mtl.Materials[0].EmissiveColor.SpectralFileName);
            Assert.Equal(2.0f, mtl.Materials[0].EmissiveColor.SpectralFactor);
        }

        [Fact]
        public void MaterialColor_EmissiveSpectral_Optional()
        {
            string content = @"
newmtl a
Ke spectral b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].EmissiveColor.IsSpectral);
            Assert.Equal("b.b", mtl.Materials[0].EmissiveColor.SpectralFileName);
            Assert.Equal(1.0f, mtl.Materials[0].EmissiveColor.SpectralFactor);
        }

        [Fact]
        public void MaterialColor_Specular_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("Ks"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKs"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKs 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKs xyz"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKs xyz 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKs spectral"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKs spectral b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nKs 0 0 0 0"));
        }

        [Fact]
        public void MaterialColor_SpecularRGB_Valid()
        {
            string content = @"
newmtl a
Ks 2.0 3.0 4.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].SpecularColor.IsRGB);
            Assert.Equal(2.0f, mtl.Materials[0].SpecularColor.Color.X);
            Assert.Equal(3.0f, mtl.Materials[0].SpecularColor.Color.Y);
            Assert.Equal(4.0f, mtl.Materials[0].SpecularColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_SpecularRGB_Optional()
        {
            string content = @"
newmtl a
Ks 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].SpecularColor.IsRGB);
            Assert.Equal(2.0f, mtl.Materials[0].SpecularColor.Color.X);
            Assert.Equal(2.0f, mtl.Materials[0].SpecularColor.Color.Y);
            Assert.Equal(2.0f, mtl.Materials[0].SpecularColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_SpecularXYZ_Valid()
        {
            string content = @"
newmtl a
Ks xyz 2.0 3.0 4.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].SpecularColor.IsXYZ);
            Assert.Equal(2.0f, mtl.Materials[0].SpecularColor.Color.X);
            Assert.Equal(3.0f, mtl.Materials[0].SpecularColor.Color.Y);
            Assert.Equal(4.0f, mtl.Materials[0].SpecularColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_SpecularXYZ_Optional()
        {
            string content = @"
newmtl a
Ks xyz 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].SpecularColor.IsXYZ);
            Assert.Equal(2.0f, mtl.Materials[0].SpecularColor.Color.X);
            Assert.Equal(2.0f, mtl.Materials[0].SpecularColor.Color.Y);
            Assert.Equal(2.0f, mtl.Materials[0].SpecularColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_SpecularSpectral_Valid()
        {
            string content = @"
newmtl a
Ks spectral b.b 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].SpecularColor.IsSpectral);
            Assert.Equal("b.b", mtl.Materials[0].SpecularColor.SpectralFileName);
            Assert.Equal(2.0f, mtl.Materials[0].SpecularColor.SpectralFactor);
        }

        [Fact]
        public void MaterialColor_SpecularSpectral_Optional()
        {
            string content = @"
newmtl a
Ks spectral b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].SpecularColor.IsSpectral);
            Assert.Equal("b.b", mtl.Materials[0].SpecularColor.SpectralFileName);
            Assert.Equal(1.0f, mtl.Materials[0].SpecularColor.SpectralFactor);
        }

        [Fact]
        public void MaterialColor_Transmission_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("Tf"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nTf"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nTf 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nTf xyz"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nTf xyz 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nTf spectral"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nTf spectral b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nTf 0 0 0 0"));
        }

        [Fact]
        public void MaterialColor_TransmissionRGB_Valid()
        {
            string content = @"
newmtl a
Tf 2.0 3.0 4.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].TransmissionColor.IsRGB);
            Assert.Equal(2.0f, mtl.Materials[0].TransmissionColor.Color.X);
            Assert.Equal(3.0f, mtl.Materials[0].TransmissionColor.Color.Y);
            Assert.Equal(4.0f, mtl.Materials[0].TransmissionColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_TransmissionRGB_Optional()
        {
            string content = @"
newmtl a
Tf 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].TransmissionColor.IsRGB);
            Assert.Equal(2.0f, mtl.Materials[0].TransmissionColor.Color.X);
            Assert.Equal(2.0f, mtl.Materials[0].TransmissionColor.Color.Y);
            Assert.Equal(2.0f, mtl.Materials[0].TransmissionColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_TransmissionXYZ_Valid()
        {
            string content = @"
newmtl a
Tf xyz 2.0 3.0 4.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].TransmissionColor.IsXYZ);
            Assert.Equal(2.0f, mtl.Materials[0].TransmissionColor.Color.X);
            Assert.Equal(3.0f, mtl.Materials[0].TransmissionColor.Color.Y);
            Assert.Equal(4.0f, mtl.Materials[0].TransmissionColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_TransmissionXYZ_Optional()
        {
            string content = @"
newmtl a
Tf xyz 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].TransmissionColor.IsXYZ);
            Assert.Equal(2.0f, mtl.Materials[0].TransmissionColor.Color.X);
            Assert.Equal(2.0f, mtl.Materials[0].TransmissionColor.Color.Y);
            Assert.Equal(2.0f, mtl.Materials[0].TransmissionColor.Color.Z);
        }

        [Fact]
        public void MaterialColor_TransmissionSpectral_Valid()
        {
            string content = @"
newmtl a
Tf spectral b.b 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].TransmissionColor.IsSpectral);
            Assert.Equal("b.b", mtl.Materials[0].TransmissionColor.SpectralFileName);
            Assert.Equal(2.0f, mtl.Materials[0].TransmissionColor.SpectralFactor);
        }

        [Fact]
        public void MaterialColor_TransmissionSpectral_Optional()
        {
            string content = @"
newmtl a
Tf spectral b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].TransmissionColor.IsSpectral);
            Assert.Equal("b.b", mtl.Materials[0].TransmissionColor.SpectralFileName);
            Assert.Equal(1.0f, mtl.Materials[0].TransmissionColor.SpectralFactor);
        }

        [Fact]
        public void Illumination_Model_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("illum"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nillum"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nillum 0 0"));
        }

        [Fact]
        public void Illumination_Model_Valid()
        {
            string content = @"
newmtl a
illum 2";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal(2, mtl.Materials[0].IlluminationModel);
        }

        [Fact]
        public void Illumination_DissolveFactor_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("d"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nd"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nd -halo"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nd 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nd -halo 0 0"));
        }

        [Fact]
        public void Illumination_DissolveFactor_Valid()
        {
            string content = @"
newmtl a
d 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.False(mtl.Materials[0].IsHaloDissolve);
            Assert.Equal(2.0f, mtl.Materials[0].DissolveFactor);
        }

        [Fact]
        public void Illumination_DissolveFactorHalo_Valid()
        {
            string content = @"
newmtl a
d -halo 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.True(mtl.Materials[0].IsHaloDissolve);
            Assert.Equal(2.0f, mtl.Materials[0].DissolveFactor);
        }

        [Fact]
        public void Illumination_SpecularExponent_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("Ns"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nNs"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nNs 0 0"));
        }

        [Fact]
        public void Illumination_SpecularExponent_Valid()
        {
            string content = @"
newmtl a
Ns 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal(2.0f, mtl.Materials[0].SpecularExponent);
        }

        [Fact]
        public void Illumination_Sharpness_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("sharpness"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nsharpness"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nsharpness 0 0"));
        }

        [Fact]
        public void Illumination_Sharpness_Valid()
        {
            string content = @"
newmtl a
sharpness 2";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal(2, mtl.Materials[0].Sharpness);
        }

        [Fact]
        public void Illumination_OpticalDensity_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("Ni"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nNi"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nNi 0 0"));
        }

        [Fact]
        public void Illumination_OpticalDensity_Valid()
        {
            string content = @"
newmtl a
Ni 2.0";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal(2.0f, mtl.Materials[0].OpticalDensity);
        }

        [Fact]
        public void Texture_AntiAliasing_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("map_aat"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_aat"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_aat 1"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_aat 0 0"));
        }

        [Theory]
        [InlineData("on", true)]
        [InlineData("off", false)]
        public void Texture_AntiAliasing_Valid(string value, bool enabled)
        {
            string content = @"
newmtl a
map_aat " + value;

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal(enabled, mtl.Materials[0].IsAntiAliasingEnabled);
        }

        [Fact]
        public void Texture_Ambient_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("map_Ka"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka b.b 0"));
        }

        [Fact]
        public void Texture_Ambient_Valid()
        {
            string content = @"
newmtl a
map_Ka b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
        }

        [Fact]
        public void Texture_Diffuse_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("map_Kd"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Kd"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Kd b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Kd b.b 0"));
        }

        [Fact]
        public void Texture_Diffuse_Valid()
        {
            string content = @"
newmtl a
map_Kd b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].DiffuseMap.FileName);
        }

        [Fact]
        public void Texture_Emissive_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("map_Ke"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ke"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ke b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ke b.b 0"));
        }

        [Fact]
        public void Texture_Emissive_Valid()
        {
            string content = @"
newmtl a
map_Ke b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].EmissiveMap.FileName);
        }

        [Fact]
        public void Texture_Specular_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("map_Ks"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ks"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ks b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ks b.b 0"));
        }

        [Fact]
        public void Texture_Specular_Valid()
        {
            string content = @"
newmtl a
map_Ks b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].SpecularMap.FileName);
        }

        [Fact]
        public void Texture_SpecularExponent_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("map_Ns"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ns"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ns b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ns b.b 0"));
        }

        [Fact]
        public void Texture_SpecularExponent_Valid()
        {
            string content = @"
newmtl a
map_Ns b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].SpecularExponentMap.FileName);
        }

        [Fact]
        public void Texture_Dissolve_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("map_d"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_d"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_d b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_d b.b 0"));
        }

        [Fact]
        public void Texture_Dissolve_Valid()
        {
            string content = @"
newmtl a
map_d b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].DissolveMap.FileName);
        }

        [Fact]
        public void Texture_Decal_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("decal"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\ndecal"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\ndecal b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\ndecal b.b 0"));
        }

        [Fact]
        public void Texture_Decal_Valid()
        {
            string content = @"
newmtl a
decal b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].DecalMap.FileName);
        }

        [Fact]
        public void Texture_Disp_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("disp"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\ndisp"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\ndisp b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\ndisp b.b 0"));
        }

        [Fact]
        public void Texture_Disp_Valid()
        {
            string content = @"
newmtl a
disp b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].DispMap.FileName);
        }

        [Fact]
        public void Texture_Bump_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("bump"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nbump"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nbump b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nbump b.b 0"));
        }

        [Fact]
        public void Texture_Bump_Valid()
        {
            string content = @"
newmtl a
bump b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].BumpMap.FileName);
        }

        [Fact]
        public void Texture_Reflection_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("refl"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nrefl"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nrefl -type"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nrefl -type sphere"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nrefl -type sphere b"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nrefl 0 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nrefl -type sphere b.b 0"));
        }

        [Fact]
        public void Texture_ReflectionUnknown_Ignore()
        {
            string content = @"
newmtl a
refl -type unknown b.b";

            var mtl = ReadMtl(content);
        }

        [Fact]
        public void Texture_ReflectionSphere_Valid()
        {
            string content = @"
newmtl a
refl -type sphere b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.Sphere.FileName);
        }

        [Fact]
        public void Texture_ReflectionCubeTop_Valid()
        {
            string content = @"
newmtl a
refl -type cube_top b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeTop.FileName);
        }

        [Fact]
        public void Texture_ReflectionCubeBottom_Valid()
        {
            string content = @"
newmtl a
refl -type cube_bottom b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeBottom.FileName);
        }

        [Fact]
        public void Texture_ReflectionCubeFront_Valid()
        {
            string content = @"
newmtl a
refl -type cube_front b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeFront.FileName);
        }

        [Fact]
        public void Texture_ReflectionCubeBack_Valid()
        {
            string content = @"
newmtl a
refl -type cube_back b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeBack.FileName);
        }

        [Fact]
        public void Texture_ReflectionCubeLeft_Valid()
        {
            string content = @"
newmtl a
refl -type cube_left b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeLeft.FileName);
        }

        [Fact]
        public void Texture_ReflectionCubeRight_Valid()
        {
            string content = @"
newmtl a
refl -type cube_right b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeRight.FileName);
        }

        [Fact]
        public void MapOptions_Throws()
        {
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -type"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -type 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -blenu"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -blenu 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -blenu 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -blenv"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -blenv 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -blenv 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -bm"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -bm 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -boost"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -boost 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -cc"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -cc 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -cc 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -clamp"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -clamp 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -clamp 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -imfchan"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -imfchan 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -imfchan 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -mm"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -mm 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -mm 0 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -o"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -o 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -s"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -s 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -t"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -t 0"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -texres"));
            Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nmap_Ka -texres 0"));
        }

        [Theory]
        [InlineData("on", true)]
        [InlineData("off", false)]
        public void MapOptions_HorizontalBlending_Valid(string value, bool expected)
        {
            string content = @"
newmtl a
map_Ka -blenu " + value + @" b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(expected, mtl.Materials[0].AmbientMap.IsHorizontalBlendingEnabled);
        }

        [Theory]
        [InlineData("on", true)]
        [InlineData("off", false)]
        public void MapOptions_VerticalBlending_Valid(string value, bool expected)
        {
            string content = @"
newmtl a
map_Ka -blenv " + value + @" b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(expected, mtl.Materials[0].AmbientMap.IsVerticalBlendingEnabled);
        }

        [Fact]
        public void MapOptions_BumpMultiplier_Valid()
        {
            string content = @"
newmtl a
map_Ka -bm 2.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.BumpMultiplier);
        }

        [Fact]
        public void MapOptions_Boost_Valid()
        {
            string content = @"
newmtl a
map_Ka -boost 2.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.Boost);
        }

        [Theory]
        [InlineData("on", true)]
        [InlineData("off", false)]
        public void MapOptions_ColorCorrection_Valid(string value, bool expected)
        {
            string content = @"
newmtl a
map_Ka -cc " + value + @" b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(expected, mtl.Materials[0].AmbientMap.IsColorCorrectionEnabled);
        }

        [Theory]
        [InlineData("on", true)]
        [InlineData("off", false)]
        public void MapOptions_Clamping_Valid(string value, bool expected)
        {
            string content = @"
newmtl a
map_Ka -clamp " + value + @" b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(expected, mtl.Materials[0].AmbientMap.IsClampingEnabled);
        }

        [Theory]
        [InlineData("r", ObjMapChannel.Red)]
        [InlineData("g", ObjMapChannel.Green)]
        [InlineData("b", ObjMapChannel.Blue)]
        [InlineData("m", ObjMapChannel.Matte)]
        [InlineData("l", ObjMapChannel.Luminance)]
        [InlineData("z", ObjMapChannel.Depth)]
        public void MapOptions_ScalarChannel_Valid(string value, ObjMapChannel expected)
        {
            string content = @"
newmtl a
map_Ka -imfchan " + value + @" b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(expected, mtl.Materials[0].AmbientMap.ScalarChannel);
        }

        [Fact]
        public void MapOptions_Modifier_Valid()
        {
            string content = @"
newmtl a
map_Ka -mm 2.0 3.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.ModifierBase);
            Assert.Equal(3.0f, mtl.Materials[0].AmbientMap.ModifierGain);
        }

        [Fact]
        public void MapOptions_Offset1_Valid()
        {
            string content = @"
newmtl a
map_Ka -o 2.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.Offset.X);
            Assert.Equal(0.0f, mtl.Materials[0].AmbientMap.Offset.Y);
            Assert.Equal(0.0f, mtl.Materials[0].AmbientMap.Offset.Z);
        }

        [Fact]
        public void MapOptions_Offset2_Valid()
        {
            string content = @"
newmtl a
map_Ka -o 2.0 3.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.Offset.X);
            Assert.Equal(3.0f, mtl.Materials[0].AmbientMap.Offset.Y);
            Assert.Equal(0.0f, mtl.Materials[0].AmbientMap.Offset.Z);
        }

        [Fact]
        public void MapOptions_Offset3_Valid()
        {
            string content = @"
newmtl a
map_Ka -o 2.0 3.0 4.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.Offset.X);
            Assert.Equal(3.0f, mtl.Materials[0].AmbientMap.Offset.Y);
            Assert.Equal(4.0f, mtl.Materials[0].AmbientMap.Offset.Z);
        }

        [Fact]
        public void MapOptions_Scale1_Valid()
        {
            string content = @"
newmtl a
map_Ka -s 2.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.Scale.X);
            Assert.Equal(1.0f, mtl.Materials[0].AmbientMap.Scale.Y);
            Assert.Equal(1.0f, mtl.Materials[0].AmbientMap.Scale.Z);
        }

        [Fact]
        public void MapOptions_Scale2_Valid()
        {
            string content = @"
newmtl a
map_Ka -s 2.0 3.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.Scale.X);
            Assert.Equal(3.0f, mtl.Materials[0].AmbientMap.Scale.Y);
            Assert.Equal(1.0f, mtl.Materials[0].AmbientMap.Scale.Z);
        }

        [Fact]
        public void MapOptions_Scale3_Valid()
        {
            string content = @"
newmtl a
map_Ka -s 2.0 3.0 4.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.Scale.X);
            Assert.Equal(3.0f, mtl.Materials[0].AmbientMap.Scale.Y);
            Assert.Equal(4.0f, mtl.Materials[0].AmbientMap.Scale.Z);
        }

        [Fact]
        public void MapOptions_Turbulence1_Valid()
        {
            string content = @"
newmtl a
map_Ka -t 2.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.Turbulence.X);
            Assert.Equal(0.0f, mtl.Materials[0].AmbientMap.Turbulence.Y);
            Assert.Equal(0.0f, mtl.Materials[0].AmbientMap.Turbulence.Z);
        }

        [Fact]
        public void MapOptions_Turbulence2_Valid()
        {
            string content = @"
newmtl a
map_Ka -t 2.0 3.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.Turbulence.X);
            Assert.Equal(3.0f, mtl.Materials[0].AmbientMap.Turbulence.Y);
            Assert.Equal(0.0f, mtl.Materials[0].AmbientMap.Turbulence.Z);
        }

        [Fact]
        public void MapOptions_Turbulence3_Valid()
        {
            string content = @"
newmtl a
map_Ka -t 2.0 3.0 4.0 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2.0f, mtl.Materials[0].AmbientMap.Turbulence.X);
            Assert.Equal(3.0f, mtl.Materials[0].AmbientMap.Turbulence.Y);
            Assert.Equal(4.0f, mtl.Materials[0].AmbientMap.Turbulence.Z);
        }

        [Fact]
        public void MapOptions_TextureResolution_Valid()
        {
            string content = @"
newmtl a
map_Ka -texres 2 b.b";

            var mtl = ReadMtl(content);

            Assert.Equal("a", mtl.Materials[0].Name);
            Assert.Equal("b.b", mtl.Materials[0].AmbientMap.FileName);
            Assert.Equal(2, mtl.Materials[0].AmbientMap.TextureResolution);
        }

        private ObjMaterialFile ReadMtl(string content)
        {
            var buffer = Encoding.UTF8.GetBytes(content);

            using (var stream = new MemoryStream(buffer, false))
            {
                return ObjMaterialFile.FromStream(stream);
            }
        }
    }
}
