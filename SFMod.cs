using Terraria.ModLoader;
using Terraria.UI;

namespace ShatteredFate;

public class SFMod : Mod {
    SFMod() => _instance = this;

    public UserInterface SFUI { get; private set; }

    public static string ModName => Instance is null ? "SFMod" : Instance.DisplayName;

    static Mod _instance;
    static Mod _musicMod;

    public static Mod Instance => _instance;
    internal static Mod MusicMod {
        get {
            if (_musicMod == null && !ModLoader.TryGetMod("ShatteredFateMusic", out _musicMod)) { _musicMod = null; }
            return _musicMod;
        }
    }

    public override void Load() {
        Loader.Load(this);
        SFUI = new UserInterface();
    }
    public override void Unload() {
        _musicMod = null;
        Loader.Unload();
    }
};