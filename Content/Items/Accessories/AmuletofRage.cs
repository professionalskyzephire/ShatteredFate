using ShatteredFate.Common.GlobalItems;
using ShatteredFate.Common.Players;
using ShatteredFate.ModUtils;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Items.Accessories;

public class AmuletofRage : ModItem {
    public override void SetDefaults() {
        Item.width = 40;
        Item.height = 40;
        Item.rare = ItemRarityID.Orange;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 5);
        Item.GetGlobalItem<RageItem>().SetMaxRage(150);
    }
    public override void UpdateAccessory(Player player, bool hideVisual) {
        RageItem rItem = Item.GetGlobalItem<RageItem>();

        if (Main.masterMode && rItem.GetMaxRage() == 150) { rItem.SetMaxRage(200); }
        else if (!Main.masterMode && rItem.GetMaxRage() != 150) { rItem.SetMaxRage(150); }

        player.AddBuff(ModContent.BuffType<Buffs.Rage>(), 1);
        if (player.GetModPlayer<RagePlayer>().GetRageStatus()) {
            player.GetDamage(DamageClass.Generic) += 0.33f;
            player.GetCritChance(DamageClass.Generic) += 33f;
            player.GetModPlayer<ModUtils.PlayersExpansions>().Hit += (Item item, ref StatModifier damage) => damage.Flat += 0.33f;
        };
        player.GetModPlayer<RagePlayer>().SetAmulet(Item);
    }
    public override void ModifyTooltips(List<TooltipLine> tooltips) => tooltips.Add(new(Mod, $"{SFMod.ModName}:Info", string.Format(Loc.GetTips("Items.AmuletofRage"), UIUtils.GetButtonName(KeyBind.GetRageKey()))));
};