using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShatteredFate.ModUtils;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI.Chat;

namespace ShatteredFate.UIElements;

public class Button {
    int i = 0;
    Vector2 _pos = new();
    Vector2 _scale = new();

    string _text = "";

    bool _hover = false;
    bool _active = false;
    bool _click = false;

    public Vector2 ButtonSize => _scale;
    public bool ActiveButton => _active;

    public void Setting(Vector2 pos, string text) {
        _pos = pos;
        _text = text;
        _scale = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, new Vector2(1f));
    }
    public bool OnHover(Action<bool> click) {
        if (UIUtils.HoverText(_pos, _text)) {
            if (UIUtils.LeftClick() && !_click) {
                click.Invoke(_active);
                _active = !_active;
                _click = true;
            }
            else { _click = false; }
            if (!_hover) { SoundEngine.PlaySound(SoundID.MenuTick); };
            _hover = true;
            return true;
        }
        _hover = false;
        return false;
    }
    public void Draw(SpriteBatch sB) => UIUtils.DrawText(sB, _text, _pos + _scale * 0.5f, orgin: _scale * .5f, color: new(Main.mouseTextColor, (int)(Main.mouseTextColor / 1.1), Main.mouseTextColor / 2, Main.mouseTextColor), color1: _hover ? Color.Brown : Color.Black, scale: new Vector2(1f) * (_hover ? 1.2f : 1f));
};