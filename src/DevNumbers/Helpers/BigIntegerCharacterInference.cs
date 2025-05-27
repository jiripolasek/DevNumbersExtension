// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Globalization;
using System.Numerics;
using System.Text;

namespace JPSoftworks.DevNumbers.Helpers;

/// <summary>
/// Utility class for inferring character representations from BigInteger values
/// across different encodings (ASCII, UTF-8, UTF-16, UTF-32)
/// </summary>
internal static class BigIntegerCharacterInference
{
    /// <summary>
    /// Represents a possible character interpretation of a BigInteger
    /// </summary>
    public class CharacterResult
    {
        /// <summary>
        /// The encoding method used for interpretation
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// The resulting character or string
        /// </summary>
        public string Character { get; set; }

        /// <summary>
        /// Whether this interpretation is valid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Hexadecimal representation of the raw data
        /// </summary>
        public string HexRepresentation { get; set; }

        /// <summary>
        /// String representation of the result
        /// </summary>
        /// <returns>Formatted string showing encoding, character, and hex representation</returns>
        public override string ToString()
        {
            return $"{this.Encoding}: '{this.Character}' ({this.HexRepresentation})";
        }
    }



    /// <summary>
    /// Gets all valid character interpretations for a BigInteger value
    /// </summary>
    /// <param name="value">The BigInteger to analyze</param>
    /// <returns>List of valid character interpretations</returns>
    public static List<CharacterResult> GetValidCharacterInterpretations(BigInteger value)
    {
        var results = new List<CharacterResult>();

        if (value >= 0 && value <= 127)
        {
            var asciiResult = TryParseAsAscii(value);
            if (asciiResult?.IsValid == true)
            {
                asciiResult.Encoding = "ASCII/UTF-8 (char)";
                results.Add(asciiResult);
            }
            return results;
        }
        else
        {
            var utf8Result = TryParseAsUtf8(value);
            if (utf8Result?.IsValid == true)
            {
                results.Add(utf8Result);
            }

            var asciiStringResult = TryParseAsAsciiString(value);
            if (asciiStringResult?.IsValid == true)
            {
                results.Add(asciiStringResult);
            }
        }

        // Try UTF-16 and UTF-32 for valid Unicode range (as code points)
        var utf16Result = TryParseAsUtf16(value);
        var utf32Result = TryParseAsUtf32(value);

        // If both UTF-16 and UTF-32 are valid and represent the same character, combine them
        if (utf16Result?.IsValid == true && utf32Result?.IsValid == true &&
            utf16Result.Character == utf32Result.Character)
        {
            var combinedResult = new CharacterResult
            {
                Encoding = "UTF-16/UTF-32 (code point)",
                Character = utf16Result.Character,
                IsValid = true,
                HexRepresentation = utf32Result.HexRepresentation // Use UTF-32's more complete hex format
            };
            results.Add(combinedResult);
        }
        else
        {
            // Add them separately if they're different or only one is valid
            if (utf16Result?.IsValid == true)
                results.Add(utf16Result);

            if (utf32Result?.IsValid == true)
                results.Add(utf32Result);
        }

        return results;
    }

