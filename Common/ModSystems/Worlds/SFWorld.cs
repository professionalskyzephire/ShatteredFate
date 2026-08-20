using Terraria;
using Terraria.ModLoader;

namespace ShatteredFate.Common.ModSystems.Worlds;

public class SFWorld : ModSystem {
    public static int WorldProgress => GetProgress();

    static int GetProgress() {
        int progress = 0;
        if (NPC.downedSlimeKing) { progress++; };
        if (NPC.downedBoss1) { progress++; };
        if (NPC.downedBoss2) { progress++; };
        if (NPC.downedQueenBee) { progress++; };
        if (NPC.downedBoss3) { progress++; };
        if (NPC.downedDeerclops) { progress++; };
        if (Main.hardMode) { progress++; };
        if (NPC.downedQueenSlime) { progress++; }
        if (NPC.downedMechBoss1) { progress++; };
        if (NPC.downedMechBoss2) { progress++; };
        if (NPC.downedMechBoss3) { progress++; };
        if (NPC.downedPlantBoss) { progress++; };
        if (NPC.downedPlantBoss) { progress++; };
        if (NPC.downedGolemBoss) { progress++; };
        if (NPC.downedEmpressOfLight) { progress++; };
        if (NPC.downedAncientCultist) { progress++; };
        if (NPC.downedMoonlord) { progress++; };
        return progress;
    }
};