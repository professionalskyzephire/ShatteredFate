namespace ShatteredFate.Core;

public class ShopItem(int targetItemType, int needItemType) {
    public int Target = targetItemType <= -1 ? throw new("The value cannot be -1 or less") : targetItemType;
    public int Need = needItemType < 0 ? 0 : needItemType;
};