using Terraria.ModLoader;
using Terraria.UI;

namespace ShatteredFate;

public class SFMod : Mod {
    SFMod() => _instance = this;

    public UserInterface SFUI { get; private set; }

    public const string VanillaTexture = "Terraria/Images/";
    public const string ExtrasPath = "ShatteredFate/Extras/";
    public const string BlankTexture = "ShatteredFate/Extras/Invisible";
    public const string MagicPixel = "ShatteredFate/Extras/MagicPixel";

    static Mod _instance;
    static Mod _musicMod;

    public static string ModName => Instance is null ? "SFMod" : Instance.DisplayName;

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