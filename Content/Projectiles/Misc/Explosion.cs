using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Projectiles.Misc;

public class Explosion : ModProjectile
{
    // Empty texture
    public override string Texture => SFMod.VanillaTexture + "Projectile_" + ProjectileID.LostSoulFriendly;

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 300;
        Projectile.timeLeft = 10;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
    }

    public override void OnKill(int timeLeft)
    {
        // Adapted from vanilla code
        SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

        for (int i = 0; i < 30; i++)
        {
            int num616 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Alpha: 100, Scale: 1.5f);
            Main.dust[num616].velocity *= 1.4f;
        }
        
        for (int j = 0; j < 20; j++)
        {
            int num617 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Alpha: 100, Scale: 2.5f);
            Main.dust[num617].noGravity = true;
            Main.dust[num617].velocity *= 7f;
            num617 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Alpha: 100, Scale: 1.5f);
            Main.dust[num617].velocity *= 3f;
        }

        for (int i = 0; i < 2; i++)
        {
            float scaleFactor = 0.4f;
            if (i == 1)
            {
                scaleFactor = 0.8f;
            }

            int num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
            Main.gore[num620].velocity *= scaleFactor;

            Gore gore97 = Main.gore[num620];
            gore97.velocity.X += 1f;

            Gore gore98 = Main.gore[num620];
            gore98.velocity.Y += 1f;

            num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
            Main.gore[num620].velocity *= scaleFactor;

            Gore gore99 = Main.gore[num620];
            gore99.velocity.X -= 1f;

            Gore gore100 = Main.gore[num620];
            gore100.velocity.Y += 1f;

            num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
            Main.gore[num620].velocity *= scaleFactor;

            Gore gore101 = Main.gore[num620];
            gore101.velocity.X += 1f;

            Gore gore102 = Main.gore[num620];
            gore102.velocity.Y -= 1f;

            num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
            Main.gore[num620].velocity *= scaleFactor;

            Gore gore103 = Main.gore[num620];
            gore103.velocity.X -= 1f;

            Gore gore104 = Main.gore[num620];
            gore104.velocity.Y -= 1f;
        }
    }
}