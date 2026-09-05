using ShatteredFate.Core;
using Terraria;
using Terraria.ModLoader;

namespace ShatteredFate.Common.GlobalNPCs;

public class CustomNPCDialogBox : GlobalNPC {
    public override void AI(NPC npc) {
        foreach (CustomDialogBox dialogBox in Manager.RegisterUI) {
            dialogBox.Update(npc);
        }
    }
};