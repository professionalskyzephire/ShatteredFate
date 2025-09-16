using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Items.Materials
{
	public class FusedGemstone : ModItem
	{
		public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;
		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;
			Item.maxStack = 1;
			Item.rare = 2;
		}
		public override void AddRecipes() => CreateRecipe().AddIngredient(180, 5).AddIngredient(181, 5).AddIngredient(179, 5).AddIngredient(177, 5).AddIngredient(178, 5).AddIngredient(182, 5).AddIngredient(999, 5).AddTile(220).Register();
	}
}