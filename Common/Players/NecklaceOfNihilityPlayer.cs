using ShatteredFate.Content.Buffs;
using ShatteredFate.Content.Items.Accessories;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ShatteredFate.Common.Players;

public class NecklaceOfNihilityPlayer : ModPlayer {
    Item _necklaceOfNihility = null;

    public bool EquipNecklace() => _necklaceOfNihility != null;
    public void SetNecklace(Item value) => _necklaceOfNihility = value;
    public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)/* tModPorter Suggestion: Return an Item array to add to the players starting items. Use ModifyStartingInventory for modifying them if needed */ {
        if (!mediumCoreDeath) {
            if (Player.name.Equals("skyzephire")) { yield return new Item(ModContent.ItemType<NecklaceOfNihility>(), 1, 0); };
        };
    }
    public override void ResetEffects() => SetNecklace(null);
    public override void PostUpdate() { if (EquipNecklace()) { Player.AddBuff(ModContent.BuffType<DarkerThanCoal>(), 1); }; }
    public override void FrameEffects() {
        if (EquipNecklace()) {
            NecklaceOfNihility n = ModContent.GetInstance<NecklaceOfNihility>();
            Player.head = EquipLoader.GetEquipSlot(Mod, n.Name, EquipType.Head);
            Player.body = EquipLoader.GetEquipSlot(Mod, n.Name, EquipType.Body);
            Player.legs = EquipLoader.GetEquipSlot(Mod, n.Name, EquipType.Legs);
        };
    }
};