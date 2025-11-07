using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace ShatteredFate.Common
{
	internal class SFDebuffNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public Dictionary<int, int> stackingDoTs = new();
		public override void ResetEffects(NPC npc) {
			foreach(int i in stackingDoTs.Keys) if(!npc.HasBuff(i)) stackingDoTs.Remove(i);
		}
		public override void UpdateLifeRegen(NPC npc, ref int damage) {
			foreach(int i in stackingDoTs.Values) npc.lifeRegen -= i;
		}
	}
}