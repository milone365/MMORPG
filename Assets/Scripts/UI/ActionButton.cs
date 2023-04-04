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
    ActionClass action;
    ActionController controller;
    float countDown = 1;
    public bool Charging()
    {
        return (countdownImage.fillAmount>0);
    }
    public void SetUpButton(ActionController controller,ActionClass action)
    {
        this.action = action;
        this.controller = controller;
        buttontext.text = this.action.key.ToString();
        var skill = action.skill;
        if(skill!=null)
        {
            icon.sprite = skill.sprite;
            countDown = skill.countDown;
        }
    }

    public void Pressed()
    {
        controller.PressButton(action);
    }

    public void SetCountDown()
    {
        countdownImage.fillAmount = 1;
    }

    public void FadeCheck()
    {
        if (action.skill == null) return;

        if(countdownImage.fillAmount>0)
        {
            countdownImage.fillAmount -= 1 / countDown * Time.deltaTime;
        }

        if(action.skill.cost<=controller.mana)
        {
            onOffimage.enabled = false;
        }
        else
        {
            onOffimage.enabled = true;
        }
    }

    public void SetSkill(Skill skill)
    {
        bool exist = false;
        foreach(var a in controller.actions)
        {
            if(a.skill==skill)
            {
                exist = true;
            }
        }
        if (exist) return;
        action.skill = skill;
        countDown = skill.countDown;
        countdownImage.fillAmount = 0;
        icon.sprite = skill.sprite;
    }
}
