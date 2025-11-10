namespace JeremyAnsel.Media.WavefrontObj;

/// <summary>
///     Settings to control the behaviour of the <see cref="ObjFileReader" />
/// </summary>
public class ObjFileReaderSettings
{
    /// <summary>
    ///     Default settings
    /// </summary>
    public static readonly ObjFileReaderSettings Default = new();

    /// <summary>
    ///     Object names normally not interpreted as a <see cref="ObjGroup" />
    ///     If this flag is set to true, object names are handled as a group.
    /// </summary>
    /// <remarks>
    ///     This flag should be set to true, when object files should be interpreted like other libraries like three.js or tinyobjloader
    /// </remarks>
    public bool HandleObjectNamesAsGroup { get; set; } = false;

    /// <summary>
    ///     Normally multiple group names are valid per line e.g. "g group_name1 group_name1"
    ///     If this flag is set to true, all after the g will be interpreted as a single group name
    /// </summary>
    /// <remarks>
    ///     This flag should be set to true, when object files should be interpreted like other libraries like three.js or tinyobjloader
    /// </remarks>
    public bool OnlyOneGroupNamePerLine { get; set; } = false;

    /// <summary>
    ///     Normally whitespaces removed during import e.g. "mtllib wl file  5.mtl" changed to "mtllib wl file 5.mtl"
    ///     If this flag is set to true, all after the mtllib will be interpreted as a single mtllib reference
    /// </summary>
    /// <remarks>
    ///     This flag should be set to true, when object files should be interpreted like other libraries like three.js or tinyobjloader
    /// </remarks>
    public bool KeepWhitespacesOfMtlLibReferences { get; set; } = false;
}