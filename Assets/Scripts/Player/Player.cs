using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
public class Player : Entity
{
    public SaveData data = new SaveData();
    CameraFollow follow;
    [SerializeField]
    float rotSpeed = 2;
    [SerializeField]
    float scrollAmount = 3;
    [SerializeField]
    float minZoom = 10, maxZoom = 120;
    [HideInInspector]
    public ActionController controller;
    const float second = 1;
    float manaCounter=1;
    public bool CanMove = true;
    [SerializeField]
    GameObject uiMan = null;
    [SerializeField]
    CharacterClass debugClass = CharacterClass.warrior;
    public bool inAction = false;
    public Inventory GetInventory()
    {
        return controller.inventory;
    }
    public Stats stats()
    {
        return data.stat;
    }
    [SerializeField]
    Transform LeftHolder = null, rightHolder = null;
    public GameObject GetModel(Transform t)
    {
        if (t.childCount < 1) return null;
        return t.GetChild(0).gameObject;
    }
    public List<Pair<Talent,int>> talentList = new List<Pair<Talent,int>>();
    Talent[] allTalents;
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
        LoadTalents();
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
        if (inAction) return;
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

    void LoadTalents()
    {
        allTalents = Resources.LoadAll<Talent>("Talents").Where(x => x.charClass == data.stat.charClass).ToArray();
        if (data.talentList.Count < 1)
        {
            foreach (var a in allTalents)
            {
                data.talentList.Add(new Pair<string, int>() { key = a.name, value = 0 });
            }
            
        }
        foreach (var a in allTalents)
        {
            for(int i=0;i<data.talentList.Count;i++)
            {
                if(a.name==data.talentList[i].key)
                {
                    talentList.Add(new Pair<Talent, int>() { key = a, value = data.talentList[i].value });
                }
            }
        }

    }

    public void UpdateTalentList()
    {
        talentList.Clear();
        foreach (var a in allTalents)
        {
            for (int i = 0; i < data.talentList.Count; i++)
            {
                if (a.name == data.talentList[i].key)
                {
                    talentList.Add(new Pair<Talent, int>() { key = a, value = data.talentList[i].value });
                }
            }
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
        stamina += Helper.TalentAmount(talentList,BonusTarget.stamina);
        intellect += Helper.TalentAmount(talentList, BonusTarget.intellect);
        CalculateStats(stamina,intellect);
        if(hp>maxHp)
        {
            hp = maxHp;
        }
        if(controller.mana>maxHp)
        {
            controller.mana = maxMana;
        }
        UIManager.instance.UpdateHP(hp, maxHp);
        UIManager.instance.UpdateMana(controller.mana, maxMana);
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
        {
            view.RPC("SpawnPopUpRpc", RpcTarget.All, heal);
            view.RPC("SyncronizeStat", RpcTarget.All, hp, maxHp);
        }
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

    public void AddExperience(int experience)
    {
        view.RPC("AddExperienceRpc", RpcTarget.All, experience);
    }

    [PunRPC]
    public void AddExperienceRpc(int experience)
    {
        if (!photonView.IsMine) return;

        data.experience += experience;
        Helper.GoNextLevel(ref data);
        UIManager.instance.SetUpPlayer(this);
        Vector3 pos = transform.position + new Vector3(0, -1, 0);
        Vector3 rot = new Vector3(-90, 0, 0);
        WorldManager.instance.SpawnEffect(Effects.LevelUp, pos, rot);
        OnChangeItem();
        UpdateUI(hp, maxHp);
    }

    public void ChangeWeapon(Equip weapon,bool isLeft)
    {
        if (weapon == null) return;
        if (weapon.model == null) return;
        view.RPC("ChangeWeaponRpc", RpcTarget.AllBuffered,weapon.name, isLeft);
        controller.CalculateAttack();
    }
    
    [PunRPC]
    public void ChangeWeaponRpc(string ItemName,bool isLeft)
    {
        var weapon = WorldManager.instance.GetEquip(ItemName);
        Transform holder = (isLeft == true) ? LeftHolder : rightHolder;
        GameObject oldWeapon = GetModel(holder);
        if (oldWeapon != null)
        {
            Destroy(oldWeapon);
        }
        GameObject newWeapon = Instantiate(weapon.model, holder);
        newWeapon.transform.localPosition = (isLeft == true) ? weapon.leftPos : weapon.rightPos;
        newWeapon.transform.localRotation = Quaternion.Euler((isLeft == true) ? weapon.leftRot : weapon.rightRot);
    }

}
