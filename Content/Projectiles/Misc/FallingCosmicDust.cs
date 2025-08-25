using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;

namespace ShatteredFate.Content.Projectiles.Misc
{
	public class FallingCosmicDust : ModProjectile
	{
		public override string Texture => "ShatteredFate/Content/Items/Materials/CosmicDust";
		public override string GlowTexture => "Terraria/Images/Extra_98";
		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.hostile = Main.remixWorld;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.alpha = 0;
			Projectile.light = 1f;
			Projectile.extraUpdates = 1;
		}
		public override void AI() {
			int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 278, 0f, 0f, 0, Color.Cyan, 0.5f);
			Main.dust[d].velocity += Projectile.velocity.RotatedBy(MathHelper.PiOver2) * 0.2f;
			Main.dust[d].position += Main.dust[d].velocity;
			Main.dust[d].noGravity = true;
			d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 278, 0f, 0f, 0, Color.Pink, 0.5f);
			Main.dust[d].velocity -= Projectile.velocity.RotatedBy(MathHelper.PiOver2) * 0.2f;
			Main.dust[d].position += Main.dust[d].velocity;
			Main.dust[d].noGravity = true;
			Projectile.rotation = Projectile.velocity.ToRotation();
			if(Projectile.velocity != Vector2.Zero) return;
			if(Projectile.position == Projectile.oldPos[Projectile.oldPos.Length - 1]) Projectile.Kill();
			if(Main.netMode == 2) return;
			Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(80f, 80f);
			Color color = Projectile.Center.X < spawnPos.X ? Color.Magenta : Color.Cyan;
			for(int i = 0; i < 4; i++) ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings { PositionInWorld = spawnPos, MovementVector = Vector2.UnitX.RotatedBy(MathHelper.PiOver2 * i) * float.Epsilon, UniqueInfoPiece = (byte)(Main.rgbToHsl(color).X * 255f)});
		}
		public override void OnKill(int timeLeft) {
			NetMessage.SendData(21, -1, -1, null, Item.NewItem(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, 0, 0, ModContent.ItemType<Content.Items.Materials.CosmicDust>(), 1, false, 0, false, false));
		}
		public override bool OnTileCollide(Vector2 oldVelocity) {
			Projectile.velocity *= 0f;
			return false;
		}
		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = (Texture2D)(Texture2D)ModContent.Request<Texture2D>(GlowTexture);
			for(int i = 1; i < Projectile.oldPos.Length; i++) Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Lerp(new Color(0, 255, 255, 0), new Color(255, 0, 255, 0), (float)i / (float)Projectile.oldPos.Length) * MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length), Projectile.oldRot[i] + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * new Vector2(MathHelper.Lerp(1.6f, 0.05f, (float)i / (float)Projectile.oldPos.Length), i < 3 ? 1.6f - (3 - i) * 0.4f : 1.6f), SpriteEffects.None, 0);
			texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			for(int i = 1; i < Projectile.oldPos.Length; i++) Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.White with {A = 0} * MathHelper.Lerp(0.75f, 0f, (float)i / (float)Projectile.oldPos.Length), Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * new Vector2(MathHelper.Lerp(1f, 0.3f, (float)i / (float)Projectile.oldPos.Length), 1f), SpriteEffects.None, 0);
			for(int i = 0; i < 3; i++) Main.EntitySpriteDraw(texture, Projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 3f + Main.GlobalTimeWrappedHourly) * 3f - Main.screenPosition, null, Color.White with {A = 0} * 0.3f, Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White with {A = 0}, Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
