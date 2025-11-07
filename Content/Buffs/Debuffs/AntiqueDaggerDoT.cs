using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace ShatteredFate.Content.Buffs.Debuffs
{
	public class AntiqueDaggerDoT : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
		public override void Update(NPC npc, ref int buffIndex) {
			var dot = npc.GetGlobalNPC<Common.SFDebuffNPC>();
			if(dot.stackingDoTs.ContainsKey(Type)) {
				dot.damage += dot.stackingDoTs[Type] * 3 + 1;
				dot.rate += dot.stackingDoTs[Type] * 5;
			}
		}
		public override bool ReApply(NPC npc, int time, int buffIndex) {
			var dot = npc.GetGlobalNPC<Common.SFDebuffNPC>();
			if(!dot.stackingDoTs.TryAdd(Type, 1) && dot.stackingDoTs[Type] < 10) dot.stackingDoTs[Type]++;
			dot.damage += dot.stackingDoTs[Type] * 3 - 1;
			return false;
		}
	}
}