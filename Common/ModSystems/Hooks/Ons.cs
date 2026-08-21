using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShatteredFate.Common.Players;
using ShatteredFate.Content.Items.Accessories;
using ShatteredFate.ModUtils;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ShatteredFate.Common.ModSystems.Hooks;

internal class Ons {
    public static void Load(Mod mod) {
        On_Item.AffixName += NewItemName; // change name of fallen stars
        On_Item.DespawnIfMeetingConditions += TransformationStar; // prevent fallen stars from despawning
        On_WorldGen.UpdateWorld += NewStars; // used for overriding vanilla fallen star spawning
        On_Projectile.AI_148_StarSpawner += NewStarAI; // New AI for star
        On_Player.AddBuff += FixRageBuff; // Remove "vanilla" rage buff if active "new" rage buff
        On_Main.DrawBuffIcon += TicSound; // Tic song if hover buffs
        On_Main.MouseText_DrawBuffTooltip += DrawBgForBuffs; // Add BG for new buffs
    }

    static string NewItemName(On_Item.orig_AffixName orig, Item self) {
        if (self.type == ItemID.FallenStar && ModContent.GetInstance<SFReworksConfig>().FallenStarReplacement) return Language.GetTextValue("Mods.ShatteredFate.Items.CosmicDust.DisplayName");
        if (self.type == ItemID.AmberStaff && ModContent.GetInstance<SFReworksConfig>().GemStaves) return orig(self).Replace(Language.GetTextValue("ItemName.Amber"), Language.GetTextValue("Mods.ShatteredFate.Items.FusedGemstone.DisplayName"));
        return orig(self);
    }
    static void TransformationStar(On_Item.orig_DespawnIfMeetingConditions orig, Item self, int i) {
        if (ModContent.GetInstance<SFReworksConfig>().FallenStarReplacement) {
            if (self.type == ItemID.FallenStar) {
                int oldStack = self.stack;
                self.ChangeItemType(ModContent.ItemType<Content.Items.Materials.CosmicDust>());
                self.stack = oldStack;
            } else { orig(self, i); }
        } else { orig(self, i); }
    }
    static void NewStars(On_WorldGen.orig_UpdateWorld orig) {
        if (ModContent.GetInstance<SFReworksConfig>().FallenStarReplacement) {
            float starfallBoost = Star.starfallBoost;
            Star.starfallBoost = 0f;
            orig();
            //directly ripped out of vanilla's code
            for (int k = 0; k < Main.dayRate; k++) {
                double num10 = Main.maxTilesX / 4200.0;
                num10 *= (double)starfallBoost / 2.0;
                if (Main.rand.Next(8000) < 10.0 * num10) {
                    int num12 = Main.rand.Next(Main.maxTilesX - 50) + 100;
                    num12 *= 16;
                    int num13 = Main.rand.Next((int)(Main.maxTilesY * 0.05));
                    num13 *= 16;
                    Vector2 vector = new(num12, num13);
                    int num14 = -1;
                    if (Main.expertMode && Main.rand.NextBool(15)) {
                        int num15 = Player.FindClosest(vector, 1, 1);
                        if (Main.player[num15].position.Y < Main.worldSurface * 16.0 && Main.player[num15].afkCounter < 3600) {
                            int num16 = Main.rand.Next(1, 640);
                            vector.X = Main.player[num15].position.X + Main.rand.Next(-num16, num16 + 1);
                            num14 = num15;
                        };
                    };
                    if (!Collision.SolidCollision(vector, 16, 16)) {
                        float num17 = Main.rand.Next(-100, 101);
                        float num18 = Main.rand.Next(200) + 100;
                        float num19 = (float)Math.Sqrt((double)(num17 * num17 + num18 * num18));
                        num19 = 12 / num19;
                        num17 *= num19;
                        num18 *= num19;
                        Projectile.NewProjectile(new EntitySource_Misc("FallingStar"), vector.X, vector.Y, num17, num18, ProjectileID.FallingStarSpawner, 0, 0f, Main.myPlayer, 0f, num14, 0f);
                    };
                };
            };
            Star.starfallBoost = starfallBoost;
        } else { orig(); }
    }
    static void NewStarAI(On_Projectile.orig_AI_148_StarSpawner orig, Projectile self) {
        if (ModContent.GetInstance<SFReworksConfig>().FallenStarReplacement) {
            self.ai[0] += (float)Main.dayRate;
            if (self.localAI[0] == 0f && Main.netMode != NetmodeID.Server) {
                self.localAI[0] = 1f;
                if (Main.LocalPlayer.position.Y < Main.worldSurface * 16.0) { Star.StarFall(self.position.X); };
            };
            if (self.owner == Main.myPlayer && self.ai[0] >= 180f) {
                if (self.ai[1] > -1f) {
                    self.velocity.X = self.velocity.X * 0.35f;
                    if (self.Center.X < Main.player[(int)self.ai[1]].Center.X) self.velocity.X = Math.Abs(self.velocity.X);
                    else self.velocity.X = -Math.Abs(self.velocity.X);
                };
                Projectile.NewProjectile(self.GetSource_FromThis(), self.position.X, self.position.Y, self.velocity.X, self.velocity.Y, ModContent.ProjectileType<Content.Projectiles.Misc.FallingCosmicDust>(), 1000, 10f, Main.myPlayer, 0f, 0f, 0f);
                self.Kill();
            };
        } else { orig(self); }
    }
    static void FixRageBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack) {
        if (type == 115) {
            RagePlayer rPlayer = Main.LocalPlayer.GetModPlayer<RagePlayer>();
            if (PlayersExpansions.CheckAcc(rPlayer.Player, ModContent.ItemType<AmuletofRage>())) {
                if (rPlayer.GetRageStatus() || rPlayer.GetCDTime() > 0) { return; };
                if (rPlayer.GetVanillaRageBuffTime() == 0) { rPlayer.SetRage(rPlayer.GetRage() + 20); };
                if (rPlayer.GetVanillaRageBuffTime() >= 0) { rPlayer.SetVanillaRageBuffTime(timeToAdd); };
            } else { orig(self, type, timeToAdd, quiet, foodHack); };
        } else { orig(self, type, timeToAdd, quiet, foodHack); };
    }
    static int TicSound(On_Main.orig_DrawBuffIcon orig, int drawBuffText, int buffSlotOnPlayer, int x, int y) {
        int num = Main.LocalPlayer.buffType[buffSlotOnPlayer];
        if (new Rectangle(x, y, TextureAssets.Buff[num].Width(), TextureAssets.Buff[num].Height()).Contains(new Point(Main.mouseX, Main.mouseY))) {
            if (Main.LocalPlayer.GetModPlayer<HoverBuffsPlayer>().BuffType.Equals(num)) { Main.LocalPlayer.GetModPlayer<HoverBuffsPlayer>().Hover = true; };
        }
        else {
            if (Main.LocalPlayer.GetModPlayer<HoverBuffsPlayer>().BuffType.Equals(num)) {
                Main.LocalPlayer.GetModPlayer<HoverBuffsPlayer>().Hover = false;
                Main.LocalPlayer.GetModPlayer<HoverBuffsPlayer>()._tic = false;
            };
        };
        return orig(drawBuffText, buffSlotOnPlayer, x, y);
    }
    static void DrawBgForBuffs(On_Main.orig_MouseText_DrawBuffTooltip orig, Main self, string buffString, ref int X, ref int Y, int buffNameHeight) {
        X += 8;
        Y += 8;

        HoverBuffsPlayer hBplayer = Main.LocalPlayer.GetModPlayer<HoverBuffsPlayer>();
        int npcCount = 0;

        if (!hBplayer._tic && hBplayer.Hover) { 
            SoundEngine.PlaySound(SoundID.MenuTick);
            hBplayer._tic = true;
        };
        if (Main.bannerMouseOver) {
            for (int i = 0; i < NPCLoader.NPCCount; i++) {
                if (Item.BannerToNPC(i) != 0 && Main.LocalPlayer.HasNPCBannerBuff(i)) { npcCount++; };
            };
        };

        int[] frame;
        int scaleX = 0;
        int maxScaleX = 0;
        int scaleY = (int)FontAssets.MouseText.Value.MeasureString("J").Y;
        int maxScaleY = (scaleY * hBplayer.GetAllHoverBuffText().Length) + 23 * npcCount - (npcCount > 3 ? (int)(2 * npcCount + 0.4 * npcCount) : 0);

        SpriteBatch sB = Main.spriteBatch;
        Texture2D[] asset = Resources.Textures.GetWindowBG();
        Color[] windowColor;

        for (int i = 0; i < hBplayer.GetAllHoverBuffText().Length; i++) {
            scaleX = (int)FontAssets.MouseText.Value.MeasureString(hBplayer.GetAllHoverBuffText()[i]).X;
            maxScaleX = scaleX > maxScaleX ? scaleX : maxScaleX;
        };
        if (!Tables.Buffs.SF.ToList().Contains(hBplayer.BuffType)) {
            if (Main.debuff[hBplayer.BuffType] && hBplayer.BuffType != 146 && hBplayer.BuffType != 147) { windowColor = [new(79, 14, 14), new(84, 14, 14, 170), new(92, 15, 15), new(114, 16, 16), new(114, 16, 16), new(132, 20, 20), new(84, 14, 14)]; frame = [4, 5]; }
            else { windowColor = [new(14, 33, 70), new(17, 41, 88, 170), new(21, 48, 101), new(26, 54, 110), new(28, 59, 119), new(33, 69, 141), new(17, 41, 88)]; frame = [2, 3];};
        } else { windowColor = [new(0, 0, 0), new(44, 44, 44, 170), new(65, 65, 65), new(89, 89, 89), new(111, 111, 111), new(159, 159, 159), new(44, 44, 44)]; frame = [0, 1]; };

        Rectangle window = new(X - 10, Y - 6, maxScaleX + 20, maxScaleY + 6);
        if (window.Right > Main.screenWidth) { X -= window.Right - Main.screenWidth + 16; };
        if (window.Left < 0) { X -= window.Left; };
        if (window.Bottom > Main.screenHeight) { Y -= window.Bottom - Main.screenHeight; };
        if (window.Top < 0) { Y -= window.Top; };

        Vector2 pos = new(X - 6, Y);

        sB.Draw(asset[1], new Rectangle((int)pos.X, (int)pos.Y - 2, maxScaleX + 12, maxScaleY - 2), null, windowColor[1], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X + 4, (int)pos.Y - 6, maxScaleX + 6, 2), null, windowColor[3], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X + 4, (int)pos.Y - 4, maxScaleX + 4, 2), null, windowColor[5], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X - 4, (int)pos.Y + 2, 2, (maxScaleY / 2)), null, windowColor[0], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X - 4, (int)pos.Y + (maxScaleY / 2), 2, (maxScaleY / 2) - 6), null, windowColor[0], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X - 2, (int)pos.Y + 2, 2, (maxScaleY / 2)), null, windowColor[4], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X - 2, (int)pos.Y + (maxScaleY / 2), 2, (maxScaleY / 2) - 6), null, windowColor[3], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X + maxScaleX + 14, (int)pos.Y + 2, 2, (maxScaleY / 2)), null, windowColor[6], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X + maxScaleX + 14, (int)pos.Y + (maxScaleY / 2), 2, (maxScaleY / 2) - 6), null, windowColor[0], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X + maxScaleX + 12, (int)pos.Y + 2, 2, (maxScaleY / 2)), null, windowColor[4], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X + maxScaleX + 12, (int)pos.Y + (maxScaleY / 2), 2, (maxScaleY / 2) - 6), null, windowColor[3], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X + 4, (int)pos.Y + maxScaleY - 2, maxScaleX + 6, 2), null, windowColor[0], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[1], new Rectangle((int)pos.X + 4, (int)pos.Y + maxScaleY - 4, maxScaleX + 6, 2), null, windowColor[2], 0f, Vector2.Zero, SpriteEffects.None, 1f);
        sB.Draw(asset[0], new(pos.X, pos.Y - 2), asset[0].Frame(1, 6, 0, frame[0]), Color.White, 0f, asset[0].Frame(1, 6, 0, frame[0]).Size() / 2f, 1f, SpriteEffects.None, 0);
        sB.Draw(asset[0], new(pos.X, pos.Y + maxScaleY - 4), asset[0].Frame(1, 6, 0, frame[1]), Color.White, 0f, asset[0].Frame(1, 6, 0, frame[1]).Size() / 2f, 1f, SpriteEffects.None, 1);
        sB.Draw(asset[0], new(pos.X + maxScaleX + 12, pos.Y - 2), asset[0].Frame(1, 6, 0, frame[0]), Color.White, 0f, asset[0].Frame(1, 6, 0, frame[0]).Size() / 2f, 1f, SpriteEffects.FlipHorizontally, 0);
        sB.Draw(asset[0], new(pos.X + maxScaleX + 12, pos.Y + maxScaleY - 4), asset[0].Frame(1, 6, 0, frame[1]), Color.White, 0f, asset[0].Frame(1, 6, 0, frame[1]).Size() / 2f, 1f, SpriteEffects.FlipHorizontally, 1);

        orig(self, buffString, ref X, ref Y, buffNameHeight);
    }

    public static void Unload() {
        On_Item.AffixName -= NewItemName;
        On_Item.DespawnIfMeetingConditions -= TransformationStar;
        On_WorldGen.UpdateWorld -= NewStars;
        On_Projectile.AI_148_StarSpawner -= NewStarAI;
        On_Player.AddBuff -= FixRageBuff;
        On_Main.DrawBuffIcon -= TicSound;
        On_Main.MouseText_DrawBuffTooltip -= DrawBgForBuffs;
    }
};