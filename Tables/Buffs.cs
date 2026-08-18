using ShatteredFate.Content.Buffs;
using ShatteredFate.Content.Buffs.Debuffs;
using static Terraria.ModLoader.ModContent;

namespace ShatteredFate.Tables;

public class Buffs {
    public static int[] SF { get; private set; } = new int[14];

    public static void Load() {
        SF = [BuffType<AntidoteBuff>(), BuffType<DarkerThanCoal>(), BuffType<HypermagnetismBuff>(), BuffType<MagnetismBuff>(), BuffType<Rage>(), BuffType<AmethystStaffCooldown>(), BuffType<AncientSkullCooldown>(), BuffType<AntiqueDaggerDoT>(), BuffType<Cooldown_OldFrypan>(), BuffType<DiamondStaffCooldown>(), BuffType<EmeraldStaffCooldown>(), BuffType<RubyStaffCooldown>(), BuffType<SanguineLeechDebuff>(), BuffType<SapphireStaffCooldown>()];
    }
    public static void UnLoad() {
        SF = [];
    }
};