namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

/// <summary>
/// Represents the formatting details of the input
/// </summary>
public record FormatInfo
{
    /// <summary>
    /// The numeric format (decimal, hex, etc.)
    /// </summary>
    public NumberBase Format { get; init; }

    /// <summary>
    /// The style of formatting used in the input
    /// </summary>
    public FormatStyle Style { get; init; }

    /// <summary>
    /// The case style used for hexadecimal digits (upper or lower)
    /// </summary>
    public bool IsUpperCase { get; init; }

    public FormatInfo(FormatStyle style, bool isUpperCase = true)
    {
        this.Style = style;
        this.IsUpperCase = isUpperCase;
    }
}