    /// <summary>
    /// Tries to interpret BigInteger as ASCII string (sequence of ASCII bytes)
    /// Only returns results for multi-byte sequences
    /// </summary>
    /// <param name="value">The BigInteger value</param>
    /// <returns>ASCII string interpretation or null if invalid or single byte</returns>
    public static CharacterResult? TryParseAsAsciiString(BigInteger value)
    {
        try
        {
            if (value <= 0)
                return null;

            byte[] bytes = value.ToByteArray();

            if (bytes.Length > 1 && bytes[^1] == 0)
                bytes = [.. bytes.Take(bytes.Length - 1)];

            Array.Reverse(bytes);

            if (bytes.Length <= 1)
                return null;

            if (bytes.Any(static b => b > 127))
                return null;

            var stringBuilder = new StringBuilder();
            var hexBytes = new List<string>();

            foreach (byte b in bytes)
            {
                hexBytes.Add($"0x{b:X2}");

                switch (b)
                {
                    case >= 32 and <= 126:
                        stringBuilder.Append((char)b);
                        break;
                    case 0:
                        stringBuilder.Append("\\0");
                        break;
                    default:
                        stringBuilder.Append(CultureInfo.InvariantCulture, $"\\x{b:X2}");
                        break;
                }
            }

            return new CharacterResult
            {
                Encoding = "ASCII (string)",
                Character = stringBuilder.ToString(),
                IsValid = true,
                HexRepresentation = string.Join(" ", hexBytes)
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Tries to interpret BigInteger as single ASCII character
    /// </summary>
    /// <param name="value">The BigInteger value</param>
    /// <returns>ASCII character interpretation or null if invalid</returns>
    public static CharacterResult? TryParseAsAscii(BigInteger value)
    {
        if (value < 0 || value > 127)
            return null;

        byte asciiValue = (byte)value;
        string character = asciiValue is >= 32 and <= 126
            ? ((char)asciiValue).ToString()
            : GetControlCharacterName(asciiValue);

        return new CharacterResult
        {
            Encoding = "ASCII",
            Character = character,
            IsValid = true,
            HexRepresentation = $"0x{asciiValue:X2}"
        };
    }

    /// <summary>
    /// Tries to interpret BigInteger as UTF-8 encoded byte sequence
    /// </summary>
    /// <param name="value">The BigInteger value</param>
    /// <returns>UTF-8 character interpretation or null if invalid</returns>
    public static CharacterResult? TryParseAsUtf8(BigInteger value)
    {
        try
        {
            // Convert BigInteger to byte array (little-endian by default)
            byte[] bytes = value.ToByteArray();

            // Remove padding zeros and reverse for big-endian interpretation
            bytes.Reverse();
            bytes = [.. bytes.SkipWhile(static b => b == 0).Reverse()];

            if (bytes.Length == 0)
                bytes = [0];

            // Try to decode as UTF-8
            string decoded = Encoding.UTF8.GetString(bytes);

            // Validate - re-encode and compare
            byte[] reencoded = Encoding.UTF8.GetBytes(decoded);
            if (!bytes.SequenceEqual(reencoded))
                return null;

            return new CharacterResult
            {
                Encoding = "UTF-8 (bytes)",
                Character = decoded,
                IsValid = true,
                HexRepresentation = string.Join(" ", bytes.Select(static b => $"0x{b:X2}"))
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Tries to interpret BigInteger as UTF-16 Unicode code point
    /// </summary>
    /// <param name="value">The BigInteger value</param>
    /// <returns>UTF-16 character interpretation or null if invalid</returns>
    public static CharacterResult? TryParseAsUtf16(BigInteger value)
    {
        try
        {
            if (value < 0 || value > 0x10FFFF)
                return null;

            int codePoint = (int)value;

            if (codePoint is >= 0xD800 and <= 0xDFFF)
                return null; // Invalid surrogate code point used alone

            string character = codePoint <= 0xFFFF
                ? ((char)codePoint).ToString()
                : char.ConvertFromUtf32(codePoint);

            string displayCharacter = GetDisplayCharacter(character, codePoint);

            return new CharacterResult
            {
                Encoding = "UTF-16 (code point)",
                Character = displayCharacter,
                IsValid = true,
                HexRepresentation = $"U+{codePoint:X4}"
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Tries to interpret BigInteger as UTF-32 Unicode code point
    /// </summary>
    /// <param name="value">The BigInteger value</param>
    /// <returns>UTF-32 character interpretation or null if invalid</returns>
    public static CharacterResult? TryParseAsUtf32(BigInteger value)
    {
        try
        {
            if (value < 0 || value > 0x10FFFF)
                return null;

            int codePoint = (int)value;

            // Check for invalid surrogate code points
            if (codePoint is >= 0xD800 and <= 0xDFFF)
                return null;

            string character = char.ConvertFromUtf32(codePoint);
            string displayCharacter = GetDisplayCharacter(character, codePoint);

            return new CharacterResult
            {
                Encoding = "UTF-32 (code point)",
                Character = displayCharacter,
                IsValid = true,
                HexRepresentation = $"U+{codePoint:X6}"
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets a display-friendly representation of a Unicode character
    /// </summary>
    /// <param name="character">The Unicode character</param>
    /// <param name="codePoint">The Unicode code point</param>
    /// <returns>Display-friendly character representation</returns>
    private static string GetDisplayCharacter(string character, int codePoint)
    {
        if (string.IsNullOrEmpty(character))
            return $"U+{codePoint:X4}";

        char firstChar = character[0];

        // Handle common non-printable ranges
        if (codePoint <= 0x1F)
        {
            // C0 control characters (0x00-0x1F)
            return GetControlCharacterName((byte)codePoint);
        }
        else if (codePoint is >= 0x7F and <= 0x9F)
        {
            // C1 control characters (0x7F-0x9F)
            return $"C1 Control (U+{codePoint:X4})";
        }
        else if (char.IsControl(firstChar))
        {
            // Other control characters
            return $"Control Character (U+{codePoint:X4})";
        }
        else if (char.IsWhiteSpace(firstChar) && !char.IsLetterOrDigit(firstChar) && firstChar != ' ')
        {
            // Special whitespace characters
            return GetWhitespaceCharacterName(codePoint);
        }
        else if (codePoint is >= 0xFFF0 and <= 0xFFFF)
        {
            // Specials block
            return $"Special Character (U+{codePoint:X4})";
        }

        // For printable characters, return as-is
        return character;
    }

    /// <summary>
    /// Gets names for special whitespace characters
    /// </summary>
    /// <param name="codePoint">Unicode code point</param>
    /// <returns>Whitespace character name</returns>
    private static string GetWhitespaceCharacterName(int codePoint)
    {
        return codePoint switch
        {
            0x00A0 => "NBSP (Non-Breaking Space)",
            0x1680 => "Ogham Space Mark",
            0x2000 => "En Quad",
            0x2001 => "Em Quad",
            0x2002 => "En Space",
            0x2003 => "Em Space",
            0x2004 => "Three-Per-Em Space",
            0x2005 => "Four-Per-Em Space",
            0x2006 => "Six-Per-Em Space",
            0x2007 => "Figure Space",
            0x2008 => "Punctuation Space",
            0x2009 => "Thin Space",
            0x200A => "Hair Space",
            0x200B => "Zero Width Space",
            0x200C => "Zero Width Non-Joiner",
            0x200D => "Zero Width Joiner",
            0x2028 => "Line Separator",
            0x2029 => "Paragraph Separator",
            0x202F => "Narrow No-Break Space",
            0x205F => "Medium Mathematical Space",
            0x3000 => "Ideographic Space",
            0xFEFF => "Zero Width No-Break Space",
            _ => $"Whitespace (U+{codePoint:X4})"
        };
    }

    /// <summary>
    /// Gets the name of ASCII control characters
    /// </summary>
    /// <param name="value">ASCII control character value</param>
    /// <returns>Control character name</returns>
    private static string GetControlCharacterName(byte value)
    {
        return value switch
        {
            0 => "NUL (Null)",
            1 => "SOH (Start of Heading)",
            2 => "STX (Start of Text)",
            3 => "ETX (End of Text)",
            4 => "EOT (End of Transmission)",
            5 => "ENQ (Enquiry)",
            6 => "ACK (Acknowledge)",
            7 => "BEL (Bell)",
            8 => "BS (Backspace)",
            9 => "HT (Horizontal Tab)",
            10 => "LF (Line Feed)",
            11 => "VT (Vertical Tab)",
            12 => "FF (Form Feed)",
            13 => "CR (Carriage Return)",
            14 => "SO (Shift Out)",
            15 => "SI (Shift In)",
            16 => "DLE (Data Link Escape)",
            17 => "DC1 (Device Control 1)",
            18 => "DC2 (Device Control 2)",
            19 => "DC3 (Device Control 3)",
            20 => "DC4 (Device Control 4)",
            21 => "NAK (Negative Acknowledge)",
            22 => "SYN (Synchronous Idle)",
            23 => "ETB (End of Transmission Block)",
            24 => "CAN (Cancel)",
            25 => "EM (End of Medium)",
            26 => "SUB (Substitute)",
            27 => "ESC (Escape)",
            28 => "FS (File Separator)",
            29 => "GS (Group Separator)",
            30 => "RS (Record Separator)",
            31 => "US (Unit Separator)",
            127 => "DEL (Delete)",
            _ => $"Control character (0x{value:X2})"
        };
    }
}
