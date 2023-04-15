using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : Spell
{

    public override void Initialize(Skill skill, Entity owner = null, Entity target = null)
    {
        Player player = owner as Player;
        int power = player.GetInventory().GetParameter(StaticStrings.intellect);
        power += skill.spellPower;
        switch (skill.spellTarget)
        {
            case SpellTarget.self:
                owner.Healing(power);
                break;
            case SpellTarget.friend:
                if (target == null) return;
                if(!target.isDeath())
                {
                    target.Healing(power);
                }
                break;
            case SpellTarget.friendsArea:
                Collider[] colliders = Physics.OverlapSphere(owner.transform.position, skill.radius);
                foreach(var c in colliders)
                {
                    if(c.tag==StaticStrings.player)
                    {
                        c.GetComponent<Player>().Healing(power);
                    }
                }    
                break;
        }
    }
}
