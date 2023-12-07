﻿// <copyright file="ObjMaterialFile.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JeremyAnsel.Media.WavefrontObj
{
    public class ObjMaterialFile
    {
        public ObjMaterialFile()
        {
            this.Materials = new List<ObjMaterial>();
        }

        public string HeaderText { get; set; }

        public List<ObjMaterial> Materials { get; private set; }

        public static ObjMaterialFile FromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return ObjMaterialFileReader.FromStream(stream);
            }
        }

        public static ObjMaterialFile FromStream(Stream stream)
        {
            return ObjMaterialFileReader.FromStream(stream);
        }

        public void WriteTo(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                ObjMaterialFileWriter.Write(this, writer);
            }
        }

        public void WriteTo(Stream stream)
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
}
