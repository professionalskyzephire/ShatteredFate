using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ShatteredFate.Common
{
	public class SFGemStavesProjectiles : GlobalProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<SFReworksConfig>().GemStaves;
		public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation) => projectile.type >= 121 && projectile.type <= 126;
		public override void SetDefaults(Projectile projectile) {
			if(projectile.type == 121) projectile.penetrate = 5;
			if(projectile.type == 122) projectile.penetrate = 3;
			if(projectile.type == 123) projectile.penetrate = 1;
			if(projectile.type == 124) projectile.penetrate = 2;
			if(projectile.type == 125) projectile.penetrate = 10;
			if(projectile.type == 126) projectile.penetrate = 4;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 60;
			
		}
		public override void AI(Projectile projectile) {
			if(projectile.type == 123 && projectile.ai[2] == 1f) {
				int target = -1;
				float maxRange = 3000f;
				foreach(NPC npc in Main.ActiveNPCs) if(npc.CanBeChasedBy(projectile, false) && npc.Distance(projectile.Center) < maxRange) {
					maxRange = projectile.Distance(npc.Center);
					target = npc.whoAmI;
				}
				if(target == -1) return;
				projectile.velocity += Vector2.Normalize(Main.npc[target].Center - projectile.Center) * 0.35f;
				projectile.velocity *= 0.95f;
			}
			if(projectile.type == 125 && projectile.ai[2] > 0f) {
				int target = -1;
				float maxRange = 3000f;
				foreach(NPC npc in Main.ActiveNPCs) if(npc.CanBeChasedBy(projectile, false) && npc.Distance(projectile.Center) < maxRange) {
					maxRange = projectile.Distance(npc.Center);
					target = npc.whoAmI;
				}
				if(target == -1) return;
				projectile.velocity += Vector2.Normalize(Main.npc[target].Center - projectile.Center) * 0.35f;
				projectile.velocity *= 0.95f;
			}
		}
		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
			if(projectile.type == 124) if(Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Content.Projectiles.Magic.LargeEmerald>()] == 0) NetMessage.SendData(27, -1, -1, null, Projectile.NewProjectile(projectile.GetSource_OnHit(target), projectile.Center, Vector2.Zero, ModContent.ProjectileType<Content.Projectiles.Magic.LargeEmerald>(), projectile.damage * 2, projectile.knockBack, projectile.owner));
			else if(Main.myPlayer == projectile.owner) foreach(Projectile p in Main.ActiveProjectiles) if(p.type == ModContent.ProjectileType<Content.Projectiles.Magic.LargeEmerald>() && p.owner == projectile.owner && p.ai[0] < 25f) {
				p.ai[0]++;
				NetMessage.SendData(27, -1, -1, null, p.whoAmI);
			}
			if(projectile.type == 126) {
				int manaHeal = Main.rand.Next(5, 11);
				Main.player[projectile.owner].ManaEffect(manaHeal);
				Main.player[projectile.owner].statMana += manaHeal;
			}
		}
	}
}