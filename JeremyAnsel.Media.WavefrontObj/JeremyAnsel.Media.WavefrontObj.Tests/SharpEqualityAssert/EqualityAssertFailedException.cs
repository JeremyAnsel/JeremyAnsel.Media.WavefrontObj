using System.ComponentModel;
using System.Runtime.Serialization;
using System;

namespace JeremyAnsel.Media.WavefrontObj.Tests.SharpEqualityAssert;

/// <summary>
/// AssertFailedException class. Used to indicate failure for a test case.
/// </summary>
public class EqualityAssertFailedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityAssertFailedException"/> class.
    /// </summary>
    /// <param name="msg"> The message. </param>
    /// <param name="ex"> The exception. </param>
    public EqualityAssertFailedException(string msg, Exception ex)
        : base(msg, ex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityAssertFailedException"/> class.
    /// </summary>
    /// <param name="msg"> The message. </param>
    public EqualityAssertFailedException(string msg)
        : base(msg)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityAssertFailedException"/> class.
    /// </summary>
    public EqualityAssertFailedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityAssertFailedException"/> class.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="context">Streaming context.</param>
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected EqualityAssertFailedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
