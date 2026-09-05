using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static ShatteredFate.ModUtils.UIUtils;

namespace ShatteredFate.UIElements;

public class DialogBoxUI(int npcType) : UIState {
    readonly TextSize _cache = new();

    Vector2 ButtonPos = new();

    public bool firstStart = true;

    public override void Update(GameTime gameTime) { if (Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != npcType) { ModContent.GetInstance<SFMod>().SFUI.SetState(null); }; }
    sealed protected override void DrawSelf(SpriteBatch spriteBatch) {
        string text = Main.npcChatText;
        if (text == "!") { return; }

        _cache.PrepareCache(text, Color.AliceBlue);
        List<List<TextSnippet>> lines = _cache.TextLines;

        float maxScaleX = 0;
        float scaleY = 0;

        for (int i = 0; i < lines.Count; i++) {
            for (int j = 0; j < lines[i].Count; j++) {
                float scaleX = TextSize.Get(lines[i][j].Text).X;
                if (scaleX > maxScaleX) { maxScaleX = scaleX; };
                scaleY = TextSize.Get(lines[i][j].Text).Y;
            }
        }

        DynamicSpriteFont font = FontAssets.MouseText.Value;
        Texture2D[] asset = Resources.Textures.GetShadyFigureUI();

        Vector2 pos = new(Main.screenWidth / 2f - maxScaleX / 2f, Main.screenHeight / 2 - (Main.screenHeight / 2.5f));

        ButtonPos = new(pos.X, pos.Y + scaleY * _cache.AmountOfLines + 8);
        SettingButton(ref ButtonPos);

        DrawTexture<Rectangle>(spriteBatch, asset[2], new((int)pos.X - 8, (int)pos.Y + (-12), (int)maxScaleX + 16, (int)(ButtonPos.Y - pos.Y + 12)), null, new(44, 44, 44), origin: Vector2.Zero);
        DrawTexture<Rectangle>(spriteBatch, asset[2], new((int)pos.X - 6, (int)pos.Y + (+14), (int)maxScaleX + 10, 2), null, new(65, 65, 65), origin: new Vector2(0, asset[0].Height) / 2f);
        DrawTexture<Rectangle>(spriteBatch, asset[2], new((int)pos.X - 6, (int)pos.Y + (+16), (int)maxScaleX + 10, 2), null, new(136, 136, 136), origin: new Vector2(0, asset[0].Height) / 2f);

        DrawTexture(spriteBatch, asset[1], pos.X(-8).Y(-8), asset[1].Frame(1, 2, 0, 0));
        DrawTexture(spriteBatch, asset[1], pos.X(8 + maxScaleX).Y(-8), asset[1].Frame(1, 2, 0, 0), effects: SpriteEffects.FlipHorizontally);
        DrawTexture(spriteBatch, asset[1], ButtonPos.X(-8).Y(scaleY / 2 - 16), asset[1].Frame(1, 2, 0, 1));
        DrawTexture(spriteBatch, asset[1], ButtonPos.X(8 + maxScaleX).Y(scaleY / 2 - 16), asset[1].Frame(1, 2, 0, 1), effects: SpriteEffects.FlipHorizontally);

        DrawText(spriteBatch, Lang.GetNPCName(npcType).Value, pos.X((maxScaleX + 110) / 2f).Y(-8), orgin: TextSize.Get(Lang.GetNPCName(npcType).Value));
        for (int i = 0; i < _cache.AmountOfLines; i++) { ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, [.. lines[i]], pos + new Vector2(0, i * 30), 0f, Color.AliceBlue, Color.Black, Vector2.Zero, Vector2.One, out _); }
        DrawButton(spriteBatch, ButtonPos);
    }
    public virtual void DrawButton(SpriteBatch spriteBatch, Vector2 pos) { }
    public virtual void SettingButton(ref Vector2 pos) { }
};