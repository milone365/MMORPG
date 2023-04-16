using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    public List<Entity> allTargets = new List<Entity>();
    public float counter = 0;
    public const float second = 1;
    public int spellPower=1;
    public virtual void Initialize(Skill skill,Entity owner=null,Entity target=null)
    {
        Player p = owner as Player;
        spellPower = p.GetInventory().GetParameter(StaticStrings.intellect);
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
                foreach(var c in colliders)
                {
                    if(c.tag==StaticStrings.player)
                    {
                        var player = c.GetComponent<Player>();
                        if (player == null) continue;
                        if(!player.isDeath())
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
    }
}
