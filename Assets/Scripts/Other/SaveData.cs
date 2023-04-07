using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData 
{
    public string characterName = "";
    public Stats stat = new Stats();
    public List<string> skills = new List<string>();
    public List<string> equip = new List<string>();
    public List<string> items = new List<string>();
}
