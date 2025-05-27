using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

public class CppStylePrefixParser() : CStylePrefixParser('\'')
{
    public override FormatStyle SupportedStyle => FormatStyle.CppStyle;
}