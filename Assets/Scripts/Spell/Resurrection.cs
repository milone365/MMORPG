using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resurrection : Spell
{
    public override void Initialize(Skill skill, Entity owner = null, Entity target = null)
    {
        Player player = null;
        if(skill.spellTarget==SpellTarget.self)
        {
            player = owner as Player;
            player.SendRequest(StaticStrings.resurrection);
            WorldManager.instance.SpawnEffect(Effects.healing, target.transform.position, Vector3.zero);
        }
        else
        {
            if (target == null) return;
            player = target as Player;
            if (player == null) return;
            player.SendRequest(StaticStrings.resurrection);
            WorldManager.instance.SpawnEffect(Effects.healing, target.transform.position, Vector3.zero);
        }
    }
}
