using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using System;
using System.Collections.Generic;

namespace ShatteredFate.Content.Projectiles.Whips
{
	public class SanguineLeech : ModProjectile
	{
		public override string Texture => "ShatteredFate/Content/Projectiles/SanguineLeechProj";
		public override string GlowTexture => "ShatteredFate/Content/Projectiles/SanguineLeechChain";
		public override void SetStaticDefaults() => ProjectileID.Sets.IsAWhip[Type] = true;
		public override void SetDefaults() {
			Projectile.DefaultToWhip();
			Projectile.usesIDStaticNPCImmunity = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 60 * Projectile.MaxUpdates;
		}
		public override bool PreAI() {
			if(Projectile.ai[0] == 0f) Projectile.localAI[0] = Projectile.velocity.Length();
			Player player = Main.player[Projectile.owner];
			float swingSpeed = player.itemTimeMax * 2f / 3f * Projectile.MaxUpdates;
			if(Projectile.ai[2] > 0f && player.channel && player.HasMinionAttackTargetNPC && Projectile.ai[0] >= swingSpeed) {
				Main.npc[player.MinionAttackTargetNPC].AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.SanguineLeechDebuff>(), 2);
				Vector2 toLatch = Main.npc[player.MinionAttackTargetNPC].Center - Main.GetPlayerArmPosition(Projectile);
				Projectile.localAI[1] = toLatch.Length() / 60f;
				if(Projectile.localAI[2] < 10f) Projectile.localAI[2]++;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity.SafeNormalize(Projectile.oldVelocity) * Projectile.localAI[0], toLatch.SafeNormalize(Vector2.Zero) * Projectile.localAI[1], Projectile.localAI[2] * 0.1f);
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
				if(Projectile.ai[0] > swingSpeed) Projectile.ai[0]--;
				else Projectile.ai[0] = swingSpeed;
				Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * (Projectile.ai[0] - 1f);
				player.direction = -Math.Sign(player.Center.X - Projectile.Center.X);
				player.heldProj = Projectile.whoAmI;
				player.itemAnimation = (int)(player.itemAnimationMax * (1f - swingSpeed / (float)player.itemTimeMax / (float)Projectile.MaxUpdates));
				player.itemTime = player.itemAnimation;
				return false;
			}
			if(Projectile.ai[0] >= swingSpeed && !player.channel) Projectile.ai[2] = 0f;
			if(Projectile.localAI[2] > 0f) {
				Projectile.localAI[2]--;
				if(Projectile.localAI[0] < Projectile.localAI[1]) Projectile.velocity = Projectile.velocity.SafeNormalize(Projectile.oldVelocity) * MathHelper.Lerp(Projectile.localAI[0], Projectile.localAI[1], Projectile.localAI[2] * 0.1f);
			}
			return true;
		}
		public override bool PreDraw(ref Color lightColor) {
			List<Vector2> list = new List<Vector2>();
			Projectile.FillWhipControlPoints(Projectile, list);
			Texture2D ropeTexture = (Texture2D)ModContent.Request<Texture2D>(GlowTexture);
			Vector2 vector = list[0];
			for(int i = 0; i < list.Count - 2; i++) {
				Vector2 vector2 = list[i];
				Vector2 vector3 = list[i + 1] - vector2;
				float rotation = vector3.ToRotation() - (float)Math.PI / 2f;
				Color color = Lighting.GetColor(vector2.ToTileCoordinates(), Color.White);
				Vector2 scale = new Vector2(1f, (vector3.Length() + 2f) / (float)ropeTexture.Height);
				Main.EntitySpriteDraw(ropeTexture, vector - Main.screenPosition, null, color, rotation, new Vector2(ropeTexture.Width / 2, 2f), scale, SpriteEffects.None, 0);
				vector += vector3;
			}
			return false;
		}
		public override void PostDraw(Color lightColor) {
			List<Vector2> list = new List<Vector2>();
			Projectile.FillWhipControlPoints(Projectile, list);
			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			Rectangle rectangle = texture.Frame(1, 5);
			Vector2 origin = new Vector2(rectangle.Width / 2, 2f);
			Vector2 vector = list[0];
			for(int i = 0; i < list.Count - 1; i++) {
				switch(i) {
					default:
						rectangle.Y = rectangle.Height;
					break;
					case 0:
						rectangle.Y = 0;
					break;
					case 9:
					case 11:
					case 13:
						rectangle.Y = rectangle.Height * 2;
					break;
					case 15:
					case 17:
						rectangle.Y = rectangle.Height * 3;
					break;
					case 19:
						rectangle.Y = rectangle.Height * 4;
					break;
				}
				Vector2 vector2 = list[i];
				Vector2 vector3 = list[i + 1] - vector2;
				float rotation = vector3.ToRotation() - (float)Math.PI / 2f;
				Color color = Lighting.GetColor(vector2.ToTileCoordinates(), Color.White);
				Vector2 scale = new Vector2(1f, (vector3.Length() + 2f) / (float)rectangle.Height);
				Main.EntitySpriteDraw(texture, vector - Main.screenPosition, rectangle, color, rotation, origin, Projectile.scale, SpriteEffects.None, 0);
				vector += vector3;
			}
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			if(Projectile.ai[2] == 0f && Main.myPlayer == Projectile.owner && target.CanBeChasedBy(Projectile, false)) {
				Projectile.ai[2]++;
				NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
				return;
			}
			if(Projectile.ai[2] == 0f) {
				if(target.CanBeChasedBy(Projectile, false)) target.AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.SanguineLeechDebuff>(), 60);
				return;
			}
			Player player = Main.player[Projectile.owner];
			int healAmount = (int)MathHelper.Lerp(7f * 1.25f, 3f, (float)player.statLife / (float)player.statLifeMax2);
			if(healAmount > 7) healAmount = 7;
			if(player.statLife < player.statLifeMax2) {
				player.statLife += healAmount;
				player.HealEffect(healAmount);
			}
		}
		public override bool? CanHitNPC(NPC target) => Projectile.ai[2] == 0f || target.whoAmI == Main.player[Projectile.owner].MinionAttackTargetNPC ? null : false;
	}
}
