using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewSkill",menuName ="ScriptableObjects/Skill")]
public class Skills :ScriptableObject
{
    public Sprite spirte;
    public AnimName animName;
}

public enum AnimName
{
    MeeleeAttack_OneHanded,
    Buff, BowShot, BlockingLoop,
    SpellCast, SpinToWin_OneHanded,
    MeeleeAttack_TwoHanded, SpinAttack_TwoWeapons,
    SpinLoop_TwoHanded
}
