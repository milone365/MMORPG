using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Entity : MonoBehaviourPun
{
    public AnimatorSync sync;
    public Rigidbody rb;
    public float moveSpeed = 3;
    public float moveMultipler = 1;
    public bool isDeath()
    {
        return (hp <= 0);
    }
    
    [SerializeField]
    protected int hp = 10;
    protected PhotonView view;
    public System.Action OnDeathEvent;
    public int maxHp;
    public int maxMana;
    public int hpMultipler = 2;
    public int manaMultipler = 2;
    [SerializeField]
    protected GameObject localUI = null;
    [SerializeField]
    protected UnityEngine.UI.Text nameText = null;
    [SerializeField]
    protected UnityEngine.UI.Slider localhpBar = null;
    [SerializeField]
    GameObject circle = null;
    [SerializeField]
    BuffSlot slotPrefab = null;
    [SerializeField]
    Transform grid = null;

    void Start()
    {
        Init();
    }
    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false) return;
        Tick();
    }
    public virtual void Init()
    {
        sync = GetComponentInChildren<AnimatorSync>();
        sync.Init();
        rb = GetComponent<Rigidbody>();
        view = PhotonView.Get(this);
    }
    public virtual void Tick()
    {

    }
    public void TakeDamage(int dmg)
    {
        if(!PhotonNetwork.IsConnected)
        {
            DebugDamage(dmg);
        }
        else
        {
            if(view==null) view = PhotonView.Get(this);
            view.RPC("DealDamage", RpcTarget.All, dmg);
        }

    }
    void DebugDamage(int dmg)
    {
        if(photonView.IsMine)
        {
            hp -= dmg;
            if (hp <= 0)
            {
                hp = 0;
                sync.IsDead(true);
                if (OnDeathEvent != null)
                {
                    OnDeathEvent.Invoke();
                }
            }
            UpdateUI(hp,maxHp);
        }
    }
    [PunRPC]
    public void DealDamage(int dmg)
    {
        if(photonView.IsMine)
        {
            hp -= dmg;
            if (hp <= 0)
            {
                hp = 0;
                sync.IsDead(true);
                if (OnDeathEvent != null)
                {
                    OnDeathEvent.Invoke();
                }
            }
            view.RPC("SyncronizeStat", RpcTarget.All, hp,maxHp);
        }
    }
    [PunRPC]
    public void SyncronizeStat(int hp,int max)
    {
        this.hp = hp;
        this.maxHp = max;
        UpdateUI(this.hp,max);
    }
    public void CalculateStats(int stamina,int intellect)
    {
        maxHp = stamina * hpMultipler;
        maxMana = intellect * manaMultipler;
    }
    public virtual void UpdateUI(int hp,int maxHp)
    {
        
    }
    public virtual void Healing(int heal)
    {

    }

    public void ShowMarker(bool val)
    {
        if(circle!=null)
        {
            circle.SetActive(val);
        }
    }
    
    public void BecameSpellTarget(Skill skill)
    {
        view.RPC("SpellTargetRpc", RpcTarget.All, skill.effectName,skill.activationTime,
            WorldManager.instance.VectorConverter(skill.effectOffset),skill.sprite.name);
    }

    [PunRPC]
    public void SpellTargetRpc(string effectName,float lifetime,float[] posOffset,string sprite)
    {
        TargetSpellCustom(lifetime, sprite);
        var prefab = WorldManager.instance.GetPrefab(effectName);
        if (prefab == null) return;
        var gameobjet= Instantiate(prefab, transform);
        gameobjet.transform.localPosition = Vector3.zero + WorldManager.instance.ToVector(posOffset);
        var slot = Instantiate(slotPrefab, grid);
        slot.Init(WorldManager.instance.GetSprite(sprite), lifetime);
        Destroy(gameobjet, lifetime);
    }

    public virtual void TargetSpellCustom(float lifetime,string sprite)
    {

    }
}
