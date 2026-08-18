using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace ShatteredFate;

public class KeyBind {
    static ModKeybind _accessoryAbilityKey;
    static ModKeybind _magnetismKey;
    static ModKeybind _rageKey;

    public static ModKeybind GetAccessoryKey() => _accessoryAbilityKey;
    public static ModKeybind GetMagnetismKey() => _magnetismKey;
    public static ModKeybind GetRageKey() => _rageKey;

    public static void Load(Mod mod) {
        _accessoryAbilityKey = KeybindLoader.RegisterKeybind(mod, "Accessory Ability Key", Keys.X);
        _magnetismKey = KeybindLoader.RegisterKeybind(mod, "Hypermagnetism Key", Keys.Q);
        _rageKey = KeybindLoader.RegisterKeybind(mod, "Rage Ability Key", Keys.R);
    }
    public static void UnLoad() { }
};