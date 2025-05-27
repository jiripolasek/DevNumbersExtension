// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Text;
using System.Text.RegularExpressions;

namespace JPSoftworks.DevNumbers.Helpers;

internal static class SwitchParser
{
    private static readonly SwitchDefinition[] Switches =
    [
        new("length", true),
        new("signed", false),
        new("unsigned", false)
    ];

    private static readonly Dictionary<string, int> LengthAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["byte"] = 8,
        ["word"] = 16,
        ["dword"] = 32,
        ["qword"] = 64,
        ["int8"] = 8,
        ["int16"] = 16,
        ["int32"] = 32,
        ["int64"] = 64,
        ["short"] = 16,
        ["int"] = 32,
        ["long"] = 64
    };

    // More precise regex with better boundary detection
    private static readonly Regex SwitchRegex = new(@"/(?<name>[^\s/:]*?)(?::(?<value>[^\s/]*))?(?=\s|$|/)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Pre-compiled regex for whitespace cleanup
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    public static ParseResult Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new ParseResult(string.Empty, new Options(), []);
        }

        var options = new Options();
        var errors = new List<ParseError>();
        var matches = SwitchRegex.Matches(input);

        if (matches.Count == 0)
        {
            return new ParseResult(input.Trim(), options, errors);
        }

        // More efficient string building using StringBuilder with ranges
        var queryBuilder = new StringBuilder(input.Length);
        var lastIndex = 0;

        foreach (Match match in matches)
        {
            // Add text before this switch to the query
            if (match.Index > lastIndex)
            {
                queryBuilder.Append(input.AsSpan(lastIndex, match.Index - lastIndex));
            }

            lastIndex = match.Index + match.Length;

            // Process the switch and collect any errors
            ProcessSwitchMatch(match, options, errors);
        }

        // Add any remaining text after the last switch
        if (lastIndex < input.Length)
        {
            queryBuilder.Append(input.AsSpan(lastIndex));
        }

        // Clean up whitespace more efficiently
        var query = WhitespaceRegex.Replace(queryBuilder.ToString(), " ").Trim();

        return new ParseResult(query, options, errors);
    }

    private static void ProcessSwitchMatch(Match match, Options options, List<ParseError> errors)
    {
        var switchName = match.Groups["name"].Value;
        var switchValue = match.Groups["value"].Value;

        // Handle missing switch name (just "/" or "/:value")
        if (string.IsNullOrEmpty(switchName))
        {
            var errorMessage = string.IsNullOrEmpty(switchValue)
                ? "Missing switch name after '/'"
                : $"Missing switch name in '/{(string.IsNullOrEmpty(switchValue) ? "" : ":" + switchValue)}'";

            errors.Add(new ParseError(ParseErrorType.MissingSwitchName, errorMessage, match.Index, match.Length));
            return;
        }

        // Check for invalid characters in switch name
        if (!IsValidSwitchName(switchName))
        {
            errors.Add(new ParseError(
                ParseErrorType.InvalidSwitchName,
                $"Invalid switch name '/{switchName}' - switch names should contain only letters, numbers, and hyphens",
                match.Index,
                match.Length));
            return;
        }

        // Find the switch definition (with fuzzy matching for better UX)
        var switchDef = FindSwitchDefinition(switchName, out var suggestion);

        if (switchDef == null)
        {
            var errorMessage = suggestion != null
                ? $"Unknown switch '/{switchName}' - did you mean '/{suggestion}'?"
                : $"Unknown switch '/{switchName}'. Available switches: {string.Join(", ", Switches.Select(s => s.Name))}";

            errors.Add(new ParseError(ParseErrorType.UnknownSwitch, errorMessage, match.Index, match.Length));
            return;
        }

        // Validate switch format
        if (switchDef.HasArgument && string.IsNullOrEmpty(switchValue))
        {
            var validValues = switchDef.Name == "length"
                ? $"Expected values: positive integer or {string.Join(", ", LengthAliases.Keys.Take(5))}..."
                : "Expected: value required";

            errors.Add(new ParseError(
                ParseErrorType.MissingArgument,
                $"Switch '/{switchName}' requires a value. {validValues}",
                match.Index,
                match.Length));
            return;
        }

        if (!switchDef.HasArgument && !string.IsNullOrEmpty(switchValue))
        {
            errors.Add(new ParseError(
                ParseErrorType.UnexpectedArgument,
                $"Switch '/{switchName}' does not accept a value",
                match.Index,
                match.Length));
            return;
        }

        // Try to process the switch
        var processingError = ProcessSwitch(options, switchDef.Name, switchValue);
        if (processingError != null)
        {
            errors.Add(new ParseError(ParseErrorType.InvalidValue, processingError, match.Index, match.Length));
        }
    }

    private static SwitchDefinition? FindSwitchDefinition(string switchName, out string? suggestion)
    {
        suggestion = null;

        // Exact match first
        var exactMatch = Switches.FirstOrDefault(s =>
            string.Equals(s.Name, switchName, StringComparison.OrdinalIgnoreCase));

        if (exactMatch != null)
        {
            return exactMatch;
        }

        // Fuzzy matching for common typos
        suggestion = FindClosestSwitch(switchName);
        return null;
    }

    private static string? FindClosestSwitch(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        var bestMatch = string.Empty;
        var bestScore = int.MaxValue;

        foreach (var switchDef in Switches)
        {
            var score = CalculateLevenshteinDistance(input.ToLowerInvariant(), switchDef.Name.ToLowerInvariant());

            // Only suggest if it's a reasonable match (within 2 character differences for reasonable length)
            if (score < bestScore && score <= Math.Max(1, Math.Min(input.Length, switchDef.Name.Length) / 2))
            {
                bestScore = score;
                bestMatch = switchDef.Name;
            }
        }

        return string.IsNullOrEmpty(bestMatch) ? null : bestMatch;
    }

    private static int CalculateLevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source))
        {
            return target?.Length ?? 0;
        }

        if (string.IsNullOrEmpty(target))
        {
            return source.Length;
        }

        var matrix = new int[source.Length + 1, target.Length + 1];

        for (var i = 0; i <= source.Length; i++)
        {
            matrix[i, 0] = i;
        }

        for (var j = 0; j <= target.Length; j++)
        {
            matrix[0, j] = j;
        }

        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                var cost = source[i - 1] == target[j - 1] ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[source.Length, target.Length];
    }

    private static string? ProcessSwitch(Options options, string switchName, string switchValue)
    {
        try
        {
            switch (switchName.ToLowerInvariant())
            {
                case "length":
                    var bitLength = ParseBitLength(switchValue);
                    if (bitLength.HasValue)
                    {
                        options.BitLength = bitLength.Value;
                        return null; // Success
                    }

                    var suggestions = string.Join(", ", LengthAliases.Keys.Take(6));
                    return
                        $"Invalid bit length '{switchValue}'. Expected: positive integer or one of [{suggestions}...]";

                case "signed":
                    options.Signed = true;
                    return null;

                case "unsigned":
                    options.Signed = false;
                    return null;

                default:
                    return $"Unknown switch: '{switchName}'";
            }
        }
        catch (Exception ex)
        {
            return $"Error processing switch: {ex.Message}";
        }
    }

    private static int? ParseBitLength(string value)
    {
        if (int.TryParse(value, out var numericValue))
        {
            return numericValue > 0 ? numericValue : null;
        }

        if (LengthAliases.TryGetValue(value, out var aliasValue))
        {
            return aliasValue;
        }

        return null; // Invalid value
    }

    private static bool IsValidSwitchName(string name)
    {
        // Allow letters, numbers, and hyphens/underscores in switch names
        return !string.IsNullOrEmpty(name) &&
               name.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
    }

    private record SwitchDefinition(string Name, bool HasArgument);
}

