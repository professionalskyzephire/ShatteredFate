using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ShatteredFate.Common.ModSystems.Recipes;

public class Group : ModSystem {
    public static int GetCopper() => RecipeGroup.recipeGroupIDs["ShatteredFate:CopperOrTin"];
    public static int GetSilver() => RecipeGroup.recipeGroupIDs["ShatteredFate:SilverOrTungsten"];
    public static int GetGold() => RecipeGroup.recipeGroupIDs["ShatteredFate:GoldOrPlatinum"];

    public override void AddRecipeGroups() {
        RecipeGroup.RegisterGroup("ShatteredFate:CopperOrTin", new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CopperBar)}", ItemID.CopperBar, ItemID.TinBar));
        RecipeGroup.RegisterGroup("ShatteredFate:SilverOrTungsten", new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}", ItemID.SilverBar, ItemID.TungstenBar));
        RecipeGroup.RegisterGroup("ShatteredFate:GoldOrPlatinum", new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", ItemID.GoldBar, ItemID.PlatinumBar));
    }
};