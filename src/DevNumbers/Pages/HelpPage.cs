// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.DevNumbers.Helpers;
using JPSoftworks.DevNumbers.Resources;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.DevNumbers.Pages;

public sealed partial class HelpPage : ContentPage
{
    private readonly IContent[] _content;

    public HelpPage()
    {
        this.Icon = Icons.Help;
        this.Title = Strings.Command_ShowHelp_Title!;
        this.Name = Strings.Command_ShowHelp_Name!;
        this.Id = "com.jpsoftworks.cmdpal.devnumbers.help";

        this._content = [new MarkdownContent(HelpPages.Help!)];
    }

    public override IContent[] GetContent() => this._content;
}