using Microsoft.Xna.Framework;
using ShatteredFate.Content.Buffs.Debuffs;
using ShatteredFate.Content.Projectiles.Magic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ShatteredFate.Common.Players;

public class SFPlayer : ModPlayer {
    public int HardmodeMusicTimer;
    private bool wasInHardmode;

    public override void PostUpdate() {
        if (HardmodeMusicTimer > 0) {
            HardmodeMusicTimer--;
        }
        if (!wasInHardmode && Main.hardMode) {
            HardmodeMusicTimer = 60 * 198;
        }
        wasInHardmode = Main.hardMode;

        // When the Curious Candle is held, add light to the item's location
        if (Player.HeldItem?.ModItem is Content.Items.Weapons.Magic.CuriousCandle) { Lighting.AddLight(Player.itemLocation, new Color(235, 93, 175).ToVector3()); }
    }

    public override void ModifyHitByProjectile(Projectile projectile, ref Player.HurtModifiers modifiers) {
        if (ModContent.GetInstance<SFReworksConfig>().GemStaves && Player.ownedProjectileCounts[ModContent.ProjectileType<LargeDiamond>()] > 0) {
            modifiers.FinalDamage /= 2;
            projectile.velocity *= -1f;
            projectile.hostile = false;
            projectile.friendly = true;
        }
    }
    public override void ModifyHitNPCWithProj(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
        if (!projectile.npcProj && !projectile.trap && projectile.IsMinionOrSentryRelated && target.HasBuff(ModContent.BuffType<SanguineLeechDebuff>())) modifiers.FlatBonusDamage += 5 * Terraria.ID.ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
    }

    public override void SaveData(TagCompound tag) {
        tag.Add("wasInHardmode", wasInHardmode);
    }

    public override void LoadData(TagCompound tag) {
        wasInHardmode = tag.GetBool("wasInHardmode");
    }
}