using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Entity : MonoBehaviourPun, Selectable
{
    public Stats stats=new Stats();
    protected Animator anim;
    protected PhotonView view;
    [SerializeField]
    protected LocalUI ui = null;
    public  int hp = 10;
    public bool isDeath=false;

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
        view.RPC("SyncronizeStat", RpcTarget.All, hp);
        if (hp <= 0)
        {
            if (PhotonNetwork.IsConnected)
            {
                view.RPC("PlayAnimation", RpcTarget.All, Helper.death);
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
        }
        if(ui!=null)
            ui.UpdaBar(hp);
    }


    public void TakeDamage(int dmg)
    {
        if (PhotonNetwork.IsConnected)
        {
            view.RPC("DamageReciver", RpcTarget.MasterClient, dmg);
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
