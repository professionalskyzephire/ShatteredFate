namespace ShatteredFate.Core;

public class ShadyFigureItem(int targetItemType, int progress, ShadyFigureItem.ItemType type) : ShopItem(targetItemType, 0) {
    public ItemType Type { get; private set; } = type;
    /// <summary>
    /// 0 - Pre Boss, 1 - King Slime, 2 - Eye Ctulhu, 3 - Any Evil Boss, 4 - Quin Bee, 5 - Skeletron, 6 - Olen, 7 - WOF, 
    /// 8 - Quin Slime, 9, 10, 11 - MechBoss, 12 - Plantera, 13 - Golem, 14 - Empreses of Light, 15 - Cultist, 16 - Moon Lord
    /// </summary>
    public int WorldProgress { get; private set; } = progress;

    public enum ItemType : byte { Weapon, Armor, Acc, Consumables, Material, Misc };
    public class ArmorSet(int[] helmet, int chainmail, int greaves, int progress) {
        public int[] Helmet { get; private set; } = helmet;
        public int Chainmail { get; private set; } = chainmail;
        public int Greaves { get; private set; } = greaves;
        public int Progress { get; private set; } = progress;
    }
};