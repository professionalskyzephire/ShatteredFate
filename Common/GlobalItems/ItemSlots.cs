using ShatteredFate.UIElements;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ShatteredFate.Common.GlobalItems;

public class ItemSlots : GlobalItem {
    public override bool InstancePerEntity => true;
    ItemInfo _info;

    public ItemInfo GetNewInfo() => _info;
    public void SetNewInfo(ItemInfo value) => _info = value;

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (_info.Info == null) { return; }
        if (GetNewInfo().Index == -1) { tooltips.AddRange(GetNewInfo().Info); }
        else if (tooltips.Count - 1 >= GetNewInfo().Index) { tooltips.InsertRange(GetNewInfo().Index, GetNewInfo().Info); }
        else { tooltips.AddRange(GetNewInfo().Info); };
    }
};