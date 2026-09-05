using Terraria;
using Terraria.ModLoader;
using ShatteredFate.Common.Players;

namespace ShatteredFate.Content.Buffs;

public class MagnetismBuff : ModBuff {
    public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<MagnetismPlayer>().SetGrabRange(player.GetModPlayer<MagnetismPlayer>().GetGrabRange() + 60);
    public override bool ReApply(Player player, int time, int buffIndex) {
        MagnetismPlayer modPlayer = player.GetModPlayer<MagnetismPlayer>();
        if (modPlayer.GetAbilityStatus()) { return false; };
        modPlayer.SetStacks(modPlayer.GetStacks() + 1);
        if (modPlayer.GetStacks() < 14) return false;

        Main.NewText(string.Format(ModUtils.Loc.GetChat("Ability.Magnetism.Unlock"), ModUtils.UIUtils.GetButtonName(KeyBind.GetMagnetismKey())), 50, 255, 130);
        modPlayer.SetAbilityStatus(true);
        return false;
    }
}