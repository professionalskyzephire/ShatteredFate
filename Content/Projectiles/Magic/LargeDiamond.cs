using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace ShatteredFate.Content.Projectiles.Magic
{
	public class LargeDiamond : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<SFReworksConfig>().GemStaves;
		public override string Texture => "Terraria/Images/Item_1527";
		public override void SetDefaults() {
			Projectile.width = Projectile.height = 44;
			Projectile.timeLeft = 2;
			Projectile.alpha = 255;
			Projectile.aiStyle = -1;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = 5;
		}
		public override void AI() {
			int i = Main.player[Projectile.owner].immuneTime;
			if(i > Projectile.localAI[0]) Projectile.penetrate--;
			if(i != Projectile.localAI[0]) Projectile.localAI[0] = i;
			Projectile.timeLeft = 2;
			Projectile.Center = Main.player[Projectile.owner].Center;
			if(Projectile.alpha > 0) Projectile.alpha -= 17;
  			if(Main.player[Projectile.owner].dead) Projectile.Kill();
		}
		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			for(int i = 0; i < Projectile.Opacity * 4f; i++) Main.EntitySpriteDraw(texture, Projectile.Center - Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i) * MathHelper.Max(((1f - Projectile.Opacity) * 4f - (3 - i)), 0f) * 64f - Main.screenPosition, null, Color.White with {A = 0} * (1f - MathHelper.Max(((1f - Projectile.Opacity) * 4f - (3 - i)), 0f)), MathHelper.PiOver2 * i, new Vector2(texture.Width / 2, texture.Height), Projectile.scale * 1.25f, SpriteEffects.None, 0);
			return false;
		}
		public override void OnKill(int timeLeft) {
			Main.player[Projectile.owner].immune = true;
			Main.player[Projectile.owner].immuneAlpha = 0;
			Main.player[Projectile.owner].immuneTime = 30; 
			for(int i = 0; i < Main.rand.Next(15, 21); i++) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.Next(120, 160) * 0.1f, ModContent.ProjectileType<LargeDiamondPiece>(), Projectile.damage, Projectile.knockBack, Projectile.owner));
			Terraria.Audio.SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);
	}

}
