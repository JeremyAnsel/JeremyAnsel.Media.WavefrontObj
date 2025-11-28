// <copyright file="AssertExtensions.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

using Xunit;

namespace JeremyAnsel.Media.WavefrontObj.Tests;

public static class AssertExtensions
{
    public static void TextEqual(string expected, string text)
    {
        Assert.Equal(expected, text, ignoreLineEndingDifferences: true);
    }
}