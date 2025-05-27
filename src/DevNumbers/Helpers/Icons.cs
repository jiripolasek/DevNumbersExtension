// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.DevNumbers.Helpers;

public static class Icons
{
    public static IconInfo BaseConverterPage { get; } = IconHelpers.FromRelativePaths("Assets\\icons\\convert_light.png", "Assets\\icons\\convert_dark.png");
    public static IconInfo StoreLogo { get; } = IconHelpers.FromRelativePath("Assets\\StoreLogo.scale-200.png");

    public static IconInfo ArrowRight { get; } = new("\uF0AF");

    public static IconInfo ArrowLeft { get; } = new("\uF0B0");

    public static IconInfo Edit { get; } = new("\uE70F");

    public static IconInfo Rename { get; } = new("\uE8AC");

    public static IconInfo Go { get; } = new("\uE8AD");

    public static IconInfo Import { get; } = new("\uE8B5");

    public static IconInfo ReturnToCall { get; } = new("\uF71A");

    public static IconInfo CheckMark { get; } = new("\uE73E");

    public static IconInfo Bullseye { get; } = new("\uF272");

    public static IconInfo Warning { get; } = new("\uE7BA");

    public static IconInfo Help { get; } = new("\uE9CE");

    public static IconInfo InfoSolid { get; } = new("\uF167");

    public static IconInfo Dial2 { get; } = new("\uF147");
    public static IconInfo Dial8 { get; } = new("\uF14D");
    public static IconInfo Dial10 { get; } = new("\uF14F");
    public static IconInfo Dial16 { get; } = new("\uF155");

    public static IconInfo Font { get; } = new("\uE8D2");

    public static IconInfo Characters { get; } = new("\uE8C1");
}