using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ShatteredFate.Common;

public class SFClientConfig : ModConfig
{
    [DefaultValue(true)]
    public bool MusicReplacementsActive { get; set; }
    public override ConfigScope Mode => ConfigScope.ClientSide;
}