using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Items.Weapons.Ranged
{
	public class OldCrossbow : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 1;
			ItemID.Sets.gunProj[Type] = true;
		}
		public override void SetDefaults() {
			Item.width = 16;
			Item.height = 16;
			Item.holdStyle = 8;
			Item.useStyle = 5;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.damage = 15;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged;
			Item.knockBack = 1;
			Item.value = Item.sellPrice(gold: 8, silver: 50);
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ModContent.ProjectileType<Content.Projectiles.Ranged.OldCrossbow>();
			Item.shootSpeed = 44f;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.useAmmo = AmmoID.Arrow;
		}
		public override void HoldItem(Player player) {
			if(player.ownedProjectileCounts[Item.shoot] <= 0 && Main.myPlayer == player.whoAmI) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot, Item.damage, Item.knockBack, player.whoAmI));
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
	}
}