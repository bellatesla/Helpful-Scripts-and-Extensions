using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public static class FavColor
{
	//Add as many as you want
    public static Color pink
    {
        get { return HexToColor("F959E7FF"); }
    }
    public static Color hotpink
    {
        get { return HexToColor("FF44EEFF"); }
    }
    public static Color yellow
    {
       get { return HexToColor("FFE200FF"); } 
    }
    public static Color teal
    {
        get { return HexToColor("00FFAAFF"); }
    }
    public static Color chartreuse
    {
        get { return HexToColor("CCEE88FF"); }
    }
    public static Color purple
    {
        get { return HexToColor("662266FF"); }
    }
    public static Color grass
    {
        get { return HexToColor("CC66CCFF"); }
    }

    public static string rarityResults { get; private set; }
    /// <summary>
    /// Basic Dice Roll returs RarityColor
    /// </summary>
    /// <returns></returns>
    public static Color GetRarityColor()
    {
        Color finalColor = Color.white;
        int result = RollDice(1, 20);
        rarityResults = String.Empty;

        if (result < 11)
        {
            rarityResults = "White - I rolled a " + result;
        }
        //Reversed order method
        else if (result >= 20)
        {
            rarityResults = "Yellow - I rolled a " + result;
            finalColor = Color.yellow;
        }
        else if (result >= 18)
        {
            rarityResults = "Magenta - I rolled a " + result;
            finalColor = Color.magenta;
        }
        else if (result >= 15)
        {
            rarityResults = "pink - I rolled a " + result;
            finalColor = Color.blue;
        }
        else if (result >= 11)
        {
            rarityResults = "Green - I rolled a " + result;
            finalColor = Color.green;
        }

        return finalColor;
    }
    /// <summary>
    /// Use RollDice(3,6) to roll 3 dice of six sides
    /// </summary>
    public static int RollDice(int rolls, int sides)
    {
        float final = 0;
        for (int i = 0; i < rolls; i++)
        {
            final += Random.value * sides;
        }
        return Mathf.FloorToInt(final + 1);
    }
    /// <summary>
    /// Convert a RGBA to HEX string color. 
    /// </summary>
    public static string ColorToHex(this Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
        return hex;
    }
    /// <summary>
    /// Convert a Hex string to RGBA color. Call myColor = HexToColor(F959E7FF);
    /// </summary>
    public static Color32 HexToColor(this string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, a);
    }
}
