// <copyright file="ObjMaterialFileWriter.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System.Globalization;

namespace JeremyAnsel.Media.WavefrontObj;

internal static class ObjMaterialFileWriter
{
    public static void Write(ObjMaterialFile mtl, StreamWriter stream)
    {
        WriteHeaderText(mtl, stream);

        foreach (ObjMaterial material in mtl.Materials)
        {
            WriteMaterial(material, stream);
        }
    }

    private static void WriteHeaderText(ObjMaterialFile mtl, StreamWriter stream)
    {
        if (string.IsNullOrEmpty(mtl.HeaderText))
        {
            return;
        }

        string[] headerLines = mtl.HeaderText!.Split('\n');

        foreach (string line in headerLines)
        {
            stream.Write('#');
            stream.WriteLine(line);
        }
    }

    private static void WriteMaterial(ObjMaterial material, StreamWriter stream)
    {
        stream.WriteLine("newmtl {0}", material.Name);

        if (material.AmbientColor != null)
        {
            WriteColor("Ka", material.AmbientColor, stream);
        }

        if (material.DiffuseColor != null)
        {
            WriteColor("Kd", material.DiffuseColor, stream);
        }

        if (material.EmissiveColor != null)
        {
            WriteColor("Ke", material.EmissiveColor, stream);
        }

        if (material.SpecularColor != null)
        {
            WriteColor("Ks", material.SpecularColor, stream);
        }

        if (material.TransmissionColor != null)
        {
            WriteColor("Tf", material.TransmissionColor, stream);
        }

        stream.WriteLine("illum {0}", material.IlluminationModel);

        stream.Write("d");
        if (material.IsHaloDissolve)
        {
            stream.Write(" -halo");
        }

        stream.Write(' ');
        stream.WriteLine(material.DissolveFactor.ToString("F6", CultureInfo.InvariantCulture));

        stream.Write("Ns ");
        stream.WriteLine(material.SpecularExponent.ToString("F6", CultureInfo.InvariantCulture));

        stream.WriteLine("sharpness {0}", material.Sharpness);

        stream.Write("Ni ");
        stream.WriteLine(material.OpticalDensity.ToString("F6", CultureInfo.InvariantCulture));

        stream.Write("map_aat ");
        if (material.IsAntiAliasingEnabled)
        {
            stream.WriteLine("on");
        }
        else
        {
            stream.WriteLine("off");
        }

        if (material.AmbientMap != null)
        {
            WriteMap("map_Ka", material.AmbientMap, stream);
        }

        if (material.DiffuseMap != null)
        {
            WriteMap("map_Kd", material.DiffuseMap, stream);
        }

        if (material.EmissiveMap != null)
        {
            WriteMap("map_Ke", material.EmissiveMap, stream);
        }

        if (material.SpecularMap != null)
        {
            WriteMap("map_Ks", material.SpecularMap, stream);
        }

        if (material.SpecularExponentMap != null)
        {
            WriteMap("map_Ns", material.SpecularExponentMap, stream);
        }

        if (material.DissolveMap != null)
        {
            WriteMap("map_d", material.DissolveMap, stream);
        }

        if (material.DecalMap != null)
        {
            WriteMap("decal", material.DecalMap, stream);
        }

        if (material.DispMap != null)
        {
            WriteMap("disp", material.DispMap, stream);
        }

        if (material.BumpMap != null)
        {
            WriteMap("bump", material.BumpMap, stream);
        }

        if (material.ReflectionMap.Sphere != null)
        {
            WriteMap("refl -type sphere", material.ReflectionMap.Sphere, stream);
        }

        if (material.ReflectionMap.CubeTop != null)
        {
            WriteMap("refl -type cube_top", material.ReflectionMap.CubeTop, stream);
        }

        if (material.ReflectionMap.CubeBottom != null)
        {
            WriteMap("refl -type cube_bottom", material.ReflectionMap.CubeBottom, stream);
        }

        if (material.ReflectionMap.CubeFront != null)
        {
            WriteMap("refl -type cube_front", material.ReflectionMap.CubeFront, stream);
        }

        if (material.ReflectionMap.CubeBack != null)
        {
            WriteMap("refl -type cube_back", material.ReflectionMap.CubeBack, stream);
        }

        if (material.ReflectionMap.CubeLeft != null)
        {
            WriteMap("refl -type cube_left", material.ReflectionMap.CubeLeft, stream);
        }

        if (material.ReflectionMap.CubeRight != null)
        {
            WriteMap("refl -type cube_right", material.ReflectionMap.CubeRight, stream);
        }

        if (material.Roughness != 0.0f)
        {
            stream.Write("Pr ");
            stream.WriteLine(material.Roughness.ToString("F6", CultureInfo.InvariantCulture));
        }

        if (material.RoughnessMap != null)
        {
            WriteMap("map_Pr", material.RoughnessMap, stream);
        }

        if (material.Metallic != 0.0f)
        {
            stream.Write("Pm ");
            stream.WriteLine(material.Metallic.ToString("F6", CultureInfo.InvariantCulture));
        }

        if (material.MetallicMap != null)
        {
            WriteMap("map_Pm", material.MetallicMap, stream);
        }

        if (material.Sheen != 0.0f)
        {
            stream.Write("Ps ");
            stream.WriteLine(material.Sheen.ToString("F6", CultureInfo.InvariantCulture));
        }

        if (material.SheenMap != null)
        {
            WriteMap("map_Ps", material.SheenMap, stream);
        }

        if (material.ClearCoatThickness != 0.0f)
        {
            stream.Write("Pc ");
            stream.WriteLine(material.ClearCoatThickness.ToString("F6", CultureInfo.InvariantCulture));
        }

        if (material.ClearCoatRoughness != 0.0f)
        {
            stream.Write("Pcr ");
            stream.WriteLine(material.ClearCoatRoughness.ToString("F6", CultureInfo.InvariantCulture));
        }

        if (material.Anisotropy != 0.0f)
        {
            stream.Write("aniso ");
            stream.WriteLine(material.Anisotropy.ToString("F6", CultureInfo.InvariantCulture));
        }

        if (material.AnisotropyRotation != 0.0f)
        {
            stream.Write("anisor ");
            stream.WriteLine(material.AnisotropyRotation.ToString("F6", CultureInfo.InvariantCulture));
        }

        if (material.Norm != null)
        {
            WriteMap("norm", material.Norm, stream);
        }
    }

