using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace ShatteredFate.Content.Buffs
{
	public class DarkerThanCoal : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex) {
			Main.dust[Dust.NewDust(player.position, player.width, player.height, 4, 0f, 0f, 0, new Color(21, 21, 31), 1f)].noGravity = true;
		}
	}
}