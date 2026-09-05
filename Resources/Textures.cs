using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria.ModLoader;

namespace ShatteredFate.Resources;

public static class Textures {
    static readonly Asset<Texture2D>[] _rageBar = new Asset<Texture2D>[2];
    static readonly Asset<Texture2D>[] _windowBG = new Asset<Texture2D>[2];
    static readonly Asset<Texture2D>[] _shadyFigureUI = new Asset<Texture2D>[3];

    public static Texture2D[] GetRageBar() => [.. _rageBar.Select(i => i.Value)];
    public static Texture2D[] GetWindowBG() => [.. _windowBG.Select(i => i.Value)];
    public static Texture2D[] GetShadyFigureUI() => [.. _shadyFigureUI.Select(i => i.Value)];

    internal static void Load(Mod mod) {
        _rageBar[0] = LoadTextures("RageBuff/BarProgess");
        _rageBar[1] = LoadTextures("RageBuff/Icon");

        _windowBG[0] = LoadTextures("WindowBg/Frame");
        _windowBG[1] = LoadTextures("WindowBg/PartofTheFrame");

        _shadyFigureUI[0] = LoadTextures("DialogueBoxes/ShadyFigure/ItemSlot");
        _shadyFigureUI[1] = LoadTextures("DialogueBoxes/ShadyFigure/BoxFrame");
        _shadyFigureUI[2] = LoadTextures("DialogueBoxes/ShadyFigure/MGPixel");

        Asset<Texture2D> LoadTextures(string name) => mod.Assets.Request<Texture2D>("Extras/" + name, AssetRequestMode.ImmediateLoad);
    }
    internal static void UnLoad() {
        Array.Clear(_rageBar);
        Array.Clear(_windowBG);
        Array.Clear(_shadyFigureUI);
    }
};