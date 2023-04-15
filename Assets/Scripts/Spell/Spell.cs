using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    public virtual void Initialize(Skill skill,Entity owner=null,Entity target=null)
    {

    }
}
