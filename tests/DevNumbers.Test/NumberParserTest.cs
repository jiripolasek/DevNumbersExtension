using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Test
{
    /// <summary>
    /// Tests for the NumberParser class with different number formats
    /// </summary>
    public class NumberParserTest
    {
        [Theory]
        [MemberData(nameof(GetNumberFormatTestData))]
        public void Parse_InputString_ReturnsCorrectValue(
            string input, 
            NumberBase expectedBase, 
            BigInteger expectedValue, 
            FormatStyle expectedStyle)
        {
            // Act
            var result = NumberParser.Parse(input);

            // Assert
            Assert.Equal(expectedBase, result.NumberBase);
            Assert.Equal(expectedValue, result.Value);
            Assert.Equal(expectedStyle, result.FormatInfo.Style);
        }

        [Theory]
        [MemberData(nameof(GetBinaryFormatsTestData))]
        public void Parse_BinaryFormats_ReturnsCorrectValue(string input, BigInteger expectedValue, FormatStyle expectedStyle)
        {
            // Act
            var result = NumberParser.Parse(input);

            // Assert
            Assert.Equal(NumberBase.Binary, result.NumberBase);
            Assert.Equal(expectedValue, result.Value);
            Assert.Equal(expectedStyle, result.FormatInfo.Style);
        }

        [Theory]
        [MemberData(nameof(GetHexadecimalFormatsTestData))]
        public void Parse_HexadecimalFormats_ReturnsCorrectValue(string input, BigInteger expectedValue, FormatStyle expectedStyle)
        {
            // Act
            var result = NumberParser.Parse(input);

            // Assert
            Assert.Equal(NumberBase.Hexadecimal, result.NumberBase);
            Assert.Equal(expectedValue, result.Value);
            Assert.Equal(expectedStyle, result.FormatInfo.Style);
        }

        [Theory]
        [MemberData(nameof(GetOctalFormatsTestData))]
        public void Parse_OctalFormats_ReturnsCorrectValue(string input, BigInteger expectedValue, FormatStyle expectedStyle)
        {
            // Act
            var result = NumberParser.Parse(input);

            // Assert
            Assert.Equal(NumberBase.Octal, result.NumberBase);
            Assert.Equal(expectedValue, result.Value);
            Assert.Equal(expectedStyle, result.FormatInfo.Style);
        }

        [Theory]
        [MemberData(nameof(GetDecimalFormatsTestData))]
        public void Parse_DecimalFormats_ReturnsCorrectValue(string input, BigInteger expectedValue, FormatStyle expectedStyle)
        {
            // Act
            var result = NumberParser.Parse(input);

            // Assert
            Assert.Equal(NumberBase.Decimal, result.NumberBase);
            Assert.Equal(expectedValue, result.Value);
            Assert.Equal(expectedStyle, result.FormatInfo.Style);
        }

        [Theory]
        [MemberData(nameof(GetCharacterFormatsTestData))]
        public void Parse_CharacterFormats_ReturnsCorrectValue(string input, BigInteger expectedValue)
        {
            // Act
            var result = NumberParser.Parse(input);

            // Assert
            Assert.Equal(NumberBase.Char, result.NumberBase);
            Assert.Equal(expectedValue, result.Value);
            Assert.Equal(FormatStyle.CharLiteral, result.FormatInfo.Style);
        }

        [Theory]
        [InlineData(null!)]
        [InlineData("")]
        [InlineData("   ")]
        public void Parse_EmptyOrWhitespaceInput_ThrowsArgumentException(string input)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => NumberParser.Parse(input));
        }

        [Theory]
        [InlineData("_____")]
        [InlineData("   _____   ")]
        public void Parse_EmptyOrWhitespaceInput_ThrowsFormatException(string input)
        {
            // Act & Assert
            Assert.Throws<FormatException>(() => NumberParser.Parse(input));
        }

        [Theory]
        [InlineData("not a number")]
        [InlineData("text123")]
        [InlineData("abc!@#")]
        [InlineData("0b102")]
        [InlineData("0B102")]
        [InlineData("0b")]
        [InlineData("0b10dec")]
        [InlineData("0x_")]
        [InlineData("0b_")]
        [InlineData("0x'")]
        [InlineData("-")]
        [InlineData("-_")]
        [InlineData("- \t_")]
        [InlineData("hex")]
        [InlineData("hexadecimal")]
        [InlineData("bin")]
        [InlineData("binary")]
        [InlineData("oct")]
        [InlineData("octal")]
        [InlineData("decimal")]
        [InlineData("16#FF#junk")]
        [InlineData("2#10#junk")]
        [InlineData("8#77#junk")]
        [InlineData("10#42#junk")]
        [InlineData("16#FF##")]
        [InlineData("'\\nXYZ'")]
        [InlineData("'\\0XYZ'")]
        [InlineData("'\\\\XYZ'")]
        [InlineData("'\\u0041XYZ'")]
        [InlineData("'\\u00410'")]
        [InlineData("'\\U00000041XYZ'")]
        [InlineData("'\\U000000410'")]
        [InlineData("'\\x41XYZ'")]
        public void Parse_InvalidInput_ThrowsFormatException(string input)
        {
            // Act & Assert
            Assert.Throws<FormatException>(() => NumberParser.Parse(input));
        }

        public static IEnumerable<object[]> GetNumberFormatTestData()
        {
            // Mix of different format types for general testing
            yield return ["42", NumberBase.Decimal, new BigInteger(42), FormatStyle.Standard];
            yield return ["0xFF", NumberBase.Hexadecimal, new BigInteger(255), FormatStyle.CSharpStyle];
            yield return ["0b1010", NumberBase.Binary, new BigInteger(10), FormatStyle.CSharpStyle];
            yield return ["052", NumberBase.Octal, new BigInteger(42), FormatStyle.CSharpStyle];
            yield return ["'A'", NumberBase.Char, new BigInteger(65), FormatStyle.CharLiteral];
            yield return ["0", NumberBase.Decimal, BigInteger.Zero, FormatStyle.Standard];
            yield return ["-0", NumberBase.Decimal, BigInteger.Zero, FormatStyle.Standard];
            yield return ["0x_0", NumberBase.Hexadecimal, BigInteger.Zero, FormatStyle.CSharpStyle];
            yield return ["0b_0", NumberBase.Binary, BigInteger.Zero, FormatStyle.CSharpStyle];
            yield return ["0hex", NumberBase.Hexadecimal, BigInteger.Zero, FormatStyle.MultiCharSuffix];
            yield return ["0bin", NumberBase.Binary, BigInteger.Zero, FormatStyle.MultiCharSuffix];
            yield return ["FF", NumberBase.Hexadecimal, new BigInteger(255), FormatStyle.Standard];
        }

        public static IEnumerable<object[]> GetBinaryFormatsTestData()
        {
            // Binary format testing with all supported styles
            yield return ["0b1010", new BigInteger(10), FormatStyle.CSharpStyle];
            yield return ["0B1010", new BigInteger(10), FormatStyle.CSharpStyle];
            yield return ["0b10'10", new BigInteger(10), FormatStyle.CppStyle];
            yield return ["1010bin", new BigInteger(10), FormatStyle.MultiCharSuffix];
            yield return ["1010binary", new BigInteger(10), FormatStyle.MultiCharSuffix];
            yield return ["%1010", new BigInteger(10), FormatStyle.SpecialCharPrefix];
            yield return ["&B1010", new BigInteger(10), FormatStyle.VBStylePrefix];
            yield return ["2#1010#", new BigInteger(10), FormatStyle.AdaStylePrefix];
            yield return ["2r1010", new BigInteger(10), FormatStyle.RStylePrefix];
            
            // Test with underscores
            yield return ["0b1010_1010", new BigInteger(170), FormatStyle.CSharpStyle];
        }

        public static IEnumerable<object[]> GetHexadecimalFormatsTestData()
        {
            // Hexadecimal format testing with all supported styles
            yield return ["0xFF", new BigInteger(255), FormatStyle.CSharpStyle];
            yield return ["0XFF", new BigInteger(255), FormatStyle.CSharpStyle];
            yield return ["0hFF", new BigInteger(255), FormatStyle.CSharpStyle];
            yield return ["FFh", new BigInteger(255), FormatStyle.SingleCharSuffix];
            yield return ["FFhex", new BigInteger(255), FormatStyle.MultiCharSuffix];
            yield return ["FFhexadecimal", new BigInteger(255), FormatStyle.MultiCharSuffix];
            yield return ["hFF", new BigInteger(255), FormatStyle.SingleCharPrefix];
            yield return ["xFF", new BigInteger(255), FormatStyle.SingleCharPrefix];
            yield return ["#FF", new BigInteger(255), FormatStyle.SpecialCharPrefix];
            yield return ["$FF", new BigInteger(255), FormatStyle.SpecialCharPrefix];
            yield return ["&HFF", new BigInteger(255), FormatStyle.VBStylePrefix];
            yield return ["16#FF#", new BigInteger(255), FormatStyle.AdaStylePrefix];
            yield return ["16#FF", new BigInteger(255), FormatStyle.AdaStylePrefix];
            yield return [" 16#FF# ", new BigInteger(255), FormatStyle.AdaStylePrefix];
            yield return ["16rFF", new BigInteger(255), FormatStyle.RStylePrefix];
            
            // Test uppercase/lowercase variations
            yield return ["0xABCDEF", new BigInteger(11259375), FormatStyle.CSharpStyle];
            yield return ["0xabcdef", new BigInteger(11259375), FormatStyle.CSharpStyle];
            
            // Test with underscores
            yield return ["0xA_B_C_D", new BigInteger(43981), FormatStyle.CSharpStyle];
            yield return ["0xA'B'C'D", new BigInteger(43981), FormatStyle.CppStyle];
        }

        public static IEnumerable<object[]> GetOctalFormatsTestData()
        {
            // Octal format testing with all supported styles
            yield return ["052", new BigInteger(42), FormatStyle.CSharpStyle];
            yield return ["52o", new BigInteger(42), FormatStyle.SingleCharSuffix];
            yield return ["52oct", new BigInteger(42), FormatStyle.MultiCharSuffix];
            yield return ["52octal", new BigInteger(42), FormatStyle.MultiCharSuffix];
            yield return ["o52", new BigInteger(42), FormatStyle.SingleCharPrefix];
            yield return ["@52", new BigInteger(42), FormatStyle.SpecialCharPrefix];
            yield return ["&O52", new BigInteger(42), FormatStyle.VBStylePrefix];
            yield return ["8#52#", new BigInteger(42), FormatStyle.AdaStylePrefix];
            yield return ["8r52", new BigInteger(42), FormatStyle.RStylePrefix];
            
            // Test with underscores
            yield return ["07_7_7", new BigInteger(511), FormatStyle.CSharpStyle];
        }

        public static IEnumerable<object[]> GetDecimalFormatsTestData()
        {
            // Decimal format testing with all supported styles
            yield return ["42", new BigInteger(42), FormatStyle.Standard];
            yield return ["42dec", new BigInteger(42), FormatStyle.MultiCharSuffix];
            yield return ["42decimal", new BigInteger(42), FormatStyle.MultiCharSuffix];
            yield return ["10#42#", new BigInteger(42), FormatStyle.AdaStylePrefix];
            yield return ["10r42", new BigInteger(42), FormatStyle.RStylePrefix];
            
            // Test with underscores
            yield return ["1_000_000", new BigInteger(1000000), FormatStyle.CSharpStyle];
        }

        public static IEnumerable<object[]> GetCharacterFormatsTestData()
        {
            // Character format testing 
            yield return ["'A'", new BigInteger(65)];
            yield return ["'a'", new BigInteger(97)];
            yield return ["'0'", new BigInteger(48)];
            yield return ["'\\n'", new BigInteger(10)]; // Newline
            yield return ["'\\t'", new BigInteger(9)]; // Tab
            yield return ["'\\0'", new BigInteger(0)]; // Null
            yield return ["'\\''", new BigInteger(39)]; // Single quote
            yield return ["'\\\"'", new BigInteger(34)]; // Double quote
            yield return ["'\\\\'", new BigInteger(92)]; // Backslash
            yield return ["'\\u0041'", new BigInteger(65)];
            yield return ["'\\U00000041'", new BigInteger(65)];
            yield return ["'\\x41'", new BigInteger(65)];
        }
    }
}
