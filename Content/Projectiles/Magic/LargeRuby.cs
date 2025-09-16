using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;

namespace ShatteredFate.Content.Projectiles.Magic
{
	public class LargeRuby : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<SFReworksConfig>().GemStaves;
		public override string Texture => "Terraria/Images/Item_1526";
		public override void SetDefaults() {
			Projectile.width = Projectile.height = 32;
			Projectile.timeLeft = 600;
			Projectile.alpha = 255;
			Projectile.aiStyle = -1;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
		}
		public override void AI() {
			if(Projectile.ai[1] > 0f) Projectile.ai[1]--;
			foreach(Projectile projectile in Main.ActiveProjectiles) if(projectile.type == (int)Projectile.ai[0]) if(projectile.Hitbox.Intersects(Projectile.Hitbox)) {
				projectile.ai[2]++;
				projectile.velocity = Vector2.Normalize(projectile.Center - Projectile.Center) * 12f;
				projectile.penetrate = 1;
				Projectile.ai[1] = 10f;
				Projectile.velocity -= projectile.velocity * 0.2f;
			}
			else if(projectile.ai[2] == 0f) {
				projectile.velocity += Vector2.Normalize(Projectile.Center - projectile.Center) * 0.35f;
				projectile.velocity *= 0.95f;
			}
			if(Projectile.Distance(Main.player[Projectile.owner].Center) > 160) Projectile.velocity += Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * 0.34f;
			Projectile.velocity *= 0.96f;
			if(Projectile.alpha > 0) Projectile.alpha -= 15;
		}
		public override void OnKill(int timeLeft) {
			for(int i = 0; i < 20; i++) {
				Vector2 spawnPos = (Projectile.rotation + Main.rand.Next(-100, 101) * 0.001f + i / 20f * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(120, 160) * 0.1f;
				ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = spawnPos * 0.4f, UniqueInfoPiece = (byte)(Main.rgbToHsl(Color.Red).X * 255f)});
			}
			Terraria.Audio.SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
		}
		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			for(int i = 0; i < 3; i++) Main.EntitySpriteDraw(texture, Vector2.Lerp(Main.player[Projectile.owner].Center, Projectile.Center, Projectile.Opacity) + Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * MathHelper.Pi + MathHelper.TwoPi / 3f * i) * (float)System.Math.Sin(Projectile.ai[1] * 0.1f * MathHelper.Pi) * 3f - Main.screenPosition, null, Color.White with {A = 0} * Projectile.Opacity * MathHelper.Lerp(0.5f, 0.4f, Projectile.ai[1] * 0.1f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}