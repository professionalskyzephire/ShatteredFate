using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Projectiles.Magic;

public class CuriousCandleFireball : ModProjectile
{
	// ----- Constants -----
	private const int State_MoveToOwner = 0;
	private const int State_MoveToTarget = 1;
	private const int State_Explode = 2;

	private const float TargetDistance = 500f;
	private const float MinDistance = 80f;
	private const float MaxDistance = 2400f;

	private const float Speed = 8f;
	private const float Inertia = 40f;

	private const int ExplosionRadius = 48;
	private const int ExplosionDustAmount = 150;

	// ----- Variables -----
	private byte _state = 0;
	private int _target = -1;
	private int _timer = 0;
	private bool _justSpawned = true;

	// ----- Shorthands -----
	private Player Player => Main.player[Projectile.owner];
	private NPC Target => Main.npc[_target];
	private bool HasTarget => _target >= 0 && _target < Main.maxNPCs;

	#region ----- Defaults -----
	public override void SetStaticDefaults()
	{
		Main.projFrames[Type] = 3;
	}

	public override void SetDefaults()
	{
		// AI
		Projectile.aiStyle = -1;

		// Entity Interation
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 5;

		// Hitbox 
		Projectile.width = 10;
		Projectile.height = 10;

		// Network
		Projectile.netImportant = true;

		// Movement
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.timeLeft = 60;

		// Visual
		Projectile.Opacity = 0f;
	}
	#endregion

