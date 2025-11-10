namespace JeremyAnsel.Media.WavefrontObj;

/// <summary>
///     Settings to control the behaviour of the <see cref="ObjMaterialFileReader" />
/// </summary>
public class ObjMaterialFileReaderSettings
{
    /// <summary>
    ///     Default settings
    /// </summary>
    public static readonly ObjMaterialFileReaderSettings Default = new();

    /// <summary>
    ///     Normally whitespaces removed during import e.g. "map_kd wl file  5.mtl" changed to "map_kd wl file 5.mtl"
    ///     If this flag is set to true, all after the map_kd will be interpreted as a single map_kd reference
    /// </summary>
    /// <remarks>
    ///     This flag should be set to true, when object files should be interpreted like other libraries like three.js or tinyobjloader
    /// </remarks>
    public bool KeepWhitespacesOfMapFileReferences { get; set; } = false;
}