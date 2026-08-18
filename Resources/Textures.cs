using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria.ModLoader;

namespace ShatteredFate.Resources;

public static class Textures {
    static readonly Asset<Texture2D>[] rageBar = new Asset<Texture2D>[2];
    static readonly Asset<Texture2D>[] windowBG = new Asset<Texture2D>[2];

    public static Texture2D[] GetRageBar() => [.. rageBar.Select(i => i.Value)];
    public static Texture2D[] GetWindowBG() => [.. windowBG.Select(i => i.Value)];

    internal static void Load(Mod mod) {
        rageBar[0] = LoadTextures("RageBuff/BarProgess");
        rageBar[1] = LoadTextures("RageBuff/Icon");

        windowBG[0] = LoadTextures("WindowBg/Frame");
        windowBG[1] = LoadTextures("WindowBg/PartofTheFrame");

        Asset<Texture2D> LoadTextures(string name) => mod.Assets.Request<Texture2D>("Extras/" + name, ReLogic.Content.AssetRequestMode.ImmediateLoad);
    }
    internal static void UnLoad() {
        Array.Clear(rageBar);
        Array.Clear(windowBG);
    }
};