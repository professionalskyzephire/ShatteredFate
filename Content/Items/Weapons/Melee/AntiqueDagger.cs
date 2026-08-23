using System;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using ShatteredFate.Content.Projectiles.Melee;
using System.IO;

namespace ShatteredFate.Content.Items.Weapons.Melee
{
	public class AntiqueDagger : ModItem {
		public override void SetStaticDefaults() => Terraria.ID.ItemID.Sets.ItemsThatAllowRepeatedRightClick[base.Item.type] = true;
		public override void SetDefaults() {
			Item.damage = 20;
			Item.knockBack = 5f;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.width = 30;
			Item.height = 62;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.rare = ItemRarityID.White;
			Item.value = Item.sellPrice(0, 0, 5, 0);
			Item.shoot = ModContent.ProjectileType<AntiqueDaggerProjectile>();
			Item.shootSpeed = 1f;
		}
		public override Nullable<bool> UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */ {
			if (player.whoAmI != Main.myPlayer) return base.UseItem(player);
			Item.useStyle = player.altFunctionUse == 2 ? ItemUseStyleID.Rapier : ItemUseStyleID.Swing;
			Item.NetStateChanged();
			return base.UseItem(player);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			if(Main.myPlayer != player.whoAmI) return false;
			if(player.altFunctionUse == 2) velocity += Main.rand.NextVector2CircularEdge(1f, 1f) * 0.2f;
			else position += Vector2.Normalize(velocity) * 24f;
			int p = Projectile.NewProjectile(source, position, velocity * (player.altFunctionUse == 2 ? 1f : 14f), type, damage, knockback, player.whoAmI, player.altFunctionUse == 2 ? 0f : 1f);
			NetMessage.SendData(27, -1, -1, null, p);
			return false;
		}
		public override float UseSpeedMultiplier(Player player) => player.altFunctionUse == 2 ? 2f : 1f;
		public override void NetSend(BinaryWriter writer) => writer.Write((byte)Item.useStyle);
		public override void NetReceive(BinaryReader reader) => Item.useStyle = reader.ReadByte();
		public override bool AltFunctionUse(Player player) => true;
	}
}
