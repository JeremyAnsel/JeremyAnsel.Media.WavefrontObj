using System.Reflection;

namespace JeremyAnsel.Media.WavefrontObj.Tests.SharpEqualityAssert;

internal class EqualityOperatorAsserter
{
    public static void AssertOperatorEquality<T>(T first, T second, T third) where T : notnull
    {
        // ReSharper disable once InconsistentNaming
        var op_equality = typeof(T).GetMethod("op_Equality", new[] { typeof(T), typeof(T) });
        if (op_equality == null)
            return; // operator== not implemented

        if (!GetBoolResult(op_equality, first, first))
            throw new EqualityAssertFailedException("Expected (first == first) to return true.");

        if (!GetBoolResult(op_equality, first, second))
            throw new EqualityAssertFailedException("Expected (first == second) to return true.");

        if (GetBoolResult(op_equality, first, third))
            throw new EqualityAssertFailedException("Expected (first == third) to return false.");

        if (GetBoolResult(op_equality, second, third))
            throw new EqualityAssertFailedException("Expected (second == third) to return false.");

        if (GetBoolResult(op_equality, first, default))
            throw new EqualityAssertFailedException("Expected (first == null) to return false.");
    }

    public static void AssertOperatorInequality<T>(T first, T second, T third) where T : notnull
    {
        // ReSharper disable once InconsistentNaming
        var op_inequality = typeof(T).GetMethod("op_Inequality", new[] { typeof(T), typeof(T) });
        if (op_inequality == null)
            return; // operator!= not implemented

        if (GetBoolResult(op_inequality, first, first))
            throw new EqualityAssertFailedException("Expected (first != first) to return false.");

        if (GetBoolResult(op_inequality, first, second))
            throw new EqualityAssertFailedException("Expected (first != second) to return false.");

        if (!GetBoolResult(op_inequality, first, third))
            throw new EqualityAssertFailedException("Expected (first != third) to return true.");

        if (!GetBoolResult(op_inequality, second, third))
            throw new EqualityAssertFailedException("Expected (second != third) to return true.");

        if (!GetBoolResult(op_inequality, first, default))
            throw new EqualityAssertFailedException("Expected (first != null) to return true.");
    }

    private static bool GetBoolResult<T>(MethodInfo method, T? first, T? second) where T : notnull
    {
        return (bool?)method.Invoke(null, new object?[] { first, second }) ?? false;
    }
}
