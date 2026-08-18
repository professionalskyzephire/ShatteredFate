// нип пропадает в мультиплеере
using ShatteredFate.Content.NPCs.Misc;
using ShatteredFate.ModUtils;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ShatteredFate.Common.Players;

public class ShadyFigurePlayer : ModPlayer {
    bool _spawnShady = false;

    public bool GetShadySpawnStatus() => _spawnShady;
    public void SetShadySpawnStatus(bool value) => _spawnShady = value;

    public override void LoadData(TagCompound tag) {
        SetShadySpawnStatus(tag.GetBool($"{SFMod.ModName}:ShadySpawned"));
    }
    public override void SaveData(TagCompound tag) {
        tag[$"{SFMod.ModName}:ShadySpawned"] = GetShadySpawnStatus();
    }

    public override void PostUpdate() {
        if (!Main.dayTime && !GetShadySpawnStatus()) {
            if (NPC.AnyNPCs(ModContent.NPCType<ShadyFigure>())) { 
                SetShadySpawnStatus(true);
                return; 
            };

            int count = 0;
            NPC[] nearbayNPC = Player.GetModPlayer<PlayersExpansions>().GetNearbyNPC();

            for (int i = 0; i < nearbayNPC.Length; i++) { if (!nearbayNPC[i].homeless) { count++; }; };
            if (count == 0) { return; };

            int scaleY = Main.rand.Next(0, 2) == 0 ? 400 : -400;
            int index = NPC.NewNPC(Player.GetSource_FromThis("Spawn Shady figure"), (int)(Player.Center.X + scaleY), NPCUtils.FindGround((int)(Player.Center.X + scaleY) / 16, (int)Player.Center.Y / 16, 32) * 16, ModContent.NPCType<ShadyFigure>());
            Main.npc[index].position.Y = NPCUtils.FindGround((int)(Player.Center.X + scaleY) / 16, (int)Player.Center.Y / 16, 32) * 16 - Main.npc[index].height;
            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs) {
                NetMessage.SendData(MessageID.SyncNPC, number: index);
            }
            SoundEngine.PlaySound(Resources.Sounds.Get("EerieSound"), Main.npc[index].position);
            Player.GetModPlayer<PlayersExpansions>().ClearNearbyNPCArray();
            SetShadySpawnStatus(true);
        };
        if (Main.dayTime && GetShadySpawnStatus()) { SetShadySpawnStatus(false); }
    }
};