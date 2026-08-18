using Microsoft.Xna.Framework;
using ShatteredFate.Common.GlobalItems;

namespace ShatteredFate.ModUtils;

public class CustomColor {
    public CustomColor(Color[] colors) => this._colors = colors;

    Color[] _colors;

    ColorType _type = ColorType.Firist;

    public void SevaColorType(ColorType value) => _type = value;
    public ColorType GetSaveColorType() => _type;

    private int _time;

    bool _endAnimation = false;

    public int GetTime() => _time;
    public void Update() => _time++;

    public bool LastColor() => _endAnimation;

    public void SetColors(Color[] colors) => this._colors = colors;
    public Color GetAnimatedItemColor(int time = 60) {
        if (_colors.Length <= 1) {
            _endAnimation = true;
            return _colors[0]; 
        };

        int totalTime = time * _colors.Length;

        int timer = _time % totalTime;
        int index = timer / time;

        float t = timer % time / (float)time;

        Color from = _colors[index];
        Color to = _colors[(index + 1) % _colors.Length];

        Update();

        if (index == _colors.Length - 1) { _endAnimation = true; };
        if (_endAnimation && index == 0) { _time = 0; _endAnimation = false; };

        return Color.Lerp(from, to, t);
    }
}