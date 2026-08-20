using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShatteredFate.Common.GlobalItems;
using ShatteredFate.Core;
using ShatteredFate.ModUtils;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace ShatteredFate.UIElements; 

public class ShopSlot(Texture2D itemSlotTexture) {
    ShopItem _insertItem = null;
    public ShopItem ItemInSlot => _insertItem;

    Texture2D _slotTexture = itemSlotTexture;
    
    int _clickTimer = 0;
    int _buyTime = 0;

    public void SetItem(int target, int value) => _insertItem = new ShopItem(target, value);
    public void SetTextureSlot(Texture2D value) => _slotTexture = value;

    public bool Hover(Vector2 pos, Player player) {
        if (_slotTexture == null) { throw new Exception("Need texture"); }
        if (UIUtils.Hover(pos, _slotTexture)) {
            player.mouseInterface = true;
            if (Main.mouseItem.type == ItemID.None) {
                Main.HoverItem = new(ItemInSlot.Target);
                ItemInfo tooltip = new(ItemInSlot.Target, -1) { Info = new Terraria.ModLoader.TooltipLine[2] };
                tooltip.Info[0] = new(SFMod.Instance, "BuyInfo", Loc.GetTips("GlobalItems.ItemSlots." + -3) + " " + Lang.GetItemName(ItemInSlot.Need) + $" ([i:{ItemInSlot.Need}])") { OverrideColor = Colors.RarityAmber };
                string text = Loc.ContainerName(player, ItemInSlot.Need, out int value1, out int value2);
                tooltip.Info[1] = new(SFMod.Instance, "HaveInfo", value1 == -1 ? text : Loc.GetTips("GlobalItems.ItemSlots." + -2) + " " + value2 + " " + text) { OverrideColor = value1 == -1 ? Color.Red : Colors.RarityGreen };
                Main.HoverItem.GetGlobalItem<ItemSlots>().SetNewInfo(tooltip);
                Main.instance.MouseText(Main.hoverItemName);
            };
            if (SFUtils.RightClickRepeat(ref _clickTimer, ref _buyTime)) { BuyItem(); };
            return true;
        }
        else{ return false; };
    }
    void BuyItem() {
        if (ItemInSlot.Target == ItemID.None) { return; };
        if (Main.mouseItem.type != ItemInSlot.Target && Main.mouseItem.type != ItemID.None) { return; };
        if (SFUtils.CheckNeedItem(Main.LocalPlayer, ItemInSlot.Need, 0, out int _, out int value)) {
            if (Main.mouseItem.type == ItemInSlot.Target) {
                if (Main.mouseItem.stack + (_buyTime == 0 ? 1 : _buyTime) >= Main.mouseItem.maxStack) {
                    Main.mouseItem.stack = Main.mouseItem.maxStack;
                    SFUtils.CheckNeedItem(Main.LocalPlayer, ItemInSlot.Need, _buyTime);
                    return;
                }
                else {
                    if (_buyTime > value) { _buyTime = value; };
                    Main.mouseItem.stack += _buyTime == 0 ? 1 : _buyTime;
                    SFUtils.CheckNeedItem(Main.LocalPlayer, ItemInSlot.Need, (_buyTime == 0 ? 1 : _buyTime));
                };
                _buyTime++;
            }
            else { 
                Main.mouseItem = new(ItemInSlot.Target);
                SFUtils.CheckNeedItem(Main.LocalPlayer, ItemInSlot.Need, (_buyTime == 0 ? 1 : _buyTime));
            };
            SoundEngine.PlaySound(SoundID.Coins);
        };
    }
    public void Draw(SpriteBatch sB, Vector2 pos, float slotScale = 1f, float itemScale = 1f) {
        sB.Draw(_slotTexture, pos, null, Color.White, 0f, _slotTexture.Size() / 2f, slotScale, SpriteEffects.None, 1);
        Main.GetItemDrawFrame(ItemInSlot.Target, out Texture2D texture, out Rectangle itemFrame);
        sB.Draw(texture, pos, itemFrame, Color.White, 0f, itemFrame.Size() / 2f, itemScale, SpriteEffects.None, 1);
    }
};