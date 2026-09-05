using Microsoft.Xna.Framework;
using ReLogic.Content;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ShatteredFate.UIElements;

public class TextSize {
    public List<List<TextSnippet>> TextLines { get; private set; }
    public int AmountOfLines { get; private set; }

    public void PrepareCache(string text, Color baseColor) {
        TextLines = Utils.WordwrapStringSmart(text, baseColor, FontAssets.MouseText.Value, 460, 10);
        AmountOfLines = TextLines.Count;
    }
    public static Vector2 Get(string text, Asset<DynamicSpriteFont> font = null) { font ??= FontAssets.MouseText; return font.Value.MeasureString(text); }
};