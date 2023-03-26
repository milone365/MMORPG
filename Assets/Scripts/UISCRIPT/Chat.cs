using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    [SerializeField]
    InputField field = null;
    [SerializeField]
    Button sendBtn = null, cancelBtn;
    [SerializeField]
    Text textPrefab = null;
    [SerializeField]
    RectTransform content = null;

    public void Send()
    {

    }

    public void CancelText()
    {
        field.text = "";
    }
}
