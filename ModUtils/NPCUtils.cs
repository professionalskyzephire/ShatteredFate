using Terraria;
using Terraria.GameContent.Bestiary;

namespace ShatteredFate.ModUtils;

public static class NPCUtils {
    public static int FindGround(int x, int startY, int npcWidth = 32) {
        for (int y = startY; y < Main.maxTilesY - 1; y++) {
            bool groundFound = true;
            for (int i = 0; i < (npcWidth + 15) / 16; i++) {
                if (!Main.tile[x + i, y].HasTile || !Main.tileSolid[Main.tile[x + i, y].TileType]) {
                    groundFound = false;
                    break;
                };
            };
            if (groundFound) { return y; };
        };
        return startY;
    }
    public static void AddBestiaryInfo(ref BestiaryEntry bestiaryEntry, string key, params IBestiaryInfoElement[] elements) {
        bestiaryEntry.Info.AddRange([new FlavorTextBestiaryInfoElement("Mods.ShatteredFate.BestiaryEntry." + key)]);
        bestiaryEntry.Info.AddRange(elements);
    }
};