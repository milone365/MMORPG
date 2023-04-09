using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOverTime : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Text text = null;
    public void Init(string s,float time)
    {
        text.text = s;
        Invoke("Disable", time);
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }
}
