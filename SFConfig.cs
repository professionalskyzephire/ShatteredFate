using System.ComponentModel;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace ShatteredFate
{
	public class SFReworksConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;
		[ReloadRequired]
		[DefaultValue(true)]
		public bool GemStaves;
	}
}