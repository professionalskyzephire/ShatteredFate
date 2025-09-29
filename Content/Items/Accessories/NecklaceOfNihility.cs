using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ShatteredFate.Content.Items.Accessories
{
	public class NecklaceOfNihility : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 1;
			if(Main.netMode == 2) return;
			int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
			int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
			int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
			ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
			ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
			ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
			ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
		}
		public override void Load() {
			if(Main.netMode == 2) return;
			EquipLoader.AddEquipTexture(Mod, ($"{Texture}_{EquipType.Head}").Replace(Name, "Nig"), EquipType.Head, this);
			EquipLoader.AddEquipTexture(Mod, ($"{Texture}_{EquipType.Body}").Replace(Name, "Nig"), EquipType.Body, this);
			EquipLoader.AddEquipTexture(Mod, ($"{Texture}_{EquipType.Legs}").Replace(Name, "Nig"), EquipType.Legs, this);
		}
		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 1;
			Item.accessory = true;
			Item.vanity = true;
			Item.hasVanityEffects = true;
			Item.rare = ModContent.RarityType<Content.Rarities.SFDevItem>();
		}
		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<SFPlayer>().NecklaceOfNihility = !hideVisual;
		public override void UpdateVanity(Player player) => player.GetModPlayer<SFPlayer>().NecklaceOfNihility = true;
	}
}