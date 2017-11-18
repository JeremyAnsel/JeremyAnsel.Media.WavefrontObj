// <copyright file="ObjMaterialFileReader.cs" company="Jérémy Ansel">
// Copyright (c) 2017 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace JeremyAnsel.Media.WavefrontObj
{
    internal static class ObjMaterialFileReader
    {
        public static ObjMaterialFile FromStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var mtl = new ObjMaterialFile();

            ObjMaterial currentMaterial = null;

            foreach (var values in LineReader.Read(stream))
            {
                switch (values[0].ToLowerInvariant())
                {
                    case "newmtl":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A newmtl statement must specify a name.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A newmtl statement has too many values.");
                        }

                        currentMaterial = new ObjMaterial
                        {
                            Name = values[1]
                        };

                        mtl.Materials.Add(currentMaterial);

                        break;

                    case "ka":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.AmbientColor = ObjMaterialFileReader.ParseMaterialColor("Ka", values);
                        break;

                    case "kd":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DiffuseColor = ObjMaterialFileReader.ParseMaterialColor("Kd", values);
                        break;

                    case "ke":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.EmissiveColor = ObjMaterialFileReader.ParseMaterialColor("Ke", values);
                        break;

                    case "ks":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.SpecularColor = ObjMaterialFileReader.ParseMaterialColor("Ks", values);
                        break;

                    case "tf":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.TransmissionColor = ObjMaterialFileReader.ParseMaterialColor("Tf", values);
                        break;

                    case "illum":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("An illum statement must specify an illumination model.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("An illum statement has too many values.");
                        }

                        currentMaterial.IlluminationModel = int.Parse(values[1], CultureInfo.InvariantCulture);
                        break;

                    case "d":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A d statement must specify a factor.");
                        }

                        if (string.Equals(values[1], "-halo", StringComparison.OrdinalIgnoreCase))
                        {
                            if (values.Length < 3)
                            {
                                throw new InvalidDataException("A d statement must specify a factor.");
                            }

                            if (values.Length != 3)
                            {
                                throw new InvalidDataException("A d statement has too many values.");
                            }

                            currentMaterial.IsHaloDissolve = true;
                            currentMaterial.DissolveFactor = float.Parse(values[2], CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            if (values.Length != 2)
                            {
                                throw new InvalidDataException("A d statement has too many values.");
                            }

                            currentMaterial.DissolveFactor = float.Parse(values[1], CultureInfo.InvariantCulture);
                        }

                        break;

                    case "ns":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A Ns statement must specify a specular exponent.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A Ns statement has too many values.");
                        }

                        currentMaterial.SpecularExponent = float.Parse(values[1], CultureInfo.InvariantCulture);
                        break;

                    case "sharpness":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A sharpness statement must specify a sharpness value.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A sharpness statement has too many values.");
                        }

                        currentMaterial.Sharpness = int.Parse(values[1], CultureInfo.InvariantCulture);
                        break;

                    case "ni":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A Ni statement must specify an optical density.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A Ni statement has too many values.");
                        }

                        currentMaterial.OpticalDensity = float.Parse(values[1], CultureInfo.InvariantCulture);
                        break;

                    case "map_aat":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A map_aat statement must specify a value.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A map_aat statement has too many values.");
                        }

                        if (string.Equals(values[1], "on", StringComparison.OrdinalIgnoreCase))
                        {
                            currentMaterial.IsAntiAliasingEnabled = true;
                        }
                        else if (string.Equals(values[1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            currentMaterial.IsAntiAliasingEnabled = false;
                        }
                        else
                        {
                            throw new InvalidDataException("A map_aat statement must specify on or off.");
                        }

                        break;

                    case "map_ka":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.AmbientMap = ObjMaterialFileReader.ParseMaterialMap("map_Ka", values);
                        break;

                    case "map_kd":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DiffuseMap = ObjMaterialFileReader.ParseMaterialMap("map_Kd", values);
                        break;

                    case "map_ke":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.EmissiveMap = ObjMaterialFileReader.ParseMaterialMap("map_Ke", values);
                        break;

                    case "map_ks":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.SpecularMap = ObjMaterialFileReader.ParseMaterialMap("map_Ks", values);
                        break;

                    case "map_ns":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.SpecularExponentMap = ObjMaterialFileReader.ParseMaterialMap("map_Ns", values);
                        break;

                    case "map_d":
                    case "map_tr":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DissolveMap = ObjMaterialFileReader.ParseMaterialMap("map_d", values);
                        break;

                    case "decal":
                    case "map_decal":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DecalMap = ObjMaterialFileReader.ParseMaterialMap("decal", values);
                        break;

                    case "disp":
                    case "map_disp":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DispMap = ObjMaterialFileReader.ParseMaterialMap("disp", values);
                        break;

                    case "bump":
                    case "map_bump":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.BumpMap = ObjMaterialFileReader.ParseMaterialMap("bump", values);
                        break;

                    case "refl":
                    case "map_refl":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 4)
                        {
                            throw new InvalidDataException("A refl statement must specify a type and a file name.");
                        }

                        if (!string.Equals(values[1], "-type", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new InvalidDataException("A refl statement must specify a type.");
                        }

                        switch (values[2].ToLowerInvariant())
                        {
                            case "sphere":
                                currentMaterial.ReflectionMap.Sphere = ObjMaterialFileReader.ParseMaterialMap("refl", values);
                                break;

                            case "cube_top":
                                currentMaterial.ReflectionMap.CubeTop = ObjMaterialFileReader.ParseMaterialMap("refl", values);
                                break;

                            case "cube_bottom":
                                currentMaterial.ReflectionMap.CubeBottom = ObjMaterialFileReader.ParseMaterialMap("refl", values);
                                break;

                            case "cube_front":
                                currentMaterial.ReflectionMap.CubeFront = ObjMaterialFileReader.ParseMaterialMap("refl", values);
                                break;

                            case "cube_back":
                                currentMaterial.ReflectionMap.CubeBack = ObjMaterialFileReader.ParseMaterialMap("refl", values);
                                break;

                            case "cube_left":
                                currentMaterial.ReflectionMap.CubeLeft = ObjMaterialFileReader.ParseMaterialMap("refl", values);
                                break;

                            case "cube_right":
                                currentMaterial.ReflectionMap.CubeRight = ObjMaterialFileReader.ParseMaterialMap("refl", values);
                                break;
                        }

                        break;
                }
            }

            return mtl;
        }

        private static ObjMaterialColor ParseMaterialColor(string statement, string[] values)
        {
            if (values.Length < 2)
            {
                throw new InvalidDataException(string.Concat("A ", statement, " statement must specify a color."));
            }

            var color = new ObjMaterialColor();

            int index = 1;

            switch (values[1].ToLowerInvariant())
            {
                case "spectral":
                    index++;

                    if (values.Length - index < 1)
                    {
                        throw new InvalidDataException(string.Concat("A ", statement, " spectral statement must specify a file name."));
                    }

                    if (!Path.HasExtension(values[index]))
                    {
                        throw new InvalidDataException("A filename must have an extension.");
                    }

                    color.SpectralFileName = values[index];
                    index++;

                    if (values.Length > index)
                    {
                        color.SpectralFactor = float.Parse(values[index], CultureInfo.InvariantCulture);
                        index++;
                    }

                    break;

                case "xyz":
                    index++;

                    if (values.Length - index < 1)
                    {
                        throw new InvalidDataException(string.Concat("A ", statement, " xyz statement must specify a color."));
                    }

                    color.UseXYZColorSpace = true;

                    var xyz = new ObjVector3();

                    xyz.X = float.Parse(values[index], CultureInfo.InvariantCulture);
                    index++;

                    if (values.Length > index)
                    {
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " xyz statement must specify a XYZ color."));
                        }

                        xyz.Y = float.Parse(values[index], CultureInfo.InvariantCulture);
                        xyz.Z = float.Parse(values[index + 1], CultureInfo.InvariantCulture);
                        index += 2;
                    }
                    else
                    {
                        xyz.Y = xyz.X;
                        xyz.Z = xyz.X;
                    }

                    color.Color = xyz;
                    break;

                default:
                    var rgb = new ObjVector3();

                    rgb.X = float.Parse(values[index], CultureInfo.InvariantCulture);
                    index++;

                    if (values.Length > index)
                    {
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " statement must specify a RGB color."));
                        }

                        rgb.Y = float.Parse(values[index], CultureInfo.InvariantCulture);
                        rgb.Z = float.Parse(values[index + 1], CultureInfo.InvariantCulture);
                        index += 2;
                    }
                    else
                    {
                        rgb.Y = rgb.X;
                        rgb.Z = rgb.X;
                    }

                    color.Color = rgb;
                    break;
            }

            if (index != values.Length)
            {
                throw new InvalidDataException(string.Concat("A ", statement, " statement has too many values."));
            }

            return color;
        }

        private static ObjMaterialMap ParseMaterialMap(string statement, string[] values)
        {
            var map = new ObjMaterialMap();

            for (int index = 0; index < values.Length;)
            {
                index++;

                if (values.Length - index < 1)
                {
                    throw new InvalidDataException(string.Concat("A ", statement, " statement must specify a filename."));
                }

                switch (values[index].ToLowerInvariant())
                {
                    case "-type":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -type option must specify a value."));
                        }

                        index++;
                        break;

                    case "-blenu":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -blenu option must specify a value."));
                        }

                        if (string.Equals(values[index + 1], "on", StringComparison.OrdinalIgnoreCase))
                        {
                            map.IsHorizontalBlendingEnabled = true;
                        }
                        else if (string.Equals(values[index + 1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            map.IsHorizontalBlendingEnabled = false;
                        }
                        else
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -blenu option must specify on or off."));
                        }

                        index++;
                        break;

                    case "-blenv":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -blenv option must specify a value."));
                        }

                        if (string.Equals(values[index + 1], "on", StringComparison.OrdinalIgnoreCase))
                        {
                            map.IsVerticalBlendingEnabled = true;
                        }
                        else if (string.Equals(values[index + 1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            map.IsVerticalBlendingEnabled = false;
                        }
                        else
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -blenv option must specify on or off."));
                        }

                        index++;
                        break;

                    case "-bm":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -bm option must specify a value."));
                        }

                        map.BumpMultiplier = float.Parse(values[index + 1], CultureInfo.InvariantCulture);

                        index++;
                        break;

                    case "-boost":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -boost option must specify a value."));
                        }

                        map.Boost = float.Parse(values[index + 1], CultureInfo.InvariantCulture);

                        index++;
                        break;

                    case "-cc":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -cc option must specify a value."));
                        }

                        if (string.Equals(values[index + 1], "on", StringComparison.OrdinalIgnoreCase))
                        {
                            map.IsColorCorrectionEnabled = true;
                        }
                        else if (string.Equals(values[index + 1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            map.IsColorCorrectionEnabled = false;
                        }
                        else
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -cc option must specify on or off."));
                        }

                        index++;
                        break;

                    case "-clamp":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -clamp option must specify a value."));
                        }

                        if (string.Equals(values[index + 1], "on", StringComparison.OrdinalIgnoreCase))
                        {
                            map.IsClampingEnabled = true;
                        }
                        else if (string.Equals(values[index + 1], "off", StringComparison.OrdinalIgnoreCase))
                        {
                            map.IsClampingEnabled = false;
                        }
                        else
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -clamp option must specify on or off."));
                        }

                        index++;
                        break;

                    case "-imfchan":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -imfchan option must specify a value."));
                        }

                        switch (values[index + 1].ToLowerInvariant())
                        {
                            case "r":
                                map.ScalarChannel = ObjMapChannel.Red;
                                break;

                            case "g":
                                map.ScalarChannel = ObjMapChannel.Green;
                                break;

                            case "b":
                                map.ScalarChannel = ObjMapChannel.Blue;
                                break;

                            case "m":
                                map.ScalarChannel = ObjMapChannel.Matte;
                                break;

                            case "l":
                                map.ScalarChannel = ObjMapChannel.Luminance;
                                break;

                            case "z":
                                map.ScalarChannel = ObjMapChannel.Depth;
                                break;

                            default:
                                throw new InvalidDataException(string.Concat("A ", statement, " -imfchan option must specify a value in (r, g, b, m, l, z)."));
                        }

                        index++;
                        break;

                    case "-mm":
                        if (values.Length - index < 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -mm option must specify a base and a gain."));
                        }

                        map.ModifierBase = float.Parse(values[index + 1], CultureInfo.InvariantCulture);
                        map.ModifierGain = float.Parse(values[index + 2], CultureInfo.InvariantCulture);

                        index += 2;
                        break;

                    case "-o":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -o option must specify at least 2 values."));
                        }

                        var offset = new ObjVector3();

                        offset.X = float.Parse(values[index + 1], CultureInfo.InvariantCulture);

                        if (values.Length - index > 3)
                        {
                            offset.Y = float.Parse(values[index + 2], CultureInfo.InvariantCulture);

                            if (values.Length - index > 4)
                            {
                                offset.Z = float.Parse(values[index + 3], CultureInfo.InvariantCulture);
                                index++;
                            }

                            index++;
                        }

                        index++;

                        map.Offset = offset;
                        break;

                    case "-s":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -s option must specify at least 2 values."));
                        }

                        var scale = new ObjVector3(1.0f, 1.0f, 1.0f);

                        scale.X = float.Parse(values[index + 1], CultureInfo.InvariantCulture);

                        if (values.Length - index > 3)
                        {
                            scale.Y = float.Parse(values[index + 2], CultureInfo.InvariantCulture);

                            if (values.Length - index > 4)
                            {
                                scale.Z = float.Parse(values[index + 3], CultureInfo.InvariantCulture);
                                index++;
                            }

                            index++;
                        }

                        index++;

                        map.Scale = scale;
                        break;

                    case "-t":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -t option must specify at least 2 values."));
                        }

                        var turbulence = new ObjVector3();

                        turbulence.X = float.Parse(values[index + 1], CultureInfo.InvariantCulture);

                        if (values.Length - index > 3)
                        {
                            turbulence.Y = float.Parse(values[index + 2], CultureInfo.InvariantCulture);

                            if (values.Length - index > 4)
                            {
                                turbulence.Z = float.Parse(values[index + 3], CultureInfo.InvariantCulture);
                                index++;
                            }

                            index++;
                        }

                        index++;

                        map.Turbulence = turbulence;
                        break;

                    case "-texres":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -texres option must specify a value."));
                        }

                        map.TextureResolution = int.Parse(values[index + 1], CultureInfo.InvariantCulture);

                        index++;
                        break;

                    default:
                        if (!Path.HasExtension(values[index]))
                        {
                            throw new InvalidDataException("A filename must have an extension.");
                        }

                        map.FileName = values[index];
                        index++;

                        if (index != values.Length)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " has too many values."));
                        }

                        break;
                }
            }

            return map;
        }
    }
}
