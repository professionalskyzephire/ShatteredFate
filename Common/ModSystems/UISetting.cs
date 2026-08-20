using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        if (invIndex != -1) {
            layers.Insert(invIndex, new LegacyGameInterfaceLayer($"{SFMod.ModName}: Shady Figure UI", () => { Mods.SFUI.Draw(sb, new()); return true; }, InterfaceScaleType.UI));
        }
    }
    public override void UpdateUI(GameTime gameTime) {
        Mods.SFUI.Update(gameTime);
    }
}