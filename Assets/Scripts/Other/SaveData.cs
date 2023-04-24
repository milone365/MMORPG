using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData 
{
    public string characterName = "";
    public Stats stat = new Stats();
    public List<string> skills = new List<string>();
    public List<Pair<string, int>> equipSkills = new List<Pair<string, int>>();
    public List<Pair<string, int>> equip = new List<Pair<string, int>>();
    public List<string> items = new List<string>();
    public string LevelName = "Level1";
    public int currentLevel = 1;
    public float x, y, z;
    public int experience = 0;
    public int talentPoint=0;
    public List<Pair<string, int>> talentList = new List<Pair<string, int>>();
    public List<QuestData> activeQuestList = new List<QuestData>();
    public List<QuestData> completedQuestList = new List<QuestData>();
}
