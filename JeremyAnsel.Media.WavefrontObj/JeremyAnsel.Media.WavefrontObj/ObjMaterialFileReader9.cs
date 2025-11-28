// <copyright file="ObjMaterialFileReader9.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

#if NET6_0_OR_GREATER

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

#if NET9_0_OR_GREATER
using SpanSplitEnumerator = System.MemoryExtensions.SpanSplitEnumerator<char>;
#else
using SpanSplitEnumerator = Polyfills.Polyfill.SpanSplitEnumerator<char>;
#endif

namespace JeremyAnsel.Media.WavefrontObj
{
    internal static class ObjMaterialFileReader
    {
        private static void MoveNextSkipEmpty(ref SpanSplitEnumerator values)
        {
            while (values.MoveNext())
            {
                if (values.Current.Start.Value != values.Current.End.Value)
                {
                    return;
                }
            }
        }

        private static ReadOnlySpan<char> GetNextValue(ref ReadOnlySpan<char> currentLine, ref SpanSplitEnumerator values)
        {
            MoveNextSkipEmpty(ref values);
            return currentLine[values.Current];
        }

        private static bool TryFloatParse(ReadOnlySpan<char> s, out float value)
        {
            return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryIntParse(ReadOnlySpan<char> s, out int value)
        {
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        [SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "Reviewed.")]
        public static ObjMaterialFile FromStream(Stream? stream, ObjMaterialFileReaderSettings settings)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var mtl = new ObjMaterialFile();
            var lineReader = new LineReader9();

            ObjMaterial? currentMaterial = null;

            int valueBufferSize = 16;
            Span<char> valueBuffer = stackalloc char[valueBufferSize];

            foreach (var currentLineString in lineReader.Read9(stream))
            {
                ReadOnlySpan<char> currentLine = currentLineString.Buffer.AsSpan()[..currentLineString.Length];

                int valuesCount = 0;

                foreach (Range range in currentLine.SplitAny(LineReader9.LineSeparators))
                {
                    if (currentLine[range].Length == 0)
                    {
                        continue;
                    }

                    valuesCount++;
                }

                SpanSplitEnumerator values = currentLine.SplitAny(LineReader9.LineSeparators);

                MoveNextSkipEmpty(ref values);

                if (values.Current.End.Value - values.Current.Start.Value > valueBufferSize)
                {
                    continue;
                }

                ReadOnlySpan<char> value0 = currentLine[values.Current];
                int value0Length = value0.ToLowerInvariant(valueBuffer);

                //if (value0Length == -1)
                //{
                //    throw new InvalidDataException("the buffer is too small");
                //}

                switch (valueBuffer[..value0Length])
                {
                    case "newmtl":
                        {
                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A newmtl statement must specify a name.");
                            }

                            var sb = new StringBuilder();

                            sb.Append(GetNextValue(ref currentLine, ref values));

                            for (int i = 2; i < valuesCount; i++)
                            {
                                sb.Append(' ');
                                sb.Append(GetNextValue(ref currentLine, ref values));
                            }

                            currentMaterial = new ObjMaterial
                            {
                                Name = sb.ToString()
                            };

                            mtl.Materials.Add(currentMaterial);

                            break;
                        }

                    case "ka":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.AmbientColor = ParseMaterialColor("Ka", ref currentLine, ref values, valuesCount);
                        break;

                    case "kd":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DiffuseColor = ParseMaterialColor("Kd", ref currentLine, ref values, valuesCount);
                        break;

                    case "ke":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.EmissiveColor = ParseMaterialColor("Ke", ref currentLine, ref values, valuesCount);
                        break;

                    case "ks":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.SpecularColor = ParseMaterialColor("Ks", ref currentLine, ref values, valuesCount);
                        break;

                    case "tf":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.TransmissionColor = ParseMaterialColor("Tf", ref currentLine, ref values, valuesCount);
                        break;

                    case "illum":
                    {
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("An illum statement must specify an illumination model.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("An illum statement has too many values.");
                        }

                        if (TryIntParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            currentMaterial.IlluminationModel = value;
                        }

                        break;
                    }
                    case "d":
                        {
                            if (currentMaterial == null)
                            {
                                throw new InvalidDataException("The material name is not specified.");
                            }

                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A d statement must specify a factor.");
                            }

                            var value1 = GetNextValue(ref currentLine, ref values);

                            if (value1.Equals("-halo", StringComparison.OrdinalIgnoreCase))
                            {
                                if (valuesCount < 3)
                                {
                                    throw new InvalidDataException("A d statement must specify a factor.");
                                }

                                if (valuesCount != 3)
                                {
                                    throw new InvalidDataException("A d statement has too many values.");
                                }

                                currentMaterial.IsHaloDissolve = true;
                                if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                                {
                                    currentMaterial.DissolveFactor = value;
                                }
                            }
                            else
                            {
                                if (valuesCount != 2)
                                {
                                    throw new InvalidDataException("A d statement has too many values.");
                                }

                                if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                                {
                                    currentMaterial.DissolveFactor = value;
                                }
                            }

                            break;
                        }

                    case "ns":
                    {
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A Ns statement must specify a specular exponent.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A Ns statement has too many values.");
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            currentMaterial.SpecularExponent = value;
                        }
                        break;
                    }
                    case "sharpness":
                    {
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A sharpness statement must specify a sharpness value.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A sharpness statement has too many values.");
                        }
                        if (TryIntParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            currentMaterial.Sharpness = value;
                        }
                        break;
                    }
                    case "ni":
                    {
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A Ni statement must specify an optical density.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A Ni statement has too many values.");
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            currentMaterial.OpticalDensity = value;
                        }
                        
