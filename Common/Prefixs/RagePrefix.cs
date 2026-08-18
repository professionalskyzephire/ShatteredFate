using ShatteredFate.Common.GlobalItems;
using ShatteredFate.ModUtils;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Common.Prefixs;

public class RagePrefix : ModPrefix {
    public override string LocalizationCategory => "Tooltips.Prefixes";

    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override bool CanRoll(Item item) => item.GetGlobalItem<RageItem>().GetMaxRage() > 0;
    public override float RollChance(Item item) => 0.2f;
    public override void Apply(Item item) => item.GetGlobalItem<RageItem>().SetScaleRage(-15);
    public override IEnumerable<TooltipLine> GetTooltipLines(Item item) { yield return new TooltipLine(Mod, "RageBonus", Loc.GetTips("Prefixes.RagePrefix.Tooltips")) { OverrideColor = Colors.RarityDarkRed }; }
};