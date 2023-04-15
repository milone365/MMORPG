using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void Init()
    {
        base.Init();
        CalculateStats(stats.Stamina, stats.Intellect);
        timer.StartTimer(wait);
        startPosition = transform.position;
        endPos = transform.position + direction;
        destination = endPos;
        OnDeathEvent = () =>
          {
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
                    }
                }
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
        hp = maxHp;
        sync.IsDead(false);
        transform.position = startPosition;
        UpdateUI(hp, maxHp);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, actionRadius);
    }

    int GetDamage()
    {
        int val = 1;

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
}
