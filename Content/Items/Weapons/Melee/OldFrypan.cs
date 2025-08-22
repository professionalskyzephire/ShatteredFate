using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShatteredFate.Content.Buffs.Debuffs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Items.Weapons.Melee
{
	public class OldFrypan : ModItem
	{
		public int attackType = 0;

		public override void SetDefaults()
		{
			// Combat
			Item.damage = 30;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 10;
			Item.useTime = Item.useAnimation = 60;
			Item.crit = 0;
			Item.GetGlobalItem<SFGlobalItem>().bonusCritDamage = 2.5f;
			Item.autoReuse = true;
			Item.noMelee = true;

			// Visual
			Item.width = 48;
			Item.height = 46;
			Item.scale = 1f;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noUseGraphic = true;
			Item.useTurn = false;

			// Proj
			Item.shoot = ModContent.ProjectileType<OldFrypan_Hold>();

			// Misc
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Blue;
			//Item.UseSound = SoundID.Item1;
			Item.channel = true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback * 2, Main.myPlayer, attackType);
			//attackType = (attackType + 1) % 2;
			attackType = 0;
			return false;
		}
	}

	public class OldFrypan_Hold : ModProjectile
	{
		public override string Texture => base.Texture.Replace("_Hold", string.Empty);

		private enum AttackStage
		{
			Prepare,
			Ready,
			Execute,
			Finish
		}

		private AttackStage CurrentStage
		{
			get => (AttackStage)Projectile.localAI[0];
			set
			{
				Projectile.localAI[0] = (float)value;
				Timer = 0;
			}
		}

		private float charge = 0f;

		private ref float InitialAngle => ref Projectile.ai[1];
		private ref float Timer => ref Projectile.ai[2];
		private ref float Progress => ref Projectile.localAI[1];
		private ref float Size => ref Projectile.localAI[2];
		private float CurrentCharge = 0;
		private bool stop = false;
		private bool ready = false;
		private float blinkOpacity = 1f;

		private float PrepTime => 70f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float ExecTime => 25f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float HideTime => 30f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

		private Player Owner => Main.player[Projectile.owner];

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
			ProjectileID.Sets.TrailingMode[Type] = 4;
			ProjectileID.Sets.TrailCacheLength[Type] = 7;
		}

		public override void SetDefaults()
		{
			Projectile.width = 44;
			Projectile.height = 42;
			Projectile.friendly = true;
			Projectile.timeLeft = 240;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
		}

		public override void OnSpawn(IEntitySource source)
		{
			Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
			InitialAngle = (float)(-Math.PI / 2 - Math.PI * 1 / 3 * Projectile.spriteDirection);
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((sbyte)Projectile.spriteDirection);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Projectile.spriteDirection = reader.ReadSByte();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			SoundEngine.PlaySound(SoundID.Item178, Projectile.Center);
			Projectile.netUpdate = true;
			if (Timer >= ExecTime / 2)
			{
				PunchCameraModifier modifier = new(Projectile.Center, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2(), 7f, 6f, 16, 160f, "ShatteredFate: OldFrypan");
				Main.instance.CameraModifiers.Add(modifier);
				Projectile.NewProjectileDirect(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero,
					ModContent.ProjectileType<OldFrypanStomp>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 0f, Main.rand.NextFloat(40f, 65f) / 2f, target.position.X > Owner.MountedCenter.X ? 1 : -1)
						.localAI[0] = target.whoAmI;
				stop = true;
			}
		}

		public override void AI()
		{
			Owner.itemAnimation = 2;
			Owner.itemTime = 2;

			Projectile.timeLeft = 10;

			if (CurrentStage != AttackStage.Execute && CurrentStage != AttackStage.Finish)
			{
				Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
				Owner.direction = Projectile.spriteDirection;
				InitialAngle = (float)(-Math.PI / 2 - Math.PI * 1 / 3 * Projectile.spriteDirection);
			}

			if (!Owner.active || Owner.dead || Owner.noItems || Owner.HeldItem.type != ModContent.ItemType<OldFrypan>() || Owner.CCed)
			{
				Projectile.Kill();
				return;
			}

			if (!stop)
			{
				switch (CurrentStage)
				{
					case AttackStage.Prepare:
						Progress = 0.5f * (1.75f * (float)Math.PI) * (1f - Timer / PrepTime);
						Size = MathHelper.SmoothStep(0f, 1f, Timer / PrepTime + 0.5f);
						charge = MathHelper.SmoothStep(0f, 1f, Timer / PrepTime);

						if (!Owner.channel)
						{
							CurrentCharge = 0.5f * (1.75f * (float)Math.PI) * (1f - Timer / PrepTime);
							CurrentStage = AttackStage.Execute;
							//Main.NewText(1f - Timer / PrepTime);
						}

						Timer += Timer * 2f / PrepTime;

						if (Timer >= PrepTime)
						{
							SoundEngine.PlaySound(SoundID.Item4);
							CurrentStage = AttackStage.Ready;
						}
						break;

					case AttackStage.Ready:
						Progress = 0f;
						Timer--;
						ready = true;

						if (!Owner.channel)
						{
							CurrentStage = AttackStage.Execute;
							//Main.NewText(1f - Timer / PrepTime);
						}
						break;

					case AttackStage.Execute:
						Progress = MathHelper.SmoothStep(CurrentCharge, 1.9f * (float)Math.PI, 0.6f * Timer / ExecTime);

						Timer += Timer / ExecTime;

						if (Timer >= ExecTime)
						{
							CurrentCharge = Progress;
							CurrentStage = AttackStage.Finish;
						}
						break;

					default:
						Progress = CurrentCharge;
						//Size = 1f - MathHelper.SmoothStep(0, 1f, Timer / HideTime);
						if (Timer >= HideTime)
						{
							Projectile.Kill();
						}
						break;
				}
			}

			if (stop && Timer >= HideTime)
			{
				Projectile.Kill();
			}

			if (ready)
			{
				blinkOpacity -= 0.06f;
			}

			// Reflect projs
			if (CurrentStage == AttackStage.Execute && ready && !Owner.HasBuff(ModContent.BuffType<AbilityCooldown>()))
			{
				foreach (var targetProj in Main.ActiveProjectiles)
				{
					if (targetProj.whoAmI != Projectile.whoAmI
					&& targetProj.hostile
					&& targetProj.damage < (Main.masterMode ? 200 : 100)
					&& targetProj.width + targetProj.height < Projectile.width + Projectile.height
					)
					{
						if (Projectile.Colliding(Projectile.Hitbox, targetProj.Hitbox))
						{
							targetProj.velocity = -targetProj.velocity;
							targetProj.hostile = false;
							targetProj.friendly = true;
							targetProj.rotation = -targetProj.rotation;

							SoundEngine.PlaySound(SoundID.Item150, Projectile.position);
							Owner.AddBuff(ModContent.BuffType<AbilityCooldown>(), 300);
						}
					}
				}
			}

			SetSwordPosition();
			Timer++;
			//Main.NewText(Timer);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 origin;
			float rotationOffset;
			SpriteEffects effects;

			if (Projectile.spriteDirection > 0)
			{
				origin = new Vector2(0, Projectile.height);
				rotationOffset = MathHelper.ToRadians(45f);
				effects = SpriteEffects.None;
			}
			else
			{
				origin = new Vector2(Projectile.width, Projectile.height);
				rotationOffset = MathHelper.ToRadians(135f);
				effects = SpriteEffects.FlipHorizontally;
			}

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Texture2D texture1 = ModContent.Request<Texture2D>(ShatteredFate.ExtrasPath + "OldFrypan_Extra1").Value;
			Texture2D texture2 = ModContent.Request<Texture2D>(ShatteredFate.ExtrasPath + "OldFrypan_Extra2").Value;

			if (CurrentStage == AttackStage.Execute && ready && !stop)
			{
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2);
					Color color = Projectile.GetAlpha(Color.LightYellow) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
					Main.spriteBatch.Draw(texture1, drawPos, null, color * Projectile.Opacity, Projectile.oldRot[k] + rotationOffset, origin, Projectile.scale - k * 0.05f, effects, 0);
				}
			}

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset,
				origin, Projectile.scale, effects, 0);

			if (ready)
			{
				Main.spriteBatch.Draw(texture2, Projectile.Center - Main.screenPosition, default, Color.White * Projectile.Opacity * blinkOpacity, Projectile.rotation + rotationOffset,
					origin, Projectile.scale, effects, 0);
			}

			return false;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			float collisionPoint = 0f;

			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
		}

		public override void CutTiles()
		{
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
		}

		public override bool? CanDamage()
		{
			if (CurrentStage == AttackStage.Prepare || CurrentStage == AttackStage.Ready || stop || charge < 0.2f)
				return false;
			return base.CanDamage();
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;

			modifiers.CritDamage += charge * 2.5f;
			modifiers.FinalDamage *= charge * 1.5f;
		}

		public void SetSwordPosition()
		{
			Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress;

			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));
			Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi / 2);

			armPosition.Y += Owner.gfxOffY;
			Projectile.Center = armPosition;
			Projectile.scale = 0.25f + Size * Owner.GetAdjustedItemScale(Owner.HeldItem);

			Owner.heldProj = Projectile.whoAmI;
		}
	}

	public class OldFrypanStomp : ModProjectile
	{
		public override string Texture => ShatteredFate.BlankTexture;

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;

			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;

			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.penetrate = -1;

			Projectile.timeLeft = 120;
			Projectile.alpha = 255;

			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 10;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			modifiers.HitDirectionOverride = (int?)Projectile.ai[2];
			modifiers.CritDamage += 2.5f;
		}

		public override bool? CanHitNPC(NPC target)
			=> target.whoAmI != Projectile.localAI[0];

		public override void AI()
		{
			float num = Projectile.ai[1];
			Projectile.ai[0] += 1f;
			if (Projectile.ai[0] > 3f)
			{
				Projectile.Kill();
				return;
			}
			Projectile.velocity = Vector2.Zero;
			Projectile.position = Projectile.Center;
			Projectile.Size = new Vector2(16f, 8f) * MathHelper.Lerp(5f, num, Utils.GetLerpValue(0f, 9f, Projectile.ai[0]));
			Projectile.Center = Projectile.position;
			if (Main.netMode == NetmodeID.Server)
			{
				return;
			}
			var point = Projectile.TopLeft.ToTileCoordinates();
			Point point2 = Projectile.BottomRight.ToTileCoordinates();
			int num3 = Projectile.width / 2;
			if ((int)Projectile.ai[0] % 3 != 0)
				return;
			int num4 = (int)Projectile.ai[0] / 3;
			for (int i = point.X; i <= point2.X; i++)
			{
				for (int j = point.Y; j <= point2.Y; j++)
				{
					if (Vector2.Distance(Projectile.Center, new Vector2(i * 16, j * 16)) > num3)
						continue;
					Tile tileSafely = Framing.GetTileSafely(i, j);
					if (!tileSafely.HasTile || !Main.tileSolid[tileSafely.TileType] || Main.tileSolidTop[tileSafely.TileType] || Main.tileFrameImportant[tileSafely.TileType])
						continue;
					Tile tileSafely2 = Framing.GetTileSafely(i, j - 1);
					if (tileSafely2.HasTile && Main.tileSolid[tileSafely2.TileType] && !Main.tileSolidTop[tileSafely2.TileType])
						continue;
					int num5 = WorldGen.KillTile_GetTileDustAmount(fail: true, tileSafely, i, j);
					for (int k = 0; k < num5 + 5; k++)
					{
						Dust obj = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
						obj.velocity.Y -= 3f + num4 * 1.5f;
						obj.velocity.Y *= Main.rand.NextFloat();
						obj.velocity.Y *= 0.75f;
						obj.scale += num4 * 0.03f;
					}
					if (num4 >= 2)
					{
						for (int m = 0; m < num5 - 1; m++)
						{
							Dust obj2 = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
							obj2.velocity.Y -= 1f + num4;
							obj2.velocity.Y *= Main.rand.NextFloat();
							obj2.velocity.Y *= 0.75f;
						}
					}
					if (num4 >= 2)
					{
						for (int m = 0; m < num5 - 1; m++)
						{
							Dust obj2 = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
							obj2.velocity.Y -= 1f + num4;
							obj2.velocity.Y *= Main.rand.NextFloat();
							obj2.velocity.Y *= 0.75f;
						}
					}
					if (num5 <= 0 || Main.rand.NextBool(3))
						continue;
				}
			}
		}
	}
}