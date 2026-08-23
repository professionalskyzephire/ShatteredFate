using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Projectiles.Magic
{
	public class LargeEmerald : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<SFReworksConfig>().GemStaves;
		public override string Texture => "Terraria/Images/Item_1525";
		public override string GlowTexture => "Terraria/Images/Extra_98";
		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults() {
			Projectile.width = Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.timeLeft = 300;
			Projectile.alpha = 255;
			Projectile.aiStyle = -1;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
		}
		public override void AI() {
			Player player = Main.player[Projectile.owner];
			if(Projectile.localAI[0] < Projectile.ai[0]) {
				Projectile.localAI[0]++;
				Projectile.localAI[1] = player.itemAnimationMax / 2;
				if(Projectile.localAI[1] == 25f) Projectile.localAI[2] = 10f;
			}
			if(Projectile.localAI[1] > 0f) Projectile.localAI[1]--;
			if(Projectile.localAI[2] > 0f) Projectile.localAI[2]--;
			if((player.itemTime == 0 || (Projectile.ai[1] > 0f && player.altFunctionUse == 2)) && Projectile.ai[0] >= 25f && Projectile.ai[1] < player.itemAnimationMax) {
				int target = -1;
				float maxRange = 3000f;
				foreach(NPC npc in Main.ActiveNPCs) if(npc.CanBeChasedBy(Projectile, false) && npc.Distance(Projectile.Center) < maxRange) {
					maxRange = Projectile.Distance(npc.Center);
					target = npc.whoAmI;
				}
				if(target > -1) Projectile.velocity += Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * 0.12f;
				else if(Main.myPlayer == player.whoAmI) {
					Projectile.velocity += Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 0.12f;
					NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
				}
				Projectile.velocity *= 0.98f;
				Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
				if(Projectile.ai[1] == 0f) {
					Projectile.localAI[2] = 10f;
					Projectile.velocity.Y += 0.1f;
					player.altFunctionUse = 2;
					player.itemAnimationMax *= 3;
					player.itemAnimation = player.itemAnimationMax;
					SoundEngine.PlaySound(SoundID.Item4, player.Center);
				}
				if(++Projectile.ai[1] == player.itemAnimationMax) {
					Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 16f;
					SoundEngine.PlaySound(SoundID.Item46, Projectile.Center);
					player.AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.EmeraldStaffCooldown>(), 300);
				}
				else return;
			}
			if(Projectile.alpha > 0) Projectile.alpha -= 15;
			if(Projectile.ai[1] < player.itemAnimationMax) {
				Projectile.position += (player.Top - player.Directions * 16f - Projectile.Center) * 0.1f;
				if(player.itemTime == 0 || player.HeldItem.type != ItemID.EmeraldStaff) Projectile.Kill();
				else Projectile.timeLeft = 300;
			}
			else {
				if(Projectile.ai[1] < 1000000f) Projectile.ai[1] = 1000000f;
				else Projectile.ai[1]++;
				Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
			}
		}
		public override void OnKill(int timeLeft) {
			if(Projectile.ai[0] >= 25f) for(int i = 0; i < 15; i++) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.Next(120, 160) * 0.1f, ModContent.ProjectileType<LargeEmeraldPiece>(), Projectile.damage, Projectile.knockBack, Projectile.owner));
			Terraria.Audio.SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
		}
		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			if(Projectile.ai[1] > 1000000f) for(int i = 1; i < Projectile.oldPos.Length; i++) if(i < Projectile.ai[1] - 1000000f) Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.White with {A = 0} * Projectile.Opacity * MathHelper.Lerp(0.5f, 0f, (float)i / (float)Projectile.oldPos.Length), Projectile.oldRot[i], texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			for(int i = 0; i < 3; i++) Main.EntitySpriteDraw(texture, Vector2.Lerp(Main.player[Projectile.owner].Center, Projectile.Center, Projectile.Opacity) + Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * MathHelper.Pi + MathHelper.TwoPi / 3f * i) * (float)System.Math.Sin(Projectile.localAI[1] * 2f / (float)Main.player[Projectile.owner].itemAnimationMax) * 6f - Main.screenPosition, null, Color.White with {A = 0} * Projectile.Opacity * MathHelper.Lerp(0.2f, 0.5f, Projectile.ai[0] / 25f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			if(Projectile.localAI[2] <= 0f) return false;
			texture = ModContent.Request<Texture2D>(GlowTexture).Value;
			for(int i = 0; i < 2; i++) Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Green with { A = 0 } * (float)System.Math.Sin(Projectile.localAI[2] * 0.1f * MathHelper.Pi), MathHelper.PiOver2 * i, texture.Size() * 0.5f, Projectile.scale * new Vector2(0.8f, 1.4f) * MathHelper.SmoothStep(1f, 0f, Projectile.localAI[2] * 0.1f), SpriteEffects.None, 0);
			return false;
		}
		public override bool ShouldUpdatePosition() => Projectile.ai[1] >= 1000000f;
		public override Nullable<bool> CanDamage()/* tModPorter Suggestion: Return null instead of true */ => Projectile.ai[1] > 1000000f ? null : false;
	}
}
