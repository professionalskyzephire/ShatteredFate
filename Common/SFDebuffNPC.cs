using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;

namespace ShatteredFate.Common
{
	internal class SFDebuffNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		public Dictionary<int, int> stackingDoTs = new();
		public int damage = 0;
		public int rate = 0;
		public override void ResetEffects(NPC npc) {
			damage = 0;
			rate = 0;
			foreach(int i in stackingDoTs.Keys) if(!npc.HasBuff(i)) stackingDoTs.Remove(i);
		}
		public override void UpdateLifeRegen(NPC npc, ref int dot) {
			dot += damage;
			npc.lifeRegen -= rate;
		}
	}
}
