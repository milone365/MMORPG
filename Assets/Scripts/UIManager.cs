using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField]
    GameObject loadingScreen = null;
    [SerializeField]
    GameObject startPanel = null;
    private void Awake()
    {
        instance = this;
    }

    public void ShowLoading(bool val)
    {
        loadingScreen.SetActive(val);
    }

    public void CloseStartScreen()
    {
        startPanel.SetActive(false);
    }
}
