using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonAction : MonoBehaviour
{
    [SerializeField]
    Text text = null;
    [SerializeField]
    GameObject onOff = null;
    [SerializeField]
    Image icon = null;
    [SerializeField]
    Image chargeImage = null;

    public KeyCode code = KeyCode.Z;
    ActionManager actionManager;

    public void INIT(ActionClass action,ActionManager man)
    {
        this.code = action.code;
        text.text = code.ToString();
        actionManager = man;
        if(action.skill!=null)
        {
            icon.sprite = action.skill.spirte;
        }
    }

    public void Push()
    {
        actionManager.OnPressButton(code);
    }
}
