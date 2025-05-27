using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine;

/// <summary>
/// Configuration for numeric base conversion and formatting
/// </summary>
public sealed record BigIntegerBaseStyle
{
    /// <summary>
    /// The numeric base (radix). Supported: 2, 8, 10, 16.
    /// </summary>
    public int Radix { get; init; } = 10;

    /// <summary>
    /// If true, allow underscores as group separators in input and output.
    /// </summary>
    public bool AllowGroupSeparator { get; init; } = true;

    /// <summary>
    /// If nonzero, group digits by this many digits when formatting.
    /// Zero disables grouping.
    /// </summary>
    public int GroupSize { get; init; }

    /// <summary>
    /// Character used for grouping separators in output.
    /// </summary>
    public char GroupSeparator { get; init; } = ' ';

    /// <summary>
    /// If true, output uppercase letters for hexadecimal formatting.
    /// </summary>
    public bool UpperCase { get; init; }

    /// <summary>
    /// If set, output as two's complement of this many bits for non-decimal bases. If null, will be calculated automatically.
    /// </summary>
    public int? SizeHintBits { get; init; }

    /// <summary>
    /// Default style: base 10, group separators allowed, no grouping, lowercase
    /// </summary>
    public static BigIntegerBaseStyle Default { get; } = new();

    /// <summary>
    /// Binary style: base 2, group separators allowed, groups of 4, lowercase
    /// </summary>
    public static BigIntegerBaseStyle Binary { get; } = new() { Radix = 2, GroupSize = 4 };

    /// <summary>
    /// Octal style: base 8, group separators allowed, groups of 3, lowercase
    /// </summary>
    public static BigIntegerBaseStyle Octal { get; } = new() { Radix = 8, GroupSize = 3 };

    /// <summary>
    /// Decimal style: base 10, group separators allowed, groups of 3, lowercase
    /// </summary>
    public static BigIntegerBaseStyle Decimal { get; } = new() { Radix = 10, GroupSize = 3 };

    /// <summary>
    /// Hexadecimal style: base 16, group separators allowed, groups of 4, lowercase
    /// </summary>
    public static BigIntegerBaseStyle Hexadecimal { get; } = new() { Radix = 16, GroupSize = 4 };

    /// <summary>
    /// Hexadecimal style: base 16, group separators allowed, groups of 4, uppercase
    /// </summary>
    public static BigIntegerBaseStyle HexadecimalUppercase { get; } = new() { Radix = 16, GroupSize = 4, UpperCase = true };

    /// <summary>
    /// Create a base style from format info
    /// </summary>
    public static BigIntegerBaseStyle FromFormatInfo(NumberBase format, FormatInfo formatInfo)
    {
        return new BigIntegerBaseStyle
        {
            Radix = format switch
            {
                NumberBase.Binary => 2,
                NumberBase.Octal => 8,
                NumberBase.Decimal => 10,
                NumberBase.Hexadecimal => 16,
                _ => 10
            },
            GroupSize = GetDefaultGroupSize(format),
            UpperCase = formatInfo.IsUpperCase
        };
    }

    /// <summary>
    /// Gets the default group size for a format
    /// </summary>
    private static int GetDefaultGroupSize(NumberBase format) => format switch
    {
        NumberBase.Binary => 4,
        NumberBase.Octal => 3,
        NumberBase.Decimal => 3,
        NumberBase.Hexadecimal => 4,
        _ => 0
    };
}
