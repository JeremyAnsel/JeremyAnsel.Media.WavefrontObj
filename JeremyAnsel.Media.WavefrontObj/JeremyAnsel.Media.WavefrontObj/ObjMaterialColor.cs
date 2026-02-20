// <copyright file="ObjMaterialColor.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

namespace JeremyAnsel.Media.WavefrontObj;

[System.Diagnostics.DebuggerDisplay("{ToDebuggerDisplayString(),nq}")]
public class ObjMaterialColor
{
    public ObjMaterialColor()
    {
        this.SpectralFactor = 1.0f;
    }

    public ObjMaterialColor(float x, float y, float z)
        : this()
    {
        this.Color = new ObjVector3(x, y, z);
    }

    public ObjMaterialColor(string? spectralFileName)
        : this()
    {
        this.SpectralFileName = spectralFileName;
    }

    public ObjMaterialColor(string? spectralFileName, float factor)
        : this()
    {
        this.SpectralFileName = spectralFileName;
        this.SpectralFactor = factor;
    }

    public ObjVector3 Color { get; set; }

    // ReSharper disable once InconsistentNaming
    public bool UseXYZColorSpace { get; set; }

    public string? SpectralFileName { get; set; }

    public float SpectralFactor { get; set; }

    public bool IsSpectral
    {
        get
        {
            return !string.IsNullOrWhiteSpace(this.SpectralFileName);
        }
    }

    // ReSharper disable once InconsistentNaming
    public bool IsRGB => !IsSpectral && !UseXYZColorSpace;

    // ReSharper disable once InconsistentNaming
    public bool IsXYZ => !IsSpectral && UseXYZColorSpace;

    private string ToDebuggerDisplayString()
    {
        if (IsSpectral) return $"Spectral:{SpectralFileName} Factor:{SpectralFactor}";
        if (UseXYZColorSpace) return $"X:{Color.X} Y:{Color.Y} Z:{Color.Z}";
        return $"R:{Color.X} G:{Color.Y} B:{Color.Z}";
    }
}