// <copyright file="ObjMaterialFileReader.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

#if !NET6_0_OR_GREATER

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace JeremyAnsel.Media.WavefrontObj
{
    internal static class ObjMaterialFileReader
    {
        [SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "Reviewed.")]
        public static ObjMaterialFile FromStream(Stream? stream, ObjMaterialFileReaderSettings settings)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var mtl = new ObjMaterialFile();
            var lineReader = new LineReader();

            ObjMaterial? currentMaterial = null;

            foreach (string currentLine in lineReader.Read(stream))
            {
                string[] values = currentLine.Split(LineReader.LineSeparators, StringSplitOptions.RemoveEmptyEntries);

                switch (values[0].ToLowerInvariant())
                {
                    case "newmtl":
                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A newmtl statement must specify a name.");
                        }

                        currentMaterial = new ObjMaterial
                        {
                            Name = string.Join(" ", values, 1, values.Length - 1)
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

                        currentMaterial.AmbientMap = ObjMaterialFileReader.ParseMaterialMap("map_Ka", values, currentLine, settings);
                        break;

                    case "map_kd":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DiffuseMap = ObjMaterialFileReader.ParseMaterialMap("map_Kd", values, currentLine, settings);
                        break;

                    case "map_ke":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.EmissiveMap = ObjMaterialFileReader.ParseMaterialMap("map_Ke", values, currentLine, settings);
                        break;

                    case "map_ks":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.SpecularMap = ObjMaterialFileReader.ParseMaterialMap("map_Ks", values, currentLine, settings);
                        break;

                    case "map_ns":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.SpecularExponentMap = ObjMaterialFileReader.ParseMaterialMap("map_Ns", values, currentLine, settings);
                        break;

                    case "map_d":
                    case "map_tr":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DissolveMap = ObjMaterialFileReader.ParseMaterialMap("map_d", values, currentLine, settings);
                        break;

                    case "decal":
                    case "map_decal":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DecalMap = ObjMaterialFileReader.ParseMaterialMap("decal", values, currentLine, settings);
                        break;

                    case "disp":
                    case "map_disp":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DispMap = ObjMaterialFileReader.ParseMaterialMap("disp", values, currentLine, settings);
                        break;

                    case "bump":
                    case "map_bump":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.BumpMap = ObjMaterialFileReader.ParseMaterialMap("bump", values, currentLine, settings);
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
                                currentMaterial.ReflectionMap.Sphere = ObjMaterialFileReader.ParseMaterialMap("refl", values, currentLine, settings);
                                break;

                            case "cube_top":
                                currentMaterial.ReflectionMap.CubeTop = ObjMaterialFileReader.ParseMaterialMap("refl", values, currentLine, settings);
                                break;

                            case "cube_bottom":
                                currentMaterial.ReflectionMap.CubeBottom = ObjMaterialFileReader.ParseMaterialMap("refl", values, currentLine, settings);
                                break;

                            case "cube_front":
                                currentMaterial.ReflectionMap.CubeFront = ObjMaterialFileReader.ParseMaterialMap("refl", values, currentLine, settings);
                                break;

                            case "cube_back":
                                currentMaterial.ReflectionMap.CubeBack = ObjMaterialFileReader.ParseMaterialMap("refl", values, currentLine, settings);
                                break;

                            case "cube_left":
                                currentMaterial.ReflectionMap.CubeLeft = ObjMaterialFileReader.ParseMaterialMap("refl", values, currentLine, settings);
                                break;

                            case "cube_right":
                                currentMaterial.ReflectionMap.CubeRight = ObjMaterialFileReader.ParseMaterialMap("refl", values, currentLine, settings);
                                break;
                        }

                        break;
                    case "pr":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A Pr statement must specify an optical density.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A Pr statement has too many values.");
                        }

                        currentMaterial.Roughness = float.Parse(values[1], CultureInfo.InvariantCulture);
                        break;
                    case "map_pr":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.RoughnessMap = ObjMaterialFileReader.ParseMaterialMap("map_Pr", values, currentLine, settings);
                        break;
                    case "pm":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A Pm statement must specify an optical density.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A Pm statement has too many values.");
                        }

                        currentMaterial.Metallic = float.Parse(values[1], CultureInfo.InvariantCulture);
                        break;
                    case "map_pm":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.MetallicMap = ObjMaterialFileReader.ParseMaterialMap("map_Pm", values, currentLine, settings);
                        break;
                    case "ps":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A Ps statement must specify an optical density.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A Ps statement has too many values.");
                        }

                        currentMaterial.Sheen = float.Parse(values[1], CultureInfo.InvariantCulture);
                        break;
                    case "map_ps":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.SheenMap = ObjMaterialFileReader.ParseMaterialMap("map_Ps", values, currentLine, settings);
                        break;
                    case "pc":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A Pc statement must specify an optical density.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A Pc statement has too many values.");
                        }

                        currentMaterial.ClearCoatThickness = float.Parse(values[1], CultureInfo.InvariantCulture);
                        break;
                    case "pcr":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A Pcr statement must specify an optical density.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A Pcr statement has too many values.");
                        }

                        currentMaterial.ClearCoatRoughness = float.Parse(values[1], CultureInfo.InvariantCulture);
                        break;
                    case "aniso":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A aniso statement must specify an optical density.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A aniso statement has too many values.");
                        }

                        currentMaterial.Anisotropy = float.Parse(values[1], CultureInfo.InvariantCulture);
                        break;
                    case "anisor":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (values.Length < 2)
                        {
                            throw new InvalidDataException("A anisor statement must specify an optical density.");
                        }

                        if (values.Length != 2)
                        {
                            throw new InvalidDataException("A anisor statement has too many values.");
                        }

                        currentMaterial.AnisotropyRotation = float.Parse(values[1], CultureInfo.InvariantCulture);
                        break;
                    case "norm":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.Norm = ObjMaterialFileReader.ParseMaterialMap("norm", values, currentLine, settings);
                        break;
                }
            }

            mtl.HeaderText = string.Join("\n", lineReader.HeaderTextLines.ToArray());

            return mtl;
        }

        [SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "Reviewed.")]
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

                    color.SpectralFileName = values[index];
                    index++;

                    if (values.Length > index)
                    {
                        color.SpectralFactor = float.Parse(values[index], CultureInfo.InvariantCulture);
                        index++;
                    }

                    break;

                case "xyz":
                {
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
                }

                default:
                {
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
            }

            if (index != values.Length)
            {
                throw new InvalidDataException(string.Concat("A ", statement, " statement has too many values."));
            }

            return color;
        }

        [SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "Reviewed.")]
        private static ObjMaterialMap ParseMaterialMap(string statement, string[] values, string currentLine, ObjMaterialFileReaderSettings settings)
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
                        index++;

                        if (values.Length - index > 2)
                        {
                            if (float.TryParse(values[index + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                            {
                                offset.Y = v;
                                index++;
                            }
                            else
                            {
                                map.Offset = offset;
                                break;
                            }

                            if (values.Length - index > 2)
                            {
                                if (float.TryParse(values[index + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                                {
                                    offset.Z = v;
                                    index++;
                                }
                            }
                        }

                        map.Offset = offset;
                        break;

                    case "-s":
                    {
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -s option must specify at least 2 values."));
                        }

                        var scale = new ObjVector3(1.0f, 1.0f, 1.0f);

                        scale.X = float.Parse(values[index + 1], CultureInfo.InvariantCulture);
                        index++;

                        if (values.Length - index > 2)
                        {
                            if (float.TryParse(values[index + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                            {
                                scale.Y = v;
                                index++;
                            }
                            else
                            {
                                map.Scale = scale;
                                break;
                            }

                            if (values.Length - index > 2)
                            {
                                if (float.TryParse(values[index + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                                {
                                    scale.Z = v;
                                    index++;
                                }
                            }
                        }

                        map.Scale = scale;
                        break;
                    }

                    case "-t":
                    {
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -t option must specify at least 2 values."));
                        }

                        var turbulence = new ObjVector3();

                        turbulence.X = float.Parse(values[index + 1], CultureInfo.InvariantCulture);
                        index++;

                        if (values.Length - index > 2)
                        {
                            if (float.TryParse(values[index + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                            {
                                turbulence.Y = v;
                                index++;
                            }
                            else
                            {
                                map.Turbulence = turbulence;
                                break;
                            }

                            if (values.Length - index > 2)
                            {
                                if (float.TryParse(values[index + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                                {
                                    turbulence.Z = v;
                                    index++;
                                }
                                else
                                {
                                    map.Turbulence = turbulence;
                                    break;
                                }
                            }
                        }

                        map.Turbulence = turbulence;
                        break;
                    }

                    case "-texres":
                        if (values.Length - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -texres option must specify a value."));
                        }

                        map.TextureResolution = int.Parse(values[index + 1], CultureInfo.InvariantCulture);

                        index++;
                        break;

                    default:
                    {
                        if (settings.KeepWhitespacesOfMapFileReferences)
                        {
                            var charsRead = 0;
                            for (var i = 1; i < index; i++)
                            {
                                charsRead += values[i].Length;
                            }

                            map.FileName = currentLine.Remove(0, statement.Length + charsRead + index);
                        }
                        else
                        {
                            string filename = string.Join(" ", values, index, values.Length - index);

                            map.FileName = filename;
                        }

                        index = values.Length;

                        break;
                    }
                }
            }

            return map;
        }
    }
}

#endif