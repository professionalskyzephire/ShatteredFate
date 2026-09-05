using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShatteredFate.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ShatteredFate.Common.ModSystems;

public class UISetting : ModSystem {
    SFMod Mods => ModContent.GetInstance<SFMod>();

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        SpriteBatch sb = Main.spriteBatch;

        int invIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        foreach (CustomDialogBox dialogBox in Manager.RegisterUI) {
            dialogBox.RegisterLayer(layers, Mods.SFUI);
        }
    }
    public override void UpdateUI(GameTime gameTime) {
        Mods.SFUI.Update(gameTime);
    }
}