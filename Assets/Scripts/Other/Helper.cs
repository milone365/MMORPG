using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Helper
{
    
    public static Color GetColor(Rarety rare)
    {
        Color c = Color.white;
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

    public static int GetParameter(Entity owner, string param)
    {
        int val = 0;
        Player player = owner as Player;
        if (player != null)
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
            if (enemy != null)
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
        if (val <= 0)
        {
            val = 1;
        }

        return val;
    }

    const int talentFrequency = 5;
    public static void GoNextLevel(ref SaveData data)
    {
        while (data.stat.Level < 100 && data.experience >= GetNextLevelExperience(data.stat.Level))
        {
            int toRemove = GetNextLevelExperience(data.stat.Level);
            if (data.experience >= toRemove)
            {
                data.experience -= toRemove;
                data.stat.Level++;
                data.stat.Stamina++;
                data.stat.Strenght++;
                data.stat.Intellect++;
                data.stat.Agility++;
                if (data.stat.Level % talentFrequency == 0)
                {
                    data.talentPoint++;
                }
            }
        }
    }

    static int GetNextLevelExperience(int level)
    {
        var val = level * 1.33 * 100;
        return (int)val;
    }

    public static int TalentAmount(List<Pair<Talent, int>> talentList, BonusTarget target)
    {
        int val = 0;
        var arr = talentList.Where(x => x.key.target == target).ToList();
        if (arr.Count < 1) return 0;

        foreach (var item in arr)
        {
            if (item.value == 0) continue;
            Percentage per = item.key.percent;
            if (item.value > 1)
            {
                per += item.value - 1;
            }

            val += GetPercent(per);
        }
        return val;
    }

    public static int GetPercent(Percentage per)
    {
        switch (per)
        {
            case Percentage.one:
                return 1;
            case Percentage.two:
                return 2;
            case Percentage.thre:
                return 3;
            case Percentage.five:
                return 4;
            case Percentage.ten:
                return 10;
            case Percentage.twenty:
                return 20;
            case Percentage.thirty:
                return 30;
            case Percentage.fifty:
                return 50;
            case Percentage.senventy:
                return 70;
            case Percentage.hundred:
                return 100;
            case Percentage.twohundred:
                return 200;
            case Percentage.threehundred:
                return 300;
            case Percentage.fourhundred:
                return 400;
            case Percentage.fivehundred:
                return 500;
        }

        return 0;
    }

    public static QuestData GetQuestData(List<QuestData>list,QuestData data)
    {
        foreach(var item in list)
        {
            if(item.questName==data.questName)
            {
                return item;
            }
        }
        return null;
    }

    public static Quest GetQuest(List<Quest> list, QuestData data)
    {
        foreach (var item in list)
        {
            if (item.data.questName == data.questName)
            {
                return item;
            }
        }
        return null;
    }
}
