using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Projectiles.Magic
{
	public class LargeEmeraldPiece : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<SFReworksConfig>().GemStaves;
		public override string Texture => "Terraria/Images/Item_179";
		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults() {
			Projectile.width = Projectile.height = 12;
			Projectile.friendly = true;
			Projectile.timeLeft = 300;
			Projectile.alpha = 255;
			Projectile.aiStyle = -1;
			Projectile.tileCollide = true;
			Projectile.DamageType = DamageClass.Magic;
		}
		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			for(int i = 1; i < Projectile.oldPos.Length; i++) Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.White with {A = 0} * Projectile.Opacity * MathHelper.Lerp(0.5f, 0f, (float)i / (float)Projectile.oldPos.Length), Projectile.oldRot[i], texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			for(int i = 0; i < 3; i++) Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White with {A = 0} * Projectile.Opacity * 0.5f, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
		public override void AI() {
			if(Projectile.alpha > 0) Projectile.alpha -= 17;
			Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
		}
		public override bool? CanDamage() => Projectile.alpha == 0 ? null : false;
	}
}