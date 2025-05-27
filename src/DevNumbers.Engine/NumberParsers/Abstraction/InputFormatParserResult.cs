using System.Numerics;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

/// <summary>
/// Result of parsing an input string as a number
/// </summary>
public record InputFormatParserResult
{
    /// <summary>
    /// The original input string
    /// </summary>
    public string RawValue { get; init; }

    /// <summary>
    /// The detected format of the input
    /// </summary>
    public NumberBase NumberBase { get; init; }

    /// <summary>
    /// The parsed numeric value
    /// </summary>
    public BigInteger Value { get; init; }

    /// <summary>
    /// Indicates whether the format was implicitly determined and could potentially be interpreted differently
    /// </summary>
    public bool IsImplicit { get; init; }

    /// <summary>
    /// Details about the formatting style used in the input
    /// </summary>
    public FormatInfo FormatInfo { get; init; }

    public InputFormatParserResult(string rawValue, NumberBase numberBase, BigInteger value, bool isImplicit, FormatInfo formatInfo)
    {
        this.RawValue = rawValue;
        this.NumberBase = numberBase;
        this.Value = value;
        this.IsImplicit = isImplicit;
        this.FormatInfo = formatInfo;
    }
}