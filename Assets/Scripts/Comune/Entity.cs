using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
public class Entity : MonoBehaviourPun, Selectable
{
    public Stats stats=new Stats();
    protected Animator anim;
    protected PhotonView view;
    [SerializeField]
    protected LocalUI ui = null;
    public  int hp = 10;
    public bool isDeath=false;
    public Action onDeathEvent;


    [PunRPC]
    public void Resurrect()
    {
        isDeath = false;
        hp = stats.maxHp;
        anim.SetBool(Helper.death, false);
        if(ui!=null)
        {
            ui.UpdaBar(hp);
        }
    }

    public void Respawn(Vector3 pos)
    {
        transform.position = pos;
        if (PhotonNetwork.IsConnected)
        {
            if (view == null)
            {
                view = PhotonView.Get(this);
            }
            view.RPC(Helper.resurrect, RpcTarget.All);
        }
    }

    [PunRPC]
    public void PlayAnimation(string s)
    {
        anim.Play(s);
    }

    [PunRPC]
    public void DamageReciver(int dmg)
    {
        if (isDeath) return;
        hp -= dmg;
        view.RPC(Helper.sycronizeStat, RpcTarget.All, hp);
        if (hp <= 0)
        {
            if (PhotonNetwork.IsConnected)
            {
                view.RPC(Helper.playAnim, RpcTarget.All, Helper.death);
            }
        }
    }

    [PunRPC]
    public void SyncronizeStat(int current)
    {
        hp = current;
        if (hp <= 0)
        {
            hp = 0;
            isDeath = true;
            anim.SetBool(Helper.death, true);
            if(onDeathEvent!=null)
            {
                onDeathEvent.Invoke();
            }
        }
        if(ui!=null)
            ui.UpdaBar(hp);
    }


    public void TakeDamage(int dmg)
    {
        if (PhotonNetwork.IsConnected)
        {
            if(view==null)
            {
                view = PhotonView.Get(this);
            }
            view.RPC(Helper.damageRecieve, RpcTarget.MasterClient, dmg);
        }
        else
        {
            DebugDamage(dmg);
        }
    }

    void DebugDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            hp = 0;
            isDeath = true;
            anim.SetBool(Helper.death, true);
            anim.Play(Helper.death);
        }
        
        if(ui!=null)
          ui.UpdaBar(hp);
    }
    public void Select()
    {

    }
}
