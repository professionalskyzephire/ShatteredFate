using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShatteredFate.Common.ModSystems.Worlds;
using ShatteredFate.Content.NPCs.Misc;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static ShatteredFate.ModUtils.UIUtils;

namespace ShatteredFate.UIElements;

public class ShadyFigureUI() : DialogBoxUI(ModContent.NPCType<ShadyFigure>()) {
    readonly ShopSlot[] _slots = new ShopSlot[40];
    readonly Button[] buttons = new Button[1];

    public override void OnInitialize() {
        for (int i = 0; i < buttons.Length; i++) {
            buttons[i] = new();
        }
        for (int i = 0; i < 40; i++) {
            _slots[i] = new(Resources.Textures.GetShadyFigureUI()[0]);
            _slots[i].SetItem(ModContent.GetInstance<ShadyFigureShop>().ShopItems[i].Target, ModContent.GetInstance<ShadyFigureShop>().ShopItems[i].Need);
        }
    }
    public override void SettingButton(ref Vector2 pos) {
        buttons[0].Setting(pos.X(4), "Shop");
        buttons[0].OnHover((i) => {
            if (i) { SoundEngine.PlaySound(SoundID.MenuOpen);}
            else { SoundEngine.PlaySound(SoundID.MenuClose); }
        });
        pos.Y += buttons[0].ButtonSize.Y;
    }
    public override void DrawButton(SpriteBatch spriteBatch, Vector2 pos) {
        if (buttons[0].ActiveButton) {
            Main.playerInventory = true;
            Main.stackSplit = 9999;
            Main.npcChatText = "";
            float xScale = 0;
            float yScale = 0;
            float x = (Main.screenWidth / 2) - 920; // 868
            float y = (Main.screenHeight / 2) - 258;

            int index = 0;

            bool hovered = false;

            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 10; j++) {
                    _slots[i * 10 + j].Draw(spriteBatch, new(x + xScale, y + yScale), 0.75f, 0.88f);
                    if (!hovered) {
                        hovered = _slots[i * 10 + j].Hover(new(x + xScale, y + yScale), Main.LocalPlayer);
                    }
                    xScale += 42.5f;
                    index++;
                }
                xScale = 0;
                yScale += 42.5f;
            }
            if (hovered) { Main.instance.MouseText(Main.hoverItemName); }
        }
        buttons[0].Draw(spriteBatch);
    }
    public override void OnDeactivate() {
        //Array.Clear(_slots);
        //Array.Clear(buttons);
    }
}