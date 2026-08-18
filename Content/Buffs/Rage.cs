using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShatteredFate.Common.Players;
using ShatteredFate.ModUtils;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Buffs;

public class Rage : ModBuff {
    static RagePlayer RPlayer => Main.LocalPlayer.GetModPlayer<RagePlayer>();

    public override void Update(Player player, ref int buffIndex) {
        int index = player.FindBuffIndex(BuffID.Rage);
        int rTime = RPlayer.GetVanillaRageBuffTime();

        if (index >= 0) {
            RPlayer.SetVanillaRageBuffTime(player.buffTime[index]);
            player.ClearBuff(BuffID.Rage);
        };
        if (rTime > 0) {
            player.GetCritChance(DamageClass.Generic) += 10;
            RPlayer.SetVanillaRageBuffTime(rTime - 1);
        };
    }
    public override bool RightClick(int buffIndex) => false;
    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare) {
        if (RPlayer.GetVanillaRageBuffTime() > 0 ) { tip += "\n" + string.Format(Loc.GetTips("Buffs.Rage.tip4"), Lang.GetBuffDescription(115), Lang.LocalizedDuration(new TimeSpan(0, 0, RPlayer.GetVanillaRageBuffTime() / 60), true, false)); };
        if (RPlayer.GetRage() != RPlayer.GetMaxRage() && RPlayer.GetCDTime() == 0 && RPlayer.GetDurationTime() == 0) { tip += "\n" + string.Format(Loc.GetTips("Buffs.Rage.tip0"), RPlayer.GetRage(), RPlayer.GetMaxRage()); }
        else if (RPlayer.GetRage() == RPlayer.GetMaxRage()) { tip += "\n" + Loc.GetTips("Buffs.Rage.tip1"); };
        if (RPlayer.GetDurationTime() > 0) { tip += "\n" + string.Format(Loc.GetTips("Buffs.Rage.tip2"), Lang.LocalizedDuration(new TimeSpan(0, 0, RPlayer.GetDurationTime() / 60), true, false)); };
        if (RPlayer.GetCDTime() > 0) { tip += "\n" + string.Format(Loc.GetTips("Buffs.Rage.tip3"), Lang.LocalizedDuration(new TimeSpan(0, 0, RPlayer.GetCDTime() / 60), true, false)); };
    }
    public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams) { drawParams.Position += RPlayer.GetDurationTime() > 0 ? Main.rand.NextVector2Circular(1f, 1f) : Vector2.Zero; return true; }
    public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams) {
        Texture2D[] asset = Resources.Textures.GetRageBar();
        Rectangle frame = new();
        int frames = RPlayer.GetVanillaRageBuffTime() > 0 ? 1 : 0;

        if (RPlayer.GetRage() <= RPlayer.GetMaxRage()) { frame = new(0, RPlayer.GetVanillaRageBuffTime() > 0 ? 16 : 0, (int)(asset[0].Width * ModUtils.UIUtils.GetProgress(RPlayer.GetRage(), RPlayer.GetMaxRage())), 8); };
        if (RPlayer.GetDurationTime() <= (10 * 60) && RPlayer.GetRageStatus()) { frame = new(0, RPlayer.GetVanillaRageBuffTime() > 0 ? 16 : 0, (int)(asset[0].Width * ModUtils.UIUtils.GetProgress(RPlayer.GetDurationTime(), 10 * 60)), 8); };
        if (RPlayer.GetCDStatus() && RPlayer.GetCDTime() <= (15 * 60)) { frame = new(0, 8, (int)(asset[0].Width * ModUtils.UIUtils.GetProgress(RPlayer.GetCDTime(), 15 * 60, true)), 8); };

        spriteBatch.Draw(asset[0], new Vector2(drawParams.Position.X + 16, drawParams.Position.Y + 44), frame, drawParams.DrawColor, 0f, asset[0].Size() / 2f, 1f, SpriteEffects.None, 0);
        spriteBatch.Draw(asset[1], new Vector2(drawParams.Position.X + 16, drawParams.Position.Y + 16), asset[1].Frame(1, 2, 0, frames), drawParams.DrawColor, 0f, asset[1].Frame(1, 2, 0, frames).Size() / 2f, 1f, SpriteEffects.None, 1);
    }
};