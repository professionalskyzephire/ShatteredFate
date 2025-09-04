using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Items.Weapons.Magic
{
    public class Starstaff : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;
        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.holdStyle = ItemHoldStyleID.HoldGuitar;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.damage = 10;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(gold: 8, silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<Content.Projectiles.Magic.Starstaff>();
            Item.shootSpeed = 14f;
            Item.noMelee = true;
            Item.mana = 10;
            Item.UseSound = SoundID.Item9;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            position.Y -= 1000;
            if (Main.myPlayer == player.whoAmI) for (int i = -1; i <= 1; i++) {
                    velocity = Vector2.Normalize(Main.MouseWorld - position) * velocity.Length();
                    int x = Projectile.NewProjectile(source, position + velocity.RotatedBy(MathHelper.PiOver2 * i) * MathHelper.Pi, velocity, type, damage, knockback, player.whoAmI, i == 0 ? -2f : player.altFunctionUse, i);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, x);
                }
            return false;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult) {
            if (player.altFunctionUse == 2) mult *= 2f;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void UseStyle(Player player, Rectangle itemFrame) {
            if (player.altFunctionUse == 2) player.itemRotation += MathHelper.PiOver4 * player.direction * 0.1f * (float)System.Math.Sin(player.itemAnimation * MathHelper.TwoPi / player.itemAnimationMax);
        }
        public override void HoldStyle(Player player, Rectangle itemFrame) => player.itemRotation += MathHelper.PiOver4 * player.direction * 0.4f;
        //Note: Currently trying to figure out how we're supposed to make this crafted by throwing everything in shimmer since wood is shimmered into dirt, making it uncraftable.
        public override void AddRecipes() => CreateRecipe().AddRecipeGroup(RecipeGroupID.Wood, 15).AddIngredient(ItemID.FallenStar, 10).AddCondition(Condition.NearShimmer).Register();
    }
}

