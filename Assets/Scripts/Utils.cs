using UnityEngine;

public static class Utils
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
    public static Color32 FirstPlaceColor => new Color32(0x33 , 0xcc , 0xcc, 0xFF);
    public static Color32 AnotherPlaceColor => new Color32(0xe5 , 0xc6 , 0x10, 0xFF);
}
