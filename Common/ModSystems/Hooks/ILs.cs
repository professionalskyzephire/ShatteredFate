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
        c.Index -= 3;
        c.RemoveRange(5);
        c.Emit(OpCodes.Ldarg, 1);
        c.Emit(OpCodes.Box, typeof(Main).GetMethod("MouseTextInner", BindingFlags.NonPublic | BindingFlags.Instance).GetParameters()[0].ParameterType);
        c.EmitDelegate<Action<object>>((info) => {
            string name = (string)info.GetType().GetField("cursorText", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(info);
            string text = (string)info.GetType().GetField("buffTooltip", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(info);
            if (text != null) { 
                Player player = Main.LocalPlayer;
                HoverBuffsPlayer hBPlayer = player.GetModPlayer<HoverBuffsPlayer>();
                hBPlayer.BuffName = name;
                if (text != "") { hBPlayer.SetBuffTooltips(text); };
                for (int i = 0; i < player.buffType.Length; i++) {
                    if (hBPlayer.BuffName.Equals(Lang.GetBuffName(player.buffType[i]))) {
                        hBPlayer.BuffType = player.buffType[i];
                        break;
                    };
                };
            }
        });
    }


    public static void Unload() {
        IL_Main.MouseTextInner -= AddValueForHoverBuffsPlayer;
    }
};