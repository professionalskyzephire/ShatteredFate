using ShatteredFate.Common.ModSystems.Hooks;
using ShatteredFate.Tables;
using Terraria.ModLoader;

namespace ShatteredFate;

public class Loader {
    // priority: 1 - table, 2 Resources, 3 - hook, 4 - custom data 5 - keyBind

    public static void Load(Mod mod) {
        Buffs.Load();

        Resources.Textures.Load(mod);
        Resources.Sounds.Load();

        Ons.Load(mod);
        ILs.Load(mod);

        KeyBind.Load(mod);
    }
    public static void Unload() {
        Buffs.UnLoad();

        Resources.Textures.UnLoad();
        Resources.Sounds.UnLoad();

        Ons.Unload();
        ILs.Unload();

        KeyBind.UnLoad();
    }
};