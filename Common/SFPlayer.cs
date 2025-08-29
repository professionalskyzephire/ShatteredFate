using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

public class SFPlayer : ModPlayer
{
    public int HardmodeMusicTimer;
    private bool wasInHardmode;
    public override void PostUpdate()
    {
        if (HardmodeMusicTimer > 0)
        {
            HardmodeMusicTimer--;
        }
        if (!wasInHardmode && Main.hardMode)
        {
            HardmodeMusicTimer = 60 * 198;
        }
        wasInHardmode = Main.hardMode;
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add("wasInHardmode", wasInHardmode);
    }

    public override void LoadData(TagCompound tag)
    {
        wasInHardmode = tag.GetBool("wasInHardmode");
    }
}