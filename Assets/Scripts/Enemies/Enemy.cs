using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class Enemy : Entity
{
    [SerializeField]
    string EnemyName = "Skeleton";
    [SerializeField]
    EnemyState estate = EnemyState.idle;
    Timer timer = new Timer();
    float delta;
    [SerializeField]
    Vector3 direction = new Vector3(0,0,5);
    Vector3 endPos;
    Vector3 startPosition;
    Vector3 destination;
    [SerializeField]
    float patrolDist = 1;
    float wait = 2;
    [SerializeField]
    bool staticEnemy = false;
    [SerializeField]
    float respawnTime = 60;
    [SerializeField]
    float actionRadius = 5;
    Player playerTarget;
    [SerializeField]
    float attackRange = 2;
    [SerializeField]
    float attackSpeed = 2;
    [SerializeField]
    float maxDistance = 15;
    public Stats stats = new Stats();
    public Equip head, body, leg, shoes, belt, shoulder;
    public Equip leftWeapon, rightWeapon;
    public int exp = 100;
    public List<Player> haveBattleList = new List<Player>();
    public List<Item> dropList = new List<Item>();
    [HideInInspector]
    public bool CanTakeDrop = true;
    public override void Init()
    {
        base.Init();
        CalculateStats(stats.Stamina + GetParameter(StaticStrings.stamina), stats.Intellect+ 
            GetParameter(StaticStrings.intellect));
        timer.StartTimer(wait);
        startPosition = transform.position;
        endPos = transform.position + direction;
        destination = endPos;
        OnDeathEvent = () =>
          {
              if(haveBattleList.Count>0)
              {
                  foreach(var item in haveBattleList)
                  {
                      item.AddExperience(exp);
                      item.GetInventory().KillRecord(EnemyName);
                  }
                  haveBattleList.Clear();
              }
              Invoke("Respawn",respawnTime);
          };
        hp = maxHp;
        if(localUI!=null)
        {
            nameText.text =EnemyName;
        }
    }
    public override void Tick()
    {
        if (isDeath()) return;
        delta = Time.deltaTime;
        Foundtarget();
        //for debug
        if (staticEnemy) return;
        switch (estate)
        {
            case EnemyState.idle:
                Idle();
                break;
            case EnemyState.patrol:
                Patrol();
                break;
            case EnemyState.Combat:
                Combat();
                break;
        }
    }

    void Foundtarget()
    {
        if (estate == EnemyState.Combat) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, actionRadius);
        foreach(var c in colliders)
        {
            if(c.tag==StaticStrings.player)
            {
                var p = c.GetComponent<Player>();
                if(p!=null)
                {
                    if(!p.isDeath())
                    {
                        playerTarget = p;
                        estate = EnemyState.Combat;
                        AddToBattleList(p);
                    }
                }
            }
        }
    }

    public void AddToBattleList(Player player)
    {
        if (player == null) return;

        bool exist = false;
        foreach(var item in haveBattleList)
        {
            if(item.gameObject==player.gameObject)
            {
                exist = true;
            }
        }
        if(!exist)
        {
            haveBattleList.Add(player);
        }
        if(playerTarget==null)
        {
            view.RPC("Allert",RpcTarget.All,WorldManager.instance.VectorConverter(player.transform.position));
        }
    }
    
    [PunRPC]
    public void Allert(float[] pos)
    {
        if (!photonView.IsMine) return;
        Vector3 position = WorldManager.instance.ToVector(pos);
        Collider[] collider = Physics.OverlapSphere(position, 1);
        foreach(var c in collider)
        {
            Player player = c.GetComponent<Player>();
            if(player!=null)
            {
                playerTarget = player;
                estate = EnemyState.Combat;
                break;
            }
        }

    }
    

    void Combat()
    {
        if (playerTarget == null || playerTarget.isDeath() || isDeath())
        {
            estate = EnemyState.patrol;
            playerTarget = null;
            return;
        }
        var pos = playerTarget.transform.position;
        float distance = Vector3.Distance(transform.position, pos);
        if(distance>maxDistance)
        {
            estate = EnemyState.patrol;
            playerTarget = null;
            return;
        }
        if(distance>attackRange)
        {
            MoveAt(pos);
        }
        else
        {
            if(!timer.timerActive(Time.deltaTime))
            {
                timer.StartTimer(attackSpeed);
                sync.PlayAnimation("atk");
                playerTarget.TakeDamage(GetDamage());
            }
        }
    }
    void Patrol()
    {
        float dist = Vector3.Distance(transform.position, destination);
        if(dist<=patrolDist)
        {
            if(destination==endPos)
            {
                destination = startPosition;
            }
            else
            {
                destination = endPos;
            }     
            timer.StartTimer(wait);
            estate = EnemyState.idle;
            sync.SetMove(false);
        }
        else
        {
            MoveAt(destination);
        }
    }

    void MoveAt(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * delta);
        Vector3 look = destination - transform.position;
        look.y = 0;
        Quaternion rot = Quaternion.LookRotation(look);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, wait * delta);
        sync.SetMove(true);
    }

    void Idle()
    {
        if(!timer.timerActive(delta))
        {
            estate = EnemyState.patrol;
            sync.SetMove(true);
        }
    }


    enum EnemyState
    {
        idle,
        patrol,
        Combat
    }

    void Respawn()
    {
        CanTakeDrop = true;
        hp = maxHp;
        view.RPC("SyncronizeStat", RpcTarget.All, hp, maxHp);
        sync.IsDead(false);
        transform.position = startPosition;
        playerTarget = null;
        UpdateUI(hp, maxHp);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, actionRadius);
    }

    int GetDamage()
    {
        int val = Helper.GetParameter(this,StaticStrings.strenght);
        return val;
    }

    public override void UpdateUI(int hp, int maxHp)
    {
        if(localUI!=null)
        {
            localhpBar.maxValue = maxHp;
            localhpBar.value = hp;
        }
    }

    public int GetParameter(string s)
    {
        var equip = AllEquip().Where(x => x != null);
        int val = 0;
        if (equip.Count() < 1) return val;

        foreach (var e in equip)
        {
            switch (s)
            {
                case StaticStrings.stamina:
                    if(e!=null)
                        val += e.stamina;
                    break;
                case StaticStrings.strenght:
                    if (e != null)
                        val += e.strenght;
                    break;
                case StaticStrings.intellect:
                    if (e != null)
                        val += e.intellect;
                    break;
                case StaticStrings.agility:
                    if (e != null)
                        val += e.agility;
                    break;
                case StaticStrings.armor:
                    if (e != null)
                        val += e.armor;
                    break;
            }
        }
        return val;
    }

    public List<Equip> AllEquip()
    {
        List<Equip> equipList = new List<Equip>();
        equipList.Add(head);
        equipList.Add(body);
        equipList.Add(leg);
        equipList.Add(shoes);
        equipList.Add(belt);
        equipList.Add(shoulder);
        equipList.Add(leftWeapon);
        equipList.Add(rightWeapon);

        return equipList;
    }
    public override void BecameSpellTarget(Skill skill, Entity owner = null)
    {
        Player player = owner as Player;
        if(player!=null)
        {
            AddToBattleList(player);
        }
        base.BecameSpellTarget(skill, owner);
    }
}
