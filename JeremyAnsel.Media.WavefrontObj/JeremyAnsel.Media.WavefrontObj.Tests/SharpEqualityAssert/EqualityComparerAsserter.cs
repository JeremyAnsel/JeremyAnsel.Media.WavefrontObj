using System.Collections.Generic;

namespace JeremyAnsel.Media.WavefrontObj.Tests.SharpEqualityAssert;

internal class EqualityComparerAsserter
{
    public static void AssertEqualityComparerEquals<T>(IEqualityComparer<T?> comparer, T first, T second, T third) where T : notnull
    {
        if (!comparer.Equals(first, first))
            throw new EqualityAssertFailedException("Expected comparer.Equals(T first, T first) to return true.");

        if (!comparer.Equals(first, second))
            throw new EqualityAssertFailedException("Expected comparer.Equals(T first, T second) to return true.");

        if (comparer.Equals(first, third))
            throw new EqualityAssertFailedException("Expected comparer.Equals(T first, T third) to return false.");

        if (comparer.Equals(second, third))
            throw new EqualityAssertFailedException("Expected comparer.Equals(T second, T third) to return false.");

        if (comparer.Equals(first, default))
            throw new EqualityAssertFailedException("Expected comparer.Equals(T first, T default(T)) to return false.");

        if (comparer.Equals(default, second))
            throw new EqualityAssertFailedException("Expected comparer.Equals(default(T), T second) to return false.");
    }

    public static void AssertComparerGetHashCode<T>(IEqualityComparer<T?> comparer, T first, T second, T third) where T : notnull
    {
        if (comparer.GetHashCode(first) != comparer.GetHashCode(second))
            throw new EqualityAssertFailedException(
                "Expected comparer.GetHashCode(first) to be equal to comparer.GetHashCode(second).");

        if (comparer.GetHashCode(first) == comparer.GetHashCode(third))
            throw new EqualityAssertFailedException(
                "Expected comparer.GetHashCode(first) to be not equal to comparer.GetHashCode(third).");

        if (comparer.GetHashCode(second) == comparer.GetHashCode(third))
            throw new EqualityAssertFailedException(
                "Expected comparer.GetHashCode(second) to be not equal to comparer.GetHashCode(third).");

        if (comparer.GetHashCode(first) == 0)
            throw new EqualityAssertFailedException("Expected comparer.GetHashCode(first) not to be equal to zero (0).");
    }
}
