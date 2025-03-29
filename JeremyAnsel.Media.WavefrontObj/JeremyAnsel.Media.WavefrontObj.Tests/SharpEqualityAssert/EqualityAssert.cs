using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System;

namespace JeremyAnsel.Media.WavefrontObj.Tests.SharpEqualityAssert;

/// <summary>
/// Can be used to assert the correctness of equality members or an equality comparer.
/// </summary>
public class EqualityAssert
{
    /// <summary>
    ///     Asserts that all equality members are implemented correctly.
    /// </summary>
    /// <typeparam name="T">Type of the object.</typeparam>
    /// <param name="first">First instance of the {T}.</param>
    /// <param name="second">
    ///     Second instance of {T}, which should be value-equal to the <paramref name="first" />, but not the
    ///     same instance.
    /// </param>
    /// <param name="third">
    ///     Third instance of {T}, which should NOT be value-equal, to neither <paramref name="first" />, nor
    ///     <paramref name="second" />.
    /// </param>
    public static void EqualityMembers<T>(T? first, T? second, T? third) where T : notnull
    {
        if (first == null)
            throw new ArgumentNullException(nameof(first));
        if (second == null)
            throw new ArgumentNullException(nameof(second));
        if (third == null)
            throw new ArgumentNullException(nameof(third));

        if (ReferenceEquals(first, second))
            throw new ArgumentException("first and second argument should be equal by value, but not by reference (equivalent, but not the same).");
        if (ReferenceEquals(first, third))
            throw new ArgumentException("first and third argument should differ by reference and by value.");
        if (ReferenceEquals(second, third))
            throw new ArgumentException("second and third argument should differ by reference and by value.");

        try
        {
            EqualsObjectAsserter.AssertEqualsObject(first, second, third);
            EqualsObjectAsserter.AssertGetHashCode(first, second, third);

            GenericEqualsAsserter.AssertGenericEquals(first as IEquatable<T?>, second as IEquatable<T?>,
                third as IEquatable<T?>);

            EqualityOperatorAsserter.AssertOperatorEquality(first, second, third);
            EqualityOperatorAsserter.AssertOperatorInequality(first, second, third);
        }
        catch (TargetInvocationException ex)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException!).Throw();
        }
    }

    /// <summary>
    ///     Asserts that the equality comparer is implemented correctly.
    /// </summary>
    /// <typeparam name="T">Type of the object.</typeparam>
    /// <param name="comparer">An instance of the comparer to test.</param>
    /// <param name="first">First instance of the {T}.</param>
    /// <param name="second">
    ///     Second instance of {T}, which should be value-equal to the <paramref name="first" />, but not the
    ///     same instance.
    /// </param>
    /// <param name="third">
    ///     Third instance of {T}, which should NOT be value-equal, to neither <paramref name="first" />, nor
    ///     <paramref name="second" />.
    /// </param>
    public static void EqualityComparer<T>(IEqualityComparer<T?> comparer, T first, T second, T third) where T : notnull
    {
        if (comparer == null)
            throw new ArgumentNullException(nameof(comparer));
        if (first == null)
            throw new ArgumentNullException(nameof(first));
        if (second == null)
            throw new ArgumentNullException(nameof(second));
        if (third == null)
            throw new ArgumentNullException(nameof(third));

        EqualityComparerAsserter.AssertEqualityComparerEquals(comparer, first, second, third);
        EqualityComparerAsserter.AssertComparerGetHashCode(comparer, first, second, third);
    }
}
