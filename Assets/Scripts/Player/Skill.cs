using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewSkill",menuName ="ScriptableObject/Skills")]
public class Skill : ScriptableObject
{
    public AnimName animName=AnimName.atk;
}

public enum AnimName
{
    atk,
    BowShot,
    BlockingLoop,
    Buff,
    MeeleeAttack_OneHanded,
    MeeleeAttack_TwoHanded,
    SpellCast,
    SpinLoop_TwoHanded,
    SpinAttack_TwoWeapons
}