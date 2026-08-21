using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShatteredFate.Common.Players;
using ShatteredFate.Content.Items.Accessories;
using ShatteredFate.ModUtils;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Common.GlobalItems;

public class RageItem : GlobalItem {
    public override bool InstancePerEntity => true;

    readonly CustomColor cColor = new([]);

    int _maxRage = 0;
    int _scaleRage = 0;

    public int GetMaxRage() => _maxRage + GetScaleRage();
    public void SetMaxRage(int value) => _maxRage = value;
    public int GetScaleRage() => _scaleRage;
    public void SetScaleRage(int value) => _scaleRage = value;

    static Color[] GetColor(int rage, int maxRage) {
        Color[] transform = [Color.White];
        if (rage == 0 || (float)rage / maxRage * 100f < 15) { transform = [Colors.RarityTrash]; }
        if (rage != 0) {
            if ((float)rage / maxRage * 100f >= 15) { transform = [new(146, 110, 110)]; };
            if ((float)rage / maxRage * 100f >= 30) { transform = [new(164, 92, 92)]; };
            if ((float)rage / maxRage * 100f >= 45) { transform = [new(200, 58, 58)]; };
            if ((float)rage / maxRage * 100f >= 50) { transform = [new(218, 40, 40)]; };
            if ((float)rage / maxRage * 100f >= 60) { transform = [new(236, 20, 20)]; };
            if ((float)rage / maxRage * 100f >= 75) { transform = [Color.Red]; };
            if ((float)rage / maxRage * 100f >= 80) { transform = [new(245, 0, 0)]; };
            if ((float)rage / maxRage * 100f >= 90) { transform = [new(235, 0, 0)]; };
        }
        return transform;
    }
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        RagePlayer rPlayer = Main.LocalPlayer.GetModPlayer<RagePlayer>();
        if (item.type == ItemID.RagePotion && PlayersExpansions.CheckAcc(rPlayer.Player, ModContent.ItemType<AmuletofRage>())) { tooltips.Insert(3, new(Mod, $"{SFMod.ModName}:RagePotion", Loc.GetTips("GlobalItems.RageItem.RagePotion"))); };
        if (GetMaxRage() > 0) {
            Color[] transform = [Color.White];

            if (rPlayer.GetRage() == 0) { cColor.SevaColorType(ColorType.Firist); }
            if (cColor.LastColor()) {
                if (rPlayer.GetRage() == GetMaxRage()) { cColor.SevaColorType(ColorType.Final); }
                if (rPlayer.GetRageStatus()) { cColor.SevaColorType(ColorType.Final); }
            }
            if (rPlayer.GetRage() < GetMaxRage()) { transform = [new(146, 110, 110), new(164, 92, 92), new(200, 58, 58), new(218, 40, 40), new(236, 20, 20), Color.Red]; }
            if (rPlayer.GetRage() == GetMaxRage() && cColor.GetSaveColorType() == ColorType.Final) { transform = [Color.Red, Color.DarkRed]; }
            if (rPlayer.GetRageStatus() && cColor.GetSaveColorType() == ColorType.Final) { transform = GetColor(rPlayer.GetDurationTime(), 10 * 60); }

            cColor.SetColors(transform);

            tooltips.Add(new(Mod, $"{SFMod.ModName}:RAbility", string.Format(Loc.GetTips("GlobalItems.RageItem.RAbility"), UIUtils.GetButtonName(KeyBind.GetRageKey()))) { OverrideColor = cColor.GetAnimatedItemColor() });
        };
    }
    public override void PostDrawTooltip(Item item, System.Collections.ObjectModel.ReadOnlyCollection<DrawableTooltipLine> lines) {
        SpriteBatch sB = Main.spriteBatch;
        foreach (DrawableTooltipLine line in lines) {
            if (line.Name == $"{SFMod.ModName}:RAbility") {
                RagePlayer rPlayer = Main.LocalPlayer.GetModPlayer<RagePlayer>();
                int barWidth = 0;

                if (rPlayer.GetRage() <= rPlayer.GetMaxRage()) { barWidth = (int)(FontAssets.MouseText.Value.MeasureString(line.Text).X * UIUtils.GetProgress(rPlayer.GetRage(), rPlayer.GetMaxRage(), true)); };
                if (rPlayer.GetDurationTime() <= (10 * 60) && rPlayer.GetRageStatus()) { barWidth = 0; };
                if (rPlayer.GetCDTime() <= (15 * 60) && rPlayer.GetCDStatus()) { barWidth = (int)(FontAssets.MouseText.Value.MeasureString(line.Text).X * UIUtils.GetProgress(rPlayer.GetCDTime(), 15 * 60, true)); };

                sB.Draw(TextureAssets.MagicPixel.Value, new Rectangle(line.X, line.Y + 9, barWidth, 4), Color.Black);
                return;
            };
        }
    }
};
public enum ColorType : byte { 
    Firist = 0, Mid = 1, Final = 2 
}