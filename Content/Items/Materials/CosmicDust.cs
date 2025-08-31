using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

 //Note from U.N. Owen: DO NOT REMOVE THIS, IT IS NEEDED FOR THE FALLEN STAR REPLACEMENT TO WORK PROPERLY!
namespace ShatteredFate.Content.Items.Materials
{
	public class CosmicDust : ModItem
	{
		public override void SetStaticDefaults() => Item.ResearchUnlockCount = CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[ItemID.FallenStar];
		public override void SetDefaults() => Item.CloneDefaults(ItemID.FallenStar);
		public override void PostUpdate() => Lighting.AddLight(Item.Center, Color.SkyBlue.ToVector3() * 0.55f * Main.essScale);
		public override void UpdateInventory(Player player) {
			int oldStack = Item.stack;
			Item.ChangeItemType(ItemID.FallenStar);
			Item.stack = oldStack;
		}
		public override bool OnPickup(Player player) {
			int oldStack = Item.stack;
			Item.ChangeItemType(ItemID.FallenStar);
			Item.stack = oldStack;
			Item.newAndShiny = true;
			return true;
		}
		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			Texture2D texture = Terraria.GameContent.TextureAssets.Item[Type].Value;
			for(int i = 0; i < 4; i++) spriteBatch.Draw(texture, Item.Center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 3f * i + Main.GlobalTimeWrappedHourly * MathHelper.Pi) * MathHelper.Min(i, 1) * 4f - Main.screenPosition, null, i > 0 ? alphaColor with {A = 0} * 0.2f : alphaColor, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
			return false;
		}
		public override Color? GetAlpha(Color lightColor) => Color.White;
	}
}
