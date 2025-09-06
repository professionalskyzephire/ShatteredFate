using Terraria;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Buffs;

public class MagnetismBuff : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<SFPlayer>().GrabRangeBoost += 60;
    }

    public override bool ReApply(Player player, int time, int buffIndex)
    {
        SFPlayer modPlayer = player.GetModPlayer<SFPlayer>();
        if (modPlayer.MagnetismAbility) return false;
        
        modPlayer.MagnetismStacks++;
        if (player.GetModPlayer<SFPlayer>().MagnetismStacks < 29) return false;
        player.GetModPlayer<SFPlayer>().MagnetismAbility = true;
        return false;
    }
}