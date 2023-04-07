using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class PopUpBase : MonoBehaviour
{
    [SerializeField]
    Button yes = null, no = null;
    [SerializeField]
    Text title = null;

    public void Init(string title="",UnityAction onClickYes=null, UnityAction onClickNo=null)
    {
        this.title.text = title;
        yes.onClick.AddListener(onClickYes);
        no.onClick.AddListener(onClickNo);
    }
}
