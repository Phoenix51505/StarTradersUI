using System;
using System.Collections.Generic;
using StarTradersUI.Api.FactionInfo;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace StarTradersUI.Utilities;

public static class FactionUtilities
{
    private static readonly (Color primary, Color secondary)[] FactionColors =
    [
        // Cosmic ( Cosmic Microwave Background )
        (new Color(255, 194, 46, 36), new Color(255, 17, 15, 47)),
        // Void ( Void Eye )
        (new Color(255, 63, 63, 116), new Color(255, 34, 32, 52)),
        // Galactic ( Galaxy )
        (new Color(255, 215, 123, 186), new Color(255, 34, 32, 52)),
        // Quantum ( 010 in super position )
        (new Color(255, 95, 205, 228), new Color(255, 48, 96, 130)),
        // Dominion ( Sword )
        (Colors.Red, Colors.OrangeRed),
        // Astro ( Night Sky )
        (new Color(255, 223, 113, 38), Colors.White),
        // Corsairs ( Skull + Crossbones )
        (Colors.White, new Color(255, 34, 32, 52)),
        // Obsidian ( Glass Knife )
        (new Color(255, 34, 32, 52), new Color(255, 69, 40, 60)),
        // Aegis ( Shield )
        (new Color(255, 102, 57, 49), new Color(255, 132, 126, 135)),
        // United ( Weird Flag )
        (Colors.Red, Colors.Blue),
        // Solitary ( 1 Flag )
        (new Color(255, 138, 111, 48), new Color(255, 89, 86, 82)),
        // Cobalt ( Cobalt Ingot )
        (Colors.DarkBlue, Colors.Blue),
        // Omega ( Omega Symbol )
        (new Color(255, 48, 96, 130), new Color(255, 34, 32, 52)),
        // Echo ( Echoing Waves )
        (Colors.SteelBlue, Colors.White),
        // Lords ( Crown )
        (new Color(255, 251, 242, 54), new Color(255, 118, 66, 138)),
        // Cult ( Cthulhu )
        (new Color(255, 118, 66, 138), Colors.Red),
        // Ancients ( Tree )
        (new Color(255, 106, 190, 48), new Color(255, 143, 86, 59)),
        // Shadow ( Shadow ) 
        (new Color(255,50,60,57), new Color(255, 34, 32, 52)),
        // Ethereal ( Ghost )
        (new Color(255,203,219,252), Colors.White),
    ];

    private static readonly Bitmap[] FactionBitmaps =
    [
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/cosmic.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/void.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/galactic.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/quantum.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/dominion.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/astro.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/corsairs.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/obsidian.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/aegis.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/united.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/solitary.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/cobalt.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/omega.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/echo.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/lords.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/cult.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/ancients.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/shadow.png"))),
        new(AssetLoader.Open(new Uri("avares://StarTradersUI/Assets/FactionIcons/ethereal.png"))),
    ];

    public static Color GetFactionTypePrimaryColor(FactionSymbol factionSymbol) =>
        FactionColors[(int)factionSymbol].primary;

    public static Color GetFactionTypeSecondaryColor(FactionSymbol factionSymbol) =>
        FactionColors[(int)factionSymbol].secondary;

    public static Bitmap GetFactionIcon(FactionSymbol factionSymbol) => FactionBitmaps[(int)factionSymbol];
}