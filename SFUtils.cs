using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;

namespace ShatteredFate;

public static class SFUtils {
    static MethodInfo _getPickaxeDamageMethod;

    static SFUtils() {
        _getPickaxeDamageMethod = typeof(Player).GetMethod("GetPickaxeDamage", BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int), typeof(int), typeof(int), typeof(int), typeof(Tile)], null);
    }
    public static int GetPickaxeDamage(this Player player, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget) {
        try { return (int)_getPickaxeDamageMethod.Invoke(player, [x, y, pickPower, hitBufferIndex, tileTarget]); } catch (Exception) { return 0; };
    }
    public static bool PickPowerIsEnoughToHurtTile(int x, int y, int pickPower, Player player) {
        if (player.GetPickaxeDamage(x, y, pickPower, player.hitTile.HitObject(x, y, 1), Main.tile[x, y]) == 0) { return false; }
        else { return true; };
    }
    public static int SignNoZero(float num) => (Math.Sign(num) == 0) ? 1 : Math.Sign(num);

    // Tiles are 16 by 16 pixels, the length of a tile on 0, 90, 180, 270 degrees is 16f
    public const float TileLength = 16f;

    // When going diagonal (45 degrees) the length of the tile is: Sqrt((16*16)+(16*16)) = 22.62...
	public const float TileDiagLength = 22.62741699796952f - TileLength;

    public static bool CanHitLine(Vector2 startPosition, Vector2 endPosition, float maxDistance = float.MaxValue, bool considerSolidTopsAsSolid = false) {
        // Given start and end positions are invalid
        if (startPosition.HasNaNs() || endPosition.HasNaNs() || startPosition == endPosition) { return false; };

        // Get the direction and step values
        Vector2 direction = startPosition.DirectionTo(endPosition);
        float diag = 1f - Math.Abs(((Math.Abs(direction.ToRotation()) % MathHelper.PiOver2) - MathHelper.PiOver4) / MathHelper.PiOver4);
        Vector2 step = Vector2.Normalize(direction) * (TileLength + TileDiagLength * diag);
        float stepLength = step.Length();

        // Protection against invalid step size.
        if (step.HasNaNs() || stepLength <= 0) { return false; };

        // Calculate the distance between start and end point
        float totalDistance = Vector2.Distance(startPosition, endPosition);

        // Limit distance
        if (maxDistance > totalDistance) { maxDistance = totalDistance; };

        // Variable to count progress
        float distance = 0;

        // Get the current tile position
        Vector2 currentPosition = startPosition;
        int currentTileX = (int)(currentPosition.X / 16f);
        int currentTileY = (int)(currentPosition.Y / 16f);

        // Loop
        while (distance < maxDistance) {
            // Determine the distance remaining
            float distanceRemaining = maxDistance - distance;

            // Limit the step size to the remaining distance, if needed
            if (distanceRemaining < stepLength) { step = Vector2.Normalize(direction) * distanceRemaining; };

            // Calculate the next point
            Vector2 nextPosition = currentPosition + step;

            // Get the next tile position
            int nextTileX = (int)(nextPosition.X / 16f);
            int nextTileY = (int)(nextPosition.Y / 16f);

            // Calculate the amount of tiles advanced
            int checkTilesX = nextTileX - currentTileX;
            int checkTilesY = nextTileY - currentTileY;

            // Loop over the tiles check intersect with the line
            for (int x = 0; checkTilesX > 0 ? x < checkTilesX + 1 : x > checkTilesX - 1; x += checkTilesX > 0 ? 1 : -1) {
                for (int y = 0; checkTilesY > 0 ? y < checkTilesY + 1 : y > checkTilesY - 1; y += checkTilesY > 0 ? 1 : -1) {
                    int tileX = currentTileX + x;
                    int tileY = currentTileY + y;

                    // Check if the tile is in the world
                    if (tileX < 0 || tileX > Main.maxTilesX || tileY < 0 || tileY > Main.maxTilesY) { continue; }

                    // Get the tile
                    Tile tile = Main.tile[tileX, tileY];

                    // The tile is not solid or actuated
                    if (!tile.HasTile || !Main.tileSolid[tile.TileType] || (!considerSolidTopsAsSolid && Main.tileSolidTop[tile.TileType]) || tile.IsActuated) { continue; }

                    switch (tile.Slope) {
                        case Terraria.ID.SlopeType.Solid: {
                            // Is not a halfblock
                            if (!tile.IsHalfBlock) {
                                if (AABBvRectangleCollision(currentPosition.X, currentPosition.Y, nextPosition.X, nextPosition.Y, tileX * 16f, tileY * 16f, tileX * 16f + 16f, tileY * 16f + 16f)) { return false; };
                            }
                            // Is a halfblock
                            else {
                                if (AABBvRectangleCollision(currentPosition.X, currentPosition.Y, nextPosition.X, nextPosition.Y, tileX * 16f, tileY * 16f + 8f, tileX * 16f + 16f, tileY * 16f + 8f)) { return false; };
                            }
                        } 
                        break;

                        case Terraria.ID.SlopeType.SlopeDownLeft: {
                            if (AABBvTriangleCollision(currentPosition, nextPosition, new Vector2(tileX * 16f, tileY * 16f), new Vector2(tileX * 16f + 16f, tileY * 16f), new Vector2(tileX * 16f + 16f, tileY * 16f + 16f))) { return false; };
                        }
                        break;

                        case Terraria.ID.SlopeType.SlopeDownRight: {
                            if (AABBvTriangleCollision(currentPosition, nextPosition, new Vector2(tileX * 16f, tileY * 16f), new Vector2(tileX * 16f + 16f, tileY * 16f), new Vector2(tileX * 16f, tileY * 16f + 16f))) { return false; };
                        }
                        break;

                        case Terraria.ID.SlopeType.SlopeUpLeft: {
                            if (AABBvTriangleCollision(currentPosition, nextPosition, new Vector2(tileX * 16f + 16f, tileY * 16f), new Vector2(tileX * 16f + 16f, tileY * 16f + 16f), new Vector2(tileX * 16f, tileY * 16f + 16f))) { return false; };
                        }
                        break;

                        case Terraria.ID.SlopeType.SlopeUpRight: {
                            if (AABBvTriangleCollision(currentPosition, nextPosition, new Vector2(tileX * 16f, tileY * 16f), new Vector2(tileX * 16f, tileY * 16f + 16f), new Vector2(tileX * 16f + 16f, tileY * 16f + 16f))) { return false; };
                        }
                        break;
                    }
                }
            }

            // Set variables for next cycle
            currentPosition = nextPosition;
            currentTileX = nextTileX;
            currentTileY = nextTileY;

            // Increase distance checked
            distance += stepLength;
        }

        return true;
    }
    public static bool AABBvRectangleCollision(float x1, float y1, float x2, float y2, float minX, float minY, float maxX, float maxY) {
        if ((x1 <= minX && x2 <= minX) || (y1 <= minY && y2 <= minY) || (x1 >= maxX && x2 >= maxX) || (y1 >= maxY && y2 >= maxY)) { return false; }

        float m = (y2 - y1) / (x2 - x1);

        float y = m * (minX - x1) + y1;
        if (y > minY && y < maxY) { return true; };

        y = m * (maxX - x1) + y1;
        if (y > minY && y < maxY) { return true; };

        float x = (minY - y1) / m + x1;
        if (x > minX && x < maxX) { return true; };

        x = (maxY - y1) / m + x1;
        if (x > minX && x < maxX) { return true; };

        return false;
    }
    public static bool AABBvTriangleCollision(Vector2 p0, Vector2 p1, Vector2 t0, Vector2 t1, Vector2 t2) {
        float f1 = Side(p0, t2, t0, t1), f2 = Side(p1, t2, t0, t1), f3 = Side(p0, t0, t1, t2), f4 = Side(p1, t0, t1, t2), f5 = Side(p0, t1, t2, t0), f6 = Side(p1, t1, t2, t0), f7 = Side(t0, t1, p0, p1), f8 = Side(t1, t2, p0, p1);

        if ((f1 < 0 && f2 < 0) || (f3 < 0 && f4 < 0) || (f5 < 0 && f6 < 0) || (f7 > 0 && f8 > 0)) { return false; };
        if ((f1 == 0 && f2 == 0) || (f3 == 0 && f4 == 0) || (f5 == 0 && f6 == 0)) { return false; };
        if ((f1 <= 0 && f2 <= 0) || (f3 <= 0 && f4 <= 0) || (f5 <= 0 && f6 <= 0) || (f7 >= 0 && f8 >= 0)) { return true; };
        if (f1 > 0 && f2 > 0 && f3 > 0 && f4 > 0 && f5 > 0 && f6 > 0) { return false; };

        return true;

        static float Side(Vector2 p, Vector2 q, Vector2 a, Vector2 b) {
            float z1 = (b.X - a.X) * (p.Y - a.Y) - (p.X - a.X) * (b.Y - a.Y);
            float z2 = (b.X - a.X) * (q.Y - a.Y) - (q.X - a.X) * (b.Y - a.Y);
            return z1 * z2;
        }
    }
    public static bool CheckNeedItem(Player player, int itemType, int need) {
        if (NeedItem(player.inventory, itemType, need)) { return true; }
        else if (NeedItem(player.bank.item, itemType, need)) { return true; }
        else if (NeedItem(player.bank2.item, itemType, need)) { return true; }
        else if (NeedItem(player.bank3.item, itemType, need)) { return true; }
        else if (NeedItem(player.bank4.item, itemType, need)) { return true; }
        else { return false; };
    }
    public static bool CheckNeedItem(Player player, int itemType, int need, out int container, out int stack) {
        if (NeedItem(player.inventory, itemType, need, out stack)) { container = 0; return true; }
        else if (NeedItem(player.bank.item, itemType, need, out stack)) { container = 1; return true; }
        else if (NeedItem(player.bank2.item, itemType, need, out stack)) { container = 2; return true; }
        else if (NeedItem(player.bank3.item, itemType, need, out stack)) { container = 3; return true; }
        else if (NeedItem(player.bank4.item, itemType, need, out stack)) { container = 4; return true; }
        else { container = -1; stack = 0; return false; };
    }
    public static bool NeedItem(Item[] inv, int target, int stack) {
        bool has = false;
        for (int i = 0; i < inv.Length; i++) {
            if (inv[i] != null) {
                if (inv[i].type == target && inv[i].stack > 0) {
                    if (inv[i].stack - stack <= 0) {
                        inv[i].stack = 0;
                        has = true;
                        break;
                    };
                    inv[i].stack -= stack;
                    has = true;
                    break;
                };
            };
        };
        return has;
    }
    public static bool NeedItem(Item[] inv, int target, int stack, out int MaxStack) {
        bool has = false;
        MaxStack = 0;
        for (int i = 0; i < inv.Length; i++) {
            if (inv[i] != null) {
                if (inv[i].type == target && inv[i].stack > 0) {
                    if (inv[i].stack - stack <= 0) {
                        inv[i].stack = 1;
                        has = true;
                        break;
                    };
                    inv[i].stack -= stack;
                    MaxStack = inv[i].stack;
                    has = true;
                    break;
                };
            };
        };
        return has;
    }
    public static bool RightClickRepeat(ref int value1, ref int value2) {
        if (!Main.mouseLeft) {
            value1 = 0;
            value2 = 0;
            return false;
        };
        if (Main.mouseLeftRelease) {
            value1 = 0;
            return true;
        };

        value1++;

        if (value1 >= 15) {
            value1 = 10;
            return true;
        };

        return false;
    }
};