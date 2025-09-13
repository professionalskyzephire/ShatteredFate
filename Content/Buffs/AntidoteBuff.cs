using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Buffs;

public class AntidoteBuff : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        player.buffImmune[BuffID.Poisoned] = true;
        if (Main.hardMode) player.buffImmune[BuffID.Venom] = true;
    }
}