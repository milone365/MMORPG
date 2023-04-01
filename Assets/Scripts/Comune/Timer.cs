using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer 
{
    public float counter=0;
    public float wait=1;

    public void StartTimer(float w=0)
    {
        if(w > 0)
        {
            wait = w;
        }
        counter = wait;
    }

    public bool timerActive(float delta)
    {
        if(counter>0)
        {
            counter -= delta;
            return true;
        }
        else
        {
            return false;
        }
    }

}
