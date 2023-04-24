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
        spellPower *= (1 + (int)(Helper.GetPercent(percentage)) / 100);
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
    one,
    two,
    thre,
    five,
    ten,
    twenty,
    thirty,
    fifty,
    senventy,
    hundred,
    twohundred,
    threehundred,
    fourhundred,
    fivehundred,
    MAX
}