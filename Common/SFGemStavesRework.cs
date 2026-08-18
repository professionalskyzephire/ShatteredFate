using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShatteredFate.Content.Buffs.Debuffs;
using ShatteredFate.Content.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Common
{
	public class SFGemStavesRework : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<SFReworksConfig>().GemStaves;
		public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.AmberStaff || item.type == ItemID.AmethystStaff || item.type == ItemID.DiamondStaff || item.type == ItemID.EmeraldStaff || item.type == ItemID.RubyStaff || item.type == ItemID.SapphireStaff || item.type == ItemID.TopazStaff;
		public override void SetDefaults(Item item) {
			item.holdStyle = -1;
			item.noUseGraphic = true;
			if(item.type == ItemID.AmberStaff) {
				item.damage = 20;
				item.mana = 20;
				item.knockBack = 7f;
				item.useTime = item.useAnimation = 20;
				item.value = Item.sellPrice(gold: 4);
			}
			else if(item.type == ItemID.AmethystStaff) {
				item.damage = 10;
				item.mana = 5;
				item.knockBack = 5f;
				item.useTime = item.useAnimation = 25;
				item.rare = 0;
				item.value = Item.sellPrice(silver: 50);
			}
			else if(item.type == ItemID.DiamondStaff) {
				item.damage = 20;
				item.mana = 10;
				item.knockBack = 7f;
				item.useTime = item.useAnimation = 30;
				item.rare = 1;
				item.value = Item.sellPrice(gold: 3);
			}
			else if(item.type == ItemID.EmeraldStaff) {
				item.damage = 13;
				item.mana = 7;
				item.knockBack = 6f;
				item.useTime = item.useAnimation = 15;
				item.value = Item.sellPrice(gold: 1);
			}
			else if(item.type == ItemID.RubyStaff) {
				item.damage = 17;
				item.mana = 10;
				item.knockBack = 6.5f;
				item.useTime = item.useAnimation = 25;
				item.value = Item.sellPrice(gold: 2);
			}
			else if(item.type == ItemID.SapphireStaff) {
				item.damage = 15;
				item.mana = 8;
				item.knockBack = 6f;
				item.useTime = item.useAnimation = 20;
				item.value = Item.sellPrice(gold: 1, silver: 25);
			}
			else if(item.type == ItemID.TopazStaff) {
				item.damage = 10;
				item.mana = 4;
				item.knockBack = 5f;
				item.useTime = item.useAnimation = 20;
				item.channel = true;
				item.rare = 0;
				item.UseSound = SoundID.Item4;
				item.value = Item.sellPrice(silver: 25);
			}
		}
  		public override void HoldItem(Item item, Player player) {
			switch(item.type) {
				case ItemID.TopazStaff:
					player.statDefense += 5;
				break;
				case ItemID.EmeraldStaff:
					player.moveSpeed += 0.1f;
				break;
			}
		}
		public override bool CanUseItem(Item item, Player player) {
			if(player.HasBuff(ModContent.BuffType<EmeraldStaffCooldown>()) && item.type == ItemID.EmeraldStaff) return false;
			if(!item.channel) item.UseSound = player.altFunctionUse == 2 ? SoundID.Item4 : SoundID.Item43;
			return item.type == ItemID.AmberStaff || player.HasItem(ModContent.ItemType<Content.Items.Materials.FusedGemstone>());
		}
		public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			if(Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[ModContent.ProjectileType<GemStaff>()] == 0) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<GemStaff>(), damage, knockback, player.whoAmI, type));
			return false;
		}
		public override void ModifyTooltips(Item item, System.Collections.Generic.List<TooltipLine> tooltips) {
			string Texture = "Mods.ShatteredFate.ItemReworks.";
			switch(item.type) {
				case ItemID.AmberStaff:
					Texture += "FusedGemstone";
				break;
				case ItemID.AmethystStaff:
					Texture += "Amethyst";
				break;
				case ItemID.DiamondStaff:
					Texture += "Diamond";
				break;
				case ItemID.EmeraldStaff:
					Texture += "Emerald";
				break;
				case ItemID.RubyStaff:
					Texture += "Ruby";
				break;
				case ItemID.SapphireStaff:
					Texture += "Sapphire";
				break;
				case ItemID.TopazStaff:
					Texture += "Topaz";
				break;
			}
			Texture += "Staff";
			foreach(TooltipLine m in tooltips) if(m.Mod == "Terraria" && m.Name == "Knockback") m.Text += "\n" + Terraria.Localization.Language.GetTextValue(Texture);
		}
		public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
			string Texture = "ShatteredFate/Content/Items/Weapons/Magic/";
			switch(item.type) {
				case ItemID.AmberStaff:
					Texture += "FusedGemstone";
				break;
				case ItemID.AmethystStaff:
					Texture += "Amethyst";
				break;
				case ItemID.DiamondStaff:
					Texture += "Diamond";
				break;
				case ItemID.EmeraldStaff:
					Texture += "Emerald";
				break;
				case ItemID.RubyStaff:
					Texture += "Ruby";
				break;
				case ItemID.SapphireStaff:
					Texture += "Sapphire";
				break;
				case ItemID.TopazStaff:
					Texture += "Topaz";
				break;
			}
			Texture += "Staff";
			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, position - Vector2.UnitY * 4f, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			string Texture = "ShatteredFate/Content/Items/Weapons/Magic/";
			switch(item.type) {
				case ItemID.AmberStaff:
					Texture += "FusedGemstone";
				break;
				case ItemID.AmethystStaff:
					Texture += "Amethyst";
				break;
				case ItemID.DiamondStaff:
					Texture += "Diamond";
				break;
				case ItemID.EmeraldStaff:
					Texture += "Emerald";
				break;
				case ItemID.RubyStaff:
					Texture += "Ruby";
				break;
				case ItemID.SapphireStaff:
					Texture += "Sapphire";
				break;
				case ItemID.TopazStaff:
					Texture += "Topaz";
				break;
			}
			Texture += "Staff";
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			spriteBatch.Draw(texture, item.Bottom - Vector2.UnitY * texture.Height / 2 - Main.screenPosition, null, lightColor, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
			return false;
		}
		public override bool AltFunctionUse(Item item, Player player) {
			if(item.type == ItemID.AmethystStaff) return player.ownedProjectileCounts[ModContent.ProjectileType<AmethystStaff>()] == 0 && !player.HasBuff(ModContent.BuffType<AmethystStaffCooldown>());
			if(item.type == ItemID.SapphireStaff) return player.ownedProjectileCounts[ModContent.ProjectileType<LargeSapphire>()] == 0 && !player.HasBuff(ModContent.BuffType<SapphireStaffCooldown>());
			if(item.type == ItemID.RubyStaff) return player.ownedProjectileCounts[ModContent.ProjectileType<LargeRuby>()] == 0 && !player.HasBuff(ModContent.BuffType<RubyStaffCooldown>());
			if(item.type == ItemID.DiamondStaff) return player.ownedProjectileCounts[ModContent.ProjectileType<LargeDiamond>()] == 0 && !player.HasBuff(ModContent.BuffType<DiamondStaffCooldown>());
			return false;
		}
		public override float UseSpeedMultiplier(Item item, Player player) => player.altFunctionUse == 2 ? 0.5f : 1f;
	}
}
