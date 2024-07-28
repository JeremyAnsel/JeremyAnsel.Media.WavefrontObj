﻿// <copyright file="ObjElement.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

namespace JeremyAnsel.Media.WavefrontObj
{
    public abstract class ObjElement
    {
        internal ObjElement()
        {
        }

        public string? ObjectName { get; set; }

        public int LevelOfDetail { get; set; }

        public string? MapName { get; set; }

        public string? MaterialName { get; set; }
    }
}
