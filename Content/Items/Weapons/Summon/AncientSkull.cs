using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;

namespace ShatteredFate.Content.Items.Weapons.Summon
{
	public class AncientSkull : ModItem
	{
		public override void SetStaticDefaults() {
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.width = 16;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.RaiseLamp;
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.damage = 25;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Summon;
			Item.knockBack = 5;
			Item.shoot = ModContent.ProjectileType<Content.Projectiles.Minions.AncientSkullMinion>();
			Item.shootSpeed = 0f;
			Item.noMelee = true;
			Item.mana = 20;
			Item.channel = true;
			Item.UseSound = SoundID.Item20;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			if(Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[type] < 10) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(source, Main.MouseWorld, Main.rand.NextVector2Circular(4f, 4f), type, damage, knockback, player.whoAmI));
			return false;
		}
		public override bool CanUseItem(Player player) => !player.HasBuff(ModContent.BuffType<Content.Buffs.Debuffs.AncientSkullCooldown>()) && player.ownedProjectileCounts[Item.shoot] < 10;
	}
}