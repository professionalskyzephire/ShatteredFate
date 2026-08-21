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

    readonly NPC[] _nearbayNPC = new NPC[200];

    public delegate void HitDelegate(Item item, ref StatModifier damage); public HitDelegate Hit;
    public delegate void EnterWorldDelegate(); public EnterWorldDelegate EnterWorld;

    readonly static FieldInfo _getModedAccSlot;

    public NPC[] GetNearbyNPC() {
        int count = 0;
        for (int i = 0; i <  Main.maxNPCs; i++) { 
            if (Main.npc[i] != null) {
                if (new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight).Intersects(Main.npc[i].Hitbox) && Main.npc[i].active) {
                    _nearbayNPC[count] = Main.npc[i];
                    count++;
                };
            }; 
        }
        NPC[] npcs = new NPC[count];
        for (int i = 0; i < count; i++) { npcs[i] = _nearbayNPC[i]; };
        return npcs;
    }
    public void ClearNearbyNPCArray() {
        for (int i = 0; i < 200; i++) { _nearbayNPC[i] = null; };
    }

    public override void PostUpdate() {
    }

    public static FieldInfo GetModedAccSlot() => _getModedAccSlot;
    public static Item[] GetModedAccItemInSlot(Player player) => (Item[])_getModedAccSlot.GetValue(player.GetModPlayer<ModAccessorySlotPlayer>());

    public static bool CheckAcc(Player player, int itemType) {
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