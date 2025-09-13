using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using System.Reflection;

namespace ShatteredFate
{
    public static class SFUtils
    {
        private static MethodInfo _getPickaxeDamageMethod;
        static SFUtils() {
            _getPickaxeDamageMethod = typeof(Player).GetMethod(
                "GetPickaxeDamage", BindingFlags.NonPublic | BindingFlags.Instance, null,
                [typeof(int), typeof(int), typeof(int), typeof(int), typeof(Tile)],
                null
            );
        }
        public static int GetPickaxeDamage(this Player player, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget) {
            try {
                object result = _getPickaxeDamageMethod.Invoke(player, [x, y, pickPower, hitBufferIndex, tileTarget]);
                return (int)result;
            } catch (Exception ex) {
                return 0;
            }
        }
        public static bool PickPowerIsEnoughToHurtTile(int x, int y, int pickPower, Player player) {
            Tile tile = Main.tile[x, y];
            _ = tile.TileType;
            int hitBufferIndex = player.hitTile.HitObject(x, y, 1);
            if (player.GetPickaxeDamage(x, y, pickPower, hitBufferIndex, tile) == 0)
                return false;

            return true;
        }
        public static int SignNoZero(float num) {
            int sign = Math.Sign(num);
            return (sign == 0) ? 1 : sign;
        }
    }
}
