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
    public class AntiqueDagger : ModItem {
        public override void SetStaticDefaults() {
            //ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
        }

        public override void SetDefaults() {
            Item.damage = 20;
            Item.knockBack = 5f;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.width = 30;
            Item.height = 62;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(0, 0, 5, 0);
             
            Item.shoot = ModContent.ProjectileType<AntiqueDaggerProjectile>();
            Item.shootSpeed = 2.8f;
        }
    }
}
