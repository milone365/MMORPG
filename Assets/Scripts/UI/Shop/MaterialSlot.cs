using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MaterialSlot : MonoBehaviour
{
    [SerializeField]
    Text text = null;
    [SerializeField]
    Image icon = null;

    public void SetUp(Sprite s,string text)
    {
        icon.sprite = s;
        this.text.text = text;
    }
}
