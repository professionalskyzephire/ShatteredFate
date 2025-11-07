using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Enums;

namespace ShatteredFate.Content.Projectiles.Melee 
{
	public class AntiqueDaggerProjectile : ModProjectile 
	{
		public override string GlowTexture => "Terraria/Images/Extra_98";
		public override void SetDefaults() {
			Projectile.Size = new Vector2(22);
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.scale = 1f;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.extraUpdates = 1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 360;
		}
		public override void AI() {
			if(Projectile.velocity.X != 0) Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
			if(Projectile.ai[0] > 0f) {
				if(Projectile.ai[0] < 30f) Projectile.ai[0]++;
				if(Projectile.ai[2] > 0f) {
					NPC npc = Main.npc[(int)Projectile.ai[2] - 1];
					Projectile.position += (npc.position - npc.oldPosition) / Projectile.MaxUpdates;
					if(Projectile.timeLeft > 60) Projectile.timeLeft = 60;
				}
				if(Projectile.timeLeft < 60) Projectile.Opacity *= Projectile.timeLeft / 30f;
				else if(Projectile.ai[2] == 0f) {
					Projectile.velocity.Y += 0.11f;
					Projectile.velocity *= 0.99f;
					if(Collision.SolidCollision(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height)) Projectile.timeLeft = 60;
				}
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
				return;
			}
			Projectile.ownerHitCheck = true;
			Projectile.hide = true;
			Player player = Main.player[Projectile.owner];
			if (++Projectile.ai[1] > player.itemAnimationMax * Projectile.MaxUpdates) {
				Projectile.Kill();
				return;
			}
			else player.heldProj = Projectile.whoAmI;
			float attackProgress = (float)Math.Sin(MathHelper.Pi * Projectile.ai[1] / (float)(player.itemAnimationMax * Projectile.MaxUpdates));
			Projectile.Opacity = MathHelper.Min(1f, attackProgress * 3f);
			Vector2 playerCenter = player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation);
			if(attackProgress < 0.2f) player.compositeFrontArm.stretch = Player.CompositeArmStretchAmount.Quarter;
			else if(attackProgress < 0.5f) player.compositeFrontArm.stretch = Player.CompositeArmStretchAmount.ThreeQuarters;
			else player.compositeFrontArm.stretch = Player.CompositeArmStretchAmount.Full;
			Projectile.Center = playerCenter + Projectile.velocity.SafeNormalize(Projectile.oldVelocity) * MathHelper.SmoothStep(-2f, 5f, attackProgress) * 2.5f * Projectile.scale;
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			if(Main.myPlayer == Projectile.owner) if(Projectile.ai[0] > 0f) {
				Projectile.ai[2] = target.whoAmI + 1;
				NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
			}
			else target.AddBuff(ModContent.BuffType<ShatteredFate.Content.Buffs.Debuffs.AntiqueDaggerDoT>(), 60);
		}
		public override void CutTiles() {
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 start = Projectile.Center;
			Vector2 end = start;
			Vector2 movingAlong = Projectile.velocity.SafeNormalize(Projectile.oldVelocity) * Projectile.scale;
			if(Projectile.ai[0] > 0f) {
				start -= movingAlong * 21f;
				end += movingAlong * 15f;
			}
			else end += movingAlong * 48f;
			Utils.PlotTileLine(start, end, 10f * Projectile.scale, DelegateMethods.CutTiles);
		}
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			Vector2 start = Projectile.Center;
			Vector2 end = start;
			Vector2 movingAlong = Projectile.velocity.SafeNormalize(Projectile.oldVelocity) * Projectile.scale;
			if(Projectile.ai[0] > 0f) {
				start -= movingAlong * 21f;
				end += movingAlong * 15f;
			}
			else end += movingAlong * 48f;
			float collisionPoint = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 10f * Projectile.scale, ref collisionPoint);
		}
		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			bool flying = Projectile.ai[0] > 0f;
			float progress = Projectile.ai[0] > 0f ? Projectile.ai[0] / 30f : Projectile.ai[1] / (float)(Main.player[Projectile.owner].itemAnimationMax * Projectile.MaxUpdates);
			float attackTimer = (float)Math.Sin(MathHelper.Pi * progress);
			if(Projectile.ai[0] == 0f && !flying) for(int i = 0; i < 4; i++) Main.EntitySpriteDraw(texture, Projectile.Center + Vector2.UnitY.RotatedBy(i * MathHelper.PiOver2) * 3f * attackTimer - Main.screenPosition, null, Color.Gold with {A = 0} * attackTimer * 0.4f, Projectile.rotation, new Vector2(15, 50), Projectile.scale, SpriteEffects.None, 0);
			else if(Projectile.timeLeft < 60) if(Projectile.timeLeft > 30) lightColor = Color.Lerp(Color.Gold with {A = 0}, lightColor, MathHelper.Max(60f, Projectile.timeLeft - 30f) / 30f);
			else lightColor = Color.Lerp(Color.Transparent, Color.Gold with {A = 0}, Projectile.timeLeft / 30f);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, (Projectile.ai[0] > 0f ? new Vector2(texture.Width) * 0.5f : new Vector2(15, 50)), Projectile.scale, SpriteEffects.None, 0);
			if(Projectile.ai[0] > 30f) return false;
			texture = (Texture2D)ModContent.Request<Texture2D>(GlowTexture);
			Main.EntitySpriteDraw(texture, Projectile.Center + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * (flying ? 15f : 50f) * Projectile.scale - Main.screenPosition, null, Color.Gold with {A = 0} * attackTimer * (flying ? 0.7f : 0.4f), MathHelper.PiOver2 * progress * Projectile.spriteDirection - MathHelper.PiOver4, texture.Size() * 0.5f, Projectile.scale * new Vector2(0.2f, 1f), SpriteEffects.None, 0);
			Main.EntitySpriteDraw(texture, Projectile.Center + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * (flying ? 15f : 50f) * Projectile.scale - Main.screenPosition, null, Color.Gold with {A = 0} * attackTimer * (flying ? 0.7f : 0.4f), MathHelper.PiOver2 * progress * Projectile.spriteDirection + MathHelper.PiOver4, texture.Size() * 0.5f, Projectile.scale * new Vector2(0.2f, 1f), SpriteEffects.None, 0);
			return false;
		}
		public override bool? CanDamage() => Projectile.ai[2] > 0f ? false : null;
		public override bool ShouldUpdatePosition() => Projectile.ai[0] > 0f && Projectile.ai[2] == 0f && !Collision.SolidCollision(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height);
	}
}
