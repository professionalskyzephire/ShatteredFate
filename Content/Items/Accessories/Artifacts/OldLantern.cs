using InnoVault;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Items.Accessories.Artifacts;

public class OldLantern : ModItem {
    public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;
    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 28;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Blue;
        Item.accessory = true;
    }
    public override void UpdateAccessory(Player player, bool hideVisual) {
        Lighting.AddLight(player.Center, new Vector3(0.2f, 0.8f, 0.8f));

        player.GetModPlayer<OldLanternPlayer>().hasOldLantern = true;
        player.GetModPlayer<OldLanternPlayer>().hideVisual = hideVisual;

        for (int i = 0; i < Main.maxNPCs; i++) {
            NPC npc = Main.npc[i];
            if (npc.active && !npc.friendly && npc.lifeMax > 5 && Vector2.Distance(player.Center, npc.Center) < 480f) { Lighting.AddLight(npc.Center, new Vector3(0.15f, 0.5f, 0.5f) * 1.6f); };
        };
    }
}
public class OldLanternPlayer : ModPlayer {
    public bool hasOldLantern = false;
    public bool hideVisual = false;

    public override void ResetEffects() {
        hasOldLantern = false;
        hideVisual = false;
    }
    public override void PostUpdate() {
        if (hasOldLantern && !hideVisual) {
            if (Player.CountProjectilesOfID<OldLanternProjectile>() == 0) {
                if (Main.myPlayer == Player.whoAmI) {
                    Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor[3]), Player.Center, Vector2.Zero, ModContent.ProjectileType<OldLanternProjectile>(), 0, 0f, Player.whoAmI);
                };
            };
        };
    }
}
public class OldLanternProjectile : ModProjectile {
    public override string Texture => "ShatteredFate/Content/Items/Accessories/Artifacts/OldLantern";
    public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = 1;
    public override void SetDefaults() {
        Projectile.width = 24;
        Projectile.height = 28;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 2;
        Projectile.alpha = 0;
    }
    public override void AI() {
        Player player = Main.player[Projectile.owner];

        if (!player.active || player.dead || !player.GetModPlayer<OldLanternPlayer>().hasOldLantern) {
            Projectile.Kill();
            return;
        }

        Projectile.timeLeft = 2;

        Vector2 targetPosition = player.Center + new Vector2(0, -40 - player.gfxOffY);

        float floatOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 3f;
        targetPosition.Y += floatOffset;

        Projectile.Center = Vector2.Lerp(Projectile.Center, targetPosition, 0.15f);

        Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.9f, 0.9f));

        Projectile.rotation = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1.5f) * 0.1f;
    }
    public override bool PreDraw(ref Color lightColor) {
        Player player = Main.player[Projectile.owner];

        if (player.GetModPlayer<OldLanternPlayer>().hideVisual) { return false; }

        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawPosition = Projectile.Center - Main.screenPosition;
        Vector2 origin = texture.Size() / 2f;

        SpriteEffects spriteEffects = player.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        for (int i = 0; i < 4; i++) {
            Vector2 offset = new Vector2(2f, 0).RotatedBy(MathHelper.TwoPi * i / 4f);
            Main.EntitySpriteDraw(texture, drawPosition + offset, null, new Color(100, 255, 255, 0) * 0.6f, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
        };

        Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

        return false;
    }
}