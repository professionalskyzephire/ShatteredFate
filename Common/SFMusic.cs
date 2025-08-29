
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate;

public class HardmodeBeginningMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/HardmodeBeginning");
    public override bool IsSceneEffectActive(Player player) => player.GetModPlayer<SFPlayer>().HardmodeMusicTimer > 0;
}

public class BloodMoonMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/BloodMoon");
    public override bool IsSceneEffectActive(Player player) => Main.bloodMoon && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class BrainMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/BrainOfCthulhu");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.BrainofCthulhu) && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class EOWMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/EaterOfWorlds");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.EaterofWorldsHead) && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class EyeMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/EyeOfCthulhu");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.EyeofCthulhu) && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class MushroomMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/GlowingMushroomBiome");
    public override bool IsSceneEffectActive(Player player) => player.ZoneGlowshroom && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class KingSlimeMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/KingSlime");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.KingSlime) && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class QueenBeeMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/QueenBee");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.QueenBee) && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class QueenSlimeMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/QueenSlime");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.QueenSlimeBoss) && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class SkeletronMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/Skeletron");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.SkeletronHead) && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class SkeletronPrimeMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/SkeletronPrime");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.SkeletronPrime) && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class SnowLegionMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/SnowLegion");
    public override bool IsSceneEffectActive(Player player) => Main.invasionType == InvasionID.SnowLegion && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class CorruptionMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/TheCorruption");
    public override bool IsSceneEffectActive(Player player) => player.ZoneCorrupt && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class CrimsonMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/TheCrimson");
    public override bool IsSceneEffectActive(Player player) => player.ZoneCrimson && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class DestroyerMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/TheDestroyer");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.TheDestroyer) && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class HallowMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/TheHallow");
    public override bool IsSceneEffectActive(Player player) => player.ZoneHallow && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class TwinsMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/TheTwins");
    public override bool IsSceneEffectActive(Player player) => (NPC.AnyNPCs(NPCID.Retinazer) || NPC.AnyNPCs(NPCID.Spazmatism)) && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class TorchGodMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/TorchGod");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.TorchGod) && ShatteredFate.clientConfig.MusicReplacementsActive;
}

public class WOFMusic : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/VanillaReplacements/WallOfFlesh");
    public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(NPCID.WallofFlesh) && ShatteredFate.clientConfig.MusicReplacementsActive;
}