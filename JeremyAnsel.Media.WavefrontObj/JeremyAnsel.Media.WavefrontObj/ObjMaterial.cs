// <copyright file="ObjMaterial.cs" company="Jérémy Ansel">
// Copyright (c) 2017 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JeremyAnsel.Media.WavefrontObj
{
    public class ObjMaterial
    {
        public ObjMaterial()
        {
            this.ReflectionMap = new ObjMaterialReflectionMap();

            this.IlluminationModel = 2;
            this.DissolveFactor = 1.0f;
            this.Sharpness = 60;
            this.OpticalDensity = 1.0f;
        }

        public ObjMaterial(string name)
            :this()
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public ObjMaterialColor AmbientColor { get; set; }

        public ObjMaterialColor DiffuseColor { get; set; }

        public ObjMaterialColor EmissiveColor { get; set; }

        public ObjMaterialColor SpecularColor { get; set; }

        public ObjMaterialColor TransmissionColor { get; set; }

        public int IlluminationModel { get; set; }

        public float DissolveFactor { get; set; }

        public bool IsHaloDissolve { get; set; }

        public float SpecularExponent { get; set; }

        public int Sharpness { get; set; }

        public float OpticalDensity { get; set; }

        public bool IsAntiAliasingEnabled { get; set; }

        public ObjMaterialMap AmbientMap { get; set; }

        public ObjMaterialMap DiffuseMap { get; set; }

        public ObjMaterialMap EmissiveMap { get; set; }

        public ObjMaterialMap SpecularMap { get; set; }

        public ObjMaterialMap SpecularExponentMap { get; set; }

        public ObjMaterialMap DissolveMap { get; set; }

        public ObjMaterialMap DecalMap { get; set; }

        public ObjMaterialMap DispMap { get; set; }

        public ObjMaterialMap BumpMap { get; set; }

        public ObjMaterialReflectionMap ReflectionMap { get; private set; }
    }
}
