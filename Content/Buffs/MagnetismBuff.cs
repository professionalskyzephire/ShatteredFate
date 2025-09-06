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
        if (player.GetModPlayer<SFPlayer>().MagnetismStacks < 14) return false;
        Main.NewText("Hypermagnetism ability unlocked! Press the assigned key to activate Hypermagnetism for 15 seconds. (1 minute cooldown)", 50, 255, 130);
        player.GetModPlayer<SFPlayer>().MagnetismAbility = true;
        return false;
    }
}