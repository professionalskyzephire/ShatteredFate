using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace ShatteredFate.ModUtils;

public static class UIUtils {
    public static float GetProgress(int startPos, int endPos, bool reflect = false) {
        float progress = (float)endPos <= 0 ? 1f : (float)startPos / (float)endPos;
        progress = MathHelper.Clamp(progress, 0f, 1f);
        if (reflect) { progress = 1f - progress; }
        return progress;
    }
    public static string[] ClearText(string[] arr) {
        for (int i = 0; i < arr.Length; i++) {
            for (int j = 0; j < arr[i].Length; j++) {
                if (arr[i][j] == '[' && arr[i][j + 1] == 'c' && arr[i][j + 2] == '/') {
                    int index = j;
                    int count = 0;
                    for (int k = 0; arr[i][j + k] != ':'; k++) { count = k + 1; }
                    arr[i] = arr[i].Remove(index, count + 1);
                    for (int k = j; arr[i][k] != ']'; k++) { index = k + 1; }
                    arr[i] = arr[i].Remove(index, 1);
                }
            }
        }
        return arr;
    }
    public static string GetButtonName(ModKeybind key) => key.GetAssignedKeys().Count > 0 ? key.GetAssignedKeys()[0] : Loc.Get("Keybinds.NotKey");
};