    private static void WriteColor(string statement, ObjMaterialColor color, StreamWriter stream)
    {
        stream.Write(statement);

        if (color.IsSpectral)
        {
            stream.Write(" spectral ");
            stream.Write(color.SpectralFileName);

            if (color.SpectralFactor != 1.0f)
            {
                stream.Write(' ');
                stream.Write(color.SpectralFactor.ToString("F6", CultureInfo.InvariantCulture));
            }

            stream.WriteLine();
        }
        else
        {
            if (color.UseXYZColorSpace)
            {
                stream.Write(" xyz");
            }

            stream.WriteLine(
                " {0} {1} {2}",
                color.Color.X.ToString("F6", CultureInfo.InvariantCulture),
                color.Color.Y.ToString("F6", CultureInfo.InvariantCulture),
                color.Color.Z.ToString("F6", CultureInfo.InvariantCulture));
        }
    }

    private static void WriteMap(string statement, ObjMaterialMap map, StreamWriter stream)
    {
        stream.Write(statement);

        if (!map.IsHorizontalBlendingEnabled)
        {
            stream.Write(" -blenu off");
        }

        if (!map.IsVerticalBlendingEnabled)
        {
            stream.Write(" -blenv off");
        }

        if (map.BumpMultiplier != 0.0f)
        {
            stream.Write(" -bm ");
            stream.Write(map.BumpMultiplier.ToString("F6", CultureInfo.InvariantCulture));
        }

        if (map.Boost != 0.0f)
        {
            stream.Write(" -boost ");
            stream.Write(map.Boost.ToString("F6", CultureInfo.InvariantCulture));
        }

        if (map.IsColorCorrectionEnabled)
        {
            stream.Write(" -cc on");
        }

        if (map.IsClampingEnabled)
        {
            stream.Write(" -clamp on");
        }

        if (map.ScalarChannel != ObjMapChannel.Luminance)
        {
            stream.Write(" -imfchan ");

            switch (map.ScalarChannel)
            {
                case ObjMapChannel.Red:
                    stream.Write("r");
                    break;

                case ObjMapChannel.Green:
                    stream.Write("g");
                    break;

                case ObjMapChannel.Blue:
                    stream.Write("b");
                    break;

                case ObjMapChannel.Matte:
                    stream.Write("m");
                    break;

                case ObjMapChannel.Depth:
                    stream.Write("z");
                    break;

                default:
                    stream.Write("l");
                    break;
            }
        }

        if (map.ModifierBase != 0.0f || map.ModifierGain != 1.0f)
        {
            stream.Write(" -mm ");
            stream.Write(map.ModifierBase.ToString("F6", CultureInfo.InvariantCulture));
            stream.Write(' ');
            stream.Write(map.ModifierGain.ToString("F6", CultureInfo.InvariantCulture));
        }

        ObjVector3 offset = map.Offset;

        if (offset.X != 0.0f || offset.Y != 0.0f || offset.Z != 0.0f)
        {
            stream.Write(" -o ");
            stream.Write(offset.X.ToString("F6", CultureInfo.InvariantCulture));
            stream.Write(' ');
            stream.Write(offset.Y.ToString("F6", CultureInfo.InvariantCulture));
            stream.Write(' ');
            stream.Write(offset.Z.ToString("F6", CultureInfo.InvariantCulture));
        }

        ObjVector3 scale = map.Scale;

        if (scale.X != 1.0f || scale.Y != 1.0f || scale.Z != 1.0f)
        {
            stream.Write(" -s ");
            stream.Write(scale.X.ToString("F6", CultureInfo.InvariantCulture));
            stream.Write(' ');
            stream.Write(scale.Y.ToString("F6", CultureInfo.InvariantCulture));
            stream.Write(' ');
            stream.Write(scale.Z.ToString("F6", CultureInfo.InvariantCulture));
        }

        ObjVector3 turbulence = map.Turbulence;

        if (turbulence.X != 0.0f || turbulence.Y != 0.0f || turbulence.Z != 0.0f)
        {
            stream.Write(" -t ");
            stream.Write(turbulence.X.ToString("F6", CultureInfo.InvariantCulture));
            stream.Write(' ');
            stream.Write(turbulence.Y.ToString("F6", CultureInfo.InvariantCulture));
            stream.Write(' ');
            stream.Write(turbulence.Z.ToString("F6", CultureInfo.InvariantCulture));
        }

        if (map.TextureResolution != 0)
        {
            stream.Write(" -texres ");
            stream.Write(map.TextureResolution);
        }

        stream.Write(' ');
        stream.WriteLine(map.FileName);
    }
}