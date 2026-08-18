using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ShatteredFate.Common.Players;

namespace ShatteredFate.Common
{
	public class SFGlobalItem : GlobalItem
	{
		public override bool InstancePerEntity => true;

		/// <summary>
		/// Add to this to give extra crit damage to the weapon. <br/>
		/// For example, +0.5f will give 50% extra crit damage (250% original damage)
		/// </summary>
		public float bonusCritDamage = 0f;

		public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (bonusCritDamage != 0)
			{
				modifiers.CritDamage += bonusCritDamage;
			}
		}

		public override void GrabRange(Item item, Player player, ref int grabRange)
		{
			grabRange += player.GetModPlayer<MagnetismPlayer>().GetGrabRange();
		}
	}
}