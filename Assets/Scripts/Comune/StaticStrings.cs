using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticStrings 
{
    public const string vertical = "Vertical";
    public const string scroll = "Mouse ScrollWheel";
    public const string mouseX = "Mouse X";
    public const string horizontal = "Horizontal";
    public const string player = "Player";
    public const string enemy = "Enemy";
    public const string follow= "CameraFollow";
    public const string move = "move";
    public const string dead = "dead";
    public const string stamina = "stamina", strenght = "strenght";
    public const string intellect="intellect", agility="agility";
    public const string armor="armor";
    public const string hp = "hp";
    public const string mana = "mana";
    public const string teleport = "teleport";
    public const string resurrection = "Resurrection";
}

public static class Effects
{
    public const string healing = "Heal";
    public const string aura = "aura";
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