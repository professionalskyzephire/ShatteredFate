using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ShatteredFate.Core;

public class CustomDialogBox(UIState state, UIInsertSetting insertSetting, int target) {
    UIState ActiveUi = null;
    UIState UI => state;

    UIInsertSetting setting = insertSetting; 
    public int NPC => target;

    public void Register() { Manager.RegisterUI.Add(this); }
    public void UpdateUI(UIState value) => ActiveUi = value;
    public void StartUI() {
        if (ActiveUi == null) {
            ActiveUi = UI;
            ModContent.GetInstance<SFMod>().SFUI.SetState(ActiveUi);
        }
    }
    public void RegisterLayer(List<GameInterfaceLayer> layers, UserInterface ui) {
        int index;
        if (setting.LayerName == null) { index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory")); }
        else { index = layers.FindIndex(layer => layer.Name.Equals(setting.LayerName)); };
        if (index != -1) { layers.Insert(index, new LegacyGameInterfaceLayer(setting.Name, () => { ui.Draw(Main.spriteBatch, new()); return true; }, InterfaceScaleType.UI)); };
    }
    public void Update(NPC npc) {
        if (npc.type == NPC) {
            Player player = Main.LocalPlayer;
            if (ActiveUi != null && (player.talkNPC == -1 || Main.npc[player.talkNPC].type != npc.type)) { ActiveUi = null; }
        }
    }
}