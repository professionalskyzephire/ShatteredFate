namespace ShatteredFate.Core;

public class ShopItem(int targetItemType, int needItemType) {
    public int Target = targetItemType <= 0 ? throw new("It needs to be greater than") : targetItemType;
    public int Need = needItemType;
};