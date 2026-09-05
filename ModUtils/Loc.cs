using Terraria;
using Terraria.Localization;

namespace ShatteredFate.ModUtils;

public static class Loc {
    public const string LocPatch = "Mods.ShatteredFate.";
    public static string Get(string name) => Language.GetTextValue(LocPatch + name);
    public static string GetTips(string name) => Language.GetTextValue(LocPatch + "Tooltips." + name);
    public static string GetNPCChat(string key) => Language.GetTextValue(LocPatch + "NPCsChat." + key);
    public static string GetChat(string key) => Language.GetTextValue(LocPatch + "ChatMsg." + key);
    public static string ContainerName(Player player, int needItemType, out int container, out int stack) {
        SFUtils.CheckNeedItem(player, needItemType, 0, out container, out stack);
        return GetTips("GlobalItems.ItemSlots." + container);
    }
}