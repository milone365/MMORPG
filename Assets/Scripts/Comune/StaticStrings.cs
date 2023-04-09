using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticStrings 
{
    public static string vertical = "Vertical";
    public static string scroll = "Mouse ScrollWheel";
    public static string mouseX = "Mouse X";
    public static string horizontal = "Horizontal";
    public static string player = "Player";
    public static string enemy = "Enemy";
    public static string follow= "CameraFollow";
    public static string move = "move";
    public static string dead = "dead";
    public const string stamina = "stamina", strenght = "strenght";
    public const string intellect="intellect", agility="agility";
    public const string armor="armor";
    public const string hp = "hp";
    public const string mana = "mana";

}

public static class Helper
{
    public static Color GetColor(Rarety rare)
    {
        Color c=Color.white;
        switch (rare)
        {
            case Rarety.comune:
                c = Color.gray;
                break;
            case Rarety.good:
                c = Color.green;
                break;
            case Rarety.rare:
                c = Color.blue;
                break;
            case Rarety.epic:
                c = Color.magenta;
                break;
            case Rarety.legendary:
                c = Color.cyan;
                break;
        }
        //c = new Color(c.r, c.g, c.b, 1);
        return c;
    }
}