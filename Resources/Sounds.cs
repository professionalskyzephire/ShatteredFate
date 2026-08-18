namespace ShatteredFate.Resources;

public static class Sounds {
    static readonly System.Collections.Generic.Dictionary<string, Terraria.Audio.SoundStyle> registerSounds = [];

    public static Terraria.Audio.SoundStyle Get(string name) => registerSounds.TryGetValue(name, out var value) == true ? value : throw new System.Exception("No item in dictionary");
    public static void Set(string name, bool music = false) => registerSounds.TryAdd(name, new("ShatteredFate/Sounds/" + (music ? "Music" : "Misc") + "/" + name));

    internal static void Load() {
        Set("Rage");
        Set("EerieSound");
    }
    internal static void UnLoad() {
        registerSounds.Clear();
    }
};