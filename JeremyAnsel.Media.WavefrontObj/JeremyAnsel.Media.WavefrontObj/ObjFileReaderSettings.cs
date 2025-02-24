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
}