using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : Spell
{

    public override void Initialize(Skill skill, Entity owner = null, Entity target = null)
    {
        base.Initialize(skill,owner,target);
        switch (skill.spellTarget)
        {
            case SpellTarget.self:
                owner.Healing(spellPower);
                WorldManager.instance.SpawnEffect(Effects.healing, owner.transform.position, Vector3.zero);
                break;
            case SpellTarget.friend:
                if (target == null) return;
                if(!target.isDeath())
                {
                    target.Healing(spellPower);
                    WorldManager.instance.SpawnEffect(Effects.healing, target.transform.position, Vector3.zero);
                }
                break;
            case SpellTarget.friendsArea:
                Collider[] colliders = Physics.OverlapSphere(owner.transform.position, skill.radius);
                foreach(var c in colliders)
                {
                    if(c.tag==StaticStrings.player)
                    {
                        var Player = c.GetComponent<Player>();
                        c.GetComponent<Player>().Healing(spellPower);
                        WorldManager.instance.SpawnEffect(Effects.healing, c.transform.position, Vector3.zero);
                    }
                }    
                break;
        }
    }
}
