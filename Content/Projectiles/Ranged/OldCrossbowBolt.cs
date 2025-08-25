using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Projectiles.Ranged
{
	public class OldCrossbowBolt : ModProjectile
	{
		public override string Texture => "ShatteredFate/Content/Projectiles/CrossbowBolt";
		public override string GlowTexture => "ShatteredFate/Content/Projectiles/CrossbowBolt_Alt";
		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults() {
			Projectile.width = Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.timeLeft = 900;
			Projectile.aiStyle = -1;
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 2;
			Projectile.penetrate = 5;
			Projectile.DamageType = DamageClass.Ranged;
		}
		public override void AI() {
			if(Projectile.timeLeft == 899) if(Projectile.ai[0] == 2f) Projectile.ai[1] = -1f;
			else Projectile.penetrate = -1;
			if(Projectile.ai[1] > 0f) {
				NPC npc = Main.npc[(int)Projectile.ai[1] - 1];
				if(!npc.active) Projectile.ai[1] = -1f;
				else if(npc.position != npc.oldPosition) Projectile.position += (npc.position - npc.oldPosition) / Projectile.MaxUpdates;
			}
			else if(ShouldUpdatePosition() && Projectile.ai[1] == -1f) {
				Projectile.velocity.Y += 0.12f;
				Projectile.velocity.Y *= 0.98f;
			}
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.spriteDirection = Projectile.direction;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			if(Main.myPlayer == Projectile.owner && Projectile.ai[1] == 0f) {
				Projectile.ai[1] = target.whoAmI + 1;
				NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
			}
		}
		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("Terraria/Images/Extra_98");
			if(Projectile.ai[1] <= 0f) for(int i = 1; i < Projectile.oldPos.Length; i++) if(Projectile.oldPos[i] != Projectile.oldPos[i - 1]) Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Gold with {A = 0} * MathHelper.Lerp(0.25f * Projectile.ai[0], 0f, (float)i / (float)Projectile.oldPos.Length), Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * new Vector2(MathHelper.Lerp(0.8f, 0.2f, (float)i / (float)Projectile.oldPos.Length), 1f), SpriteEffects.None, 0);
			texture = (Texture2D)ModContent.Request<Texture2D>(Projectile.ai[0] == 2f ? GlowTexture : Texture);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, texture.Width, texture.Height / Main.projFrames[Projectile.type]), lightColor, Projectile.rotation, new Vector2(texture.Width - 7, texture.Height * 0.5f), Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
			return false;
		}
		public override bool? CanDamage() => Projectile.ai[1] <= 0f ? null : false; 
		public override bool ShouldUpdatePosition() => !Collision.SolidCollision(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height) && Projectile.ai[1] <= 0f;
	}
}