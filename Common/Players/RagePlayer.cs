using ShatteredFate.Common.GlobalItems;
using ShatteredFate.Content.Items.Accessories;
using ShatteredFate.ModUtils;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;

namespace ShatteredFate.Common.Players;

public class RagePlayer : ModPlayer {
    Item _amuletOfRage = null;

    internal Dictionary<int, bool> equipAcc = [];

    int _rage = 0;
    int _maxRage = 0;
    int _duration = 0;
    int _cooldown = 0;
    int _origRageBuffTime = 0;

    bool _activeRage = false;
    bool _startCD = false;

    public Item GetAmulet() { return _amuletOfRage; }
    public void SetAmulet(Item item) { _amuletOfRage = item; }

    public int GetRage() => _rage;
    public void SetRage(int value) {
        value = value <= -1 ? 0 : value;
        _rage = value > GetMaxRage() && GetMaxRage() != 0 ? GetMaxRage() : value;
    }
    public int GetMaxRage() {
        int value = 0;
        for (int i = 0; i < 3; i++) { if (Player.armor[i].type != ItemID.None) { value += cheakRage(i); } };
        for (int i = 0; i < AccessorySlotLoader.MaxVanillaSlotCount; i++) { if (Player.armor[i].type != ItemID.None) { value += cheakRage(i); } };
        for (int i = 0; i < Player.GetModPlayer<ModAccessorySlotPlayer>().SlotCount; i++) { if (PlayersExpansions.GetModedAccItemInSlot(Player)[i].type != ItemID.None) { value += cheakRage(i); } };
        SetMaxRage(value);
        int cheakRage(int index) {
            if (Player.armor[index].GetGlobalItem<RageItem>().GetMaxRage() == 0) { return 0; }
            else { equipAcc.TryAdd(Player.armor[index].type, true); return Player.armor[index].GetGlobalItem<RageItem>().GetMaxRage(); };
        }
        return _maxRage;
    }
    public void SetMaxRage(int value) => _maxRage = value;
    public int GetDurationTime() => _duration;
    public void SetDurationTime(int value) {
        if (value < 0) { value = 0; };
        _duration = value;
    }
    public int GetCDTime() => _cooldown;
    public void SetCDTime(int value) {
        if (value < 0) { value = 0; };
        _cooldown = value;
    }
    public int GetVanillaRageBuffTime() => _origRageBuffTime;
    public void SetVanillaRageBuffTime(int value) => _origRageBuffTime = value;

    public bool GetRageStatus() => _activeRage;
    public void SetRageStatus(bool value) {
        _activeRage = value;
        if (value) {
            SetDurationTime(10 * 60);
            SetRage(0);
            GetMaxRage();
            SoundEngine.PlaySound(Resources.Sounds.Get("Rage"), Player.position);
        };
    }
    public bool GetCDStatus() => _startCD;
    public void SetCDStatus(bool value) {
        _startCD = value;
        if (!value) { equipAcc = []; };
    }

    public override void LoadData(TagCompound tag) {
        SetRage(tag.GetInt($"{SFMod.ModName}: RageCount"));
        SetDurationTime(tag.GetInt($"{SFMod.ModName}: RageDurationCount"));
        SetCDTime(tag.GetInt($"{SFMod.ModName}: RageCDCount"));
        SetVanillaRageBuffTime(tag.GetInt($"{SFMod.ModName}: RageBuffTime"));

        SetRageStatus(tag.GetBool($"{SFMod.ModName}: RageStatus"));
        SetCDStatus(tag.GetBool($"{SFMod.ModName}: RageCDStatus"));
    }
    public override void SaveData(TagCompound tag) {
        tag[$"{SFMod.ModName}: RageCount"] = GetRage();
        tag[$"{SFMod.ModName}: RageDurationCount"] = GetDurationTime();
        tag[$"{SFMod.ModName}: RageCDCount"] = GetCDTime();
        tag[$"{SFMod.ModName}: RageBuffTime"] = GetVanillaRageBuffTime();

        tag[$"{SFMod.ModName}: RageStatus"] = GetRageStatus();
        tag[$"{SFMod.ModName}: RageCDStatus"] = GetCDStatus();
    }
    public override void ResetEffects() {
        SetAmulet(null);
        SetMaxRage(0);
    }
    public override void ProcessTriggers(Terraria.GameInput.TriggersSet triggersSet) {
        if (KeyBind.GetRageKey().JustPressed && GetRage() >= GetMaxRage() && GetMaxRage() > 0) { SetRageStatus(true); };
    }
    public override void PostUpdate() {
        foreach (int accType in equipAcc.Keys) if (!PlayersExpansions.CheckAcc(Player, accType)) { return; };
        if (GetDurationTime() == 0 && !GetCDStatus() && GetRageStatus()) {
            SetCDTime(15 * 60);
            SetCDStatus(true);
            SetRageStatus(false);
        };
        if (GetDurationTime() > 0 && GetRageStatus()) { SetDurationTime(GetDurationTime() - 1); };
        if (GetCDTime() > 0) { SetCDTime(GetCDTime() - 1); };
        if (GetCDTime() == 0 && GetCDStatus()) {
            SoundEngine.PlaySound(SoundID.MaxMana, Player.position);
            for (int i = 0; i < 17; i++) {
                int index = Dust.NewDust(Player.position, Player.width, Player.height, DustID.LifeDrain, 0f, 0f, 255, default, (float)Main.rand.Next(20, 26) * 0.1f);
                Main.dust[index].noLight = true;
                Main.dust[index].velocity *= 0.5f;
            };
        };
        if (GetCDTime() == 0) { SetCDStatus(false); };
    }
    public override void OnHurt(Player.HurtInfo info) {
        if (GetRageStatus() || GetCDTime() > 0) { return; };
        if (GetAmulet() == null) { return; };
        if (GetAmulet().type == ModContent.ItemType<AmuletofRage>()) {
            int multHP = (int)(Player.statLifeMax2 * 0.1f);
            if (info.Damage > multHP) { SetRage(GetRage() + (info.Damage - multHP)); };
        };
    }
};