using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Talent",menuName ="ScriptableObject/Talent")]
public class Talent : ScriptableObject
{
    public Percentage percent = Percentage.one;
    public BonusTarget target;
    public int maxLevel = 1;
    public Sprite sprite;
    public string description;
    public CharacterClass charClass = CharacterClass.warrior;
    public string talentName = "";
}

public enum BonusTarget
{
    stamina, strenght,
    intellect, agility,
    armor,healingSpell,
    
}