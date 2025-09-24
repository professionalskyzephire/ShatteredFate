using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Projectiles.Minions
{
	public class AncientSkullMinion : ModProjectile
	{
		public override void SetStaticDefaults() => Main.projFrames[Type] = 5;
		public override void SetDefaults() {
			Projectile.width = Projectile.height = 24;
			Projectile.timeLeft = 1200;
			Projectile.alpha = 255;
			Projectile.aiStyle = -1;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
		}
		public override void AI() {
			if(Projectile.alpha > 0) Projectile.alpha -= 51;
			if(++Projectile.frameCounter >= 20) Projectile.frameCounter = 0;
			Projectile.frame = Projectile.frameCounter / 4;
			if(Projectile.velocity.X != 0f) Projectile.spriteDirection = System.Math.Sign(Projectile.velocity.X);
			Projectile.extraUpdates = (int)Projectile.ai[0];
			if(Projectile.ai[0] > 0f) {
				int d = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 278, 0f, 0f, 0, new Color(99, 255, 236), Projectile.scale * 0.6f);
				Main.dust[d].velocity = -Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-10, 11) * 0.2f)) * 0.2f;
				Main.dust[d].noGravity = true;
				Projectile.rotation = Projectile.velocity.ToRotation();
				if(Projectile.spriteDirection < 0) Projectile.rotation += MathHelper.Pi;
				Projectile.tileCollide = true;
			}
			else {
				int target = -1;
				float maxRange = 480f;
				Player player = Main.player[Projectile.owner];
				if(!player.channel || player.HeldItem.ModItem is not Content.Items.Weapons.Summon.AncientSkull) foreach(NPC npc in Main.ActiveNPCs) if(npc.CanBeChasedBy(Projectile) && npc.Distance(Projectile.Center) < maxRange) {
					target = npc.whoAmI;
					maxRange = npc.Distance(Projectile.Center);
				}
				if(target > -1) {
					Projectile.velocity = Vector2.Normalize(Main.npc[target].Center + Main.npc[target].velocity - Projectile.Center) * 16f;
					Projectile.ai[0] = 1f;
					Projectile.netUpdate = true;
					SoundEngine.PlaySound(SoundID.Item46, Projectile.Center);
					return;
				}
				foreach(Projectile p in Main.ActiveProjectiles) if(p.type == Type && p.owner == Projectile.owner && p.Hitbox.Intersects(Projectile.Hitbox) && p.whoAmI != Projectile.whoAmI) {
					Projectile.velocity += Vector2.Normalize(Projectile.Center - p.Center);
					break;
				}
				Projectile.velocity += Vector2.Normalize(player.Center - Projectile.Center) * 0.22f;
				if(player.Distance(Projectile.Center) > 32f) Projectile.velocity *= 0.98f;
			}
		}
		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, texture.Height / Main.projFrames[Type]), Color.White * Projectile.Opacity, Projectile.rotation, new Vector2(texture.Width, texture.Height / Main.projFrames[Type]) * 0.5f, Projectile.scale, Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			return false;
		}
		public override void OnKill(int timeLeft) {
			if(Main.player[Projectile.owner].ownedProjectileCounts[Type] == 1) Main.player[Projectile.owner].AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.AncientSkullCooldown>(), 1200);
			SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.Center);
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
			for(int i = 0; i < 36; i++) {
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 278, 0f, 0f, 0, new Color(99, 255, 236), Projectile.scale);
				Main.dust[d].velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-10, 11) * 0.2f + i) * 10f) * Main.rand.Next(5, 21) * 0.02f;
				Main.dust[d].noGravity = true;
			}
			Projectile.Center = Projectile.position;
			Projectile.Size *= 2f;
			Projectile.Damage();
		}
		public override bool? CanDamage() => Projectile.ai[0] == 1f ? null : false;
	}
}