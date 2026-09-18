// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using JPSoftworks.DevNumbers.Helpers;

namespace JPSoftworks.DevNumbers.Test;

public class BigIntegerCharacterInferenceTest
{
    [Theory]
    [InlineData(0, "\0", "NUL (Null)")]
    [InlineData(9, "\t", "HT (Horizontal Tab)")]
    [InlineData(10, "\n", "LF (Line Feed)")]
    [InlineData(65, "A", "A")]
    [InlineData(127, "\u007F", "DEL (Delete)")]
    [InlineData(160, "\u00A0", "NBSP (Non-Breaking Space)")]
    [InlineData(233, "\u00E9", "\u00E9")]
    [InlineData(0x1F600, "\U0001F600", "\U0001F600")]
    public void GetValidCharacterInterpretations_CodePoint_PreservesRawCharacter(int value, string expected, string expectedDisplay)
    {
        var result = Assert.Single(BigIntegerCharacterInference.GetValidCharacterInterpretations(value));

        Assert.Equal(expected, result.Character);
        Assert.Equal(expectedDisplay, result.DisplayCharacter);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0, "\0")]
    [InlineData(10, "\n")]
    [InlineData(160, "\u00A0")]
    [InlineData(233, "\u00E9")]
    [InlineData(0x1F600, "\U0001F600")]
    public void TryParseAsUnicode_CodePoint_PreservesRawCharacter(int value, string expected)
    {
        Assert.Equal(expected, BigIntegerCharacterInference.TryParseAsUtf16(value)?.Character);
        Assert.Equal(expected, BigIntegerCharacterInference.TryParseAsUtf32(value)?.Character);
    }

    [Theory]
    [InlineData(0x4142, "AB", "AB")]
    [InlineData(0x410042, "A\0B", "A\\0B")]
    [InlineData(0x410A42, "A\nB", "A\\x0AB")]
    public void TryParseAsAsciiString_ByteSequence_PreservesRawText(int value, string expected, string expectedDisplay)
    {
        var result = BigIntegerCharacterInference.TryParseAsAsciiString(value);

        Assert.NotNull(result);
        Assert.Equal(expected, result.Character);
        Assert.Equal(expectedDisplay, result.DisplayCharacter);
    }

    [Theory]
    [InlineData(0L, "\0", "0x00")]
    [InlineData(0xC3A9L, "\u00E9", "0xC3 0xA9")]
    [InlineData(0xE282ACL, "\u20AC", "0xE2 0x82 0xAC")]
    [InlineData(0xF09F9880L, "\U0001F600", "0xF0 0x9F 0x98 0x80")]
    [InlineData(0xC3A900L, "\u00E9\0", "0xC3 0xA9 0x00")]
    [InlineData(0x41C3A9L, "A\u00E9", "0x41 0xC3 0xA9")]
    public void TryParseAsUtf8_ByteSequence_DecodesInWrittenOrder(long value, string expected, string expectedHex)
    {
        var result = BigIntegerCharacterInference.TryParseAsUtf8(value);

        Assert.NotNull(result);
        Assert.Equal(expected, result.Character);
        Assert.Equal(expectedHex, result.HexRepresentation);
        Assert.Contains(BigIntegerCharacterInference.GetValidCharacterInterpretations(value), r => r.Character == expected);
    }

    [Theory]
    [InlineData(-1L)]
    [InlineData(0xC3L)]
    [InlineData(0xA9C3L)]
    [InlineData(0xC0AFL)]
    [InlineData(0xEDA080L)]
    [InlineData(0xF4908080L)]
    public void TryParseAsUtf8_InvalidByteSequence_ReturnsNull(long value)
    {
        Assert.Null(BigIntegerCharacterInference.TryParseAsUtf8(value));
    }
}