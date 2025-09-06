using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Buffs;

public class HypermagnetismBuff : ModBuff
{
    public override string Texture => "ShatteredFate/Content/Buffs/MagnetismBuff";
    public override void Update(Player player, ref int buffIndex)
    {
        for (int i = 0; i < Main.maxItems; i++)
        {
            Item item = Main.item[i];
            if (item.active && item.noGrabDelay == 0 && ItemLoader.CanPickup(item, player))
            {
                item.beingGrabbed = true;
                Vector2 val = player.Center - item.Center;
                Vector2 val2 = item.velocity * 4f;
                Vector2 val3 = val;
                item.velocity = (val2 + val * (20f / val3.Length())) * 0.2f;
            }
        }
    }
}