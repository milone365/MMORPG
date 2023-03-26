using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LocalUI : MonoBehaviour
{
    [SerializeField]
    Slider hpBar = null;

    int maxHp = 0;
    public void Init(int maxHp,int hp)
    {
        this.maxHp = maxHp;
        hpBar.maxValue = maxHp;
        UpdaBar(hp);
    }
    
    public void UpdaBar(int hp)
    {
        hpBar.value = hp;
    }
}
