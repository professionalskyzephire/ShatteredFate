using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Items.Materials
{
	public class CosmicDust : ModItem
	{
		public override void SetStaticDefaults() => Item.ResearchUnlockCount = CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[ItemID.FallenStar];
		public override void SetDefaults() => Item.CloneDefaults(ItemID.FallenStar);
		public override void PostUpdate() => Lighting.AddLight(Item.Center, Color.Cyan.ToVector3() * 0.55f * Main.essScale);
		public override Color? GetAlpha(Color lightColor) => Color.White;
	}
}