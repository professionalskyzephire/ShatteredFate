using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using ShatteredFate.Content.Projectiles.Melee;

namespace ShatteredFate.Content.Items.Weapons.Melee
{
    public class WreckingBall : ModItem {
        public override void SetStaticDefaults() {
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
        }

        public override void SetDefaults() {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.knockBack = 10f;
            Item.width = 36;
            Item.height = 60;
            Item.damage = 25;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<WreckingBallProjectile>();
            Item.shootSpeed = 12f;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.channel = true;
            Item.noMelee = true;
        }
    }
}
