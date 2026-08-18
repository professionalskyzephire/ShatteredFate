using Microsoft.Xna.Framework;
using ShatteredFate.Content.Buffs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ShatteredFate.Common.Players;

public class MagnetismPlayer : ModPlayer {
    int _magnetismStacks = 0;
    int _cooldown = 0;
    int _grabRangeBoost = 0;

    bool _magnetismAbility = false;

    public int GetStacks() => _magnetismStacks;
    public void SetStacks(int value) => _magnetismStacks = value;
    public int GetCooldown() => _cooldown;
    public void SetCooldown(int value) => _cooldown = value >= 0 ? value : 0;
    public int GetGrabRange() => _grabRangeBoost;
    public void SetGrabRange(int value) => _grabRangeBoost = value;

    public bool GetAbilityStatus() => _magnetismAbility;
    public void SetAbilityStatus(bool value) => _magnetismAbility = value;

    public override void LoadData(TagCompound tag) {
        SetCooldown(tag.GetInt($"{SFMod.ModName}:MagnetismCD"));
        SetAbilityStatus(tag.GetBool($"{SFMod.ModName}:MagnetismAbility"));
    }
    public override void SaveData(TagCompound tag) {
        tag[$"{SFMod.ModName}:MagnetismCD"] = GetCooldown();
        tag[$"{SFMod.ModName}:MagnetismAbility"] = GetAbilityStatus();
    }
    public override void ResetEffects() => SetGrabRange(0);
    public override void ProcessTriggers(Terraria.GameInput.TriggersSet triggersSet) {
        if (GetCooldown() > 0) {
            if (KeyBind.GetMagnetismKey().JustPressed) { Main.NewText("Hypermagnetism is on cooldown for " + (GetCooldown() / 60 + 1) + " more seconds.", Color.Orange); };
        };
        if (KeyBind.GetMagnetismKey().JustPressed && GetAbilityStatus() && GetCooldown() == 0) {
            Player.AddBuff(ModContent.BuffType<HypermagnetismBuff>(), 60 * 15);
            SetCooldown(3600);
        };
    }
    public override void PostUpdate() {
        if (!GetAbilityStatus() && GetStacks() > 0 && !Player.HasBuff<MagnetismBuff>()) { SetStacks(0); }
        if (GetCooldown() > 0) { SetCooldown(GetCooldown() - 1); }
    }
}
