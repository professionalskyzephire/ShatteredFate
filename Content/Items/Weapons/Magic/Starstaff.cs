using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;

namespace ShatteredFate.Content.Items.Weapons.Magic
{
	public class Starstaff : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 1;
			ItemID.Sets.gunProj[Type] = true;
		}
		public override void SetDefaults() {
			Item.width = 16;
			Item.height = 16;
			Item.holdStyle = ItemHoldStyleID.HoldGuitar;
			Item.useStyle = ItemUseStyleID.RaiseLamp;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.damage = 10;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Magic;
			Item.knockBack = 5;
			Item.value = Item.sellPrice(gold: 8, silver: 50);
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ModContent.ProjectileType<Content.Projectiles.Magic.Starstaff>();
			Item.shootSpeed = 14f;
			Item.noMelee = true;
			Item.mana = 10;
			Item.UseSound = SoundID.Item9;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			position.Y -= 480;
			if(Main.myPlayer == player.whoAmI) for(int i = 0; i < 3; i++) {
				position.Y -= i * 64;
				position.X += Main.rand.Next(-128, 129);
				velocity = Vector2.Normalize(Main.MouseWorld - position) * velocity.Length();
				int x = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, player.altFunctionUse, player.altFunctionUse == 2 ? -1f : Main.MouseWorld.Y);
				NetMessage.SendData(27, -1, -1, null, x);
				if(i > 0) continue;
				x = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, -2f, -1f);
				NetMessage.SendData(27, -1, -1, null, x);
			}
			return false;
		}
		public override void ModifyManaCost(Player player, ref float reduce, ref float mult) {
			if(player.altFunctionUse == 2) mult *= 2f;
		}
		public override bool AltFunctionUse(Player player) => true;
		public override void UseStyle(Player player, Rectangle itemFrame) {
			if(player.altFunctionUse == 2) player.itemRotation += MathHelper.PiOver4 * player.direction * 0.1f * (float)System.Math.Sin(player.itemAnimation * MathHelper.TwoPi / player.itemAnimationMax);
		}
		public override void HoldStyle(Player player, Rectangle itemFrame) => player.itemRotation += MathHelper.PiOver4 * player.direction * 0.4f;
	}
}