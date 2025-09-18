using Microsoft.Xna.Framework;
using ShatteredFate.Common;
using System;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate
{
    public class SFMod : Mod
    {
        public const string VanillaTexture = "Terraria/Images/";
        public const string ExtrasPath = "ShatteredFate/Extras/";
        public const string BlankTexture = "ShatteredFate/Extras/Invisible";
        public const string MagicPixel = "ShatteredFate/Extras/MagicPixel";
        internal static SFClientConfig ClientConfig;
        private static Mod _musicMod;

        public static ModKeybind AccessoryAbilityKey;
        public static ModKeybind MagnetismKey;
        internal static Mod MusicMod {
            get {
                if (_musicMod == null && !ModLoader.TryGetMod("ShatteredFateMusic", out _musicMod)) {
                    _musicMod = null;
                }
                return _musicMod;
            }
        }

        public override void Load() {
            ClientConfig = ModContent.GetInstance<SFClientConfig>();
            AccessoryAbilityKey = KeybindLoader.RegisterKeybind(this, "Accessory Ability Key", Keys.X);
            MagnetismKey = KeybindLoader.RegisterKeybind(this, "Hypermagnetism Key", Keys.Q);
            //change name of fallen stars
			Terraria.On_Item.AffixName += (orig, self) => {
				//change name of fallen stars
				if(self.type == 75 && ModContent.GetInstance<SFReworksConfig>().FallenStarReplacement) return Language.GetTextValue("Mods.ShatteredFate.Items.CosmicDust.DisplayName");
				//change name of amber staff
				if(self.type == 3377 && ModContent.GetInstance<SFReworksConfig>().GemStaves) return orig(self).Replace(Language.GetTextValue("ItemName.Amber"), Language.GetTextValue("Mods.ShatteredFate.Items.FusedGemstone.DisplayName"));
				return orig(self);
			};
            //prevent fallen stars from despawning
            if(ModContent.GetInstance<SFReworksConfig>().FallenStarReplacement) Terraria.On_Item.DespawnIfMeetingConditions += (orig, self, i) => {
                if (self.type == ItemID.FallenStar) {
                    int oldStack = self.stack;
                    self.ChangeItemType(ModContent.ItemType<Content.Items.Materials.CosmicDust>());
                    self.stack = oldStack;
                }
                else orig(self, i);
            };
            //used for overriding vanilla fallen star spawning
            if(ModContent.GetInstance<SFReworksConfig>().FallenStarReplacement) Terraria.On_WorldGen.UpdateWorld += (orig) => {
                float starfallBoost = Star.starfallBoost;
                Star.starfallBoost = 0f;
                orig();
                //directly ripped out of vanilla's code
                for (int k = 0; k < Main.dayRate; k++) {
                    double num10 = Main.maxTilesX / 4200.0;
                    num10 *= (double)starfallBoost / 2.0;
                    if (Main.rand.Next(8000) < 10.0 * num10) {
                        int num11 = 12;
                        int num12 = Main.rand.Next(Main.maxTilesX - 50) + 100;
                        num12 *= 16;
                        int num13 = Main.rand.Next((int)(Main.maxTilesY * 0.05));
                        num13 *= 16;
                        Vector2 vector = new Vector2(num12, num13);
                        int num14 = -1;
                        if (Main.expertMode && Main.rand.Next(15) == 0) {
                            int num15 = Player.FindClosest(vector, 1, 1);
                            if (Main.player[num15].position.Y < Main.worldSurface * 16.0 && Main.player[num15].afkCounter < 3600) {
                                int num16 = Main.rand.Next(1, 640);
                                vector.X = Main.player[num15].position.X + Main.rand.Next(-num16, num16 + 1);
                                num14 = num15;
                            }
                        }
                        if (!Collision.SolidCollision(vector, 16, 16)) {
                            float num17 = Main.rand.Next(-100, 101);
                            float num18 = Main.rand.Next(200) + 100;
                            float num19 = (float)Math.Sqrt((double)(num17 * num17 + num18 * num18));
                            num19 = num11 / num19;
                            num17 *= num19;
                            num18 *= num19;
                            Projectile.NewProjectile(new EntitySource_Misc("FallingStar"), vector.X, vector.Y, num17, num18, ProjectileID.FallingStarSpawner, 0, 0f, Main.myPlayer, 0f, num14, 0f);
                        }
                    }
                }
                Star.starfallBoost = starfallBoost;
            };
            if(ModContent.GetInstance<SFReworksConfig>().FallenStarReplacement) Terraria.On_Projectile.AI_148_StarSpawner += (orig, self) => {
                self.ai[0] += (float)Main.dayRate;
                if (self.localAI[0] == 0f && Main.netMode != NetmodeID.Server) {
                    self.localAI[0] = 1f;
                    if (Main.LocalPlayer.position.Y < Main.worldSurface * 16.0) Star.StarFall(self.position.X);
                }
                if (self.owner == Main.myPlayer && self.ai[0] >= 180f) {
                    if (self.ai[1] > -1f) {
                        self.velocity.X = self.velocity.X * 0.35f;
                        if (self.Center.X < Main.player[(int)self.ai[1]].Center.X) self.velocity.X = Math.Abs(self.velocity.X);
                        else self.velocity.X = -Math.Abs(self.velocity.X);
                    }
                    Projectile.NewProjectile(self.GetSource_FromThis(), self.position.X, self.position.Y, self.velocity.X, self.velocity.Y, ModContent.ProjectileType<Content.Projectiles.Misc.FallingCosmicDust>(), 1000, 10f, Main.myPlayer, 0f, 0f, 0f);
                    self.Kill();
                }
            };
        }

        public override void Unload() {
            ClientConfig = null;
            _musicMod = null;
        }
    }
}
