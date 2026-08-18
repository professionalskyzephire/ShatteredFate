using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ShatteredFate;

public class SFReworksConfig : ModConfig {
	public override ConfigScope Mode => ConfigScope.ServerSide;

    [ReloadRequired]
    [DefaultValue(true)]
    public bool GemStaves;

    [ReloadRequired]
	[DefaultValue(true)]
	public bool FallenStarReplacement;
}