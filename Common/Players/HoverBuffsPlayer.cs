using ShatteredFate.ModUtils;
using Terraria.ModLoader;

namespace ShatteredFate.Common.Players;

public class HoverBuffsPlayer : ModPlayer {
    int _buffType = -1;

    string _buffName = "";
    string _buffTooltips = "";

    bool _hover = false;
    internal bool _tic = false;

    public int GetHoverBuff() => _buffType;
    public void SetHoverBuff(int value) => _buffType = value;

    public string GetHoverBuffName() => _buffName;
    public void SetHoverBuffName(string value) => _buffName = value is null ? "" : _buffName = value;

    public string[] GetHoverBuffTooltipsArray() => _buffTooltips.Split("\n");
    public void SetHoverBuffTooltips(string value) => _buffTooltips += value;

    public bool IsHover() => _hover;
    public void SetHover(bool value) => _hover = value;

    public string[] GetAllHoverBuffText() {
        string[] text = new string[GetHoverBuffTooltipsArray().Length + 1];
        string[] clearTips = UIUtils.ClearText(GetHoverBuffTooltipsArray());
        text[0] = UIUtils.ClearText([GetHoverBuffName()])[0];
        for (int i = 1; i < text.Length; i++) { text[i] = clearTips[i - 1]; }
        return text;
    }

    public override void ResetEffects() {
        if(!IsHover()) { _buffType = -1; };
        _buffName = "";
        _buffTooltips = "";
    }
}