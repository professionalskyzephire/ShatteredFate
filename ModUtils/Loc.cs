using Terraria.Localization;

namespace ShatteredFate.ModUtils;

public static class Loc {
    public const string LocPatch = "Mods.ShatteredFate.";
    public static string Get(string name) => Language.GetTextValue(LocPatch + name);
    public static string GetTips(string name) => Language.GetTextValue(LocPatch + "Tooltips." + name);
}