using ShatteredFate.Common.GlobalItems;
using Terraria;
using Terraria.ModLoader;

namespace ShatteredFate.Common.Prefixs;

public class RagePrefix : ModPrefix {
    public override string LocalizationCategory => "Tooltips.Prefixes";

    public override PrefixCategory Category => PrefixCategory.Accessory;
    public override bool CanRoll(Item item) => item.GetGlobalItem<RageItem>().GetMaxRage() > 0;
    public override float RollChance(Item item) => 0.2f;
    public override void Apply(Item item) => item.GetGlobalItem<RageItem>().SetScaleRage(-15);
    public override System.Collections.Generic.IEnumerable<TooltipLine> GetTooltipLines(Item item) { yield return new TooltipLine(Mod, SFMod.ModName + ":Rage bonus", ShatteredFate.ModUtils.Loc.GetTips("Prefixes.RagePrefix.Tooltips")) { OverrideColor = Terraria.ID.Colors.RarityDarkRed }; }
};