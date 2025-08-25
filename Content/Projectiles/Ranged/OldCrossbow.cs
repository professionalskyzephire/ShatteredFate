using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Projectiles.Ranged
{
	public class OldCrossbow : ModProjectile
	{
		public override void SetStaticDefaults() {
			Main.projFrames[Type] = 3;
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}
		public override void SetDefaults() {
			Projectile.width = 64;
			Projectile.height = 28;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 2;
			Projectile.hide = true;
			Projectile.netImportant = true;
			Projectile.DamageType = DamageClass.Ranged;
		}
		public override void AI() {
			Player player = Main.player[Projectile.owner];
			if(player.HeldItem.ModItem is not Content.Items.Weapons.Ranged.OldCrossbow || player.dead) {
				Projectile.Kill();
				return;
			}
			if(player.whoAmI == Main.myPlayer) {
				Projectile.velocity = Vector2.Normalize(Main.MouseWorld - player.MountedCenter);
				NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
			}
			if(Projectile.velocity.X != 0) player.direction = Projectile.velocity.X > 0 ? 1 : -1;
			Projectile.spriteDirection = Projectile.direction = player.direction;
			player.itemRotation = Projectile.rotation = (Projectile.velocity * Projectile.direction).ToRotation();
			if(player.HeldItem.ModItem is Content.Items.Weapons.Ranged.OldCrossbow) {
				Projectile.timeLeft = 2;
				bool hasAmmo = player.PickAmmo(player.HeldItem, out int projToShoot, out float speed, out int Damage, out float KnockBack, out int usedAmmoItemId, Projectile.localAI[0] > 1 || Projectile.ai[1] > 0 || Projectile.ai[2] == 0f);
				if(Projectile.localAI[0] > 0) {
					Projectile.localAI[0]--;
					player.itemTime = player.itemAnimation = 2;
				}
				if(hasAmmo && Projectile.ai[1] == 0 && Projectile.localAI[0] <= 0) {
					Projectile.ai[0] = player.HeldItem.useTime;
					Projectile.ai[1] = player.controlUseTile ? 2f : 1f;
				}
				else if(Projectile.ai[0] <= 0 && (player.controlUseItem || (player.controlUseTile && Projectile.ai[2] == 0f)) && Projectile.ai[1] > 0) {
					if(Projectile.ai[2] == 1f && Main.myPlayer == player.whoAmI) if(Projectile.ai[1] == 1f) {
						int p = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, usedAmmoItemId), Projectile.Center, Vector2.Normalize(Projectile.velocity) * speed / 4f, ModContent.ProjectileType<Content.Projectiles.Ranged.OldCrossbowBolt>(), Damage, KnockBack, Projectile.owner, Projectile.ai[1]);
						NetMessage.SendData(27, -1, -1, null, p);
					}
					else for(int i = -2; i <= 2; i++) {
						int p = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, usedAmmoItemId), Projectile.Center, Vector2.Normalize(Projectile.velocity).RotatedBy(i * MathHelper.PiOver4 * 0.2f) * speed / 4f, ModContent.ProjectileType<Content.Projectiles.Ranged.OldCrossbowBolt>(), Damage, KnockBack, Projectile.owner, Projectile.ai[1]);
						NetMessage.SendData(27, -1, -1, null, p);
					}
					Projectile.localAI[0] = (int)((float)player.HeldItem.useTime / 3f);
					hasAmmo &= Projectile.ai[2] == 1f;
					Projectile.ai[2] = 0f;
					if(hasAmmo) hasAmmo = player.PickAmmo(player.HeldItem, out projToShoot, out speed, out Damage, out KnockBack, out usedAmmoItemId, false);
					if(hasAmmo) SoundEngine.PlaySound(Projectile.ai[1] == 1f ? SoundID.Item102 : SoundID.DD2_BallistaTowerShot, player.position);
					Projectile.ai[1] = 0f;
				}
				if(Projectile.ai[0] > 0) if(--Projectile.ai[0] == 0f) Projectile.ai[2] = 1f;
				float reloadTime = (Projectile.ai[0] / (float)player.HeldItem.useTime) * 4f;
				float reloadHand = 0f;
				if(Projectile.ai[0] <= (float)(player.HeldItem.useTime / 4)) {
					if(Projectile.ai[1] <= 0) Projectile.frame = 0;
					Projectile.rotation += MathHelper.Lerp(0, Projectile.rotation * 0.15f, reloadTime);
				}
				else if(Projectile.ai[2] == 0f && !player.controlUseItem && !player.controlUseTile) {
					Projectile.frame = player.itemTime = player.itemAnimation = 0;
					Projectile.ai[0] = Projectile.localAI[0] = 0f;
				}
				else {
					reloadTime--;
					if(reloadTime >= 2f) SoundEngine.PlaySound(SoundID.Item149, player.position);
					player.compositeBackArm.stretch = (Player.CompositeArmStretchAmount)(int)reloadTime;
					reloadTime /= 3f;
					Projectile.frame = (int)MathHelper.Lerp(Main.projFrames[Projectile.type], 0f, reloadTime);
					Projectile.rotation *= MathHelper.SmoothStep(0.15f, 0, reloadTime);
					if(reloadTime < 0.1f) Projectile.rotation += MathHelper.SmoothStep(MathHelper.PiOver4 * player.direction * player.gravDir, 0f, MathHelper.Clamp(reloadTime, 0f, 0.1f) * 10f);
					else if(reloadTime > 0.9f) Projectile.rotation += MathHelper.SmoothStep(MathHelper.PiOver4 * player.direction * player.gravDir, 0f, MathHelper.Clamp(reloadTime - 0.9f, 0f, 0.1f) * 10f);
					else if(reloadTime > 0f) Projectile.rotation += MathHelper.PiOver4 * player.direction * player.gravDir;
					reloadHand *= MathHelper.SmoothStep(0.15f, 0, reloadTime);
					reloadHand += MathHelper.PiOver4 * player.direction * player.gravDir;
				}
				player.heldProj = Projectile.whoAmI;
				player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation * player.gravDir) - (Projectile.spriteDirection * MathHelper.PiOver2));
				player.SetCompositeArmBack(enabled: true, reloadHand != 0f ? player.compositeBackArm.stretch : Player.CompositeArmStretchAmount.Full, ((Projectile.rotation * 1.15f - reloadHand) * player.gravDir) - (Projectile.spriteDirection * MathHelper.PiOver2));
				Projectile.position = (new Vector2(player.Center.X - Projectile.width / 2 - (float)(Projectile.spriteDirection * 2), player.MountedCenter.Y - Projectile.height / 2)) - (new Vector2(2, ((player.bodyFrame.Y >= player.bodyFrame.Height * 7 && player.bodyFrame.Y <= player.bodyFrame.Height * 9) || (player.bodyFrame.Y >= player.bodyFrame.Height * 14 && player.bodyFrame.Y <= player.bodyFrame.Height * 16) ? 4 : 2)) * player.Directions) + new Vector2(Projectile.spriteDirection * 26, (player.gravDir == 1 ? -3 : 5) - 1).RotatedBy(Projectile.rotation);
			}
		}
		public override bool PreDraw(ref Color lightColor) {
			Player player = Main.player[Projectile.owner];
			lightColor *= player.stealth;
			SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			if(player.gravDir == -1) spriteEffects |= SpriteEffects.FlipVertically;
			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, texture.Width, texture.Height / Main.projFrames[Projectile.type]), lightColor, Projectile.rotation, new Vector2(texture.Width, texture.Height / Main.projFrames[Projectile.type]) * 0.5f, Projectile.scale, spriteEffects, 0);
			if(Projectile.ai[0] < player.HeldItem.useTime / 4 && Projectile.ai[1] == 0f) return false;
			if(Projectile.frame == 0) return false;
			texture = (Texture2D)ModContent.Request<Texture2D>(Projectile.ai[1] == 2f ? "ShatteredFate/Content/Projectiles/CrossbowBolt_Alt" : "ShatteredFate/Content/Projectiles/CrossbowBolt");
			Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(MathHelper.Lerp(20f, 12f, (Projectile.frame + 1) / 3f) * Projectile.spriteDirection, -4 * Main.player[Projectile.owner].gravDir).RotatedBy(Projectile.rotation) * Projectile.scale - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, spriteEffects, 0);
			if(Projectile.ai[1] == 2f) for(int i = 0; i < 4; i++) Main.EntitySpriteDraw(texture, Projectile.Center + Vector2.UnitY.RotatedBy(i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly) * 3f + new Vector2(MathHelper.Lerp(20f, 12f, (Projectile.frame + 1) / 3f) * Projectile.spriteDirection, -4 * Main.player[Projectile.owner].gravDir).RotatedBy(Projectile.rotation) * Projectile.scale - Main.screenPosition, null, Color.Gold with {A = 0} * 0.2f, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, spriteEffects, 0);
			return false;
		}
		public override bool? CanDamage() => false;
		public override bool ShouldUpdatePosition() => false;
	}
}