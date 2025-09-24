using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;

namespace ShatteredFate.Content.Projectiles.Magic
{
	public class SandSpell : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_656";
		public override void SetDefaults() {
			Projectile.CloneDefaults(656);
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 1200;
			Projectile.alpha = 255;
		}
		public override bool PreAI() {
			if(Projectile.timeLeft < 6 && Projectile.ai[2] == 2f) {
				Projectile.alpha += 51;
				Projectile.rotation += 0.15f;
				if(Projectile.rotation > MathHelper.Pi) Projectile.rotation -= MathHelper.TwoPi;
				return false;
			}
			return true;
		}
		public override void AI() {
			if(Projectile.ai[2] == 1f) {
				Projectile.timeLeft = 5;
				Projectile.ai[2] = 2f;
			}
			else if(Projectile.alpha > 0) Projectile.alpha -= 17;
			else {
				Player player = Main.player[Projectile.owner];
				float pullDist = 160f;
				foreach(Item item in Main.ActiveItems) if(!item.beingGrabbed && getPull(item.Center) < pullDist) item.velocity += (getPullOrigin(item.Center) - item.Center).SafeNormalize(Vector2.Zero) * (1f - getPull(item.Center) / pullDist);
				foreach(NPC npc in Main.ActiveNPCs) if(npc.knockBackResist > 0f && getPull(npc.Center) < pullDist) npc.velocity += (getPullOrigin(npc.Center) - npc.Center).SafeNormalize(Vector2.Zero) * npc.knockBackResist * (1f - getPull(npc.Center) / pullDist);
				if(player.HeldItem.ModItem is not ShatteredFate.Content.Items.Weapons.Magic.SandSpell || !player.channel || Projectile.timeLeft == 2) Projectile.ai[2] = 1f;
			}
			Projectile.rotation += 0.15f;
			if(Projectile.rotation > MathHelper.Pi) Projectile.rotation -= MathHelper.TwoPi;
		}
		private float getPull(Vector2 center) {
			if(Math.Abs(center.Y - Projectile.Center.Y) < Projectile.height / 2) return Math.Abs(center.X - Projectile.Center.X);
			return Vector2.Distance(getPullOrigin(center), center);
		}
		private Vector2 getPullOrigin(Vector2 center) {
			if(Math.Abs(center.Y - Projectile.Center.Y) < Projectile.height / 2) return new Vector2(Projectile.Center.X, center.Y);
			else if(center.Y > Projectile.Bottom.Y) return Projectile.Bottom;
			else if(center.Y < Projectile.Top.Y) return Projectile.Top;
			return Projectile.Center;
		}
		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			lightColor = Color.Peru with {A = 0} * Projectile.Opacity;
			for(int k = 0; k < 90; k++) Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(Vector2.UnitX.RotatedBy(Projectile.rotation + k * 0.3f).X * 3f, MathHelper.Lerp(-216f, 216f, (float)k / 90f)) - Main.screenPosition, null, lightColor * MathHelper.Lerp(0.1f, 0.4f, Vector2.UnitX.RotatedBy(MathHelper.Pi * (float)k / 90f).Y), Projectile.rotation + k * 0.1f, texture.Size() * 0.5f, Projectile.scale * MathHelper.Lerp(1.4f, 0.6f, (float)k / 90f), SpriteEffects.None, 0);
			return false;
		}
		public override bool ShouldUpdatePosition() => false;
	}
}