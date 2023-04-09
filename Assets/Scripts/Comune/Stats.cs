using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats 
{
    public int Level = 1;
    public int Stamina = 1;
    public int Strenght = 1;
    public int Agility = 1;
    public int Intellect = 1;
    public int manaXsecond = 5;
    public CharacterClass charClass = CharacterClass.warrior;
}

public enum CharacterClass
{
    warrior,
    mage,
    priest,
    paladin,
    shaman,
    druid,
    rogue,
    ranger,
    warlock
}