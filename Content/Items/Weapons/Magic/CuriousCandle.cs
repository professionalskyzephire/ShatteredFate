using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;
using ShatteredFate.Content.Projectiles.Magic;

namespace ShatteredFate.Content.Items.Weapons.Magic;

public class CuriousCandle : ModItem
{
    // ----- Constants -----
    private const int SpawnCooldown = 120;
    private const int MaxFireballAmount = 10;

    // ----- Variables -----
    private static int _spawnTimer = 0; // Note: can be static because is only used on local player

    public override void SetStaticDefaults()
    {
        ItemID.Sets.AnimatesAsSoul[Type] = true;
        Main.RegisterItemAnimation(Type, new CuriousCandleAnimation());
        Item.ResearchUnlockCount = 1;
    }

    public override void SetDefaults()
    {
        // Information
        Item.value = Item.sellPrice(silver: 50);
        Item.rare = ItemRarityID.White;

        // Hitbox
        Item.width = 30;
        Item.height = 50;

        // Usage and animation
        Item.holdStyle = ItemHoldStyleID.HoldFront;
        Item.useStyle = ItemUseStyleID.None;

        // Damage, knockback, damage type (stats)
        Item.damage = 11;
        Item.DamageType = DamageClass.Magic;
        Item.knockBack = 4f;
    }

	public override void HoldItem(Player player)
    {
		// Make sure to only run the projectile spawn logic on the player
		if (player.whoAmI != Main.myPlayer)
        {
            return;
        }

        // Check if there are fireball slots remaining
        if (player.ownedProjectileCounts[ProjectileType<CuriousCandleFireball>()] >= MaxFireballAmount)
        {
            return;
        }

        // Count up the fireball spawn timer and check if it exceeds the spawn cooldown.
        if (_spawnTimer++ < SpawnCooldown)
        {
            return;
        }

        // Spawn a fireball projectile.
        Projectile.NewProjectile(
            player.GetSource_ItemUse_WithPotentialAmmo(Item, ItemID.None),
			player.RotatedRelativePoint(player.MountedCenter) + new Vector2(20 * player.direction, -22 * player.gravDir).RotatedBy(player.fullRotation),
            Vector2.Zero,
            ProjectileType<CuriousCandleFireball>(),
            player.GetWeaponDamage(Item),
            player.GetWeaponKnockback(Item),
            Main.myPlayer);

        // Reset the spawn timer
        _spawnTimer = 0;
    }

	public override void HoldItemFrame(Player player)
	{
		base.HoldItemFrame(player);
	}

	public override void HoldStyle(Player player, Rectangle heldItemFrame)
	{
        player.itemLocation.X -= 12 * player.direction;
        player.itemLocation.Y += 12 * player.gravDir;
	}

	public override void Update(ref float gravity, ref float maxFallSpeed)
	{
		// Add light to the item's center
		Lighting.AddLight(Item.Center, new Color(235, 93, 175).ToVector3());

	}

	private class CuriousCandleAnimation : DrawAnimation
    {
        // ----- Constants -----
        private const int FrameAmount = 8;
        private const int FrameTime = 5;
        private const int Padding = 2;

        // ----- Variables -----
        private int _timer;
        private int _frame;

        public override void Update()
        {
            if (++_timer >= FrameTime)
            {
                _timer = 0;

                if (++_frame >= FrameAmount)
                {
                    _frame = 0;
                }
            }
        }

        public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1)
        {
            return new Rectangle(
                0,
                texture.Height / FrameAmount * _frame + Padding,
                texture.Width,
                texture.Height / FrameAmount - Padding);
        }
    }
}
