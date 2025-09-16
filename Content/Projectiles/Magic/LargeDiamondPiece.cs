using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Projectiles.Magic
{
	public class LargeDiamondPiece : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<SFReworksConfig>().GemStaves;
		public override string Texture => "Terraria/Images/Item_182";
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
			for(int i = 0; i < 3; i++) Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White with {A = 0} * Projectile.Opacity * 0.5f, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
		public override void AI() {
			if(Projectile.alpha > 0) Projectile.alpha -= 17;
			Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
		}
	}
}