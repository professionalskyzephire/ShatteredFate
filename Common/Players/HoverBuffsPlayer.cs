using ShatteredFate.ModUtils;
using Terraria.ModLoader;

namespace ShatteredFate.Common.Players;

public class HoverBuffsPlayer : ModPlayer {
    int _buffType = -1;

    string _buffName = "";
    string _buffTooltips = "";

    bool _hover = false;
    internal bool _tic = false;

    public int BuffType { get => _buffType; set => _buffType = value; }

    public string BuffName { get => _buffName; set => _buffName = value is null ? "" : value; }

    public string[] GetBuffTooltips() => _buffTooltips.Split("\n");
    public void SetBuffTooltips(string value) => _buffTooltips += value;

    public bool Hover { get => _hover; set => _hover = value; }

    public string[] GetAllHoverBuffText() {
        string[] text = new string[GetBuffTooltips().Length + 1];
        string[] clearTips = UIUtils.ClearText(GetBuffTooltips());
        text[0] = UIUtils.ClearText([BuffName])[0];
        for (int i = 1; i < text.Length; i++) { text[i] = clearTips[i - 1]; }
        return text;
    }

    public override void ResetEffects() {
        if(!Hover) { _buffType = -1; };
        _buffName = "";
        _buffTooltips = "";
    }
}