using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShatteredFate.Common.ModSystems.Worlds;
using ShatteredFate.Content.NPCs.Misc;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ShatteredFate.UIElements;

public class ShadyFigureUI : UIState {
    readonly ShopSlot[] _slots = new ShopSlot[40];

    public override void OnInitialize() {
        for (int i = 0; i < 40; i++) {
            _slots[i] = new(Resources.Textures.GetShadyFigureUI()[0]);
            _slots[i].SetItem(ModContent.GetInstance<ShadyFigureShop>().ShopItems[i].Target, ModContent.GetInstance<ShadyFigureShop>().ShopItems[i].Need);
        }
    }
    public override void Update(GameTime gameTime) {
        if (Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<ShadyFigure>()) {
            ModContent.GetInstance<SFMod>().SFUI.SetState(null);
        }
    }
    public override void Draw(SpriteBatch spriteBatch) {
        int xScale = 0;
        int yScale = 0;
        float x = (Main.screenWidth / 2) - 920; // 868
        float y = (Main.screenHeight / 2) - 254;

        int index = 0;

        bool hovered = false;

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 10; j++) {
                _slots[i * 10 + j].Draw(spriteBatch, new(x + xScale, y + yScale), 0.65f, 0.75f);
                if (!hovered) {
                    hovered = _slots[i * 10 + j].Hover(new(x + xScale, y + yScale), Main.LocalPlayer);
                }
                xScale += 40;
                index++;
            }
            xScale = 0;
            yScale += 40;
        }
        if (hovered) { Main.instance.MouseText(Main.hoverItemName); }
    }
}