using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

public class CSharpStylePrefixParser() : CStylePrefixParser('_')
{
    public override FormatStyle SupportedStyle => FormatStyle.CSharpStyle;
}