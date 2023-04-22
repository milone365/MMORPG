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
    public const string inAction="inAction";
}

public static class Effects
{
    public const string healing = "Heal";
    public const string aura = "aura";
    public const string DamagePopUp = "DamagePopUp";
    public const string HealPopUp = "HealPopUp";
    public const string LevelUp = "LevelUp";
    public const string Slash = "Slash";
    
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

    public static int GetParameter(Entity owner,string param)
    {
        int val = 0;
        Player player = owner as Player;
        if(player!=null)
        {
            switch (param)
            {
                case StaticStrings.strenght:
                    val += player.GetInventory().GetParameter(param) + 
                        player.data.stat.Strenght + player.strengtBonus.GetBonus();
                    break;
                case StaticStrings.agility:
                    val += player.GetInventory().GetParameter(param) +
    player.data.stat.Agility + player.agilityBonus.GetBonus();
                    break;
                case StaticStrings.intellect:
                    val += player.GetInventory().GetParameter(param) +
    player.data.stat.Intellect + player.intellectBonus.GetBonus();
                    break;
                case StaticStrings.armor:
                    val += player.GetInventory().GetParameter(param) + player.armorBonus.GetBonus();
                    break;
            }
        }
        else
        {
            Enemy enemy = owner as Enemy;
            if(enemy!=null)
            {
                switch (param)
                {
                    case StaticStrings.strenght:
                        val += enemy.GetParameter(param) +
                            enemy.stats.Strenght + enemy.strengtBonus.GetBonus();
                        break;
                    case StaticStrings.agility:
                        val += enemy.GetParameter(param) +
        enemy.stats.Agility + enemy.agilityBonus.GetBonus();
                        break;
                    case StaticStrings.intellect:
                        val += enemy.GetParameter(param) +
        enemy.stats.Intellect + enemy.intellectBonus.GetBonus();
                        break;
                    case StaticStrings.armor:
                        val += enemy.GetParameter(param) + enemy.armorBonus.GetBonus();
                        break;
                }
            }
        }
        if(val<=0)
        {
            val = 1;
        }
        
        return val;
    }

    public static void GoNextLevel(ref SaveData data)
    {
        while(data.stat.Level<100 && data.experience>=GetNextLevelExperience(data.stat.Level))
        {
            int toRemove = GetNextLevelExperience(data.stat.Level);
            if (data.experience>=toRemove)
            {
                data.experience -= toRemove;
                data.stat.Level++;
                data.stat.Stamina++;
                data.stat.Strenght++;
                data.stat.Intellect++;
                data.stat.Agility++;
            }
        }
    }

    static int GetNextLevelExperience(int level)
    {
        var val = level * 1.33 * 100;
        return (int)val;
    }
}