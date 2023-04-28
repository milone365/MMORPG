using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewSkill",menuName ="ScriptableObject/Skills")]
public class Skill : ScriptableObject
{
    public AnimName animName=AnimName.SpellCast;
    public Sprite sprite;
    public int requireLevel = 3;
    public int cost = 2;
    public string desctiption = "";
    public float countDown = 3;
    public float activationTime = 5;
    public float skillRange = 15;
    public SpellTarget spellTarget=SpellTarget.enemy;
    public int spellPower = 1;
    public float radius = 5;
    public Spell spellPrefab = null;
    public Vector3 effectOffset = new Vector3(0, -1, 0);
    public string effectName = "BlackAura";

    
}

public enum SpellTarget
{
    self,
    friend,
    enemy,
    friendsArea,
    enemiesArea
}

public enum AnimName
{
    AtkRight,
    AtkLeft,
    AtkDouble,
    Unarmed,
    BowShot,
    BlockingLoop,
    Buff,
    MeeleeAttack_OneHanded,
    MeeleeAttack_TwoHanded,
    SpellCast,
    SpinLoop_TwoHanded,
    SpinAttack_TwoWeapons,
    MiningLoop,
    Gathering
}