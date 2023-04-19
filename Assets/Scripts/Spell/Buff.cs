using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : Spell
{
    [SerializeField]
    SpellType spellType = SpellType.buff;
    public int strenght, intellect, agility, armor;
    public override void Initialize(Skill skill, Entity owner = null, Entity target = null)
    {
        base.Initialize(skill, owner, target);
        foreach (var item in allTargets)
        {
            item.BecameSpellTarget(skill,owner);
        }
        ApllyEffect();
        Destroy(gameObject, skill.activationTime);
    }

    void ApllyEffect()
    {
        foreach (var item in allTargets)
        {
            if (item == null) continue;
            switch (spellType)
            {
                case SpellType.buff:
                    SetBuff(item);
                    break;
                case SpellType.curse:
                    SetCurse(item);
                    break;
            }
        }
    }
    void SetBuff(Entity target,bool add=true)
    {
        if(add==true)
        {
            target.strengtBonus.bonus += strenght;
            target.agilityBonus.bonus += agility;
            target.armorBonus.bonus += armor;
            target.intellectBonus.bonus += intellect;
        }
        else
        {
            target.strengtBonus.bonus -= strenght;
            target.agilityBonus.bonus -= agility;
            target.armorBonus.bonus -= armor;
            target.intellectBonus.bonus -= intellect;
        }

    }
    void SetCurse(Entity target, bool add = true)
    {
        if(add==true)
        {
            target.strengtBonus.malus += strenght;
            target.agilityBonus.malus += agility;
            target.armorBonus.malus += armor;
            target.intellectBonus.malus += intellect;
        }
        else
        {
            target.strengtBonus.malus -= strenght;
            target.agilityBonus.malus -= agility;
            target.armorBonus.malus -= armor;
            target.intellectBonus.malus -= intellect;
        }
    }

    private void OnDestroy()
    {
        switch (spellType)
        {
            case SpellType.buff:
                foreach (var item in allTargets)
                {
                    SetBuff(item,false);
                }
                break;
            case SpellType.curse:
                foreach (var item in allTargets)
                {
                    SetCurse(item,false);
                }
                break;
        }
    }
}
