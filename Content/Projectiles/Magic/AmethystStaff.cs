using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Projectiles.Magic
{
	public class AmethystStaff : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<SFReworksConfig>().GemStaves;
		public override string Texture => "ShatteredFate/Content/Items/Weapons/Magic/AmethystStaff";
		public override void SetDefaults() {
			Projectile.width = Projectile.height = 1;
			Projectile.timeLeft = 600;
			Projectile.alpha = 255;
			Projectile.aiStyle = -1;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
		}
		public override void AI() {
			if(Projectile.alpha > 0) Projectile.alpha -= 51;
			Player player = Main.player[Projectile.owner];
			if(player.HeldItem.type != ItemID.AmethystStaff) {
				Projectile.Kill();
				return;
			}
			if(Main.myPlayer == Projectile.owner) {
				Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center);
				if(player.itemTime == player.itemTimeMax - 1) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Vector2.Normalize(Projectile.velocity) * 22f, Projectile.velocity * player.HeldItem.shootSpeed, (int)Projectile.ai[0], (int)(Projectile.damage * 1.5), Projectile.knockBack, player.whoAmI));
				NetMessage.SendData(27, -1, -1, null, Projectile.whoAmI);
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * 1.25f * player.direction * player.gravDir - MathHelper.ToRadians(player.direction * player.gravDir) * 0.25f;
			if(player.direction < 0) Projectile.rotation += MathHelper.Pi;
			Projectile.position += (player.Top - player.Directions * 16f - Projectile.Center) * 0.1f;
		}
		public override bool PreDraw(ref Color lightColor) {
			Player player = Main.player[Projectile.owner];
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			for(int i = 0; i < 3; i++) Main.EntitySpriteDraw(texture, Projectile.Center + Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * MathHelper.Pi * player.direction * player.gravDir + MathHelper.TwoPi / 3f * i) - Main.screenPosition, null, Color.Purple with {A = 0} * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * player.HeldItem.scale, player.direction > 0 ? player.gravDir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically : player.gravDir > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0);
			return false;
		}
		public override void OnKill(int timeLeft) => Main.player[Projectile.owner].AddBuff(ModContent.BuffType<Content.Buffs.Debuffs.AmethystStaffCooldown>(), 900);
		public override bool ShouldUpdatePosition() => false;
	}
}
