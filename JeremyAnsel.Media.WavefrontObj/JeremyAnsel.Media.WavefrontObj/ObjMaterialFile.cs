// <copyright file="ObjMaterialFile.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System.Text;

namespace JeremyAnsel.Media.WavefrontObj;

public class ObjMaterialFile
{
    public ObjMaterialFile()
    {
        Materials = new List<ObjMaterial>();
    }

    public string? HeaderText { get; set; }

    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
    public List<ObjMaterial> Materials { get; private set; }

    public static ObjMaterialFile FromFile(string? path)
    {
        return FromFile(path, ObjMaterialFileReaderSettings.Default);
    }

    public static ObjMaterialFile FromFile(string? path, ObjMaterialFileReaderSettings settings)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            return ObjMaterialFileReader.FromStream(stream, settings);
        }
    }

    public static ObjMaterialFile FromStream(Stream? stream)
    {
        return FromStream(stream, ObjMaterialFileReaderSettings.Default);
    }
        
    public static ObjMaterialFile FromStream(Stream? stream, ObjMaterialFileReaderSettings settings)
    {
        return ObjMaterialFileReader.FromStream(stream, settings);
    }

    public void WriteTo(string? path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        using (var writer = new StreamWriter(path))
        {
            ObjMaterialFileWriter.Write(this, writer);
        }
    }

    public void WriteTo(Stream? stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using (var writer = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, true))
        {
            ObjMaterialFileWriter.Write(this, writer);
        }
    }
}