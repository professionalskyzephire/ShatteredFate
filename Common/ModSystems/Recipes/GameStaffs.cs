using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Common.ModSystems.Recipes;

public class GameStaffs : ModSystem {
    public override void PostAddRecipes() {
        for (int i = 0; i < Recipe.numRecipes; i++) {
            if (ModContent.GetInstance<SFReworksConfig>().GemStaves) if (Main.recipe[i].createItem.type == ItemID.TopazStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Topaz, 5).AddIngredient(ItemID.Wood, 10).AddTile(TileID.WorkBenches);
            else if (Main.recipe[i].createItem.type == ItemID.AmethystStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Amethyst, 5).AddIngredient(ItemID.StoneBlock, 15).AddTile(TileID.WorkBenches);
            else if (Main.recipe[i].createItem.type == ItemID.EmeraldStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Emerald, 5).AddRecipeGroup(Group.GetCopper(), 10).AddTile(TileID.Anvils);
            else if (Main.recipe[i].createItem.type == ItemID.SapphireStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Sapphire, 5).AddRecipeGroup(RecipeGroupID.IronBar, 10).AddTile(TileID.Anvils);
            else if (Main.recipe[i].createItem.type == ItemID.RubyStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Ruby, 5).AddRecipeGroup(Group.GetSilver(), 10).AddTile(TileID.Anvils);
            else if (Main.recipe[i].createItem.type == ItemID.DiamondStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Diamond, 5).AddRecipeGroup(Group.GetGold(), 10).AddTile(TileID.Anvils);
            else if (Main.recipe[i].createItem.type == ItemID.AmberStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.TopazStaff).AddIngredient(ItemID.AmethystStaff).AddIngredient(ItemID.EmeraldStaff).AddIngredient(ItemID.SapphireStaff).AddIngredient(ItemID.RubyStaff).AddIngredient(ItemID.DiamondStaff).AddIngredient(ModContent.ItemType<Content.Items.Materials.FusedGemstone>()).AddTile(TileID.Anvils);
        }
    }
};