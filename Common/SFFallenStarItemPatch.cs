using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ShatteredFate.Content.Items.Materials;

namespace ShatteredFate.Common
{
	public class SFFallenStarItemPatch : GlobalItem
	{
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == Terraria.ID.ItemID.FallenStar;
		public override void SetDefaults(Item item) {
			item.useTime = item.useAnimation = item.useStyle = 0;
			item.UseSound = null;
		}
		public override void OnSpawn(Item item, Terraria.DataStructures.IEntitySource source) {
			int oldStack = item.stack;
			item.ChangeItemType(ModContent.ItemType<CosmicDust>());
			item.stack = oldStack;
		}
		public override void PostUpdate(Item item) {
			int oldStack = item.stack;
			item.ChangeItemType(ModContent.ItemType<CosmicDust>());
			item.stack = oldStack;
		}
		public override void ModifyTooltips(Item item, System.Collections.Generic.List<TooltipLine> tooltips) {
			foreach(TooltipLine m in tooltips) if(m.Mod == "Terraria" && m.Name.StartsWith("Tooltip")) m.Text = "";
		}
		public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
			spriteBatch.Draw(Terraria.GameContent.TextureAssets.Item[ModContent.ItemType<CosmicDust>()].Value, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			Texture2D texture = Terraria.GameContent.TextureAssets.Item[ModContent.ItemType<CosmicDust>()].Value;
			spriteBatch.Draw(texture, item.Center - Main.screenPosition, null, Color.White, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
			return false;
		}
	}

}
