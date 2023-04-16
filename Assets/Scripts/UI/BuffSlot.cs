using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuffSlot : MonoBehaviour
{
    [SerializeField]
    Text spellText = null;
    [SerializeField]
    Image icon = null;
    float counter = 0;

    public void Init(Sprite sprite,float activeTime)
    {
        icon.sprite = sprite;
        counter = activeTime;
        Destroy(gameObject, activeTime);
    }
    void Update()
    {
        if (spellText == null) return;
        counter -= Time.deltaTime;
        spellText.text = ((int)counter).ToString();
    }
}
