using Microsoft.Xna.Framework;
using ShatteredFate.Content.Items.Weapons.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ShatteredFate.Content.Tiles.Furniture;

public class CuriousCandleTile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileCut[Type] = false;
		Main.tileSolid[Type] = false;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileNoAttach[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateHeights = [20];
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.DrawYOffset = -4;
		TileObjectData.newTile.CoordinatePadding = 2;

		TileObjectData.newTile.Origin = new Point16(0, 0);

		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidWithTop | AnchorType.Table, TileObjectData.newTile.Width, 0);

		TileObjectData.addTile(Type);

		TileID.Sets.DisableSmartCursor[Type] = true;

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(125, 110, 113), name);

		RegisterItemDrop(ModContent.ItemType<CuriousCandle>(), null);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		float pulse = Main.rand.Next(28, 42) * 0.005f;
		pulse += (270 - Main.mouseTextColor) / 700f;
		r = 0.922f + pulse;
		g = 0.364f + pulse;
		b = 0.686f + pulse;
	}

	public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
	{
		if (visible && Main.rand.NextBool(2))
		{
			Dust dust = Dust.NewDustDirect(new Vector2(i * 16 + 1, j * 16 - 6), 8, 8, DustID.Enchanted_Pink, 0f, 0f, 0, default, Main.rand.NextFloat(0.7f, 1.1f));
			dust.velocity = new Vector2(0f, -Main.rand.NextFloat(0.1f, 0.5f)).RotatedByRandom(MathHelper.ToRadians(30));
			dust.noGravity = false;
			dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;
		}
	}
}