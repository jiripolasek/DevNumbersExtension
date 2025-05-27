namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

/// <summary>
/// Represents the formatting style used in the input
/// </summary>
public enum FormatStyle
{
    /// <summary>
    /// Standard format with no prefix or suffix
    /// </summary>
    Standard,

    /// <summary>
    /// C-style prefix (0x, 0b, 0)
    /// </summary>
    CStylePrefix,

    /// <summary>
    /// C++ style prefix (0x, 0b, 0) with apostrophe separator
    /// </summary>
    CppStyle,

    /// <summary>
    /// C# style prefix (0x, 0b, 0) with underscore (_) separator
    /// </summary>
    CSharpStyle,

    /// <summary>
    /// Single character suffix (h, o)
    /// </summary>
    SingleCharSuffix,

    /// <summary>
    /// Multi-character suffix (bin, oct, binary)
    /// </summary>
    MultiCharSuffix,

    /// <summary>
    /// Single character prefix (h)
    /// </summary>
    SingleCharPrefix,

    /// <summary>
    /// Special character prefix (#, $, %)
    /// </summary>
    SpecialCharPrefix,

    /// <summary>
    /// Visual Basic style prefix (&amp;H, &amp;B, &amp;O)
    /// </summary>
    VBStylePrefix,

    /// <summary>
    /// Ada-style base prefix (16#, 2#, 8#)
    /// </summary>
    AdaStylePrefix,

    /// <summary>
    /// R-style base prefix (16r, 2r, 8r)
    /// </summary>
    RStylePrefix,

    /// <summary>
    /// Character literal ('a')
    /// </summary>
    CharLiteral
}