// <copyright file="ObjVector3.cs" company="Jérémy Ansel">
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
    public struct ObjVector3
    {
        private float x;

        private float y;

        private float z;

        public ObjVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public ObjVector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 1.0f;
        }

        public ObjVector3(float x)
        {
            this.x = x;
            this.y = 0.0f;
            this.z = 1.0f;
        }

        public float X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public float Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        public float Z
        {
            get { return this.z; }
            set { this.z = value; }
        }
    }
}
