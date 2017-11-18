// <copyright file="ObjMaterialReflectionMap.cs" company="Jérémy Ansel">
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
    public class ObjMaterialReflectionMap
    {
        public ObjMaterialMap Sphere { get; set; }

        public ObjMaterialMap CubeTop { get; set; }

        public ObjMaterialMap CubeBottom { get; set; }

        public ObjMaterialMap CubeFront { get; set; }

        public ObjMaterialMap CubeBack { get; set; }

        public ObjMaterialMap CubeLeft { get; set; }

        public ObjMaterialMap CubeRight { get; set; }
    }
}
