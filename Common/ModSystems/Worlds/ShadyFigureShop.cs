using ShatteredFate.Core;
using ShatteredFate.Tables;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ShatteredFate.Common.ModSystems.Worlds;

public class ShadyFigureShop : ModSystem {
    public ShopItem[] ShopItems { get; private set; } = new ShopItem[40];

    public bool FilledStatus { get; private set; } = false;

    public override void LoadWorldData(TagCompound tag) {
        if (tag.ContainsKey($"{SFMod.ModName}:Active Shop Items")) {
            ShopItems = [.. tag.GetList<TagCompound>($"{SFMod.ModName}:Active Shop Items").Select(FromTag)];
        }
        else { ShopItems = []; }
        FilledStatus = tag.GetBool($"{SFMod.ModName}:SetupShopItem");
    }
    public override void SaveWorldData(TagCompound tag) {
        tag[$"{SFMod.ModName}:Active Shop Items"] = ShopItems.Select(ToTag).ToArray();

        tag[$"{SFMod.ModName}:SetupShopItem"] = FilledStatus;
    }
    public override void NetSend(BinaryWriter writer) {
        int[] target = new int[40];
        int[] value = new int[40];

        writer.Write(40);

        for (int i = 0; i < 40; i++) {
            target[i] = ShopItems[i].Target;
            value[i] = ShopItems[i].Need;
        }

        foreach (int element in target) { writer.Write(element); }
        foreach (int element in value) { writer.Write(element); }

        writer.Write(FilledStatus);
    }
    public override void NetReceive(BinaryReader reader) {
        int[] target = new int[40];
        int[] value = new int[40];
        int count;

        count = reader.ReadInt32();

        for (int i = 0; i < count; i++) { target[i] = reader.ReadInt32(); };
        for (int i = 0; i < count; i++) { value[i] = reader.ReadInt32(); };

        for (int i = 0; i < target.Length; i++) {
            ShopItems[i].Target = target[i];
            ShopItems[i].Need = value[i];
        }

        FilledStatus = reader.ReadBoolean();
    }
    public override void PostUpdateWorld() {
        //Main.NewText(SFWorld.WorldProgress);
        if (Main.dayTime && FilledStatus) { FilledStatus = false; }
        if (FilledStatus) { return; };
        if (!FilledStatus && !Main.dayTime) { FillShop(); }
    }

    static bool TryGetRandomItem(IEnumerable<ShadyFigureItem> source, HashSet<int> used, out int target) {
        List<ShadyFigureItem> available = [];

        foreach (ShadyFigureItem item in source) {
            if (item.WorldProgress > SFWorld.WorldProgress) { continue; }
            if (used.Contains(item.Target)) { continue; }
            available.Add(item);
        }

        if (available.Count == 0) {
            target = 0;
            return false;
        }

        target = Main.rand.Next(available).Target;
        used.Add(target);

        return true;
    }
    void FillShop() {
        HashSet<int> used = [];

        if (ShopItems.Length < 40) { ShopItems = new ShopItem[40]; }

        List<ShadyFigureItem> priceItems = [];
        priceItems.AddRange(Items.Material);
        priceItems.AddRange(Items.Misc);
        priceItems.AddRange(Items.Weapon);

        for (int i = 0; i < 3; i++) {
            if (TryGetRandomItem(Items.Armor, used, out int value)) {
                if (TryGetRandomItem(priceItems, used, out int prise)) {
                    if (CheckArmorSet(value, out ShadyFigureItem.ArmorSet value2)) {
                        ShopItems[0] = new(Main.rand.Next(value2.Helmet), prise);
                        TryGetRandomItem(priceItems, used, out int prise1);
                        ShopItems[1] = new(value2.Chainmail, prise1);
                        TryGetRandomItem(priceItems, used, out int prise2);
                        ShopItems[2] = new(value2.Greaves, prise2);
                        break;
                    }
                    ShopItems[i] = new(value, 1);
                }
            }
        }

        priceItems.RemoveRange(Items.Material.Count + Items.Misc.Count, Items.Weapon.Count);
        priceItems.AddRange(Items.Armor);

        for (int i = 3; i < 11; i++) {
            if (TryGetRandomItem(Items.Weapon, used, out int value) && TryGetRandomItem(priceItems, used, out int prise)) {
                ShopItems[i] = new(value, prise);
            }
        }

        priceItems.AddRange(Items.Acc);
        priceItems.AddRange(Items.Consumables);
        priceItems.AddRange(Items.Material);
        priceItems.AddRange(Items.Misc);
        priceItems.AddRange(Items.Weapon);

        List<ShadyFigureItem> randomItems = [];

        randomItems.AddRange(Items.Acc);
        randomItems.AddRange(Items.Consumables);
        randomItems.AddRange(Items.Material);
        randomItems.AddRange(Items.Misc);

        for (int i = 11; i < 40; i++) {
            if (TryGetRandomItem(randomItems, used, out int target) && TryGetRandomItem(priceItems, used, out int value)) {
                ShopItems[i] = new(target, value);
            }
        }
        for (int i = 0; i < 40; i++) {
            if (ShopItems[i] == null) { ShopItems[i] = new(1, 1); }
        }

        FilledStatus = true;
    }
    static bool CheckArmorSet(int itemType, out ShadyFigureItem.ArmorSet set) {
        bool check = false;
        set = null;
        for (int i = 0; i < Items.ArmorSet.Count; i++) {
            if (Items.ArmorSet[i].Helmet.Contains(itemType) || Items.ArmorSet[i].Chainmail == itemType || Items.ArmorSet[i].Greaves == itemType) {
                check = true;
                set = Items.ArmorSet[i];
                break;
            }
        }
        return check;
    }

    TagCompound ToTag(ShopItem item) {
        if (item == null) {
            return new TagCompound {
                ["Target"] = 0,
                ["Need"] = 0
            };
        }
        else {
            return new TagCompound {
                ["Target"] = item.Target,
                ["Need"] = item.Need
            };
        }
    }
    ShopItem FromTag(TagCompound tag) => new(tag.GetInt("Target"), tag.GetInt("Need"));
};