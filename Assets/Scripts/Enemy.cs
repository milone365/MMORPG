using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : Entity
{
    
    [SerializeField]
    float moveSpeed = 5;
    Vector3 start, End;
    [SerializeField]
    int patrolX = 5;
    [SerializeField]
    int patrolZ = 5;
    Vector3 destination;
    [SerializeField]
    float minDist = 2;
    float wait = 2;
    float counter = 0;
    [SerializeField]
    bool staticEnemy = false;
    public List<Item> itemList = new List<Item>();
    [SerializeField]
    NetworkPlayer playerTarget = null;
    [SerializeField]
    EnemyState estate = EnemyState.Patrol;
    [SerializeField]
    float atkCounter = 0;
    [SerializeField]
    float attackDealy = 1.7f;
    [SerializeField]
    float meleeAtkDist = 1.5f;
    [SerializeField]
    float autoMoveSpeed = 3;
    [SerializeField]
    float radius = 5;
    [SerializeField]
    float rotSpeed = 3;
    private void Start()
    {
        anim = GetComponent<Animator>();
        start = transform.position;
        view = Photon.Pun.PhotonView.Get(this);
        ui.Init(stats.maxHp, hp);
        if (!photonView.IsMine) return;
        End = transform.position + new Vector3(Random.Range(-patrolX,patrolX), transform.position.y, 
            Random.Range(-patrolZ,patrolZ));
        destination = End;
        counter = wait;
        hp = stats.maxHp;
    }
    private void Update()
    {
        if (!photonView.IsMine) return;
        if (isDeath) return;
        SearchPlayer();
        switch (estate)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Combat:
                Combat();
                break;
        }

    }

    void SearchPlayer()
    {
        if(playerTarget==null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach(var item in colliders)
            {
                if(item.tag==Helper.player)
                {
                    NetworkPlayer p = item.GetComponent<NetworkPlayer>();
                    if (p == null) continue;
                    if(!p.isDeath)
                    {
                        playerTarget = p;
                        estate = EnemyState.Combat;
                        break;
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void Patrol()
    {
        if (staticEnemy) return;
        float dist = Vector3.Distance(transform.position, destination);
        bool moving = true;
        if (dist <= minDist)
        {
            moving = false;
            counter -= Time.deltaTime;
            if (counter <= 0)
            {
                counter = wait;
                if (destination == End)
                {
                    destination = start;
                }
                else
                {
                    destination = End;
                }
            }

        }
        else
        {
            transform.rotation = Quaternion.LookRotation(destination);
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }
        anim.SetBool(Helper.move, moving);

    }

    void Combat()
    {
        if(playerTarget==null)
        {
            estate = EnemyState.Patrol;
        }
        else
        {
            if(playerTarget.isDeath)
            {
                estate = EnemyState.Patrol;
                playerTarget = null;
                return;
            }
            bool moving = false;

            Vector3 playerPos = playerTarget.transform.position;
            float distance = Vector3.Distance(transform.position, playerPos);
            if(distance<=meleeAtkDist)
            {
                atkCounter -= Time.deltaTime;
                if(atkCounter<=0)
                {
                    atkCounter = attackDealy;
                    if(Photon.Pun.PhotonNetwork.IsConnected)
                    {
                        view.RPC(Helper.playAnim, Photon.Pun.RpcTarget.All, Helper.atk);
                    }
                    else
                    {
                        Debug.Log("not connected");
                        anim.Play(Helper.atk);
                    }
                }
            }
            else
            {
                moving = true;
                transform.position = Vector3.MoveTowards(transform.position, playerPos,
                    Time.deltaTime * autoMoveSpeed);
            }
            Vector3 direction = playerPos - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime *
               rotSpeed);
            anim.SetBool(Helper.move, moving);
        }
    }
    int GetAttack()
    {
        int amount = stats.atk;

        return amount;
    }
    public void CompleteAttack()
    {
        if (!photonView.IsMine) return;
        if(playerTarget!=null)
        {
            playerTarget.TakeDamage(GetAttack());
        }
    }

    enum EnemyState
    {
        Patrol,
        Combat
    }
}
