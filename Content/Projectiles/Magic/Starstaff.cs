using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Projectiles.Magic
{
	public class Starstaff : ModProjectile
	{
		public override string Texture => "ShatteredFate/Content/Projectiles/StarstaffStar";
		public override string GlowTexture => "Terraria/Images/Extra_98";
		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults() {
			Projectile.width = Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.timeLeft = 600;
			Projectile.aiStyle = -1;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.penetrate = -1;
		}
		public override void AI() {
			if(Projectile.ai[0] == -2f) {
				Projectile.velocity *= 0f;
				Projectile.extraUpdates = 0;
				if(Projectile.timeLeft > 5) Projectile.timeLeft = 5;
				Projectile.alpha += 51;
				Projectile.Center = Main.player[Projectile.owner].Bottom + new Vector2(32, -56) * Main.player[Projectile.owner].Directions * Main.player[Projectile.owner].HeldItem.scale;
				return;
			}
			else if(Projectile.ai[0] == -1f) {
				Projectile.velocity *= 0f;
				Projectile.extraUpdates = 0;
				if(Projectile.timeLeft > 5) {
					Projectile.timeLeft = 5;
					Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4, Projectile.Bottom);
				}
				Projectile.alpha += 51;
				return;
			}
			else if(Projectile.ai[0] == 2f && Main.myPlayer == Projectile.owner) {
				Projectile.velocity += Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 0.35f;
				if(Projectile.Distance(Main.MouseWorld) > 16f) Projectile.velocity *= 0.95f;
				if(Projectile.ai[2] != 0f) Projectile.position += Projectile.velocity.RotatedBy(MathHelper.PiOver2) * (float)System.Math.Sin(MathHelper.Pi * Projectile.timeLeft * 0.1f) * Projectile.ai[2];
				NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
			}
			else if(Projectile.ai[2] != 0f) Projectile.position += Projectile.velocity.RotatedBy(MathHelper.PiOver2) * (float)System.Math.Sin(MathHelper.Pi * Projectile.timeLeft * 0.1f) * Projectile.ai[2];
			Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3());
			if(Projectile.ai[1] != -1f && Projectile.Bottom.Y > Projectile.ai[1] && !Projectile.tileCollide) Projectile.tileCollide = !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);
			Projectile.rotation += Projectile.direction * 0.2f;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			if(Main.myPlayer == Projectile.owner) {
				Projectile.ai[0] = -1f;
				NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
			}
		}
		public override bool OnTileCollide(Vector2 oldVelocity) {
			if(Main.myPlayer == Projectile.owner) {
				Projectile.velocity *= 0f;
				Projectile.ai[0] = -1f;
				NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
			}
			return false;
		}
		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = (Texture2D)(Texture2D)ModContent.Request<Texture2D>(GlowTexture);
			for(int i = 1; i < Projectile.oldPos.Length; i++) Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Lerp(new Color(255, 0, 255, 0), new Color(0, 255, 255, 0), (float)i / (float)Projectile.oldPos.Length) * MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length) * Projectile.Opacity, (Projectile.oldPos[i] - Projectile.oldPos[i - 1]).ToRotation() + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * new Vector2(MathHelper.Lerp(1.6f, 0.05f, (float)i / (float)Projectile.oldPos.Length), i < 3 ? 1.6f - (3 - i) * 0.4f : 1.6f) * (Projectile.timeLeft > 5 ? 1f : (1f - (float)Projectile.timeLeft / 5f) * 2f), SpriteEffects.None, 0);
			texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			for(int i = 1; i < Projectile.oldPos.Length; i++) if(Projectile.oldPos[i] != Projectile.oldPos[i - 1]) Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.White with {A = 0} * MathHelper.Lerp(0.75f, 0f, (float)i / (float)Projectile.oldPos.Length) * Projectile.Opacity, Projectile.oldRot[i] - MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * MathHelper.Lerp(1f, 0.2f, (float)i / (float)Projectile.oldPos.Length), SpriteEffects.None, 0);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, texture.Width, texture.Height / Main.projFrames[Projectile.type]), lightColor * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale * (Projectile.timeLeft > 5 ? 1f : (1f - (float)Projectile.timeLeft / 5f) * 2f), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
			for(int i = 0; i < 3; i++) Main.EntitySpriteDraw(texture, Projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 3f + Main.GlobalTimeWrappedHourly) * 3f * (Projectile.timeLeft > 5 ? 1f : (1f - (float)Projectile.timeLeft / 5f) * 2f) - Main.screenPosition, null, Color.White with {A = 0} * 0.3f * Projectile.Opacity, Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * (Projectile.timeLeft > 5 ? 1f : (1f - (float)Projectile.timeLeft / 5f) * 2f), SpriteEffects.None, 0);
			return false;
		}
	}
}
