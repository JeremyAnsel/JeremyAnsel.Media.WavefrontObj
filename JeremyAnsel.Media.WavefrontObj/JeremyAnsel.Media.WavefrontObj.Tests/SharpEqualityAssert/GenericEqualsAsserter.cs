using System;

namespace JeremyAnsel.Media.WavefrontObj.Tests.SharpEqualityAssert;

internal class GenericEqualsAsserter
{
    public static void AssertGenericEquals<T>(IEquatable<T?>? first, IEquatable<T?>? second, IEquatable<T?>? third) where T : notnull
    {
        if (first == null || second == null || third == null)
            return; // if not implemented, no check required.

        // ReSharper disable once EqualExpressionComparison
        if (!first.Equals((T)first))
            throw new EqualityAssertFailedException("Expected first.Equals<T>(T first) to return true.");

        if (!first.Equals((T)second))
            throw new EqualityAssertFailedException("Expected first.Equals<T>(T second) to return true.");

        if (first.Equals((T)third))
            throw new EqualityAssertFailedException("Expected first.Equals<T>(T third) to return false.");

        if (second.Equals((T)third))
            throw new EqualityAssertFailedException("Expected second.Equals<T>(T third) to return false.");

        if (first.Equals(default))
            throw new EqualityAssertFailedException("Expected first.Equals(default(T)) to return false.");
    }
}
