using Mono.Cecil.Cil;
using MonoMod.Cil;
using ShatteredFate.Common.Players;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace ShatteredFate.Common.ModSystems.Hooks;

internal class ILs {
    public static void Load(Mod mod) {
        IL_Main.MouseTextInner += AddValueForHoverBuffsPlayer; // Adding data for Hover Buffs Player
    }

    static void AddValueForHoverBuffsPlayer(ILContext il) {
        ILCursor c = new(il);

        c.TryGotoNext(MoveType.After, i => i.MatchLdstr(""));
        c.Index += 2;
        c.Emit(OpCodes.Ldarg, 1);
        c.Emit(OpCodes.Box, typeof(Main).GetMethod("MouseTextInner", BindingFlags.NonPublic | BindingFlags.Instance).GetParameters()[0].ParameterType);
        c.EmitDelegate<Action<object>>((info) => {
            Player player = Main.LocalPlayer;
            HoverBuffsPlayer hBPlayer = player.GetModPlayer<HoverBuffsPlayer>();
            if (hBPlayer.GetHoverBuffName() != "") { return; };
            hBPlayer.SetHoverBuffName((string)info.GetType().GetField("cursorText", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(info));
            hBPlayer.SetHoverBuffTooltips((string)info.GetType().GetField("buffTooltip", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(info));
            for (int i = 0; i < player.buffType.Length; i++) {
                if (hBPlayer.GetHoverBuffName() == Lang.GetBuffName(player.buffType[i])) {
                    hBPlayer.SetHoverBuff(player.buffType[i]);
                    break;
                };
            };
        });
    }


    public static void Unload() {
        IL_Main.MouseTextInner -= AddValueForHoverBuffsPlayer;
    }
};