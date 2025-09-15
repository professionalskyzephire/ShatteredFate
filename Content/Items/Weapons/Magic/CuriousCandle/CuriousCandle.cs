using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;

namespace ShatteredFate.Content.Items.Weapons.Magic.CuriousCandle;

public class CuriousCandle : ModItem
{
    // ----- Constants -----
    private const int SpawnCooldown = 120;
    private const int MaxFireballAmount = 10;

    // ----- Variables -----
    private static int _spawnTimer = 0;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.AnimatesAsSoul[Type] = true;
        Main.RegisterItemAnimation(Type, new CuriousCandleAnimation());
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
        if (player.ownedProjectileCounts[ProjectileType<CuriousCandle_Fireball>()] >= MaxFireballAmount)
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
            player.Center,
            Vector2.Zero,
            ProjectileType<CuriousCandle_Fireball>(),
            player.GetWeaponDamage(Item),
            player.GetWeaponKnockback(Item),
            Main.myPlayer);

		// Reset the spawn timer
		_spawnTimer = 0;
	}

	private class CuriousCandleAnimation : DrawAnimation
	{
        // ----- Constants -----
        private const int FrameAmount = 8;
        private const int FrameTime = 5;

        // ----- Variables -----
        private int _timer;
        private int _frame;

		public override void Update()
		{
            if (_timer++ >= FrameTime)
            {
                _timer = 0;

                if (_frame++ >= FrameAmount)
                { 
                    _frame = 0;
                }
            }
		}

		public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1)
		{
            return new Rectangle(
                0,
                texture.Height / FrameAmount * _frame,
                texture.Width,
                texture.Height / FrameAmount);
		}
	}
}