internal enum ParseErrorType
{
    UnknownSwitch,
    MissingArgument,
    UnexpectedArgument,
    InvalidValue,
    MissingSwitchName,
    InvalidSwitchName
}

internal record ParseError(
    ParseErrorType Type,
    string Message,
    int Position,
    int Length)
{
    public override string ToString() => $"{this.Type}: {this.Message} (at position {this.Position})";
}

internal record ParseResult(
    string Query,
    Options Options,
    IReadOnlyList<ParseError> Errors)
{
    public bool HasErrors => this.Errors.Count > 0;

    public bool IsValid => !this.HasErrors;

    public override string ToString()
    {
        var result = $"Query: '{this.Query}', {this.Options}";
        if (this.HasErrors)
        {
            result += $", Errors: [{string.Join("; ", this.Errors.Select(e => e.Message))}]";
        }

        return result;
    }
}

internal record Options
{
    public int BitLength { get; set; } = -1;
    public bool? Signed { get; set; }

    public override string ToString()
    {
        var parts = new List<string>();

        if (this.BitLength > 0)
        {
            parts.Add($"BitLength={this.BitLength}");
        }

        if (this.Signed.HasValue)
        {
            parts.Add($"Signed={this.Signed.Value}");
        }

        return parts.Count > 0 ? $"Options({string.Join(", ", parts)})" : "Options()";
    }
}