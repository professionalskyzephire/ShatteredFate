using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShatteredFate.Common.Players;

namespace ShatteredFate.Content.Items.Accessories;

public class PackOExplosives : ModItem {
    public override void SetDefaults() {
        Item.width = 54;
        Item.height = 58;
        Item.accessory = true;
    }
    public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.Dynamite, 10).AddIngredient(ItemID.Leather, 5).AddTile(TileID.WorkBenches).Register();
    public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<PackOExplosivesPlayer>().SetPackOExplosives(Item);
};