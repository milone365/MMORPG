using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : Entity
{
    CameraFollow follow;
    [SerializeField]
    float rotSpeed = 2;
    [SerializeField]
    float scrollAmount = 3;
    [SerializeField]
    float minZoom = 10, maxZoom = 120;
    ActionController controller;
    const float second = 1;
    float manaCounter=1;
    public SaveData data = new SaveData();
    public bool CanMove = true;
    [SerializeField]
    GameObject uiMan = null;
    [SerializeField]
    CharacterClass debugClass = CharacterClass.warrior;
    public Inventory GetInventory()
    {
        return controller.inventory;
    }
    public Stats stats()
    {
        return data.stat;
    }

    public override void Init()
    {
        base.Init();
        if (!photonView.IsMine) return;

        if (uiMan != null)
        {
            Instantiate(uiMan);
        }
        data = CharacterCreate.selectedData;
        if (data == null)
        {
            data = new SaveData();
            data.stat = CharacterCreate.GetStat(debugClass);
            data.characterName = "Gustav";
        }
        controller = GetComponent<ActionController>();
        controller.sync = sync;
        controller.Init(this);
        OnChangeItem();
        hp = maxHp;
        var f = Resources.Load<CameraFollow>(StaticStrings.follow);
        follow= Instantiate(f, transform.position, transform.rotation);
        follow.Init(transform);
        WorldManager.instance.playerList.Add(transform);
        UIManager.instance.SetUpPlayer(this);
        OnDeathEvent = () =>
          {
              UIManager.instance.deathPanel.SetActive(true);
          };
        if (view == null)
        {
            view = PhotonView.Get(this);
        }
        if(Photon.Pun.PhotonNetwork.IsConnected)
        {
            view.RPC("LocalBarUpdate", RpcTarget.AllBuffered, data.characterName, hp, maxHp);
        }
        localUI.SetActive(false);
    }

    public override void Tick()
    {
        if(controller.mana < maxMana)
        {
            manaCounter -= Time.deltaTime;
            if(manaCounter<=0)
            {
                manaCounter = second;
                controller.mana += stats().manaXsecond;
                if (controller.mana > maxMana) controller.mana = maxMana;
                UIManager.instance.UpdateMana(controller.mana, maxMana);
            }
        }
        controller.MouseLeft();
        if(!CanMove)
        {
            return;
        }
        UseCamera();
        if (isDeath()) return;
        float x = Input.GetAxisRaw(StaticStrings.horizontal);
        float y = Input.GetAxisRaw(StaticStrings.vertical);
        Vector3 move = (transform.right * x) + (transform.forward * y);
        move *= Time.deltaTime * moveMultipler * moveSpeed;
        move.y = rb.velocity.y;
        rb.velocity = move;
        sync.Move(x, y);
        controller.Tick(follow.transform,x,y);
    }

    void UseCamera()
    {
        float scroll = Input.GetAxisRaw(StaticStrings.scroll);
        if (scroll != 0)
        {
            float val = scrollAmount * scroll;
            val += follow.cam.fieldOfView;
            val = Mathf.Clamp(val, minZoom, maxZoom);
            follow.cam.fieldOfView = val;
        }
        if(Input.GetMouseButton(0))
        {
            float x = Input.GetAxisRaw(StaticStrings.mouseX);
            Vector3 rot = follow.transform.rotation.eulerAngles;
            follow.transform.rotation = Quaternion.Euler(rot.x, rot.y + x * rotSpeed, rot.z);

        }
    }

    
    public void Respawn(bool inPlace=false)
    {
        if(inPlace==false)
        {
            transform.position = WorldManager.instance.respawnPoint.position;
        }
        hp = maxHp;
        sync.IsDead(false);
        UIManager.instance.UpdateHP(hp, maxHp);
        if(Photon.Pun.PhotonNetwork.IsConnected)
        {
            view.RPC("SyncronizeStat", Photon.Pun.RpcTarget.All, hp,maxHp);
        }
        WorldManager.instance.SpawnEffect(Effects.aura, transform.position + new Vector3(0, -1, 0), new Vector3(-90, 0, 0));
    }
    
    public void OnChangeItem()
    {
        int stamina = stats().Stamina + controller.inventory.GetParameter(StaticStrings.stamina);
        int intellect=stats().Intellect+ controller.inventory.GetParameter(StaticStrings.intellect);
        CalculateStats(stamina,intellect);
        if(hp>maxHp)
        {
            hp = maxHp;
        }
        if(controller.mana>maxHp)
        {
            controller.mana = maxMana;
        }
    }
    public void LockPlayer()
    {
        CanMove = false;
        rb.velocity = Vector3.zero;
        sync.Move(0, 0);
    }
    public override void UpdateUI(int current,int max)
    {
        if (photonView.IsMine)
        {
            UIManager.instance.UpdateHP(current, max);
            if(view==null)
            {
                view = PhotonView.Get(this);
            }
            if (Photon.Pun.PhotonNetwork.IsConnected)
                view.RPC("LocalBarUpdate", RpcTarget.All,data.characterName,hp,maxHp);
        }
    }

    [PunRPC]
    public void LocalBarUpdate(string Name,int h,int m)
    {
        nameText.text = Name;
        localhpBar.maxValue = m;
        localhpBar.value = h;
    }

    public override void Healing(int heal)
    {
        if (isDeath()) return;
        hp += heal;
        if (hp > maxHp) hp = maxHp;
        if (view == null)
        {
            view = PhotonView.Get(this);
        }
        if (Photon.Pun.PhotonNetwork.IsConnected)
            view.RPC("SyncronizeStat", RpcTarget.All, hp, maxHp);
    }

    public void SendRequest(string Request)
    {
        if (Photon.Pun.PhotonNetwork.IsConnected)
            view.RPC("RequestRpc", RpcTarget.All, Request);
    }

    [PunRPC]
    public void RequestRpc(string request)
    {
        if (!photonView.IsMine) return;

        switch(request)
        {
            case StaticStrings.resurrection:
                UIManager.instance.ShowResurrectionRequest();
                break;
        }
    }

    public override void TargetSpellCustom(float lifetime, string sprite)
    {
        if(photonView.IsMine)
        {
            UIManager.instance.GenerateSlot(lifetime, sprite);
        }     
    }
}
