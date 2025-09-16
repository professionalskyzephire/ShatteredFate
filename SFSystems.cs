using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ShatteredFate
{
	public class SFSystems : ModSystem
	{
		public static RecipeGroup CopperOrTin;
		public static RecipeGroup SilverOrTungsten;
		public static RecipeGroup GoldOrPlatinum;
		public override void Unload() {
			CopperOrTin = null;
			SilverOrTungsten = null;
			GoldOrPlatinum = null;
		}
		public override void AddRecipeGroups() {
			CopperOrTin = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CopperBar)}", ItemID.CopperBar, ItemID.TinBar);
			RecipeGroup.RegisterGroup("ShatteredFate:CopperOrTin", CopperOrTin);
			SilverOrTungsten = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}", ItemID.SilverBar, ItemID.TungstenBar);
			RecipeGroup.RegisterGroup("ShatteredFate:SilverOrTungsten", SilverOrTungsten);
			GoldOrPlatinum = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", ItemID.GoldBar, ItemID.PlatinumBar);
			RecipeGroup.RegisterGroup("ShatteredFate:GoldOrPlatinum", GoldOrPlatinum);

		}
		public override void PostAddRecipes() {
			for(int i = 0; i < Recipe.numRecipes; i++) {
				if(ModContent.GetInstance<SFReworksConfig>().GemStaves) if(Main.recipe[i].createItem.type == ItemID.TopazStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Topaz, 5).AddIngredient(ItemID.Wood, 10).AddTile(TileID.WorkBenches);
				else if(Main.recipe[i].createItem.type == ItemID.AmethystStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Amethyst, 5).AddIngredient(ItemID.StoneBlock, 15).AddTile(TileID.WorkBenches);
				else if(Main.recipe[i].createItem.type == ItemID.EmeraldStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Emerald, 5).AddRecipeGroup("ShatteredFate:CopperOrTin", 10).AddTile(TileID.Anvils);
				else if(Main.recipe[i].createItem.type == ItemID.SapphireStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Sapphire, 5).AddRecipeGroup(RecipeGroupID.IronBar, 10).AddTile(TileID.Anvils);
				else if(Main.recipe[i].createItem.type == ItemID.RubyStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Ruby, 5).AddRecipeGroup("ShatteredFate:SilverOrTungsten", 10).AddTile(TileID.Anvils);
				else if(Main.recipe[i].createItem.type == ItemID.DiamondStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.Diamond, 5).AddRecipeGroup("ShatteredFate:GoldOrPlatinum", 10).AddTile(TileID.Anvils);
				else if(Main.recipe[i].createItem.type == ItemID.AmberStaff) Main.recipe[i] = Recipe.Create(Main.recipe[i].createItem.type, 1).AddIngredient(ItemID.TopazStaff).AddIngredient(ItemID.AmethystStaff).AddIngredient(ItemID.EmeraldStaff).AddIngredient(ItemID.SapphireStaff).AddIngredient(ItemID.RubyStaff).AddIngredient(ItemID.DiamondStaff).AddIngredient(ModContent.ItemType<Content.Items.Materials.FusedGemstone>()).AddTile(TileID.Anvils);
			}
		}
	}
}