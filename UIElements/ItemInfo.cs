using Terraria.ModLoader;

namespace ShatteredFate.UIElements;

public struct ItemInfo(int item, int index = -1) {
    public TooltipLine[] Info { get; set; } = [];
    public int ItemID { get; set; } = item;
    public int Index { get; set; } = index;
};