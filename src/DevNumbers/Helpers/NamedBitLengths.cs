// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.DevNumbers.Helpers;

internal static class NamedBitLengths
{
    public static string? GetKnownName(int bitLength)
    {
        return bitLength switch
        {
            8 => "BYTE",
            16 => "WORD",
            32 => "DWORD",
            64 => "QWORD",
            _ => null
        };
    }

    public static string? GetName(int bitLength)
    {
        return bitLength switch
        {
            8 => "BYTE",
            16 => "WORD",
            32 => "DWORD",
            64 => "QWORD",
            _ => $"{bitLength} bits"
        };
    }
}