using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Projectiles.Melee
{
    public class WreckingBallProjectile : ModProjectile {
        private static Asset<Texture2D> chainTexture;

        private enum AIState {
            Spinning,
            LaunchingForward,
            Retracting,
            UnusedState,
            ForcedRetracting,
            Ricochet,
            Dropping
        }

        private AIState CurrentAIState {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float StateTimer => ref Projectile.ai[1];
        public ref float CollisionCounter => ref Projectile.localAI[0];
        public ref float SpinningStateTimer => ref Projectile.localAI[1];

        public override void Load() {
            chainTexture = ModContent.Request<Texture2D>(Texture + "Chain");
        }

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults() {
            Projectile.netImportant = true;
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI() {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f) {
                Projectile.Kill();
                return;
            }
            if (Main.myPlayer == Projectile.owner && Main.mapFullscreen) {
                Projectile.Kill();
                return;
            }

            Vector2 mountedCenter = player.MountedCenter;
            bool doFastThrowDust = false;
            bool shouldOwnerHitCheck = false;
            int launchTimeLimit = 20;
            float launchSpeed = 14f;
            float maxLaunchLength = 900f;
            float retractAcceleration = 1f;
            float maxRetractSpeed = 14f;
            float forcedRetractAcceleration = 6f;
            float maxForcedRetractSpeed = 15f;
            float unusedRetractAcceleration = 1f;
            float unusedMaxRetractSpeed = 16f;
            int defaultHitCooldown = 10;
            int spinHitCooldown = 20;
            int movingHitCooldown = 10;
            int ricochetTimeLimit = launchTimeLimit + 5;

            float meleeSpeedMultiplier = player.GetTotalAttackSpeed(DamageClass.Melee);
            launchSpeed *= meleeSpeedMultiplier;
            unusedRetractAcceleration *= meleeSpeedMultiplier;
            unusedMaxRetractSpeed *= meleeSpeedMultiplier;
            retractAcceleration *= meleeSpeedMultiplier;
            maxRetractSpeed *= meleeSpeedMultiplier;
            forcedRetractAcceleration *= meleeSpeedMultiplier;
            maxForcedRetractSpeed *= meleeSpeedMultiplier;
            float launchRange = launchSpeed * launchTimeLimit;
            float maxDroppedRange = launchRange + 160f;
            Projectile.localNPCHitCooldown = defaultHitCooldown;
            bool canDrop = false;


            switch (CurrentAIState) {
                case AIState.Spinning: {
                        shouldOwnerHitCheck = true;
                        if (Projectile.owner == Main.myPlayer) {
                            Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * player.direction);
                            player.ChangeDir((unitVectorTowardsMouse.X > 0f).ToDirectionInt());
                            if (!player.channel)
                            {
                                CurrentAIState = AIState.LaunchingForward;
                                StateTimer = 0f;
                                Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
                                Projectile.Center = mountedCenter;
                                Projectile.netUpdate = true;
                                Projectile.ResetLocalNPCHitImmunity();
                                Projectile.localNPCHitCooldown = movingHitCooldown;
                                break;
                            }
                        }
                        SpinningStateTimer += 1f;
                        Vector2 offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 10f * (SpinningStateTimer / 60f) * player.direction);

                        offsetFromPlayer.Y *= 0.8f;
                        if (offsetFromPlayer.Y * player.gravDir > 0f) {
                            offsetFromPlayer.Y *= 0.5f;
                        }
                        Projectile.Center = mountedCenter + offsetFromPlayer * 30f + new Vector2(0, player.gfxOffY);
                        Projectile.velocity = Vector2.Zero;
                        Projectile.localNPCHitCooldown = spinHitCooldown; // set the hit speed to the spinning hit speed
                        break;
                    }
                case AIState.LaunchingForward: {
                        //Projectile.tileCollide = false;
                        doFastThrowDust = true;
                        bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
                        shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;
                        if (player.controlUseItem && canDrop)
                        {
                            CurrentAIState = AIState.Dropping;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.2f;
                            break;
                        }
                        if (shouldSwitchToRetracting) {
                            CurrentAIState = AIState.Retracting;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.3f;
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        Projectile.localNPCHitCooldown = movingHitCooldown;
                        TryMineTiles(Projectile.position, Projectile.width, Projectile.height);

                        break;
                    }
                case AIState.Retracting: {
                        Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                        if (Projectile.Distance(mountedCenter) <= maxRetractSpeed) {
                            Projectile.Kill();
                            return;
                        }
                        if (player.controlUseItem && canDrop)
                        {
                            CurrentAIState = AIState.Dropping;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.2f;
                        } else {
                            Projectile.velocity *= 0.98f;
                            Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxRetractSpeed, retractAcceleration);
                            player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        }
                        
                        break;
                    }
                case AIState.ForcedRetracting: {
                        Projectile.tileCollide = false;
                        Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                        if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed) {
                            Projectile.Kill();
                            return;
                        }
                        Projectile.velocity *= 0.98f;
                        Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
                        Vector2 target = Projectile.Center + Projectile.velocity;
                        Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
                        if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f) {
                            Projectile.Kill();
                            return;
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        break;
                    }
                case AIState.Ricochet:
                    if (StateTimer++ >= ricochetTimeLimit) {
                        CurrentAIState = AIState.Dropping;
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                    } else {
                        Projectile.localNPCHitCooldown = movingHitCooldown;
                        Projectile.velocity.Y += 0.6f;
                        Projectile.velocity.X *= 0.95f;
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                    }
                    break;
                case AIState.Dropping:
                    if (!player.controlUseItem || Projectile.Distance(mountedCenter) > maxDroppedRange || !canDrop) {
                        CurrentAIState = AIState.ForcedRetracting;
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                    } else {
                        Projectile.velocity.Y += 0.8f;
                        Projectile.velocity.X *= 0.95f;
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                    }
                    break;
            }


            Projectile.direction = (Projectile.velocity.X > 0f).ToDirectionInt();
            Projectile.spriteDirection = Projectile.direction;
            Projectile.ownerHitCheck = shouldOwnerHitCheck;

            bool freeRotation = CurrentAIState == AIState.Ricochet || CurrentAIState == AIState.Dropping;
            if (freeRotation) {
                if (Projectile.velocity.Length() > 1f)
                    Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
                else
                    Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
            } else {
                Vector2 vectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;
            }


            Projectile.timeLeft = 2;
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
            player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
            if (Projectile.Center.X < mountedCenter.X) {
                player.itemRotation += (float)Math.PI;
            }
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            int dustRate = 15;
            if (doFastThrowDust)
                dustRate = 1;

        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (CurrentAIState == AIState.LaunchingForward) {
                Projectile.velocity = oldVelocity;
                return false;
            }

            int defaultLocalNPCHitCooldown = 10;
            int impactIntensity = 0;
            Vector2 velocity = Projectile.velocity;
            float bounceFactor = 0.2f;
            if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Ricochet) {
                bounceFactor = 0.4f;
            }

            if (CurrentAIState == AIState.Dropping) {
                bounceFactor = 0f;
            }

            if (oldVelocity.X != Projectile.velocity.X) {
                if (Math.Abs(oldVelocity.X) > 4f) {
                    impactIntensity = 1;
                }

                Projectile.velocity.X = (0f - oldVelocity.X) * bounceFactor;
                CollisionCounter += 1f;
            }

            if (oldVelocity.Y != Projectile.velocity.Y) {
                if (Math.Abs(oldVelocity.Y) > 4f) {
                    impactIntensity = 1;
                }

                Projectile.velocity.Y = (0f - oldVelocity.Y) * bounceFactor;
                CollisionCounter += 1f;
            }

            if (CurrentAIState == AIState.LaunchingForward) {
                CurrentAIState = AIState.Ricochet;
                Projectile.localNPCHitCooldown = defaultLocalNPCHitCooldown;
                Projectile.netUpdate = true;
                Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
                Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
                impactIntensity = 2;
                Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);
                Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity);
                Projectile.position -= velocity;
            }

            if (impactIntensity > 0) {
                Projectile.netUpdate = true;
                for (int i = 0; i < impactIntensity; i++) {
                    Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
                }

                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            }

            if (CurrentAIState != AIState.UnusedState && CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f) {
                CurrentAIState = AIState.ForcedRetracting;
                Projectile.netUpdate = true;
            }
            return false;
        }
        private void TryMineTiles(Vector2 position, int width, int height) {
            int startX = (int)(position.X / 16f);
            int endX = (int)((position.X + width) / 16f);
            int startY = (int)(position.Y / 16f);
            int endY = (int)((position.Y + height) / 16f);

            startX = Math.Max(startX, 0);
            endX = Math.Min(endX, Main.maxTilesX - 1);
            startY = Math.Max(startY, 0);
            endY = Math.Min(endY, Main.maxTilesY - 1);

            bool hitUnbreakableTile = false;
            float hits = 0;
            for (int i = startX; i <= endX; i++) {
                for (int j = startY; j <= endY; j++) {
                    Tile tile = Main.tile[i, j];
                    if (tile.HasTile && !Main.tileHammer[tile.TileType]) {
                        if (WorldGen.SolidTile3(i, j)) { hits += 0.1f; }
                        var player = Main.player[Projectile.owner];
                        if (SFUtils.PickPowerIsEnoughToHurtTile(i, j, 59, player)) {
                            player.PickTile(i, j, 50);
                            if (WorldGen.SolidTile3(i, j)) {
                                StateTimer++;
                                hits += 0.8f; 
                            }
                            if (TileID.Sets.Grass[tile.TileType] || TileID.Sets.GrassSpecial[tile.TileType] || Main.tileMoss[tile.TileType] || TileID.Sets.tileMossBrick[tile.TileType]) {
                                player.PickTile(i, j, 10000);
                            }
                            player.PickTile(i, j, 10000);
                        }
                        if (tile.HasTile) {
                            hitUnbreakableTile = true;
                        }
                    }
                }
            }
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * (Projectile.velocity.Length() - hits);

            if (hitUnbreakableTile && CurrentAIState == AIState.LaunchingForward) {
                CurrentAIState = AIState.Retracting;
                StateTimer = 0f;
                Projectile.netUpdate = true;
            }
        }
        public override bool? CanDamage() {
            if (CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f) {
                return false;
            }
            return base.CanDamage();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            if (CurrentAIState == AIState.Spinning) {
                Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
                Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
                shortestVectorFromPlayerToTarget.Y /= 0.8f;
                float hitRadius = 55f;
                return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
            if (CurrentAIState == AIState.Spinning) {
                modifiers.SourceDamage *= 1.2f;
            }
            else if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting) {
                modifiers.SourceDamage *= 2f;
            }

            modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();

            if (CurrentAIState == AIState.Spinning) {
                modifiers.Knockback *= 0.25f;
            }
            else if (CurrentAIState == AIState.Dropping) {
                modifiers.Knockback *= 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

            playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

            Rectangle? chainSourceRectangle = null;
            float chainHeightAdjustment = 0f;

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0) {
                chainSegmentLength = 10;
            }
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

            while (chainLengthRemainingToDraw > 0f) {
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                var chainTextureToDraw = chainTexture;

                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }

            if (CurrentAIState == AIState.LaunchingForward || true) {
                Texture2D projectileTexture = TextureAssets.Projectile[Type].Value;
                Vector2 drawOrigin = new Vector2(projectileTexture.Width * 0.5f, Projectile.height * 0.5f);
                SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                int afterimageCount = Math.Min(Projectile.oldPos.Length - 1, (int)StateTimer);
                for (int k = afterimageCount; k > 0; k--) {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.5f;
                    Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
                }
            }
            return true;
        }
    }
}
