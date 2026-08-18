using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ShatteredFate.Common.ModSystems.Worlds;

public class ShadyFigureShop : ModSystem {
    readonly int[] _allShopItems = new int[10];
    readonly int[] _currentItems = new int[3];

    bool _isFilled = false;
    bool _isSetup = false;

    public int[] GetRegisterArray()  => _allShopItems;
    public int GetRegisterItem(int index) => _allShopItems[index];
    public void SetRegisterItem(int index, int item) => _allShopItems[index] = item;
    public int[] GetCurrentShopArray() => _currentItems;
    public int GetCurrentShopItem(int index) => _currentItems[index];
    public void SetCurrentShopItem(int index, int item) => _currentItems[index] = item;

    public bool GetFilledStatus() => _isFilled;
    public bool SetFilledStatus(bool value) => _isFilled = value;
    public bool GetSetupStatus() => _isSetup;
    public void SetSetupStatus(bool value) => _isSetup = value;

    public override void LoadWorldData(TagCompound tag) {
        int[] arr = tag.GetIntArray($"{SFMod.ModName}:ShopItems"); for (int i = 0; i < arr.Length; i++) { SetCurrentShopItem(i, arr[i]); }
   
        SetSetupStatus(tag.GetBool($"{SFMod.ModName}:SetupShopItem"));
    }
    public override void SaveWorldData(TagCompound tag) {
        tag[$"{SFMod.ModName}:ShopItems"] = GetCurrentShopArray();

        tag[$"{SFMod.ModName}:SetupShopItem"] = GetSetupStatus();
    }
    public override void NetSend(BinaryWriter writer) {
        writer.Write(GetRegisterArray().Length);
        foreach (int element in GetRegisterArray()) { writer.Write(element); }
        writer.Write(GetCurrentShopArray().Length);
        foreach (int element in GetCurrentShopArray()) { writer.Write(element); }

        writer.Write(GetSetupStatus());
    }
    public override void NetReceive(BinaryReader reader) {
        int count;

        count = reader.ReadInt32(); 
        for (int i = 0; i < count; i++) { SetRegisterItem(i, reader.ReadInt32()); }; 

        count = reader.ReadInt32();
        for (int i = 0; i < count; i++) { SetCurrentShopItem(i, reader.ReadInt32()); };
            
        SetSetupStatus(reader.ReadBoolean());
    }
    public override void PostUpdateWorld() {
        if (!GetFilledStatus()) {
            SetRegisterItem(0, 401);
            SetRegisterItem(1, 403);
            SetRegisterItem(2, 404);
            for (int i = 3; i < 10; i++) { SetRegisterItem(i, i); };
            SetFilledStatus(true);
        };
        if (!Main.dayTime && !GetSetupStatus()) {
            for (int i = 0; i < 3; i++) {
                bool alreadyThere = false;
                int num = Main.rand.Next(0, 10);

                for (int j = 0; j < i; j++) {
                    if (GetCurrentShopItem(j) == GetRegisterItem(num) || GetCurrentShopItem(j) == 0) {
                        alreadyThere = true;
                        break;
                    };
                };
                if (!alreadyThere) {
                    SetCurrentShopItem(i, GetRegisterItem(num));
                    if (GetCurrentShopItem(i) == 401 || GetCurrentShopItem(i) == 403 || GetCurrentShopItem(i) == 404) {
                        SetCurrentShopItem(0, 401);
                        SetCurrentShopItem(1, 403);
                        SetCurrentShopItem(2, 404);
                        break;
                    };
                };
            };
            SetSetupStatus(true);
        };
        if (Main.dayTime && GetSetupStatus()) { SetSetupStatus(false); }
    }
};