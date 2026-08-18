using ShatteredFate.Content.Projectiles.Misc;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ShatteredFate.Common.Players;

public class PackOExplosivesPlayer : ModPlayer {
    Item _packOExplosives = null;

    int _cooldown = 0;

    public bool EquipPackOExplosives() => _packOExplosives != null;
    public void SetPackOExplosives(Item value) => _packOExplosives = value;

    public int GetCD() => _cooldown;
    public void SetCD(int value) => _cooldown = value;

    public override void PostUpdate() {
        if (GetCD() > 0) { SetCD(GetCD() - 1); };
    }
    public override void ResetEffects() {
        SetPackOExplosives(null);
    }

    public override void OnHurt(Player.HurtInfo info) {
        if (EquipPackOExplosives() && GetCD() == 0) {
            SetCD(180);
            int proj = Projectile.NewProjectile(Player.GetSource_Misc("BOOM"), Player.Center, Microsoft.Xna.Framework.Vector2.Zero, ModContent.ProjectileType<Explosion>(), 500, 0f);
            Player.Hurt(PlayerDeathReason.ByProjectile(Player.whoAmI, proj), 500, 0);
        };
    }
};