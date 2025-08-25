using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ShatteredFate
{
	public class ShatteredFate : Mod
	{
 		public override void Load() {
   			//used for overriding vanilla fallen star spawning
			Terraria.On_WorldGen.UpdateWorld += (orig) => {
				float starfallBoost = Star.starfallBoost;
				Star.starfallBoost = 0f;
				orig();
				//directly ripped out of vanilla's code
				for(int k = 0; k < Main.dayRate; k++) {
					double num10 = (double)Main.maxTilesX / 4200.0;
					num10 *= (double)starfallBoost / 2.0;
					if((double)Main.rand.Next(8000) < 10.0 * num10) {
						int num11 = 12;
						int num12 = Main.rand.Next(Main.maxTilesX - 50) + 100;
						num12 *= 16;
						int num13 = Main.rand.Next((int)((double)Main.maxTilesY * 0.05));
						num13 *= 16;
						Vector2 vector = new Vector2((float)num12, (float)num13);
						int num14 = -1;
						if(Main.expertMode && Main.rand.Next(15) == 0) {
							int num15 = (int)Player.FindClosest(vector, 1, 1);
							if((double)Main.player[num15].position.Y < Main.worldSurface * 16.0 && Main.player[num15].afkCounter < 3600) {
								int num16 = Main.rand.Next(1, 640);
								vector.X = Main.player[num15].position.X + (float)Main.rand.Next(-num16, num16 + 1);
								num14 = num15;
							}
						}
						if(!Collision.SolidCollision(vector, 16, 16)) {
							float num17 = (float)Main.rand.Next(-100, 101);
							float num18 = (float)(Main.rand.Next(200) + 100);
							float num19 = (float)Math.Sqrt((double)(num17 * num17 + num18 * num18));
							num19 = (float)num11 / num19;
							num17 *= num19;
							num18 *= num19;
							Projectile.NewProjectile(new EntitySource_Misc("FallingStar"), vector.X, vector.Y, num17, num18, 720, 0, 0f, Main.myPlayer, 0f, (float)num14, 0f);
						}
					}
				}
				Star.starfallBoost = starfallBoost;
			};
			Terraria.On_Projectile.AI_148_StarSpawner += (orig, self) => {
				self.ai[0] += (float)Main.dayRate;
				if(self.localAI[0] == 0f && Main.netMode != 2) {
					self.localAI[0] = 1f;
					if((double)Main.LocalPlayer.position.Y < Main.worldSurface * 16.0) Star.StarFall(self.position.X);
				}
				if(self.owner == Main.myPlayer && self.ai[0] >= 180f) {
					if(self.ai[1] > -1f) {
						self.velocity.X = self.velocity.X * 0.35f;
						if(self.Center.X < Main.player[(int)self.ai[1]].Center.X) self.velocity.X = Math.Abs(self.velocity.X);
						else self.velocity.X = -Math.Abs(self.velocity.X);
					}
					Projectile.NewProjectile(self.GetSource_FromThis(), self.position.X, self.position.Y, self.velocity.X, self.velocity.Y, ModContent.ProjectileType<Content.Projectiles.Misc.FallingCosmicDust>(), 1000, 10f, Main.myPlayer, 0f, 0f, 0f);
					self.Kill();
				}
			};
		}
		public const string VanillaTexture = "Terraria/Images/";
		public const string ExtrasPath = "ShatteredFate/Extras/";
		public const string BlankTexture = "ShatteredFate/Extras/Invisible";
		public const string MagicPixel = "ShatteredFate/Extras/MagicPixel";
	}
}
