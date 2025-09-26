using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Items.Weapons.Summon.Whips
{
	public class SanguineLeech : ModItem
	{
		public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;
		public override void SetDefaults() {
			Item.DefaultToWhip(ModContent.ProjectileType<Content.Projectiles.Whips.SanguineLeech>(), 10, 3f, 5f);
			Item.rare = 4;
			Item.value = Item.sellPrice(gold: 15);
			Item.channel = true;
		}
		public override bool MeleePrefix() => true;
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => player.ownedProjectileCounts[Item.shoot] <= 0;
	}
}