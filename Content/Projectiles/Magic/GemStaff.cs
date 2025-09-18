using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using System;

namespace ShatteredFate.Content.Projectiles.Magic
{
	public class GemStaff : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<SFReworksConfig>().GemStaves;
		public override string Texture => "Terraria/Images/Extra_98";
		public override string GlowTexture => "Terraria/Images/Extra_174";
		public override void SetDefaults() {
			Projectile.width = Projectile.height = 1;
			Projectile.timeLeft = 2;
			Projectile.aiStyle = -1;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.hide = true;
		}
		public override void AI() {
			Player player = Main.player[Projectile.owner];
			player.heldProj = Projectile.whoAmI;
			if(Main.myPlayer == Projectile.owner) {
				Projectile.velocity = Vector2.Normalize(Main.MouseWorld - player.Center);
				NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
			}
			if(Projectile.velocity.X != 0) player.direction = Math.Sign(Projectile.velocity.X);
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * 1.25f * player.direction * player.gravDir;
			if(player.direction < 0) Projectile.rotation += MathHelper.Pi;
			if(player.itemAnimation > 0) {
				Projectile.timeLeft = 2;
				float animTime = (float)player.itemAnimation / (float)player.itemAnimationMax * 4f;
				float armRotOff = 0f;
				float armRotOff2 = 0f;
				switch(player.HeldItem.type) {
					case ItemID.AmberStaff:
						Projectile.rotation += MathHelper.ToRadians(player.direction * player.gravDir) * 0.5f;
						if(Main.myPlayer == player.whoAmI && player.itemTime == player.itemTimeMax - 1) {
							Projectile.ai[0] = Main.rand.Next(new int[] {121, 122, 123, 124, 125, 126, (int)Projectile.ai[0]});
							NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
							NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Vector2.Normalize(Projectile.velocity) * 48f, Projectile.velocity * player.HeldItem.shootSpeed, (int)Projectile.ai[0], Projectile.damage, Projectile.knockBack, player.whoAmI));
						}
					break;
					case ItemID.AmethystStaff:
						Projectile.rotation -= MathHelper.ToRadians(player.direction * player.gravDir) * 0.25f;
						if(player.altFunctionUse < 2) {
							armRotOff2 = MathHelper.PiOver2 * -1.4f * player.direction * player.gravDir;
							if(Main.myPlayer == player.whoAmI && player.itemTime == player.itemTimeMax - 1) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Vector2.Normalize(Projectile.velocity) * 42f, Projectile.velocity * player.HeldItem.shootSpeed, (int)Projectile.ai[0], Projectile.damage, Projectile.knockBack, player.whoAmI));
						}
						else if(animTime > 3f) {
							animTime -= 3f;
							armRotOff -= MathHelper.SmoothStep(MathHelper.PiOver2, 0f, animTime) * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, 0f, animTime) * player.direction * player.gravDir;
						}
						else if(animTime > 2f) {
							animTime -= 2f;
							armRotOff -= MathHelper.PiOver2 * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver2, MathHelper.PiOver4 * 3f, animTime) * player.direction * player.gravDir;
						}
						else if(animTime > 1f) {
							animTime -= 1f;
							armRotOff -= MathHelper.PiOver2 * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, MathHelper.PiOver2, animTime) * player.direction * player.gravDir;
						}
						else {
							armRotOff -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, MathHelper.PiOver2, animTime) * player.direction * player.gravDir;
							if(player.itemTime == 1) {
								SoundEngine.PlaySound(SoundID.Item8, player.Center);
								if(Main.myPlayer == player.whoAmI) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AmethystStaff>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Projectile.ai[0]));
							}
							Projectile.rotation -= MathHelper.SmoothStep(0f, MathHelper.PiOver4 * 3f, animTime) * player.direction * player.gravDir;
						}
					break;
					case ItemID.DiamondStaff:
						Projectile.rotation -= MathHelper.ToRadians(player.direction * player.gravDir) * 2f;
						if(player.altFunctionUse < 2) {
							if(Main.myPlayer == player.whoAmI && player.itemTime == player.itemTimeMax - 1) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Vector2.Normalize(Projectile.velocity) * 60f, Projectile.velocity * player.HeldItem.shootSpeed, (int)Projectile.ai[0], Projectile.damage, Projectile.knockBack, player.whoAmI));
						}
						else if(animTime > 3f) {
							animTime -= 3f;
							armRotOff -= MathHelper.SmoothStep(MathHelper.PiOver2, 0f, animTime);
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, 0f, animTime) * player.direction * player.gravDir;
						}
						else if(animTime > 2f) {
							animTime -= 2f;
							armRotOff -= MathHelper.PiOver2 * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver2, MathHelper.PiOver4 * 3f, animTime) * player.direction * player.gravDir;
						}
						else if(animTime > 1f) {
							animTime -= 1f;
							armRotOff -= MathHelper.PiOver2 * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, MathHelper.PiOver2, animTime) * player.direction * player.gravDir;
						}
						else {
							armRotOff -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, MathHelper.PiOver2, animTime) * player.direction * player.gravDir;
							if(player.itemTime == 1) {
								SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, player.Center);
								if(Main.myPlayer == player.whoAmI) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.Center, Vector2.Zero, ModContent.ProjectileType<LargeDiamond>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Projectile.ai[0]));
							}
							Projectile.rotation -= MathHelper.SmoothStep(0f, MathHelper.PiOver4 * 3f, animTime) * player.direction * player.gravDir;
						}
					break;
					case ItemID.EmeraldStaff:
						Projectile.rotation -= MathHelper.ToRadians(player.direction * player.gravDir) * 0.5f;
						if(player.altFunctionUse < 2) {
							if(Main.myPlayer == player.whoAmI && player.itemTime == player.itemTimeMax - 1) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Vector2.Normalize(Projectile.velocity) * 48f, Projectile.velocity * player.HeldItem.shootSpeed, (int)Projectile.ai[0], Projectile.damage, Projectile.knockBack, player.whoAmI));
						}
						else if(animTime > 3f) {
							animTime -= 3f;
							armRotOff -= MathHelper.SmoothStep(MathHelper.PiOver2, 0f, animTime) * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, 0f, animTime) * player.direction * player.gravDir;
						}
						else if(animTime > 2f) {
							animTime -= 2f;
							armRotOff -= MathHelper.PiOver2 * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver2, MathHelper.PiOver4 * 3f, animTime) * player.direction * player.gravDir;
						}
						else if(animTime > 1f) {
							animTime -= 1f;
							armRotOff -= MathHelper.PiOver2 * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, MathHelper.PiOver2, animTime) * player.direction * player.gravDir;
						}
						else {
							armRotOff -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, MathHelper.PiOver2, animTime) * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(0f, MathHelper.PiOver4 * 3f, animTime) * player.direction * player.gravDir;
						}
					break;
					case ItemID.RubyStaff:
						Projectile.rotation += MathHelper.ToRadians(player.direction * player.gravDir) * 2f;
						if(player.altFunctionUse < 2) {
							if(Main.myPlayer == player.whoAmI && player.itemTime == player.itemTimeMax - 1) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Vector2.Normalize(Projectile.velocity) * 56f, Projectile.velocity * player.HeldItem.shootSpeed, (int)Projectile.ai[0], Projectile.damage, Projectile.knockBack, player.whoAmI));
						}
						else if(animTime > 3f) {
							animTime -= 3f;
							armRotOff -= MathHelper.SmoothStep(MathHelper.PiOver2, 0f, animTime) * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, 0f, animTime) * player.direction * player.gravDir;
						}
						else if(animTime > 2f) {
							animTime -= 2f;
							armRotOff -= MathHelper.PiOver2 * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver2, MathHelper.PiOver4 * 3f, animTime) * player.direction * player.gravDir;
						}
						else if(animTime > 1f) {
							animTime -= 1f;
							armRotOff -= MathHelper.PiOver2 * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, MathHelper.PiOver2, animTime) * player.direction * player.gravDir;
						}
						else {
							armRotOff -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, MathHelper.PiOver2, animTime) * player.direction * player.gravDir;
							if(player.itemTime == 1) {
								SoundEngine.PlaySound(SoundID.Item46, player.Center);
								if(Main.myPlayer == player.whoAmI) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<LargeRuby>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Projectile.ai[0]));
							}
							Projectile.rotation -= MathHelper.SmoothStep(0f, MathHelper.PiOver4 * 3f, animTime) * player.direction * player.gravDir;
						}
					break;
					case ItemID.SapphireStaff:
						Projectile.rotation += MathHelper.ToRadians(player.direction * player.gravDir) * player.direction * player.gravDir;
						if(player.altFunctionUse < 2) {
							if(Main.myPlayer == player.whoAmI && player.itemTime == player.itemTimeMax - 1) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Vector2.Normalize(Projectile.velocity) * 56f, Projectile.velocity * player.HeldItem.shootSpeed, (int)Projectile.ai[0], Projectile.damage, Projectile.knockBack, player.whoAmI));
						}
						else if(animTime > 3f) {
							animTime -= 3f;
							armRotOff -= MathHelper.SmoothStep(MathHelper.PiOver2, 0f, animTime);
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, 0f, animTime) * player.direction * player.gravDir;
						}
						else if(animTime > 2f) {
							animTime -= 2f;
							armRotOff -= MathHelper.PiOver2 * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver2, MathHelper.PiOver4 * 3f, animTime) * player.direction * player.gravDir;
						}
						else if(animTime > 1f) {
							animTime -= 1f;
							armRotOff -= MathHelper.PiOver2 * player.direction * player.gravDir;
							Projectile.rotation -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, MathHelper.PiOver2, animTime) * player.direction * player.gravDir;
						}
						else {
							armRotOff -= MathHelper.SmoothStep(MathHelper.PiOver4 * 3f, MathHelper.PiOver2, animTime) * player.direction * player.gravDir;
							if(player.itemTime == 1) {
								SoundEngine.PlaySound(SoundID.Item46, player.Center);
								if(Main.myPlayer == player.whoAmI) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<LargeSapphire>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Projectile.ai[0]));
							}
							Projectile.rotation -= MathHelper.SmoothStep(0f, MathHelper.PiOver4 * 3f, animTime) * player.direction * player.gravDir;
						}
					break;
					case ItemID.TopazStaff:
						armRotOff2 = MathHelper.PiOver2 * -1.4f * player.direction * player.gravDir;
						Projectile.rotation -= MathHelper.ToRadians(player.direction * player.gravDir) * 0.25f;
						if(player.channel) {
							Projectile.ai[1]++;
							if(player.itemAnimation < player.itemAnimationMax - 1) player.itemAnimation++;
							if(player.itemTime < player.itemTimeMax - 1) player.itemTime++;
							break;
						}
						else if(animTime < 3f && animTime > 2f) {
							animTime -= 2f;
							Projectile.rotation -= MathHelper.PiOver4 * 0.4f * (float)Math.Sin(animTime * MathHelper.Pi) * player.direction * player.gravDir;
							if(animTime > 0.75f) break;
						}
						else if(animTime < 2f) {
							animTime /= 2f;
							Projectile.rotation += MathHelper.PiOver4 * 0.2f * (float)Math.Sin(animTime * MathHelper.Pi) * player.direction * player.gravDir;
						}
						else break;
						if(Projectile.ai[1] > 0f) {
							SoundEngine.PlaySound(SoundID.Item43, player.Center);
							if(Main.myPlayer == player.whoAmI) for(int i = 0; i < MathHelper.Min(Projectile.ai[1], player.itemTimeMax * 5f) / (float)player.itemTimeMax; i++) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Vector2.Normalize(Projectile.velocity) * 42f, Projectile.velocity * player.HeldItem.shootSpeed + Main.rand.NextVector2Circular(2, 2), (int)Projectile.ai[0], Projectile.damage, Projectile.knockBack, player.whoAmI));
							Projectile.ai[1] = 0f;
						}
					break;
				}
				player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation + armRotOff) * player.gravDir);
				Projectile.Center = player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation);
				player.compositeFrontArm.rotation += armRotOff2;
				player.SetCompositeArmBack(enabled: true, Player.CompositeArmStretchAmount.Full, (Projectile.Center + Vector2.Normalize(Projectile.velocity) * 32f - player.Center).ToRotation() * player.gravDir - MathHelper.PiOver2 * (player.direction - 1) - MathHelper.PiOver2 * player.direction * player.gravDir);
			}
		}
		public override bool PreDraw(ref Color lightColor) {
			Player player = Main.player[Projectile.owner];
			string Texture = "ShatteredFate/Content/Items/Weapons/Magic/";
			Vector2 flash = Vector2.Zero;
			Color color = Color.Transparent;
			switch(player.HeldItem.type) {
				case ItemID.AmberStaff:
					flash += new Vector2(28, -46);
					switch(Projectile.ai[0]) {
						default:
							color = Color.Orange;
						break;
						case 121:
							color = Color.Purple;
						break;
						case 122:
							color = Color.Yellow;
						break;
						case 123:
							color = Color.Blue;
						break;
						case 124:
							color = Color.Green;
						break;
						case 125:
							color = Color.Red;
						break;
						case 126:
							color = Color.White;
						break;
					}
					Texture += "FusedGemstone";
				break;
				case ItemID.AmethystStaff:
					color = Color.Purple;
					flash += new Vector2(24, -38);
					Texture += "Amethyst";
				break;
				case ItemID.DiamondStaff:
					color = Color.White;
					flash += new Vector2(32, -48);
					Texture += "Diamond";
				break;
				case ItemID.EmeraldStaff:
					color = Color.Green;
					flash += new Vector2(26, -44);
					Texture += "Emerald";
				break;
				case ItemID.RubyStaff:
					color = Color.Red;
					flash += new Vector2(28, -48);
					Texture += "Ruby";
				break;
				case ItemID.SapphireStaff:
					color = Color.Blue;
					flash += new Vector2(32, -48);
					Texture += "Sapphire";
				break;
				case ItemID.TopazStaff:
					color = Color.Yellow;
					flash += new Vector2(26, -40);
					Texture += "Topaz";
				break;
			}
			Texture += "Staff";
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, new Vector2(player.direction * player.gravDir > 0 ? 4 : texture.Width - 4, player.gravDir > 0 ? texture.Height - 6 : 6), Projectile.scale * player.HeldItem.scale, player.direction > 0 ? player.gravDir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically : player.gravDir > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0);
			flash *= player.Directions * Projectile.scale * player.HeldItem.scale;
			flash = flash.RotatedBy(Projectile.rotation);
			color *= (float)System.Math.Sin(MathHelper.Pi * (float)player.itemAnimation / (float)player.itemAnimationMax);
			color.A = 0;
			texture = ModContent.Request<Texture2D>(this.Texture).Value;
			for(int i = 0; i < 2; i++) Main.EntitySpriteDraw(texture, Projectile.Center + flash - Main.screenPosition, null, color * Projectile.Opacity, MathHelper.PiOver2 * i + MathHelper.SmoothStep(MathHelper.Pi, 0f, player.itemAnimation / (float)player.itemAnimationMax) * player.direction * player.gravDir, texture.Size() * 0.5f, Projectile.scale * player.HeldItem.scale * new Vector2(0.4f, 0.7f), SpriteEffects.None, 0);
			if(player.channel && player.HeldItem.type == ItemID.TopazStaff) {
				texture = ModContent.Request<Texture2D>(GlowTexture).Value;
				Main.EntitySpriteDraw(texture, Projectile.Center + flash - Main.screenPosition, null, color * Projectile.Opacity * (float)System.Math.Sin(MathHelper.Pi * Projectile.ai[1] / 5f / (float)player.itemTimeMax), 0f, texture.Size() * 0.5f, Projectile.scale * player.HeldItem.scale * MathHelper.SmoothStep(1f, 0f, Projectile.ai[1] / 5f / (float)player.itemTimeMax), SpriteEffects.None, 0);
			}
			return false;
		}
		public override bool ShouldUpdatePosition() => false;
	}
}
