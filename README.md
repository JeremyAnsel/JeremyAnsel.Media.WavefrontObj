# JeremyAnsel.Media.WavefrontObj

[![Build status](https://ci.appveyor.com/api/projects/status/lepi4cvf6gqjyy9c/branch/master?svg=true)](https://ci.appveyor.com/project/JeremyAnsel/jeremyansel-media-wavefrontobj/branch/master)
[![Code coverage](https://raw.githubusercontent.com/JeremyAnsel/JeremyAnsel.Media.WavefrontObj/gh-pages/coverage/badge_combined.svg)](https://jeremyansel.github.io/JeremyAnsel.Media.WavefrontObj/coverage/)
[![NuGet Version](https://img.shields.io/nuget/v/JeremyAnsel.Media.WavefrontObj)](https://www.nuget.org/packages/JeremyAnsel.Media.WavefrontObj)
![License](https://img.shields.io/github/license/JeremyAnsel/JeremyAnsel.Media.WavefrontObj)

JeremyAnsel.Media.WavefrontObj is a .Net library to handle Wavefront Obj .obj and .mtl files.

Description     | Value
----------------|----------------
License         | [The MIT License (MIT)](https://github.com/JeremyAnsel/JeremyAnsel.Media.WavefrontObj/blob/master/LICENSE.txt)
Documentation   | http://jeremyansel.github.io/JeremyAnsel.Media.WavefrontObj
Code coverage   | https://jeremyansel.github.io/JeremyAnsel.Media.WavefrontObj/coverage/
Source code     | https://github.com/JeremyAnsel/JeremyAnsel.Media.WavefrontObj
Nuget           | https://www.nuget.org/packages/JeremyAnsel.Media.WavefrontObj
Build           | https://ci.appveyor.com/project/JeremyAnsel/jeremyansel-media-wavefrontobj/branch/master

# Usage

```csharp
// Sample: read an obj file with textures.
// The obj file (.obj) contains the geometry of the 3d object. The material file (.mtl) defines the textures.

// open an obj file
ObjFile objFile = ObjFile.FromFile("objfile.obj");

// open the associated material file
ObjMaterialFile objMaterialFile = ObjMaterialFile.FromFile("objfile.mtl");

// Get the materials
foreach (ObjMaterial material in objMaterialFile.Materials)
{
    // name of the material
    string? name = material.Name;

    // filename of the texture
    string? filename = material.DiffuseMap?.FileName;
}

// The 3d geometry contains faces, vertices, normals, and texture coordinates.
// The vertices, normals, and texture coordinates are defined in global lists.
// The faces contain indices to these lists. The indices start at 1.

// Get the vertices
foreach (ObjVertex v in objFile.Vertices)
{
    // a vertex has a position and an optional color
    ObjVector4 position = v.Position;
    ObjVector4? color = v.Color;
}

// Get the normals
foreach (ObjVector3 v in objFile.VertexNormals)
{
}

// Get the texture coordinates
foreach (ObjVector3 v in objFile.TextureVertices)
{
}

// Get the faces
foreach (ObjFace face in objFile.Faces)
{
    // name of the texture as defined in the material file
    // face.MaterialName is the name of the material as in material.Name
    string? textureName = face.MaterialName;

    // points of the face
    // for a triangle there are 3 points
    // for a quad there are 4 points
    // there can be more points for a polygon
    List<ObjTriplet> points = face.Vertices;

    foreach (ObjTriplet point in points)
    {
        // index into the global vertices list
        int vertexIndex = point.Vertex - 1;

        // index into the global normals list
        int normalIndex = point.Normal - 1;

        // index into the global texture coordinates list
        int textureCoordinatesIndex = point.Texture - 1;
    }
}
```

An invalid index is 0.
In the specs an index can be positive or negative.
Positive means that the index starts at the beginning of the global list.
Negative means that the index starts at the end of the list.
While reading the library converts negative indices to positive indices.
That is why the invalid index is 0.
