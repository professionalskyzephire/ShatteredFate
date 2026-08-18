using ShatteredFate.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Items.Accessories;

public class NecklaceOfNihility : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 1;
        if (Main.netMode == NetmodeID.Server) { return; }
        ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
        ArmorIDs.Body.Sets.HidesTopSkin[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = true;
        ArmorIDs.Body.Sets.HidesArms[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = true;
        ArmorIDs.Legs.Sets.HidesBottomSkin[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs)] = true;
    }
    public override void Load() {
        if (Main.netMode == NetmodeID.Server) { return; };
        EquipLoader.AddEquipTexture(Mod, ($"{Texture}_{EquipType.Head}").Replace(Name, "Nig"), EquipType.Head, this);
        EquipLoader.AddEquipTexture(Mod, ($"{Texture}_{EquipType.Body}").Replace(Name, "Nig"), EquipType.Body, this);
        EquipLoader.AddEquipTexture(Mod, ($"{Texture}_{EquipType.Legs}").Replace(Name, "Nig"), EquipType.Legs, this);
    }
    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.maxStack = 1;
        Item.accessory = true;
        Item.vanity = true;
        Item.hasVanityEffects = true;
        Item.rare = ModContent.RarityType<Rarities.SFDevItem>();
    }
    public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<NecklaceOfNihilityPlayer>().SetNecklace(!hideVisual ? Item : null);
    public override void UpdateVanity(Player player) => player.GetModPlayer<NecklaceOfNihilityPlayer>().SetNecklace(Item);
};