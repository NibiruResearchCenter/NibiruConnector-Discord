// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.Text.RegularExpressions;

namespace NibiruConnector.Extensions;

public static partial class StringExtension
{
    [GeneratedRegex("§[0-9a-fk-or]")]
    private static partial Regex MinecraftFormattingRegex();
    
    public static string RemoveMinecraftFormatting(this string str)
    {
        return MinecraftFormattingRegex().Replace(str, string.Empty);
    }

    public static string GetPlainTextMarkdown(this string str)
    {
        return $"""
                ```text
                {str}
                ```
                """;
    }
}
