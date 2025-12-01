// <copyright file="ObjMaterialFileReaderTests.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xunit;

namespace JeremyAnsel.Media.WavefrontObj.Tests;

public class ObjMaterialFileReaderTests
{
    [Fact]
    public void Parsing_UnknownStatement_Valid()
    {
        string content = "unknown";
        var exception = Record.Exception(() => ReadMtl(content));
        Assert.Null(exception);
    }

    [Fact]
    public void Parsing_UnknownLongStatement_Valid()
    {
        string content = "unknown_unknown_unknown_unknown";
        var exception = Record.Exception(() => ReadMtl(content));
        Assert.Null(exception);
    }

    [Fact]
    public void Parsing_UnknownSpacesStatement_Valid()
    {
        string content = "unknown \t  \t\t 0";
        var exception = Record.Exception(() => ReadMtl(content));
        Assert.Null(exception);
    }

    [Fact]
    public void Parsing_SpacesStatement_Valid()
    {
        string content = "newmtl \t  \t\t a";

        var mtl = ReadMtl(content);

        Assert.Single(mtl.Materials);
        Assert.Equal("a", mtl.Materials[0].Name);
    }

    [Fact]
    public void HeaderText_Valid()
    {
        string content = @"
# header line 1

# header line 2

# header \
line 3
newmtl a
# comment
";

        var mtl = ReadMtl(content);

        Assert.Equal("\n header line 1\n\n header line 2\n\n header line 3", mtl.HeaderText);
        Assert.Single(mtl.Materials);
        Assert.Equal("a", mtl.Materials[0].Name);
    }

    [Fact]
    public void MaterialName_NoName_Throws()
    {
        Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl"));
    }

    [Fact]
    public void MaterialName_NewName_Valid()
    {
        string content = "newmtl a with spaces";

        var mtl = ReadMtl(content);

        Assert.Single(mtl.Materials);
        Assert.Equal("a with spaces", mtl.Materials[0].Name);
    }

    [Fact]
    public void MaterialName_NewNamea_Valid()
    {
        string content = "newmtl Plastica (1)";

        var mtl = ReadMtl(content);

        Assert.Single(mtl.Materials);
        Assert.Equal("Plastica (1)", mtl.Materials[0].Name);
    }

