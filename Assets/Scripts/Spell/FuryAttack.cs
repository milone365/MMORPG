using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuryAttack : Spell
{
    [SerializeField]
    Percentage percentage = Percentage.ten;
    public override void Initialize(Skill skill, Entity owner = null, Entity target = null)
    {
        base.Initialize(skill, owner, target);
        spellPower *= (1 + (int)percentage / 100);
        foreach(var t in allTargets)
        {
            t.TakeDamage(spellPower);
            WorldManager.instance.SpawnEffect(skill.effectName, owner.spawnPoint.position,
                owner.spawnPoint.rotation.eulerAngles);
        }
        Destroy(gameObject);
    }
}

public enum Percentage
{
    one=1,
    two=2,
    three=3,
    five=5,
    ten=10,
    twenty=20,
    thirty=30,
    fifty=50,
    senventy=70,
    hundred=100,
    twohundred=200,
    threehundred=300,
    fourhundred=400,
    fivehundred=500
}