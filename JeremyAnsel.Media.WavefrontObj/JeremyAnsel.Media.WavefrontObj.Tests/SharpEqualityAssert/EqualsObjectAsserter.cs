namespace JeremyAnsel.Media.WavefrontObj.Tests.SharpEqualityAssert;

internal class EqualsObjectAsserter
{
    public static void AssertEqualsObject(object first, object second, object third)
    {
        // ReSharper disable once EqualExpressionComparison
        if (!first.Equals(first))
            throw new EqualityAssertFailedException("Expected first.Equals((object) first) to return true.");

        if (!first.Equals(second))
            throw new EqualityAssertFailedException("Expected first.Equals((object) second) to return true.");

        if (first.Equals(third))
            throw new EqualityAssertFailedException("Expected first.Equals((object) third) to return false.");

        if (second.Equals(third))
            throw new EqualityAssertFailedException("Expected second.Equals((object) third) to return false.");

        if (first.Equals(null))
            throw new EqualityAssertFailedException("Expected first.Equals((object) null) to return false.");

        if (first.Equals(new object()))
            throw new EqualityAssertFailedException("Expected first.Equals(new object()) to return false.");
    }

    public static void AssertGetHashCode<T>(T first, T second, T third) where T : notnull
    {
        if (first.GetHashCode() != second.GetHashCode())
            throw new EqualityAssertFailedException("Expected first.GetHashCode() to be equal to second.GetHashCode().");

        if (first.GetHashCode() == third.GetHashCode())
            throw new EqualityAssertFailedException("Expected first.GetHashCode() to be not equal to third.GetHashCode().");

        if (second.GetHashCode() == third.GetHashCode())
            throw new EqualityAssertFailedException("Expected second.GetHashCode() to be not equal to third.GetHashCode().");

        if (first.GetHashCode() == 0)
            throw new EqualityAssertFailedException("Expected first.GetHashCode() not to be equal to zero (0).");
    }
}
