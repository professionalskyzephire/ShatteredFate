using ShatteredFate.Content.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Common.ModSystems.Worlds;

public class ChestLoot : ModSystem {
    public override void PostWorldGen() {
        AddContainersLoot(2, 6, ModContent.ItemType<AmuletofRage>());
    }
    public static void AddContainersLoot(int style, int chance, int item, int min = 0, int max = 0) {
        for (int chestIndex = 0; chestIndex < 1000; chestIndex++) {
            Chest chest = Main.chest[chestIndex];
            if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == style * 36) {
                if (Main.rand.NextBool(chance)) {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++) {
                        if (chest.item[inventoryIndex].type == ItemID.None) {
                            chest.item[inventoryIndex].SetDefaults(item);
                            int a = max != 0 ? 1 : 0;
                            chest.item[inventoryIndex].stack = Main.rand.Next(min, max + a);
                            break;
                        }
                    }
                }
            }
        }
    }
};