using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActionButton : MonoBehaviour
{
    [SerializeField]
    Image icon = null;
    [SerializeField]
    Image countdownImage = null, onOffimage = null;
    [SerializeField]
    Text buttontext = null;
    KeyCode code=KeyCode.None;
    ActionController controller;
    public void SetUpButton(ActionController controller,KeyCode key)
    {
        this.code = key;
        this.controller = controller;
        buttontext.text = key.ToString();
    }

    public void Pressed()
    {
        controller.PressButton(code);
    }
}
