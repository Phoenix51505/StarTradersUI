using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using StarTradersUI.Api.WaypointInfo;

namespace StarTradersUI.Utilities;

public static class WaypointUtilities
{
    private static readonly Bitmap[] WaypointTraitImages = new Bitmap[Enum.GetValues<WaypointTraitSymbol>().Length];

    static WaypointUtilities()
    {
        foreach (var value in Enum.GetValues<WaypointTraitSymbol>())
        {
            var num = (int)value + 1;
            WaypointTraitImages[num - 1] =
                new Bitmap(AssetLoader.Open(
                    new Uri($"avares://StarTradersUI/Assets/TraitIcons/startraders_trait_sprites{num}.png")));
        }
    }

    public static Bitmap GetWaypointTraitImage(WaypointTraitSymbol symbol) => WaypointTraitImages[(int)symbol];
}