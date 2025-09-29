using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Rarities
{
	public class SFDevItem : ModRarity
	{
		public override Color RarityColor => Color.Lerp(new Color(46, 189, 84), Color.White, (float)System.Math.Sin(Terraria.Main.GlobalTimeWrappedHourly * MathHelper.Pi));
		public override int GetPrefixedRarity(int offset, float valueMult) => Type;
	}
}