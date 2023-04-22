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
    public Bonus strengtBonus = new Bonus();
    public Bonus intellectBonus = new Bonus();
    public Bonus agilityBonus = new Bonus();
    public Bonus armorBonus = new Bonus();

    public Transform spawnPoint = null;

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
        if(dmg<=0)
        {
            dmg = 1;
        }
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
            var damage= dmg -= Helper.GetParameter(this, StaticStrings.armor);
            if (damage <= 1) damage = 1;
            hp -= damage;

            view.RPC("SpawnPopUpRpc", RpcTarget.All, -damage);
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
    
    public virtual void BecameSpellTarget(Skill skill,Entity owner=null)
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

    [PunRPC]
    public void SpawnPopUpRpc(int amount)
    {
        GameObject gameObject = null;
        float x = Random.Range(-1.5f, 1.5f);
        float z = Random.Range(-1.5f, 1.5f);
        float offset = 0.5f;
        Vector3 pos=transform.position + new Vector3(x,offset,z);
        Quaternion rot = Camera.main.transform.rotation;
        if(amount<0)
        {
            gameObject = Instantiate(WorldManager.instance.GetPrefab(Effects.DamagePopUp),pos,rot);
        }
        else
        {
            gameObject = Instantiate(WorldManager.instance.GetPrefab(Effects.HealPopUp),pos,rot);
        }
        if(gameObject!=null)
        {
            gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = amount.ToString();
        }
    }

}


[System.Serializable]
public class Bonus
{
    public int bonus = 0;
    public int malus = 0;

    public int GetBonus()
    {
        return bonus - malus;
    }
}