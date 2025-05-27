namespace JPSoftworks.DevNumbers.Engine;

internal static class CharExtensions
{
    public static bool IsHexLetter(this char c)
    {
        return c is >= 'A' and <= 'F' or >= 'a' and <= 'f';
    }

    public static bool IsHexLetterUppercase(this char c)
    {
        return c is >= 'A' and <= 'F';
    }

    public static bool IsBinaryDigit(this char c)
    {
        return c is '0' or '1';
    }

    public static bool IsOctalDigit(this char c)
    {
        return c is >= '0' and <= '7';
    }
}