using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;

namespace ShatteredFate.Content.Projectiles.Magic
{
	public class LargeSapphire : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<SFReworksConfig>().GemStaves;
		public override string Texture => "Terraria/Images/Item_1524";
		public override void SetDefaults() {
			Projectile.width = Projectile.height = 32;
			Projectile.timeLeft = 600;
			Projectile.alpha = 255;
			Projectile.aiStyle = -1;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
		}
		public override void AI() {
			foreach(Projectile projectile in Main.ActiveProjectiles) if((Projectile.ai[1] >= 10f || projectile.type == (int)Projectile.ai[0]) && projectile.whoAmI != Projectile.whoAmI && projectile.Hitbox.Intersects(Projectile.Hitbox)) {
				if(Projectile.ai[1] < 10f) Projectile.ai[1]++;
				else {
					for(int i = 0; i < 10; i++) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_ItemUse(Main.player[Projectile.owner].HeldItem), Projectile.Center, Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.Next(80, 110) * 0.1f, (int)Projectile.ai[0], Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f, 1f));
					Projectile.Kill();
				}
				projectile.Kill();
			}
			if(Projectile.Distance(Main.player[Projectile.owner].Center) > 160) Projectile.velocity += Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * 0.34f;
			Projectile.velocity *= 0.96f;
			if(Projectile.alpha > 0) Projectile.alpha -= 15;
		}
		public override void OnKill(int timeLeft) {
			for(int i = 0; i < 20; i++) {
				Vector2 spawnPos = (Projectile.rotation + Main.rand.Next(-100, 101) * 0.001f + i / 20f * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(120, 160) * 0.1f;
				ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center, MovementVector = spawnPos * 0.4f, UniqueInfoPiece = (byte)(Main.rgbToHsl(Color.Blue).X * 255f)});
			}
			Terraria.Audio.SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
			Main.player[Projectile.owner].AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.SapphireStaffCooldown>(), 600);
		}
		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			for(int i = 0; i < 3; i++) Main.EntitySpriteDraw(texture, Vector2.Lerp(Main.player[Projectile.owner].Center, Projectile.Center, Projectile.Opacity) + Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * MathHelper.Pi + MathHelper.TwoPi / 3f * i) * Projectile.ai[1] * 0.1f - Main.screenPosition, null, Color.White with {A = 0} * Projectile.Opacity * 0.5f, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
