using System.Collections.Generic;

namespace BetterVanilla.Ui.SourceGenerator.Models;

/// <summary>
/// Root configuration loaded from ui-aliases.json.
/// </summary>
public sealed class AliasConfig
{
    public string Version { get; set; } = "1.0";
    public string? DefaultBundle { get; set; }
    public string? DefaultNamespace { get; set; }
    public Dictionary<string, AliasDefinition> Aliases { get; set; } = new();
}

/// <summary>
/// Definition of a single alias.
/// </summary>
public sealed class AliasDefinition
{
    public string Prefab { get; set; } = string.Empty;
    public string Component { get; set; } = string.Empty;
    public string? Bundle { get; set; }
}

/// <summary>
/// Simple JSON parser for ui-aliases.json (to avoid System.Text.Json dependency in netstandard2.0).
/// </summary>
public static class AliasConfigParser
{
    public static AliasConfig Parse(string json)
    {
        var config = new AliasConfig();

        // Simple state-machine JSON parser
        var i = 0;
        SkipWhitespace(json, ref i);

        if (i >= json.Length || json[i] != '{')
            return config;
        i++;

        while (i < json.Length)
        {
            SkipWhitespace(json, ref i);
            if (i >= json.Length || json[i] == '}')
                break;

            var key = ReadString(json, ref i);
            SkipWhitespace(json, ref i);

            if (i >= json.Length || json[i] != ':')
                break;
            i++;

            SkipWhitespace(json, ref i);

            switch (key)
            {
                case "version":
                    config.Version = ReadString(json, ref i);
                    break;
                case "defaultBundle":
                    config.DefaultBundle = ReadString(json, ref i);
                    break;
                case "defaultNamespace":
                    config.DefaultNamespace = ReadString(json, ref i);
                    break;
                case "aliases":
                    config.Aliases = ParseAliases(json, ref i);
                    break;
                default:
                    SkipValue(json, ref i);
                    break;
            }

            SkipWhitespace(json, ref i);
            if (i < json.Length && json[i] == ',')
                i++;
        }

        return config;
    }

    private static Dictionary<string, AliasDefinition> ParseAliases(string json, ref int i)
    {
        var aliases = new Dictionary<string, AliasDefinition>();

        SkipWhitespace(json, ref i);
        if (i >= json.Length || json[i] != '{')
            return aliases;
        i++;

        while (i < json.Length)
        {
            SkipWhitespace(json, ref i);
            if (i >= json.Length || json[i] == '}')
            {
                i++;
                break;
            }

            var aliasName = ReadString(json, ref i);
            SkipWhitespace(json, ref i);

            if (i >= json.Length || json[i] != ':')
                break;
            i++;

            var aliasDef = ParseAliasDefinition(json, ref i);
            aliases[aliasName] = aliasDef;

            SkipWhitespace(json, ref i);
            if (i < json.Length && json[i] == ',')
                i++;
        }

        return aliases;
    }

    private static AliasDefinition ParseAliasDefinition(string json, ref int i)
    {
        var def = new AliasDefinition();

        SkipWhitespace(json, ref i);
        if (i >= json.Length || json[i] != '{')
            return def;
        i++;

        while (i < json.Length)
        {
            SkipWhitespace(json, ref i);
            if (i >= json.Length || json[i] == '}')
            {
                i++;
                break;
            }

            var key = ReadString(json, ref i);
            SkipWhitespace(json, ref i);

            if (i >= json.Length || json[i] != ':')
                break;
            i++;

            SkipWhitespace(json, ref i);

            switch (key)
            {
                case "prefab":
                    def.Prefab = ReadString(json, ref i);
                    break;
                case "component":
                    def.Component = ReadString(json, ref i);
                    break;
                case "bundle":
                    def.Bundle = ReadString(json, ref i);
                    break;
                default:
                    SkipValue(json, ref i);
                    break;
            }

            SkipWhitespace(json, ref i);
            if (i < json.Length && json[i] == ',')
                i++;
        }

        return def;
    }

    private static void SkipWhitespace(string json, ref int i)
    {
        while (i < json.Length && char.IsWhiteSpace(json[i]))
            i++;
    }

    private static string ReadString(string json, ref int i)
    {
        SkipWhitespace(json, ref i);
        if (i >= json.Length || json[i] != '"')
            return string.Empty;

        i++; // Skip opening quote
        var start = i;

        while (i < json.Length && json[i] != '"')
        {
            if (json[i] == '\\' && i + 1 < json.Length)
                i += 2;
            else
                i++;
        }

        var result = json.Substring(start, i - start);
        if (i < json.Length)
            i++; // Skip closing quote

        // Handle escape sequences
        result = result
            .Replace("\\\"", "\"")
            .Replace("\\\\", "\\")
            .Replace("\\n", "\n")
            .Replace("\\r", "\r")
            .Replace("\\t", "\t");

        return result;
    }

    private static void SkipValue(string json, ref int i)
    {
        SkipWhitespace(json, ref i);
        if (i >= json.Length)
            return;

        var c = json[i];
        if (c == '"')
        {
            ReadString(json, ref i);
        }
        else if (c == '{')
        {
            SkipObject(json, ref i);
        }
        else if (c == '[')
        {
            SkipArray(json, ref i);
        }
        else
        {
            // Skip number, true, false, null
            while (i < json.Length && !char.IsWhiteSpace(json[i]) && json[i] != ',' && json[i] != '}' && json[i] != ']')
                i++;
        }
    }

    private static void SkipObject(string json, ref int i)
    {
        if (i >= json.Length || json[i] != '{')
            return;

        var depth = 1;
        i++;

        while (i < json.Length && depth > 0)
        {
            if (json[i] == '{')
                depth++;
            else if (json[i] == '}')
                depth--;
            else if (json[i] == '"')
            {
                ReadString(json, ref i);
                i--; // ReadString advances past string, but loop will increment
            }

            i++;
        }
    }

    private static void SkipArray(string json, ref int i)
    {
        if (i >= json.Length || json[i] != '[')
            return;

        var depth = 1;
        i++;

        while (i < json.Length && depth > 0)
        {
            if (json[i] == '[')
                depth++;
            else if (json[i] == ']')
                depth--;
            else if (json[i] == '"')
            {
                ReadString(json, ref i);
                i--; // ReadString advances past string, but loop will increment
            }

            i++;
        }
    }
}
