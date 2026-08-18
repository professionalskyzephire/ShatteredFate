using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace ShatteredFate.ModUtils;

public class PlayersExpansions : ModPlayer {
    static PlayersExpansions() {
        _getModedAccSlot = typeof(ModAccessorySlotPlayer).GetField("exAccessorySlot", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    readonly NPC[] _nearbayNPC = new NPC[255];

    public delegate void HitDelegate(Item item, ref StatModifier damage); public HitDelegate Hit;
    public delegate void EnterWorldDelegate(); public EnterWorldDelegate EnterWorld;

    readonly static FieldInfo _getModedAccSlot;

    public NPC[] GetNearbyNPC() {
        int count = 0;
        for (int i = 0; i < 255; i++) { if (_nearbayNPC[i] != null) { count++; }; };
        NPC[] npcs = new NPC[count];
        for (int i = 0; i < count; i++) { npcs[i] = _nearbayNPC[i]; };
        return npcs;
    }
    public void SetNearbyNPC(int index, NPC npcType) => _nearbayNPC[index] = npcType;
    public void ClearNearbyNPCArray() {
        for (int i = 0; i < 255; i++) { SetNearbyNPC(i, null); };
    }

    public override void PostUpdate() {
        int index = 0;
        foreach (NPC npc in Main.ActiveNPCs) {
            if (!npc.townNPC) { continue; };
            if (new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight).Intersects(npc.Hitbox) && npc.active) { SetNearbyNPC(index, npc); index++; };
        };
    }

    public static FieldInfo GetModedAccSlot() => _getModedAccSlot;
    public static Item[] GetModedAccItemInSlot(Player player) => (Item[])_getModedAccSlot.GetValue(player.GetModPlayer<ModAccessorySlotPlayer>());

    public static bool CheackAcc(Player player, int itemType) {
        bool value = false;
        for (int i = 0; i < 3; i++) { if (player.armor[i].type == itemType) { value = true; break; } }
        for (int i = 0; i < AccessorySlotLoader.MaxVanillaSlotCount; i++) { if (player.armor[i].type == itemType) { value = true; break; } }
        for (int i = 0; i < player.GetModPlayer<ModAccessorySlotPlayer>().SlotCount; i++) { if (player.armor[i].type == itemType) { value = true; break; } }
        return value;
    }

    public override void ResetEffects() {
        Hit = null;
        EnterWorld = null;
    }
    public override void OnEnterWorld() => EnterWorld?.Invoke();
    public override void ModifyWeaponDamage(Item item, ref StatModifier damage) => Hit?.Invoke(item, ref damage);
};