    [Theory]
    [MemberData(nameof(MaterialColorInvalidTestData))]
    public void MaterialColor_Ambient_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Ka");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(MaterialColorValidTestData))]
    public void MaterialColor_Ambient_Valid(string materialStringTemplate, ObjMaterialColor expected)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Ka");
        var parsed = ReadMtl(materialString);
        Assert.Equivalent(expected, parsed.Materials[0].AmbientColor);
    }

    [Theory]
    [MemberData(nameof(MaterialColorInvalidTestData))]
    public void MaterialColor_Diffuse_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Kd");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(MaterialColorValidTestData))]
    public void MaterialColor_Diffuse_Valid(string materialStringTemplate, ObjMaterialColor expected)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Kd");
        var parsed = ReadMtl(materialString);
        Assert.Equivalent(expected, parsed.Materials[0].DiffuseColor);
    }

    [Theory]
    [MemberData(nameof(MaterialColorInvalidTestData))]
    public void MaterialColor_Specular_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Ks");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(MaterialColorValidTestData))]
    public void MaterialColor_Specular_Valid(string materialStringTemplate, ObjMaterialColor expected)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Ks");
        var parsed = ReadMtl(materialString);
        Assert.Equivalent(expected, parsed.Materials[0].SpecularColor);
    }

    [Theory]
    [MemberData(nameof(MaterialColorInvalidTestData))]
    public void MaterialColor_Emissive_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Ke");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(MaterialColorValidTestData))]
    public void MaterialColor_Emissive_Valid(string materialStringTemplate, ObjMaterialColor expected)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Ke");
        var parsed = ReadMtl(materialString);
        Assert.Equivalent(expected, parsed.Materials[0].EmissiveColor);
    }

    [Theory]
    [MemberData(nameof(MaterialColorInvalidTestData))]
    public void MaterialColor_Transmission_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Tf");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(MaterialColorValidTestData))]
    public void MaterialColor_Transmission_Valid(string materialStringTemplate, ObjMaterialColor expected)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Tf");
        var parsed = ReadMtl(materialString);
        Assert.Equivalent(expected, parsed.Materials[0].TransmissionColor);
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

    [Theory]
    [MemberData(nameof(ObjectMaterialMapInvalidTestData))]
    public void Texture_Ambient_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Ka");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapValidTestData))]
    public void Texture_Ambient_Valid(string materialStringTemplate, string expectedFilename, bool keepWhitespaces)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Ka");
        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings { KeepWhitespacesOfMapFileReferences = keepWhitespaces});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapInvalidTestData))]
    public void Texture_Diffuse_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Kd");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapValidTestData))]
    public void Texture_Diffuse_Valid(string materialStringTemplate, string expectedFilename, bool keepWhitespaces)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Kd");
        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings { KeepWhitespacesOfMapFileReferences = keepWhitespaces});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].DiffuseMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].DiffuseMap?.FileName);
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapInvalidTestData))]
    public void Texture_Emissive_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Ke");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapValidTestData))]
    public void Texture_Emissive_Valid(string materialStringTemplate, string expectedFilename, bool keepWhitespaces)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Ke");
        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings { KeepWhitespacesOfMapFileReferences = keepWhitespaces});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].EmissiveMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].EmissiveMap?.FileName);
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapInvalidTestData))]
    public void Texture_Specular_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Ks");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapValidTestData))]
    public void Texture_Specular_Valid(string materialStringTemplate, string expectedFilename, bool keepWhitespaces)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Ks");
        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings { KeepWhitespacesOfMapFileReferences = keepWhitespaces});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].SpecularMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].SpecularMap?.FileName);
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapInvalidTestData))]
    public void Texture_SpecularExponent_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Ns");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapValidTestData))]
    public void Texture_SpecularExponent_Valid(string materialStringTemplate, string expectedFilename, bool keepWhitespaces)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Ns");
        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings { KeepWhitespacesOfMapFileReferences = keepWhitespaces});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].SpecularExponentMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].SpecularExponentMap?.FileName);
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapInvalidTestData))]
    public void Texture_Dissolve_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_d");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapValidTestData))]
    public void Texture_Dissolve_Valid(string materialStringTemplate, string expectedFilename, bool keepWhitespaces)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_d");
        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings { KeepWhitespacesOfMapFileReferences = keepWhitespaces});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].DissolveMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].DissolveMap?.FileName);
    }

    [Fact]
    public void Texture_Decal_Throws()
    {
        Assert.Throws<InvalidDataException>(() => ReadMtl("decal"));
        Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\ndecal"));
    }

    [Fact]
    public void Texture_Decal_Valid()
    {
        string content = @"
newmtl a
decal b.b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].DecalMap);
        Assert.Equal("b.b", mtl.Materials[0].DecalMap?.FileName);
    }

    [Fact]
    public void Texture_Decal_ValidWithoutExtension()
    {
        string content = @"
newmtl a
decal b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].DecalMap);
        Assert.Equal("b", mtl.Materials[0].DecalMap?.FileName);
    }

    [Fact]
    public void Texture_Decal_ValidWithExtensionIncludingWhitespace()
    {
        string content = @"
newmtl a
decal b.b 0";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].DecalMap);
        Assert.Equal("b.b 0", mtl.Materials[0].DecalMap?.FileName);
    }

    [Fact]
    public void Texture_Disp_Throws()
    {
        Assert.Throws<InvalidDataException>(() => ReadMtl("disp"));
        Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\ndisp"));
    }

    [Fact]
    public void Texture_Disp_Valid()
    {
        string content = @"
newmtl a
disp b.b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].DispMap);
        Assert.Equal("b.b", mtl.Materials[0].DispMap?.FileName);
    }

    [Fact]
    public void Texture_Disp_ValidWithoutExtension()
    {
        string content = @"
newmtl a
disp b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].DispMap);
        Assert.Equal("b", mtl.Materials[0].DispMap?.FileName);
    }

    [Fact]
    public void Texture_Disp_ValidWithExtensionIncludingWhitespace()
    {
        string content = @"
newmtl a
disp b.b 0";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].DispMap);
        Assert.Equal("b.b 0", mtl.Materials[0].DispMap?.FileName);
    }

    [Fact]
    public void Texture_Bump_Throws()
    {
        Assert.Throws<InvalidDataException>(() => ReadMtl("bump"));
        Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nbump"));
    }

    [Fact]
    public void Texture_Bump_Valid()
    {
        string content = @"
newmtl a
bump b.b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].BumpMap);
        Assert.Equal("b.b", mtl.Materials[0].BumpMap?.FileName);
    }

    [Fact]
    public void Texture_Reflection_Throws()
    {
        Assert.Throws<InvalidDataException>(() => ReadMtl("refl"));
        Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nrefl"));
        Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nrefl {00000000-0000-0000-0000-000000000000}"));
        Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nrefl -type"));
        Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nrefl -type sphere"));
        Assert.Throws<InvalidDataException>(() => ReadMtl("newmtl a\nrefl 0 0 0"));
    }

    [Fact]
    public void Texture_ReflectionUnknown_Ignore()
    {
        string content = @"
newmtl a
refl -type unknown b.b";

        _ = ReadMtl(content);
    }

    [Fact]
    public void Texture_ReflectionSphere_Valid()
    {
        string content = @"
newmtl a
refl -type sphere b.b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.Sphere);
        Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.Sphere?.FileName);
    }

    [Fact]
    public void Texture_ReflectionSphere_ValidWithoutExtension()
    {
        string content = @"
newmtl a
refl -type sphere b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.Sphere);
        Assert.Equal("b", mtl.Materials[0].ReflectionMap.Sphere?.FileName);
    }

    [Fact]
    public void Texture_ReflectionSphere_ValidWithExtensionIncludingWhitespace()
    {
        string content = @"
newmtl a
refl -type sphere b.b 0";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.Sphere);
        Assert.Equal("b.b 0", mtl.Materials[0].ReflectionMap.Sphere?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeTop_Valid()
    {
        string content = @"
newmtl a
refl -type cube_top b.b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeTop);
        Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeTop?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeTop_ValidWithoutExtension()
    {
        string content = @"
newmtl a
refl -type cube_top b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeTop);
        Assert.Equal("b", mtl.Materials[0].ReflectionMap.CubeTop?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeTop_ValidWithExtensionIncludingWhitespace()
    {
        string content = @"
newmtl a
refl -type cube_top b.b 0";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeTop);
        Assert.Equal("b.b 0", mtl.Materials[0].ReflectionMap.CubeTop?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeBottom_Valid()
    {
        string content = @"
newmtl a
refl -type cube_bottom b.b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeBottom);
        Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeBottom?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeBottom_ValidWithoutExtension()
    {
        string content = @"
newmtl a
refl -type cube_bottom b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeBottom);
        Assert.Equal("b", mtl.Materials[0].ReflectionMap.CubeBottom?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeBottom_ValidWithExtensionIncludingWhitespace()
    {
        string content = @"
newmtl a
refl -type cube_bottom b.b 0";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeBottom);
        Assert.Equal("b.b 0", mtl.Materials[0].ReflectionMap.CubeBottom?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeFront_Valid()
    {
        string content = @"
newmtl a
refl -type cube_front b.b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeFront);
        Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeFront?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeFront_ValidWithoutExtension()
    {
        string content = @"
newmtl a
refl -type cube_front b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeFront);
        Assert.Equal("b", mtl.Materials[0].ReflectionMap.CubeFront?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeFront_ValidWithExtensionIncludingWhitespace()
    {
        string content = @"
newmtl a
refl -type cube_front b.b 0";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeFront);
        Assert.Equal("b.b 0", mtl.Materials[0].ReflectionMap.CubeFront?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeBack_Valid()
    {
        string content = @"
newmtl a
refl -type cube_back b.b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeBack);
        Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeBack?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeBack_ValidWithoutExtension()
    {
        string content = @"
newmtl a
refl -type cube_back b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeBack);
        Assert.Equal("b", mtl.Materials[0].ReflectionMap.CubeBack?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeBack_ValidWithExtensionIncludingWhitespace()
    {
        string content = @"
newmtl a
refl -type cube_back b.b 0";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeBack);
        Assert.Equal("b.b 0", mtl.Materials[0].ReflectionMap.CubeBack?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeLeft_Valid()
    {
        string content = @"
newmtl a
refl -type cube_left b.b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeLeft);
        Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeLeft?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeLeft_ValidWithoutExtension()
    {
        string content = @"
newmtl a
refl -type cube_left b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeLeft);
        Assert.Equal("b", mtl.Materials[0].ReflectionMap.CubeLeft?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeLeft_ValidWithExtensionIncludingWhitespace()
    {
        string content = @"
newmtl a
refl -type cube_left b.b 0";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeLeft);
        Assert.Equal("b.b 0", mtl.Materials[0].ReflectionMap.CubeLeft?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeRight_Valid()
    {
        string content = @"
newmtl a
refl -type cube_right b.b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeRight);
        Assert.Equal("b.b", mtl.Materials[0].ReflectionMap.CubeRight?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeRight_ValidWithoutExtension()
    {
        string content = @"
newmtl a
refl -type cube_right b";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeRight);
        Assert.Equal("b", mtl.Materials[0].ReflectionMap.CubeRight?.FileName);
    }

    [Fact]
    public void Texture_ReflectionCubeRight_ValidWithExtensionIncludingWhitespace()
    {
        string content = @"
newmtl a
refl -type cube_right b.b 0";

        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].ReflectionMap.CubeRight);
        Assert.Equal("b.b 0", mtl.Materials[0].ReflectionMap.CubeRight?.FileName);
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
    [InlineData("on", true, "b.b", "b.b", false)]
    [InlineData("off", false, "b.b", "b.b", true)]
    [InlineData("on", true, "b   b", "b b", false)]
    [InlineData("off", false, "b   b", "b   b", true)]
    public void MapOptions_HorizontalBlending_Valid(string value, bool expected, string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -blenu " + value + " " + filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(expected, mtl.Materials[0].AmbientMap?.IsHorizontalBlendingEnabled);
    }

    [Theory]
    [InlineData("on", true, "b.b", "b.b", false)]
    [InlineData("off", false, "b.b", "b.b", true)]
    [InlineData("on", true, "b   b", "b b", false)]
    [InlineData("off", false, "b   b", "b   b", true)]
    public void MapOptions_VerticalBlending_Valid(string value, bool expected, string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -blenv " + value + " " + filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings { KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(expected, mtl.Materials[0].AmbientMap?.IsVerticalBlendingEnabled);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_BumpMultiplier_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -bm 2.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.BumpMultiplier);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_Boost_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -boost 2.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.Boost);
    }

    [Theory]
    [InlineData("on", true, "b.b", "b.b", false)]
    [InlineData("off", false, "b.b", "b.b", true)]
    [InlineData("on", true, "b   b", "b b", false)]
    [InlineData("off", false, "b   b", "b   b", true)]
    public void MapOptions_ColorCorrection_Valid(string value, bool expected, string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -cc " + value + @" "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(expected, mtl.Materials[0].AmbientMap?.IsColorCorrectionEnabled);
    }

    [Theory]
    [InlineData("on", true, "b.b", "b.b", false)]
    [InlineData("off", false, "b.b", "b.b", true)]
    [InlineData("on", true, "b   b", "b b", false)]
    [InlineData("off", false, "b   b", "b   b", true)]
    public void MapOptions_Clamping_Valid(string value, bool expected, string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -clamp " + value + @" "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(expected, mtl.Materials[0].AmbientMap?.IsClampingEnabled);
    }

    [Theory]
    [InlineData("r", ObjMapChannel.Red, "b.b", "b.b", false)]
    [InlineData("g", ObjMapChannel.Green, "b.b", "b.b", false)]
    [InlineData("b", ObjMapChannel.Blue, "b.b", "b.b", false)]
    [InlineData("m", ObjMapChannel.Matte, "b.b", "b.b", false)]
    [InlineData("l", ObjMapChannel.Luminance, "b.b", "b.b", false)]
    [InlineData("z", ObjMapChannel.Depth, "b.b", "b.b", false)]
    [InlineData("r", ObjMapChannel.Red, "b  b", "b b", false)]
    [InlineData("g", ObjMapChannel.Green, "b  b", "b b", false)]
    [InlineData("b", ObjMapChannel.Blue, "b  b", "b b", false)]
    [InlineData("m", ObjMapChannel.Matte, "b  b", "b b", false)]
    [InlineData("l", ObjMapChannel.Luminance, "b  b", "b b", false)]
    [InlineData("z", ObjMapChannel.Depth, "b  b", "b b", false)]
    [InlineData("r", ObjMapChannel.Red, "b  b", "b  b", true)]
    [InlineData("g", ObjMapChannel.Green, "b  b", "b  b", true)]
    [InlineData("b", ObjMapChannel.Blue, "b  b", "b  b", true)]
    [InlineData("m", ObjMapChannel.Matte, "b  b", "b  b", true)]
    [InlineData("l", ObjMapChannel.Luminance, "b  b", "b  b", true)]
    [InlineData("z", ObjMapChannel.Depth, "b  b", "b  b", true)]
    public void MapOptions_ScalarChannel_Valid(string value, ObjMapChannel expected, string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -imfchan " + value + " "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(expected, mtl.Materials[0].AmbientMap?.ScalarChannel);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_Modifier_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -mm 2.0 3.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.ModifierBase);
        Assert.Equal(3.0f, mtl.Materials[0].AmbientMap?.ModifierGain);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_Offset1_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -o 2.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.Offset.X);
        Assert.Equal(0.0f, mtl.Materials[0].AmbientMap?.Offset.Y);
        Assert.Equal(0.0f, mtl.Materials[0].AmbientMap?.Offset.Z);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_Offset2_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -o 2.0 3.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.Offset.X);
        Assert.Equal(3.0f, mtl.Materials[0].AmbientMap?.Offset.Y);
        Assert.Equal(0.0f, mtl.Materials[0].AmbientMap?.Offset.Z);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_Offset3_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -o 2.0 3.0 4.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.Offset.X);
        Assert.Equal(3.0f, mtl.Materials[0].AmbientMap?.Offset.Y);
        Assert.Equal(4.0f, mtl.Materials[0].AmbientMap?.Offset.Z);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_Scale1_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -s 2.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.Scale.X);
        Assert.Equal(1.0f, mtl.Materials[0].AmbientMap?.Scale.Y);
        Assert.Equal(1.0f, mtl.Materials[0].AmbientMap?.Scale.Z);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_Scale2_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -s 2.0 3.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.Scale.X);
        Assert.Equal(3.0f, mtl.Materials[0].AmbientMap?.Scale.Y);
        Assert.Equal(1.0f, mtl.Materials[0].AmbientMap?.Scale.Z);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_Scale3_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -s 2.0 3.0 4.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.Scale.X);
        Assert.Equal(3.0f, mtl.Materials[0].AmbientMap?.Scale.Y);
        Assert.Equal(4.0f, mtl.Materials[0].AmbientMap?.Scale.Z);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_Turbulence1_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -t 2.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.Turbulence.X);
        Assert.Equal(0.0f, mtl.Materials[0].AmbientMap?.Turbulence.Y);
        Assert.Equal(0.0f, mtl.Materials[0].AmbientMap?.Turbulence.Z);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_Turbulence2_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -t 2.0 3.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.Turbulence.X);
        Assert.Equal(3.0f, mtl.Materials[0].AmbientMap?.Turbulence.Y);
        Assert.Equal(0.0f, mtl.Materials[0].AmbientMap?.Turbulence.Z);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_Turbulence3_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -t 2.0 3.0 4.0 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2.0f, mtl.Materials[0].AmbientMap?.Turbulence.X);
        Assert.Equal(3.0f, mtl.Materials[0].AmbientMap?.Turbulence.Y);
        Assert.Equal(4.0f, mtl.Materials[0].AmbientMap?.Turbulence.Z);
    }

    [Theory]
    [InlineData("b.b", "b.b", false)]
    [InlineData("b.b", "b.b", true)]
    [InlineData("b   b", "b b", false)]
    [InlineData("b   b", "b   b", true)]
    public void MapOptions_TextureResolution_Valid(string filename, string expectedFilename, bool keepWhitespace)
    {
        string content = @"
newmtl a
map_Ka -texres 2 "+filename;

        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings {KeepWhitespacesOfMapFileReferences = keepWhitespace});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].AmbientMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].AmbientMap?.FileName);
        Assert.Equal(2, mtl.Materials[0].AmbientMap?.TextureResolution);
    }

    [Theory]
    [MemberData(nameof(SingleFloatInvalidTestData))]
    public void PbrExtensions_Roughness_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Pr");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(SingleFloatValidTestData))]
    public void PbrExtensions_Roughness_Valid(string materialStringTemplate, float expected)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Pr");
        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.Equal(expected, mtl.Materials[0].Roughness);
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapInvalidTestData))]
    public void PbrExtensions_RoughnessMap_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Pr");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapValidTestData))]
    public void PbrExtensions_RoughnessMapValid_Throws(string materialStringTemplate, string expectedFilename, bool keepWhitespaces)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Pr");
        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings { KeepWhitespacesOfMapFileReferences = keepWhitespaces});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].RoughnessMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].RoughnessMap?.FileName);
    }

    [Theory]
    [MemberData(nameof(SingleFloatInvalidTestData))]
    public void PbrExtensions_Metallic_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Pm");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(SingleFloatValidTestData))]
    public void PbrExtensions_Metallic_Valid(string materialStringTemplate, float expected)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Pm");
        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.Equal(expected, mtl.Materials[0].Metallic);
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapInvalidTestData))]
    public void PbrExtensions_MetallicMap_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Pm");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapValidTestData))]
    public void PbrExtensions_MetallicMapValid_Throws(string materialStringTemplate, string expectedFilename, bool keepWhitespaces)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Pm");
        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings { KeepWhitespacesOfMapFileReferences = keepWhitespaces});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].MetallicMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].MetallicMap?.FileName);
    }

    [Theory]
    [MemberData(nameof(SingleFloatInvalidTestData))]
    public void PbrExtensions_Sheen_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Ps");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(SingleFloatValidTestData))]
    public void PbrExtensions_Sheen_Valid(string materialStringTemplate, float expected)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Ps");
        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.Equal(expected, mtl.Materials[0].Sheen);
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapInvalidTestData))]
    public void PbrExtensions_SheenMap_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Ps");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapValidTestData))]
    public void PbrExtensions_SheenMapValid_Throws(string materialStringTemplate, string expectedFilename, bool keepWhitespaces)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "map_Ps");
        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings { KeepWhitespacesOfMapFileReferences = keepWhitespaces});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].SheenMap);
        Assert.Equal(expectedFilename, mtl.Materials[0].SheenMap?.FileName);
    }

    [Theory]
    [MemberData(nameof(SingleFloatInvalidTestData))]
    public void PbrExtensions_ClearCoatThickness_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Pc");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(SingleFloatValidTestData))]
    public void PbrExtensions_ClearCoatThickness_Valid(string materialStringTemplate, float expected)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Pc");
        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.Equal(expected, mtl.Materials[0].ClearCoatThickness);
    }

    [Theory]
    [MemberData(nameof(SingleFloatInvalidTestData))]
    public void PbrExtensions_ClearCoatRoughness_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Pcr");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(SingleFloatValidTestData))]
    public void PbrExtensions_ClearCoatRoughness_Valid(string materialStringTemplate, float expected)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "Pcr");
        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.Equal(expected, mtl.Materials[0].ClearCoatRoughness);
    }

    [Theory]
    [MemberData(nameof(SingleFloatInvalidTestData))]
    public void PbrExtensions_Anisotropy_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "aniso");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(SingleFloatValidTestData))]
    public void PbrExtensions_Anisotropy_Valid(string materialStringTemplate, float expected)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "aniso");
        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.Equal(expected, mtl.Materials[0].Anisotropy);
    }

    [Theory]
    [MemberData(nameof(SingleFloatInvalidTestData))]
    public void PbrExtensions_AnisotropyRotation_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "anisor");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(SingleFloatValidTestData))]
    public void PbrExtensions_AnisotropyRotation_Valid(string materialStringTemplate, float expected)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "anisor");
        var mtl = ReadMtl(content);

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.Equal(expected, mtl.Materials[0].AnisotropyRotation);
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapInvalidTestData))]
    public void PbrExtensions_Norm_Throws(string materialStringTemplate)
    {
        var materialString = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "norm");
        Assert.Throws<InvalidDataException>(() => ReadMtl(materialString));
    }

    [Theory]
    [MemberData(nameof(ObjectMaterialMapValidTestData))]
    public void PbrExtensions_NormValid_Throws(string materialStringTemplate, string expectedFilename, bool keepWhitespaces)
    {
        var content = string.Format(CultureInfo.InvariantCulture, materialStringTemplate, "norm");
        var mtl = ReadMtl(content, new ObjMaterialFileReaderSettings { KeepWhitespacesOfMapFileReferences = keepWhitespaces});

        Assert.Equal("a", mtl.Materials[0].Name);
        Assert.NotNull(mtl.Materials[0].Norm);
        Assert.Equal(expectedFilename, mtl.Materials[0].Norm?.FileName);
    }


    private ObjMaterialFile ReadMtl(string content, ObjMaterialFileReaderSettings? settings = null)
    {
        var buffer = Encoding.UTF8.GetBytes(content);

        using (var stream = new MemoryStream(buffer, false))
        {
            return ObjMaterialFile.FromStream(stream, settings ?? ObjMaterialFileReaderSettings.Default);
        }
    }

    public static IEnumerable<object[]> MaterialColorInvalidTestData()
    {
        yield return ["{0}"];
        yield return ["newmtl a\n{0}"];
        yield return ["newmtl a\n{0} 0 0"];
        yield return ["newmtl a\n{0} xyz"];
        yield return ["newmtl a\n{0} xyz 0 0"];
        yield return ["newmtl a\n{0} spectral"];
        yield return ["newmtl a\n{0} 0 0 0 0"];
    }

    public static IEnumerable<object[]> MaterialColorValidTestData()
    {
        yield return ["newmtl a\n{0} 2.0 3.0 4.0", new ObjMaterialColor { Color = new ObjVector3(2.0f, 3.0f, 4.0f) }];
        yield return ["newmtl a\n{0} 0.800000011920929 0.800000011920929 0.800000011920929", new ObjMaterialColor { Color = new ObjVector3(0.800000011920929f, 0.800000011920929f, 0.800000011920929f) }];
        yield return ["newmtl a\n{0} 2.0", new ObjMaterialColor { Color = new ObjVector3(2.0f, 2.0f, 2.0f) }];
        yield return ["newmtl a\n{0} xyz 2.0 3.0 4.0", new ObjMaterialColor { Color = new ObjVector3(2.0f, 3.0f, 4.0f), UseXYZColorSpace = true}];
        yield return ["newmtl a\n{0} xyz 2.0", new ObjMaterialColor { Color = new ObjVector3(2.0f, 2.0f, 2.0f), UseXYZColorSpace = true}];
        yield return ["newmtl a\n{0} spectral b.b 2.0", new ObjMaterialColor { SpectralFactor = 2.0f, SpectralFileName = "b.b"}];
        yield return ["newmtl a\n{0} spectral b 2.0", new ObjMaterialColor { SpectralFactor = 2.0f, SpectralFileName = "b"}];
        yield return ["newmtl a\n{0} spectral b", new ObjMaterialColor { SpectralFactor = 1.0f, SpectralFileName = "b"}];
    }

    public static IEnumerable<object[]> SingleFloatInvalidTestData()
    {
        yield return ["{0}"];
        yield return ["newmtl a\n{0}"];
        yield return ["newmtl a\n{0} 0 0"];
    }

    public static IEnumerable<object[]> SingleFloatValidTestData()
    {
        yield return ["newmtl a\n{0} 1.5", 1.5f];
    }


    public static IEnumerable<object[]> ObjectMaterialMapInvalidTestData()
    {
        yield return ["{0}"];
        yield return ["newmtl a\n{0}"];
    }

    public static IEnumerable<object[]> ObjectMaterialMapValidTestData()
    {
        yield return ["newmtl a\n{0} b", "b", false];
        yield return ["newmtl a\n{0} b.b", "b.b", false];
        yield return ["newmtl a\n{0} b.b 0", "b.b 0", false];
        yield return
            ["newmtl a\n{0} {{00000000-0000-0000-0000-000000000000}}", "{00000000-0000-0000-0000-000000000000}", false];
        yield return
        [
            "newmtl a\n{0} {{00000000-0000-0000-0000-000000000000}} {{00000000-0000-0000-0000-000000000000}}",
            "{00000000-0000-0000-0000-000000000000} {00000000-0000-0000-0000-000000000000}", 
            false
        ];
        yield return
        [
            "newmtl a\n{0} {{00000000-0000-0000-0000-000000000000}}   {{00000000-0000-0000-0000-000000000000}}",
            "{00000000-0000-0000-0000-000000000000} {00000000-0000-0000-0000-000000000000}", 
            false
        ];
        yield return
        [
            "newmtl a\n{0} {{00000000-0000-0000-0000-000000000000}}   {{00000000-0000-0000-0000-000000000000}}",
            "{00000000-0000-0000-0000-000000000000}   {00000000-0000-0000-0000-000000000000}", 
            true
        ];
        yield return ["newmtl a\n{0} b   b 0", "b b 0", false];
        yield return ["newmtl a\n{0} b   b 0", "b   b 0", true];
        yield return ["newmtl a\n{0} a  whitespace", "a whitespace", false];
        yield return ["newmtl a\n{0} a  whitespace", "a  whitespace", true];
    }
}