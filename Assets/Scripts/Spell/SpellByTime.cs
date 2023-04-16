using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellByTime : Spell
{
    [SerializeField]
    SpellType spellType = SpellType.buff;
    public override void Initialize(Skill skill, Entity owner = null, Entity target = null)
    {
        base.Initialize(skill, owner, target);
        foreach(var item in allTargets)
        {
            item.BecameSpellTarget(skill);
        }
        Destroy(gameObject, skill.activationTime);
    }

    private void Update()
    {
        counter += Time.deltaTime;
        if(counter>=second)
        {
            counter = 0;
            ApllyEffect();
        }
    }

    void ApllyEffect()
    {
        foreach(var item in allTargets)
        {
            if (item == null) continue;
            switch (spellType)
            {
                case SpellType.buff:
                    item.Healing(spellPower);
                    break;
                case SpellType.curse:
                    item.TakeDamage(spellPower);
                    break;
            }
        }
    }

}

public enum SpellType
{
    buff,
    curse
}