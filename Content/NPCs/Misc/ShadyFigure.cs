using ShatteredFate.Common.ModSystems.Worlds;
using ShatteredFate.ModUtils;
using ShatteredFate.UIElements;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.NPCs.Misc;

[AutoloadHead]
public class ShadyFigure : ModNPC {
    ShadyFigureUI chat;

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
        Player player = Main.LocalPlayer;
        if (player.talkNPC == -1 || Main.npc[player.talkNPC].type != Type) {
            chat = null;
        }
    }
    public override string GetChat() => Loc.GetNPCChat("ShadyFigure.Says." + Main.rand.Next(0, 5));
    public override void SetChatButtons(ref string button, ref string button2) {
        button = Loc.GetNPCChat("ShadyFigure.Button.0");
    }
    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (firstButton) {
            Main.playerInventory = true;
            Main.stackSplit = 9999;
            Main.npcChatText = "";

            Player player = Main.LocalPlayer;
            TryGetTalkNPC(player, out NPC npc);
            if (chat == null) {
                chat = new ShadyFigureUI();
                ModContent.GetInstance<SFMod>().SFUI.SetState(chat);
            }

            SoundEngine.PlaySound(SoundID.MenuOpen);
        }
    }
    public static bool TryGetTalkNPC(Player player, out NPC npc) {
        npc = null;

        int index = player.talkNPC;
        if (index < 0 || index >= Main.npc.Length) {
            return false;
        }

        npc = Main.npc[index];
        return npc.active;
    }
}