	#region ----- Drawing -----
	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Type].Value;

		Main.EntitySpriteDraw(
			texture,
			Projectile.Center - Main.screenPosition,
			new Rectangle(
				0,
				texture.Height / Main.projFrames[Type] * Projectile.frame,
				texture.Width,
				texture.Height / Main.projFrames[Type]),
			lightColor,
			Projectile.rotation,
			new Vector2(5, 19),
			Projectile.scale,
			Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 
			0);


		return false;
	}
	#endregion

	#region ----- AI -----
	public override void AI()
	{
		// Check if the owner of the projectile is valid.
		if (!OwnerCheck())
		{
			// Forcefully explode the fireballs
			Explode();
		}

		// Play animations when the projectile spawns in.
		if (_justSpawned)
		{
			if (!Main.dedServ)
			{
				// Spawn dust
				int amount = Main.rand.Next(8, 24);
				for (int i = 0; i < amount; i++)
				{
					Dust dust = Dust.NewDustDirect(
						Projectile.Center, 
						0, 
						0,
						DustID.Enchanted_Pink,
						Main.rand.NextFloat(-2f, 2f),
						Main.rand.NextFloat(-2f, 2f),
						0,
						default,
						Main.rand.NextFloat(1.1f, 1.5f));

					dust.noGravity = true;
				}

				// Play sound
				SoundEngine.PlaySound(SoundID.Item20 with { Volume = 0.75f });
			}
			// Toggle spawn boolean
			_justSpawned = false;
		}

		// Run the main state machine for the projectile.
		switch (_state)
		{
			case State_MoveToOwner:
				{
					// Keep the projectile alive
					Projectile.timeLeft = 3;

					// Search for the closest target within range
					AqcuireTarget();

					// Move to the owner
					OwnerMovement();

					// A target has been found
					if (HasTarget)
					{
						_state = State_MoveToTarget;
						Projectile.netUpdate = true;
					}
				}
				break;

			case State_MoveToTarget:
				{
					// Keep the projectile alive
					Projectile.timeLeft = 3;

					// Search for the closest target within range
					AqcuireTarget();

					// The target has been lost
					if (!HasTarget)
					{
						_state = State_MoveToOwner;
						Projectile.netUpdate = true;
						break;
					}

					// Move to the target
					TargetMovement();
				}
				break;

			case State_Explode:
				{
					// Resize the projectile hitbox
					Projectile.Resize(ExplosionRadius * 2, ExplosionRadius * 2);

					// Make sure the projectile is killed after this tick.
					if (Projectile.timeLeft > 1)
					{
						Projectile.timeLeft = 1;
					}

					// The OnKill hook will take care of dust and sound.
				}
				break;
		}

		HandleVisuals();
	}

	private bool OwnerCheck()
	{
		return Projectile.owner >= 0
			&& Projectile.owner < Main.maxPlayers
			&& Player != null
			&& Player.active
			&& !Player.ghost
			&& !Player.dead;
	}

	private void AqcuireTarget()
	{
		// Track the closest target
		int closestTarget = -1;

		// Set the maximum target distance that the fireball
		// is allowed to target.
		float targetDistance = TargetDistance;

		// Loop over all the NPCs in the npc array
		foreach (NPC target in Main.npc)
		{
			// Check if the target can be chased.
			if (target == null || !target.CanBeChasedBy(this))
			{
				continue;
			}

			// Get the distance to the target.
			float distance = Vector2.Distance(target.Center, Projectile.Center);

			// Check the distance to the new potential target.
			// If the target is in range and the fireball has line of sight,
			// consider it as a potential new target.
			// Bypass distance check if this is a previously selected target.
			if ((target.whoAmI != _target && distance >= targetDistance) || !SFUtils.CanHitLine(Projectile.Center, target.Center))
			{
				continue;
			}

			// Set the new potential target
			closestTarget = target.whoAmI;

			// Overwrite the new closest distance
			targetDistance = distance;
		}

		// Trigger a netupdate when the target has been changed
		if (closestTarget != _target)
		{
			_target = closestTarget;
			Projectile.netUpdate = true;
		}
	}

	private void OwnerMovement()
	{
		// Calculate the distance to owner as vector2 and float
		Vector2 direction = GetIdlePosition() - Projectile.Center;
		float distance = direction.Length();

		// Get the base speed and half inertia
		float speed = Speed;
		float halfInertia = Inertia / 2f;

		// Correct speed based on the distance to the owner
		if (distance > MinDistance)
		{
			float quotient = (distance - MinDistance) / (MaxDistance - MinDistance);
			speed *= 1f + quotient;
		}
		else
		{
			float quotient = distance / MinDistance;
			speed *= quotient * 2f;
			halfInertia *= 1f + 0.5f * (quotient - 1f);
		}

		// Normalize the direction
		direction = direction.SafeNormalize(Vector2.Zero);

		// Calculate the new velocity
		Projectile.velocity = (Projectile.velocity * halfInertia + direction * speed) / (halfInertia + 1);
	}

	private void TargetMovement()
	{
		// Get the difference between the target
		// and the fireball projectile.
		Vector2 direction = Target.Center - Projectile.Center;
		float distance = direction.Length();

		// Get the base speed and half inertia
		float speed = Speed;
		float halfInertia = Inertia / 2f;

		// Correct speed based on the distance to the owner
		if (distance <= MinDistance * 2f)
		{
			float quotient = distance / (MinDistance * 2f);
			speed *= 0.25f + 0.75f * quotient;
			halfInertia *= 1f + 0.5f * (quotient - 1f);
		}

		// Normalize
		direction = direction.SafeNormalize(Vector2.Zero);

		// Calculate the new velocity
		Projectile.velocity = (Projectile.velocity * halfInertia + direction * speed) / (halfInertia + 1);
	}

	private Vector2 GetIdlePosition()
	{
		const float distance = 20f;
		const float offset = 5f;

		// The amount of fireballs this player owns.
		int count = Player.ownedProjectileCounts[Type];

		// Get the center of the player
		Vector2 center = Player.RotatedRelativePoint(Player.MountedCenter);

		// Add offset on the X-axis based on the direction,
		// this places the fireballs at the player's back.
		center.X -= Player.direction * 15f;

		// Add offset on the Y-axis based on the gravity
		// direction.
		center.Y -= Player.gravDir * 10f;

		// When there is a single fireball active, use a position
		// just behind the owner.
		if (count <= 1)
		{
			return center;
		}

		// Count the amount of fireballs
		int otherFireballs = 0;

		// Loop over all fireball before this one
		for (int i = 0; i < Projectile.whoAmI; i++)
		{
			if (Main.projectile[i].active
				&& Main.projectile[i].owner == Projectile.owner
				&& Main.projectile[i].type == Projectile.type)
			{
				otherFireballs++;
			}
		}

		// Calculate the amount of times the angle need to be divided.
		int divider = count * 2;

		// Calculate the angle of each segment
		float angle = 360f / divider;

		int p = 0;
		for (int i = 0; i < divider; i++)
		{
			if (i % 2 == 1)
			{
				if (p == count - otherFireballs - 1)
				{
					angle = angle * i;
					break;
				}

				p++;
			}
		}

		return center + new Vector2(0, -distance - Math.Abs(angle - (angle < 180 ? 90f : 270f)) / 90f * offset).RotatedBy(MathHelper.ToRadians(angle));
	}

	private void Explode()
	{
		// Check if the projectile is already in the explode state
		if (_state != State_Explode)
		{
			_state = State_Explode;
			Projectile.netUpdate = true;
		}
	}

	private void HandleVisuals()
	{
		// During the explosion state, hide the projectile sprite
		if (_state == State_Explode)
		{
			Projectile.Opacity = 0f;
			return;
		}

		// Add light to the projectile's center
		Lighting.AddLight(Projectile.Center, new Color(235, 93, 175).ToVector3());

		// Increase the opacity for the fade in effect
		if (_state != State_Explode)
		{
			Projectile.Opacity += 0.1f;
		}

		// Increase the frame counter
		if (++Projectile.frameCounter > 5)
		{
			Projectile.frameCounter = 0;

			if (++Projectile.frame >= Main.projFrames[Type])
			{ 
				Projectile.frame = 0;
			}
		}
	}

	private void ExplodeEffects()
	{
		// Do not run sound and dust on server
		if (Main.dedServ)
		{
			return;
		}

		// Play explosion sound
		SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

		// Spawn dust
		for (int i = 0; i < ExplosionDustAmount; i++)
		{
			// Calculate a random velocity
			Vector2 velocity = new Vector2(Main.rand.NextFloat(0.3f, 2f), 0f).RotatedByRandom(MathHelper.Pi);

			// Spawn dust
			Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Enchanted_Pink, velocity.X, velocity.Y, 100, default, Main.rand.NextFloat(1.2f, 1.8f));
		}
	}
	#endregion

	#region ----- Other -----
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		// Upon hitting an enemy, make the fireball explode
		Explode();
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		// Ignore all the enemies defense (1f means 100% ignored).
		modifiers.ScalingArmorPenetration += 1f;
	}

	public override bool? CanCutTiles()
	{
		return _state != State_MoveToOwner ? base.CanCutTiles() : false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		// When not in the exploding state, run the default collision
		if (_state != State_Explode)
		{
			return base.Colliding(projHitbox, targetHitbox);
		}

		// ---- Explosion collision circle vs Rectangle + Tile check -----

		// Get the positions of the circle
		float px = projHitbox.Center.X;
		float py = projHitbox.Center.Y;

		// Clamp the position of the circle on top of the rectangle
		px = Math.Clamp(px, targetHitbox.X, targetHitbox.X + targetHitbox.Width);
		py = Math.Clamp(py, targetHitbox.Y, targetHitbox.Y + targetHitbox.Height);

		// Check the position on the rectangle vs the circle position and radius
		if (Math.Pow(projHitbox.Center.Y - py, 2) + Math.Pow(projHitbox.Center.X - px, 2) >= Math.Pow(ExplosionRadius, 2))
		{
			// Target is outside the explosion range
			return false;
		}

		// Check if the target can be hit by the explosion, check tiles
		if (!SFUtils.CanHitLine(projHitbox.Center.ToVector2(), new Vector2(px, py)))
		{
			// Tiles in the way
			return false;
		}

		// All checks good
		return true;
	}

	public override void OnKill(int timeLeft)
	{
		// Run Explosion effects
		ExplodeEffects();
	}
	#endregion

	#region ----- Network -----
	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(_state);
		writer.Write(_target);
		writer.Write(_timer);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		_state = reader.ReadByte();
		_target = reader.ReadInt32();
		_timer = reader.ReadInt32();
	}
	#endregion
}