                        break;
                    }
                    case "map_aat":
                        {
                            if (currentMaterial == null)
                            {
                                throw new InvalidDataException("The material name is not specified.");
                            }

                            if (valuesCount < 2)
                            {
                                throw new InvalidDataException("A map_aat statement must specify a value.");
                            }

                            if (valuesCount != 2)
                            {
                                throw new InvalidDataException("A map_aat statement has too many values.");
                            }

                            var value1 = GetNextValue(ref currentLine, ref values);

                            if (value1.Equals("on", StringComparison.OrdinalIgnoreCase))
                            {
                                currentMaterial.IsAntiAliasingEnabled = true;
                            }
                            else if (value1.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                currentMaterial.IsAntiAliasingEnabled = false;
                            }
                            else
                            {
                                throw new InvalidDataException("A map_aat statement must specify on or off.");
                            }

                            break;
                        }

                    case "map_ka":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.AmbientMap = ParseMaterialMap("map_Ka", ref currentLine, ref values, valuesCount, settings);
                        break;

                    case "map_kd":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DiffuseMap = ParseMaterialMap("map_Kd", ref currentLine, ref values, valuesCount, settings);
                        break;

                    case "map_ke":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.EmissiveMap = ParseMaterialMap("map_Ke", ref currentLine, ref values, valuesCount, settings);
                        break;

                    case "map_ks":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.SpecularMap = ParseMaterialMap("map_Ks", ref currentLine, ref values, valuesCount, settings);
                        break;

                    case "map_ns":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.SpecularExponentMap = ParseMaterialMap("map_Ns", ref currentLine, ref values, valuesCount, settings);
                        break;

                    case "map_d":
                    case "map_tr":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DissolveMap = ParseMaterialMap("map_d", ref currentLine, ref values, valuesCount, settings);
                        break;

                    case "decal":
                    case "map_decal":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DecalMap = ParseMaterialMap("decal", ref currentLine, ref values, valuesCount, settings);
                        break;

                    case "disp":
                    case "map_disp":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.DispMap = ParseMaterialMap("disp", ref currentLine, ref values, valuesCount, settings);
                        break;

                    case "bump":
                    case "map_bump":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.BumpMap = ParseMaterialMap("bump", ref currentLine, ref values, valuesCount, settings);
                        break;

                    case "refl":
                    case "map_refl":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 4)
                        {
                            throw new InvalidDataException("A refl statement must specify a type and a file name.");
                        }

                        ObjMaterialMap materialMap = ParseMaterialMap("refl", ref currentLine, ref values, valuesCount, settings, out MaterialMapType materialMapType);

                        switch (materialMapType)
                        {
                            case MaterialMapType.Sphere:
                                currentMaterial.ReflectionMap.Sphere = materialMap;
                                break;

                            case MaterialMapType.CubeTop:
                                currentMaterial.ReflectionMap.CubeTop = materialMap;
                                break;

                            case MaterialMapType.CubeBottom:
                                currentMaterial.ReflectionMap.CubeBottom = materialMap;
                                break;

                            case MaterialMapType.CubeFront:
                                currentMaterial.ReflectionMap.CubeFront = materialMap;
                                break;

                            case MaterialMapType.CubeBack:
                                currentMaterial.ReflectionMap.CubeBack = materialMap;
                                break;

                            case MaterialMapType.CubeLeft:
                                currentMaterial.ReflectionMap.CubeLeft = materialMap;
                                break;

                            case MaterialMapType.CubeRight:
                                currentMaterial.ReflectionMap.CubeRight = materialMap;
                                break;
                        }

                        break;
                    case "pr":
                    {
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A Pr statement must specify an optical density.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A Pr statement has too many values.");
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            currentMaterial.Roughness = value;
                        }

                        break;
                    }
                    case "map_pr":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.RoughnessMap = ParseMaterialMap("map_Pr", ref currentLine, ref values, valuesCount, settings);
                        break;
                    case "pm":
                    {
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A Pm statement must specify an optical density.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A Pm statement has too many values.");
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            currentMaterial.Metallic = value;
                        }
                        break;
                    }
                    case "map_pm":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.MetallicMap = ParseMaterialMap("map_Pm", ref currentLine, ref values, valuesCount, settings);
                        break;
                    case "ps":
                    {
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A Ps statement must specify an optical density.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A Ps statement has too many values.");
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            currentMaterial.Sheen = value;
                        }
                        break;
                    }
                    case "map_ps":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.SheenMap = ParseMaterialMap("map_Ps", ref currentLine, ref values, valuesCount, settings);
                        break;
                    case "pc":
                    {
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A Pc statement must specify an optical density.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A Pc statement has too many values.");
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            currentMaterial.ClearCoatThickness = value;
                        }
                        break;
                    }
                    case "pcr":
                    {
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A Pcr statement must specify an optical density.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A Pcr statement has too many values.");
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            currentMaterial.ClearCoatRoughness = value;
                        }
                        break;
                    }
                    case "aniso":
                    {
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A aniso statement must specify an optical density.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A aniso statement has too many values.");
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            currentMaterial.Anisotropy = value;
                        }

                        break;
                    }
                    case "anisor":
                    {
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        if (valuesCount < 2)
                        {
                            throw new InvalidDataException("A anisor statement must specify an optical density.");
                        }

                        if (valuesCount != 2)
                        {
                            throw new InvalidDataException("A anisor statement has too many values.");
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            currentMaterial.AnisotropyRotation = value;
                        }
                        break;
                    }
                    case "norm":
                        if (currentMaterial == null)
                        {
                            throw new InvalidDataException("The material name is not specified.");
                        }

                        currentMaterial.Norm = ParseMaterialMap("norm", ref currentLine, ref values, valuesCount, settings);
                        break;
                }
            }

            mtl.HeaderText = string.Join("\n", lineReader.HeaderTextLines.ToArray());

            return mtl;
        }

        [SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "Reviewed.")]
        private static ObjMaterialColor ParseMaterialColor(ReadOnlySpan<char> statement, ref ReadOnlySpan<char> currentLine, ref SpanSplitEnumerator values, int valuesCount)
        {
            if (valuesCount < 2)
            {
                throw new InvalidDataException(string.Concat("A ", statement, " statement must specify a color."));
            }

            var color = new ObjMaterialColor();

            ReadOnlySpan<char> value1 = GetNextValue(ref currentLine, ref values);
            Span<char> valueBuffer = stackalloc char[value1.Length];
            int value1Length = value1.ToLowerInvariant(valueBuffer);

            //if (value1Length == -1)
            //{
            //    throw new InvalidDataException("the buffer is too small");
            //}

            int index = 1;

            switch (valueBuffer[..value1Length])
            {
                case "spectral":
                {
                    index++;

                    if (valuesCount - index < 1)
                    {
                        throw new InvalidDataException(string.Concat("A ", statement,
                            " spectral statement must specify a file name."));
                    }

                    color.SpectralFileName = GetNextValue(ref currentLine, ref values).ToString();
                    index++;

                    if (valuesCount > index)
                    {
                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                        {
                            color.SpectralFactor = value;
                        }

                        index++;
                    }

                    break;
                }
                case "xyz":
                {
                    index++;

                    if (valuesCount - index < 1)
                    {
                        throw new InvalidDataException(string.Concat("A ", statement,
                            " xyz statement must specify a color."));
                    }

                    color.UseXYZColorSpace = true;

                    var xyz = new ObjVector3();
                    if (TryFloatParse(GetNextValue(ref currentLine, ref values), out var value))
                    {
                        xyz.X = value;
                    }

                    index++;

                    if (valuesCount > index)
                    {
                        if (valuesCount - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement,
                                " xyz statement must specify a XYZ color."));
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out value))
                        {
                            xyz.Y = value;
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out value))
                        {
                            xyz.Z = value;
                        }

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
                    if (TryFloatParse(value1, out var value))
                    {
                        rgb.X = value;
                    }

                    index++;

                    if (valuesCount > index)
                    {
                        if (valuesCount - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement,
                                " statement must specify a RGB color."));
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out value))
                        {
                            rgb.Y = value;
                        }

                        if (TryFloatParse(GetNextValue(ref currentLine, ref values), out value))
                        {
                            rgb.Z = value;
                        }

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

            if (index != valuesCount)
            {
                throw new InvalidDataException(string.Concat("A ", statement, " statement has too many values."));
            }

            return color;
        }

        private enum MaterialMapType
        {
            None,
            Sphere,
            CubeTop,
            CubeBottom,
            CubeFront,
            CubeBack,
            CubeLeft,
            CubeRight
        }

        [SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "Reviewed.")]
        private static ObjMaterialMap ParseMaterialMap(ReadOnlySpan<char> statement, ref ReadOnlySpan<char> currentLine, ref SpanSplitEnumerator values, int valuesCount, ObjMaterialFileReaderSettings settings)
        {
            return ParseMaterialMap(statement, ref currentLine, ref values, valuesCount, settings, out _);
        }

        [SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "Reviewed.")]
        private static ObjMaterialMap ParseMaterialMap(ReadOnlySpan<char> statement, ref ReadOnlySpan<char> currentLine, ref SpanSplitEnumerator values, int valuesCount, ObjMaterialFileReaderSettings settings, out MaterialMapType materialMapType)
        {
            materialMapType = MaterialMapType.None;

            var map = new ObjMaterialMap();
            var charsRead = 0;

            int valueBufferSize = 16;
            Span<char> valueBuffer = stackalloc char[valueBufferSize];

            for (int index = 0; index < valuesCount;)
            {
                index++;

                if (valuesCount - index < 1)
                {
                    throw new InvalidDataException(string.Concat("A ", statement, " statement must specify a filename."));
                }

                ReadOnlySpan<char> value1 = GetNextValue(ref currentLine, ref values);
                int value1Length = value1.ToLowerInvariant(valueBuffer);
                
                // Value1Length is -1 when the buffer is too small.
                // This should only happen if the value is a file name and not an option
                if (value1Length == -1)
                {
                    if (index == 1)
                    {
                        if (statement.Equals("refl", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new InvalidDataException("A refl statement must specify a type.");
                        }    
                    }
                    if (settings.KeepWhitespacesOfMapFileReferences)
                    {
                        map.FileName = new string(currentLine[(statement.Length + charsRead + 1)..]);
                    }
                    else
                    {
                        var sb = new StringBuilder();

                        sb.Append(value1);

                        for (int i = index + 1; i < valuesCount; i++)
                        {
                            sb.Append(' ');
                            sb.Append(GetNextValue(ref currentLine, ref values));
                        }

                        string filename = sb.ToString();

                        map.FileName = filename;
                    }
                    
                    return map;
                }

                charsRead += value1Length;

                if (statement.Equals("refl", StringComparison.OrdinalIgnoreCase))
                {
                    if (index == 1)
                    {
                        if (!MemoryExtensions.Equals(valueBuffer[..value1Length], "-type", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new InvalidDataException("A refl statement must specify a type.");
                        }
                    }
                }

                switch (valueBuffer[..value1Length])
                {
                    case "-type":
                        if (valuesCount - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -type option must specify a value."));
                        }

                        if (statement.Equals("refl", StringComparison.OrdinalIgnoreCase))
                        {
                            ReadOnlySpan<char> value2 = GetNextValue(ref currentLine, ref values);
                            int value2Length = value2.ToLowerInvariant(valueBuffer);
                            charsRead += value2Length;

                            //if (value2Length == -1)
                            //{
                            //    throw new InvalidDataException("the buffer is too small");
                            //}

                            switch (valueBuffer[..value2Length])
                            {
                                case "sphere":
                                    materialMapType = MaterialMapType.Sphere;
                                    break;

                                case "cube_top":
                                    materialMapType = MaterialMapType.CubeTop;
                                    break;

                                case "cube_bottom":
                                    materialMapType = MaterialMapType.CubeBottom;
                                    break;

                                case "cube_front":
                                    materialMapType = MaterialMapType.CubeFront;
                                    break;

                                case "cube_back":
                                    materialMapType = MaterialMapType.CubeBack;
                                    break;

                                case "cube_left":
                                    materialMapType = MaterialMapType.CubeLeft;
                                    break;

                                case "cube_right":
                                    materialMapType = MaterialMapType.CubeRight;
                                    break;
                            }
                        }
                        else
                        {
                            GetNextValue(ref currentLine, ref values);
                        }

                        index++;
                        break;

                    case "-blenu":
                        {
                            if (valuesCount - index < 2)
                            {
                                throw new InvalidDataException(string.Concat("A ", statement, " -blenu option must specify a value."));
                            }

                            var value2 = GetNextValue(ref currentLine, ref values);
                            charsRead += value2.Length;

                            if (value2.Equals("on", StringComparison.OrdinalIgnoreCase))
                            {
                                map.IsHorizontalBlendingEnabled = true;
                            }
                            else if (value2.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                map.IsHorizontalBlendingEnabled = false;
                            }
                            else
                            {
                                throw new InvalidDataException(string.Concat("A ", statement, " -blenu option must specify on or off."));
                            }

                            index++;
                            break;
                        }

                    case "-blenv":
                        {
                            if (valuesCount - index < 2)
                            {
                                throw new InvalidDataException(string.Concat("A ", statement, " -blenv option must specify a value."));
                            }

                            var value2 = GetNextValue(ref currentLine, ref values);
                            charsRead += value2.Length;

                            if (value2.Equals("on", StringComparison.OrdinalIgnoreCase))
                            {
                                map.IsVerticalBlendingEnabled = true;
                            }
                            else if (value2.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                map.IsVerticalBlendingEnabled = false;
                            }
                            else
                            {
                                throw new InvalidDataException(string.Concat("A ", statement, " -blenv option must specify on or off."));
                            }

                            index++;
                            break;
                        }

                    case "-bm":
                    {
                        if (valuesCount - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -bm option must specify a value."));
                        }

                        var value2 = GetNextValue(ref currentLine, ref values);
                        charsRead += value2.Length;

                        if (TryFloatParse(value2, out var value))
                        {
                            map.BumpMultiplier = value;
                        }

                        index++;
                        break;
                    }
                    case "-boost":
                    {
                        if (valuesCount - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement,
                                " -boost option must specify a value."));
                        }

                        var value2 = GetNextValue(ref currentLine, ref values);
                        charsRead += value2.Length;

                        if (TryFloatParse(value2, out var value))
                        {
                            map.Boost = value;
                        }

                        index++;
                        break;
                    }
                    case "-cc":
                        {
                            if (valuesCount - index < 2)
                            {
                                throw new InvalidDataException(string.Concat("A ", statement, " -cc option must specify a value."));
                            }

                            var value2 = GetNextValue(ref currentLine, ref values);
                            charsRead += value2.Length;

                            if (value2.Equals("on", StringComparison.OrdinalIgnoreCase))
                            {
                                map.IsColorCorrectionEnabled = true;
                            }
                            else if (value2.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                map.IsColorCorrectionEnabled = false;
                            }
                            else
                            {
                                throw new InvalidDataException(string.Concat("A ", statement, " -cc option must specify on or off."));
                            }

                            index++;
                            break;
                        }

                    case "-clamp":
                        {
                            if (valuesCount - index < 2)
                            {
                                throw new InvalidDataException(string.Concat("A ", statement, " -clamp option must specify a value."));
                            }

                            var value2 = GetNextValue(ref currentLine, ref values);
                            charsRead += value2.Length;

                            if (value2.Equals("on", StringComparison.OrdinalIgnoreCase))
                            {
                                map.IsClampingEnabled = true;
                            }
                            else if (value2.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                map.IsClampingEnabled = false;
                            }
                            else
                            {
                                throw new InvalidDataException(string.Concat("A ", statement, " -clamp option must specify on or off."));
                            }

                            index++;
                            break;
                        }

                    case "-imfchan":
                        {
                            if (valuesCount - index < 2)
                            {
                                throw new InvalidDataException(string.Concat("A ", statement, " -imfchan option must specify a value."));
                            }

                            ReadOnlySpan<char> value2 = GetNextValue(ref currentLine, ref values);
                            int value2Length = value2.ToLowerInvariant(valueBuffer);
                            charsRead += value2Length;

                            //if (value2Length == -1)
                            //{
                            //    throw new InvalidDataException("the buffer is too small");
                            //}

                            switch (valueBuffer[..value2Length])
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
                        }

                    case "-mm":
                    {
                        if (valuesCount - index < 3)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement, " -mm option must specify a base and a gain."));
                        }

                        var value2 = GetNextValue(ref currentLine, ref values);
                        charsRead += value2.Length;

                        if (TryFloatParse(value2, out var value))
                        {
                            map.ModifierBase = value;
                        }
                        value2 = GetNextValue(ref currentLine, ref values);
                        charsRead += value2.Length;

                        if (TryFloatParse(value2, out value))
                        {
                            map.ModifierGain = value;
                        }

                        index += 2;
                        break;
                    }


                    case "-o":
                    {
                        if (valuesCount - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement,
                                " -o option must specify at least 2 values."));
                        }

                        var offset = new ObjVector3();
                        var value2 = GetNextValue(ref currentLine, ref values);

                        if (TryFloatParse(value2, out var value))
                        {
                            offset.X = value;
                            index++;
                            charsRead += value2.Length;
                        }
                        else
                        {
                            map.Offset = offset;
                            break;
                        }

                        if (valuesCount - index > 2)
                        {
                            value2 = GetNextValue(ref currentLine, ref values);
                            if (TryFloatParse(value2, out value))
                            {
                                offset.Y = value;
                                index++;
                                charsRead += value2.Length;
                            }
                            else
                            {
                                map.Offset = offset;
                                break;
                            }

                            if (valuesCount - index > 2)
                            {
                                value2 = GetNextValue(ref currentLine, ref values);
                                if (TryFloatParse(value2, out value))
                                {
                                    offset.Z = value;
                                    index++;
                                    charsRead += value2.Length;
                                }
                                else
                                {
                                    map.Offset = offset;
                                    break;
                                }
                            }
                        }
                        
                        map.Offset = offset;
                        break;
                    }
                    case "-s":
                    {
                        if (valuesCount - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement,
                                " -s option must specify at least 2 values."));
                        }

                        var scale = new ObjVector3(1.0f, 1.0f, 1.0f);
                        
                        var value2 = GetNextValue(ref currentLine, ref values);

                        if (TryFloatParse(value2, out var value))
                        {
                            scale.X = value;
                            index++;
                            charsRead += value2.Length;
                        }
                        else
                        {
                            map.Scale = scale;
                            break;
                        }

                        if (valuesCount - index > 2)
                        {
                            value2 = GetNextValue(ref currentLine, ref values);
                            if (TryFloatParse(value2, out value))
                            {
                                scale.Y = value;
                                index++;
                                charsRead += value2.Length;
                            }
                            else
                            {
                                map.Scale = scale;
                                break;
                            }

                            if (valuesCount - index > 2)
                            {
                                value2 = GetNextValue(ref currentLine, ref values);
                                if (TryFloatParse(value2, out value))
                                {
                                    scale.Z = value;
                                    index++;
                                    charsRead += value2.Length;
                                }
                                else
                                {
                                    map.Scale = scale;
                                    break;
                                }
                            }
                        }

                        map.Scale = scale;
                        break;
                    }
                    case "-t":
                    {
                        if (valuesCount - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement,
                                " -t option must specify at least 2 values."));
                        }

                        var turbulence = new ObjVector3();
                        
                        var value2 = GetNextValue(ref currentLine, ref values);

                        if (TryFloatParse(value2, out var value))
                        {
                            turbulence.X = value;
                            index++;
                            charsRead += value2.Length;
                        }
                        else
                        {
                            map.Turbulence = turbulence;
                            break;
                        }

                        if (valuesCount - index > 2)
                        {
                            value2 = GetNextValue(ref currentLine, ref values);
                            if (TryFloatParse(value2, out value))
                            {
                                turbulence.Y = value;
                                index++;
                                charsRead += value2.Length;
                            }
                            else
                            {
                                map.Turbulence = turbulence;
                                break;
                            }

                            if (valuesCount - index > 2)
                            {
                                value2 = GetNextValue(ref currentLine, ref values);
                                if (TryFloatParse(value2, out value))
                                {
                                    turbulence.Z = value;
                                    index++;
                                    charsRead += value2.Length;
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
                    {
                        if (valuesCount - index < 2)
                        {
                            throw new InvalidDataException(string.Concat("A ", statement,
                                " -texres option must specify a value."));
                        }
                        var value2 = GetNextValue(ref currentLine, ref values);
                        charsRead += value2.Length;
                        if (TryIntParse(value2, out var value))
                        {
                            map.TextureResolution = value;
                        }

                        index++;
                        break;
                    }
                    default:
                        {
                            if (settings.KeepWhitespacesOfMapFileReferences)
                            {
                                map.FileName = new string(currentLine[(statement.Length + index + charsRead - value1.Length)..]);
                            }
                            else
                            {
                                var sb = new StringBuilder();

                                sb.Append(value1);

                                for (int i = index + 1; i < valuesCount; i++)
                                {
                                    sb.Append(' ');
                                    sb.Append(GetNextValue(ref currentLine, ref values));
                                }

                                string filename = sb.ToString();

                                map.FileName = filename;
                            }
                            index = valuesCount;

                            break;
                        }
                }
            }

            return map;
        }
    }
}

#endif
