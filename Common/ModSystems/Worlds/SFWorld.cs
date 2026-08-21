using Terraria;
using Terraria.ModLoader;

namespace ShatteredFate.Common.ModSystems.Worlds;

public class SFWorld : ModSystem {
    public static int WorldProgress => GetProgress();

    static int GetProgress() {
        int progress = 0;
        if (NPC.downedSlimeKing) { progress++; };
        if (NPC.downedBoss1 && progress == 1) { progress++; };
        if (NPC.downedBoss2 && progress == 2) { progress++; };
        if (NPC.downedQueenBee && progress == 3) { progress++; };
        if (NPC.downedBoss3 && progress == 4) { progress++; };
        if (NPC.downedDeerclops && progress == 5) { progress++; };
        if (Main.hardMode && progress == 6) { progress++; };
        if (NPC.downedQueenSlime && progress == 7) { progress++; }
        if (NPC.downedMechBoss1 && progress == 8) { progress++; };
        if (NPC.downedMechBoss2 && progress == 9) { progress++; };
        if (NPC.downedMechBoss3 && progress == 10) { progress++; };
        if (NPC.downedPlantBoss && progress == 11) { progress++; };
        if (NPC.downedPlantBoss && progress == 12) { progress++; };
        if (NPC.downedGolemBoss && progress == 13) { progress++; };
        if (NPC.downedEmpressOfLight && progress == 14) { progress++; };
        if (NPC.downedAncientCultist && progress == 15) { progress++; };
        if (NPC.downedMoonlord && progress == 16) { progress++; };
        return progress;
    }
};