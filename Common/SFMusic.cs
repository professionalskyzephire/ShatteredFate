using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Common;

public abstract class SceneMusicLoaden : ModSceneEffect
{
    public override bool IsLoadingEnabled(Mod mod) => SFMod.MusicMod != null;
}

public class HardmodeBeginningMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => SFMod.MusicMod != null ? -1 : MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/HardmodeBeginning");
    public override bool IsSceneEffectActive(Player player) => player.GetModPlayer<SFPlayer>().HardmodeMusicTimer > 0;
}

public class BloodMoonMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/BloodMoon");
    public override bool IsSceneEffectActive(Player player) => Main.bloodMoon && SFMod.ClientConfig.MusicReplacementsActive;
}

public class BrainMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/BrainOfCthulhu");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.BrainofCthulhu) && SFMod.ClientConfig.MusicReplacementsActive;
}

public class EOWMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/EaterOfWorlds");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.EaterofWorldsHead) && SFMod.ClientConfig.MusicReplacementsActive;
}

public class EyeMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/EyeOfCthulhu");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.EyeofCthulhu) && SFMod.ClientConfig.MusicReplacementsActive;
}

public class MushroomMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/GlowingMushroomBiome");
    public override bool IsSceneEffectActive(Player player) => player.ZoneGlowshroom && SFMod.ClientConfig.MusicReplacementsActive;
}

public class KingSlimeMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/KingSlime");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.KingSlime) && SFMod.ClientConfig.MusicReplacementsActive;
}

public class QueenBeeMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/QueenBee");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.QueenBee) && SFMod.ClientConfig.MusicReplacementsActive;
}

public class QueenSlimeMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/QueenSlime");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.QueenSlimeBoss) && SFMod.ClientConfig.MusicReplacementsActive;
}

public class SkeletronMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/Skeletron");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.SkeletronHead) && SFMod.ClientConfig.MusicReplacementsActive;
}

public class SkeletronPrimeMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/SkeletronPrime");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.SkeletronPrime) && SFMod.ClientConfig.MusicReplacementsActive;
}

public class SnowLegionMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/SnowLegion");
    public override bool IsSceneEffectActive(Player player) => Main.invasionType == InvasionID.SnowLegion && SFMod.ClientConfig.MusicReplacementsActive;
}

public class CorruptionMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/TheCorruption");
    public override bool IsSceneEffectActive(Player player) => player.ZoneCorrupt && SFMod.ClientConfig.MusicReplacementsActive;
}

public class CrimsonMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/TheCrimson");
    public override bool IsSceneEffectActive(Player player) => player.ZoneCrimson && SFMod.ClientConfig.MusicReplacementsActive;
}

public class DestroyerMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/TheDestroyer");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.TheDestroyer) && SFMod.ClientConfig.MusicReplacementsActive;
}

public class HallowMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/TheHallow");
    public override bool IsSceneEffectActive(Player player) => player.ZoneHallow && SFMod.ClientConfig.MusicReplacementsActive;
}

public class TwinsMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/TheTwins");
    public override bool IsSceneEffectActive(Player player) => (NPC.AnyNPCs(NPCID.Retinazer) || NPC.AnyNPCs(NPCID.Spazmatism)) && SFMod.ClientConfig.MusicReplacementsActive;
}

public class TorchGodMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/TorchGod");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.TorchGod) && SFMod.ClientConfig.MusicReplacementsActive;
}

public class WOFMusic : SceneMusicLoaden
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(SFMod.MusicMod, "Music/Vanilla/WallOfFlesh");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.WallofFlesh) && SFMod.ClientConfig.MusicReplacementsActive;
}