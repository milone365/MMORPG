using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuryAttack : Spell
{
    [SerializeField]
    Percentage percentage = Percentage.ten;
    [SerializeField]
    string parameter = StaticStrings.agility;
    public override void Initialize(Skill skill, Entity owner = null, Entity target = null)
    {
        spellPower = Helper.GetParameter(owner, parameter);
        spellPower += skill.spellPower;
        Collider[] colliders = Physics.OverlapSphere(transform.position, skill.radius);
        switch (skill.spellTarget)
        {
            case SpellTarget.self:
                allTargets.Add(owner);
                break;
            case SpellTarget.friend:
            case SpellTarget.enemy:
                allTargets.Add(target);
                break;
            case SpellTarget.friendsArea:
                foreach (var c in colliders)
                {
                    if (c.tag == StaticStrings.player)
                    {
                        var player = c.GetComponent<Player>();
                        if (player == null) continue;
                        if (!player.isDeath())
                        {
                            allTargets.Add(player);
                        }
                    }
                }
                break;
            case SpellTarget.enemiesArea:
                foreach (var c in colliders)
                {
                    if (c.tag == StaticStrings.enemy)
                    {
                        var enemy = c.GetComponent<Enemy>();
                        if (enemy == null) continue;
                        if (!enemy.isDeath())
                        {
                            allTargets.Add(enemy);
                        }
                    }
                }
                break;
        }
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