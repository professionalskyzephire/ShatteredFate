using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;

namespace ShatteredFate.Content.Items.Weapons.Magic
{
	public class SandSpell : ModItem
	{
		public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;
		public override void SetDefaults() {
			Item.width = 16;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.RaiseLamp;
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.damage = 25;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Magic;
			Item.knockBack = 0;
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ModContent.ProjectileType<Content.Projectiles.Magic.SandSpell>();
			Item.shootSpeed = 0f;
			Item.noMelee = true;
			Item.mana = 20;
			Item.channel = true;
			Item.UseSound = SoundID.Item60;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			if(Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[type] == 0) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI));
			return false;
		}
	}
}