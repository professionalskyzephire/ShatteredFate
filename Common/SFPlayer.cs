using ShatteredFate;
using ShatteredFate.Content.Buffs;
using ShatteredFate.Content.Projectiles.Misc;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Localization;

public class SFPlayer : ModPlayer
{
    public int HardmodeMusicTimer;
    private bool wasInHardmode;
    
    public int GrabRangeBoost;
    
    public int MagnetismStacks;
    public bool MagnetismAbility;
    private int MagnetismAbilityCooldown;

    public bool PackOExplosives;
    // In case you somehow survive the explosion
    public int PackOExplosivesCooldown;
    public override void PostUpdate()
    {
        if (!MagnetismAbility && MagnetismStacks > 0 && !Player.HasBuff<MagnetismBuff>())
        {
            MagnetismStacks = 0;
        }
        
        if (PackOExplosivesCooldown > 0)
        {
            PackOExplosivesCooldown--;
        }
        
        if (MagnetismAbilityCooldown > 0)
        {
            MagnetismAbilityCooldown--;
            if (SFMod.MagnetismKey.JustPressed)
            {
                Main.NewText("Hypermagnetism is on cooldown for " + (MagnetismAbilityCooldown / 60 + 1) + " more seconds.", Color.Orange);
            }
        }
        else if (SFMod.MagnetismKey.JustPressed && MagnetismAbility)
        {
            Player.AddBuff(ModContent.BuffType<HypermagnetismBuff>(), 60 * 15);
            MagnetismAbilityCooldown = 3600;
        }
        
        if (HardmodeMusicTimer > 0)
        {
            HardmodeMusicTimer--;
        }
        if (!wasInHardmode && Main.hardMode)
        {
            HardmodeMusicTimer = 60 * 198;
        }
        wasInHardmode = Main.hardMode;
    }

    public override void ResetEffects()
    {
        GrabRangeBoost = 0;
        PackOExplosives = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (PackOExplosives && PackOExplosivesCooldown == 0)
        {
            PackOExplosivesCooldown = 60 * 3;
            int proj = Projectile.NewProjectile(Player.GetSource_Misc(""), Player.Center, Vector2.Zero,
                ModContent.ProjectileType<Explosion>(), 500, 0f);
            Player.Hurt(PlayerDeathReason.ByProjectile(Player.whoAmI, proj), 500, 0);
        }
    }
	public override void ModifyHitByProjectile(Projectile projectile, ref Player.HurtModifiers modifiers) {
		if(ModContent.GetInstance<SFReworksConfig>().GemStaves && Player.ownedProjectileCounts[ModContent.ProjectileType<Content.Projectiles.Magic.LargeDiamond>()] > 0) {
			modifiers.FinalDamage /= 2;
			projectile.velocity *= -1f;
			projectile.hostile = false;
			projectile.friendly = true;
		}
	}
    public override void SaveData(TagCompound tag)
    {
        tag.Add("wasInHardmode", wasInHardmode);
        tag.Add("MagnetismAbility", MagnetismAbility);
    }

    public override void LoadData(TagCompound tag)
    {
        wasInHardmode = tag.GetBool("wasInHardmode");
        MagnetismAbility = tag.GetBool("MagnetismAbility");
    }
}
