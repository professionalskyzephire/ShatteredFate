using ShatteredFate.Common.ModSystems.Worlds;
using ShatteredFate.ModUtils;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.NPCs.Misc;

[AutoloadHead]
public class ShadyFigure : ModNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 25;
        NPCID.Sets.NoTownNPCHappiness[Type] = true;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() { Velocity = -1f, Direction = -1 });
    }
    public override void SetDefaults() {
        NPC.friendly = true;
        NPC.townNPC = true;
        NPC.lifeMax = 1;
        NPC.defense = 1;
        NPC.width = 24;
        NPC.height = 42;
        NPC.aiStyle = NPCAIStyleID.Passive;
        AnimationType = NPCID.GoblinTinkerer;
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => NPCUtils.AddBestiaryInfo(ref bestiaryEntry, "ShadyFigure", BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime, BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface);
    public override bool CanBeHitByNPC(NPC attacker) => false;
    public override void AI() {
        if (Main.dayTime) { NPC.active = false; }
    }
    public override string GetChat() {
        return base.GetChat();
    }
    public override void ModifyActiveShop(string shopName, Item[] items) {
        int[] cItems = ModContent.GetInstance<ShadyFigureShop>().GetCurrentShopArray();
        for (int i = 0; i < cItems.Length; i++) {
            items[i] = new(cItems[i]);
        }
    }
    public override void SetChatButtons(ref string button, ref string button2) {
        button = Lang.inter[28].Value;
    }
    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (firstButton) {
            shopName = "ShadyFigureShop";
        }
    }
}
