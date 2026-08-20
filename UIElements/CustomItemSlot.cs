using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using ShatteredFate.ModUtils;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.Audio;

namespace ShatteredFate.UIElements;

public class CustomItemSlot(Texture2D itemSlotTexture) {
    Item _insertItem = null;
    public Item ItemInSlot => _insertItem;

    Texture2D _slotTexture = itemSlotTexture;

    public void SetTextureSlot(Texture2D value) => _slotTexture = value;

    public void Hover(Vector2 pos, Player player) {
        if (!UIUtils.Hover(pos, _slotTexture)) { return; }
        _insertItem ??= new(ItemID.None);
        player.mouseInterface = true;
        if (ItemInSlot.type != ItemID.None) {
            Main.HoverItem = new(ItemInSlot.type);
            Main.instance.MouseText(Main.hoverItemName);
        }
    }
    public void HandleClick(Vector2 pos, Func<Item, bool> func) {
        if (_slotTexture == null) { throw new Exception("Need texture"); }
        if (!UIUtils.Hover(pos, _slotTexture)) { return; };
        if (UIUtils.LeftClick()) {
            if (_insertItem.type == ItemID.None) { SetItem(func); return; };
            if (Main.mouseItem.type == ItemID.None) { GetItem(); return; };
        };
    }
    void SetItem(Func<Item, bool> func) {
        if (Main.mouseItem.type == ItemID.None) { return; };
        if (!func(Main.mouseItem)) { return; };
        _insertItem = new Item(Main.mouseItem.type);
        Main.mouseItem.TurnToAir();
    }
    void GetItem() {
        if (_insertItem.type == ItemID.None) { return; }
        Main.mouseItem = new(_insertItem.type);
        _insertItem = new(ItemID.None);
        SoundEngine.PlaySound(SoundID.Grab);
    }
    public void Draw(SpriteBatch sB, Vector2 pos, float slotScale = 1f, float itemScale = 1f) {
        sB.Draw(_slotTexture, pos, null, Color.White, 0f, _slotTexture.Size() / 2f, slotScale, SpriteEffects.None, 1);

        if (_insertItem != null) {
            sB.Draw(TextureAssets.Item[_insertItem.type].Value, pos, null, Color.White, 0f, TextureAssets.Item[_insertItem.type].Size() / 2f, itemScale, SpriteEffects.None, 1);
        };
    }
};
