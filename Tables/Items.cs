using ShatteredFate.Core;
using System.Collections.Generic;
using static ShatteredFate.ID.WorldProgress;

namespace ShatteredFate.Tables;

public static class Items {
    public static List<ShadyFigureItem> Weapon { get; private set; } = [];
    public static List<ShadyFigureItem> Armor { get; private set; } = [];
    public static List<ShadyFigureItem> Acc { get; private set; } = [];
    public static List<ShadyFigureItem> Consumables { get; private set; } = [];
    public static List<ShadyFigureItem> Material { get; private set; } = [];
    public static List<ShadyFigureItem> Misc { get; private set; } = [];
    public static List<ShadyFigureItem.ArmorSet> ArmorSet { get; private set; } = [];


    public static void Load() {
        RegisterWeapon(1, PreBoss);
        RegisterWeapon(4, PreBoss);
        RegisterConsumables(5, PreBoss);
        RegisterWeapon(6, PreBoss);
        RegisterWeapon(7, PreBoss);
        RegisterMisc(8, PreBoss);
        RegisterMisc(9, PreBoss);
        RegisterWeapon(10, PreBoss);
        for (int i = 11; i < 15; i++) { RegisterMaterial(i, PreBoss); }
        for (int i = 15; i < 19; i++) { RegisterAcc(i, PreBoss); }
        for (int i = 19; i < 24; i++) { RegisterMaterial(i, PreBoss); }
        RegisterWeapon(24, PreBoss);
        RegisterConsumables(28, PreBoss);
        RegisterMisc(29, PreBoss);
        RegisterMisc(31, PreBoss);
        RegisterMaterial(38, PreBoss);
        for (int i = 39; i < 43; i++) { RegisterWeapon(i, PreBoss); }
        RegisterMisc(43, PreBoss);
        for (int i = 44; i < 48; i++) { RegisterWeapon(i, PreBoss); }
        RegisterAcc(49, PreBoss);
        RegisterMisc(50, PreBoss);
        RegisterWeapon(51, PreBoss);
        RegisterAcc(53, PreBoss);
        RegisterAcc(54, PreBoss);
        RegisterWeapon(55, PreBoss);
        RegisterMaterial(56, PreBoss);
        RegisterMaterial(57, PreBoss);
        RegisterWeapon(64, PreBoss);
        RegisterWeapon(65, PreBoss);
        RegisterMaterial(67, PreBoss);
        RegisterMaterial(68, PreBoss);
        RegisterMaterial(69, PreBoss);
        RegisterMisc(70, PreBoss);
        RegisterMaterial(75, PreBoss);
        RegisterMisc(84, PreBoss);
        RegisterMaterial(85, PreBoss);
        RegisterMaterial(86, PreBoss);
        RegisterArmor(88, PreBoss);
        for (int i = 95; i < 100; i++) { RegisterWeapon(i, PreBoss); }
        for (int i = 103; i < 105; i++) { RegisterWeapon(i, EvilBoss); }
        for (int i = 109; i < 111; i++) { RegisterWeapon(i, PreBoss); }
        RegisterAcc(111, PreBoss);
        RegisterWeapon(112, Skeletron);
        RegisterWeapon(113, Skeletron);
        RegisterMisc(115, PreBoss);
        for (int i = 119; i < 123; i++) { RegisterWeapon(i, EvilBoss); }
        RegisterWeapon(127, EvilBoss);
        RegisterAcc(128, PreBoss);
        RegisterMisc(148, Skeletron);

        RegisterArmorSet(new(89, 80, 76, PreBoss));
        RegisterArmorSet(new(90, 81, 77, PreBoss));
        RegisterArmorSet(new(91, 82, 78, PreBoss));
        RegisterArmorSet(new(92, 83, 79, PreBoss));
        RegisterArmorSet(new(102, 101, 100, EvilBoss));
    }

    public static void RegisterWeapon(int target, int progress) => Weapon.Add(new(target, progress, ShadyFigureItem.ItemType.Weapon));
    public static void RegisterArmor(int target, int progress) => Armor.Add(new(target, progress, ShadyFigureItem.ItemType.Armor));
    public static void RegisterArmorSet(ShadyFigureItem.ArmorSet set) {
        Armor.Add(new(set.Helmet, set.Progress, ShadyFigureItem.ItemType.Armor));
        Armor.Add(new(set.Chainmail, set.Progress, ShadyFigureItem.ItemType.Armor));
        Armor.Add(new(set.Greaves, set.Progress, ShadyFigureItem.ItemType.Armor));
        ArmorSet.Add(set);
    }
    public static void RegisterAcc(int target, int progress) => Acc.Add(new(target, progress, ShadyFigureItem.ItemType.Acc));
    public static void RegisterConsumables(int target, int progress) => Consumables.Add(new(target, progress, ShadyFigureItem.ItemType.Consumables));
    public static void RegisterMaterial(int target, int progress) => Material.Add(new(target, progress, ShadyFigureItem.ItemType.Material));
    public static void RegisterMisc(int target, int progress) => Misc.Add(new(target, progress, ShadyFigureItem.ItemType.Misc));

    public static void UnLoad() {
        Weapon.Clear();
        Armor.Clear();
        Acc.Clear();
        Consumables.Clear();
        Material.Clear();
        Misc.Clear();
    }
};