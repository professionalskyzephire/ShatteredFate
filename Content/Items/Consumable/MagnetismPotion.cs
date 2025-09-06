using ShatteredFate.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Items.Consumable;

public class MagnetismPotion : ModItem
{
    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useAnimation = Item.useTime = 15;
        Item.consumable = true;
        Item.buffTime = 3600;
        Item.buffType = ModContent.BuffType<MagnetismBuff>();
        Item.width = 24;
        Item.height = 36;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(silver: 50);
        Item.ResearchUnlockCount = 20;
        Item.maxStack = 999;
    }
}