using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Enemy : MonoBehaviourPun ,Selectable
{
    [SerializeField]
    int maxHp = 10;
    [SerializeField]
    int hp = 10;
    Animator anim;
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
    public bool isDeath = false;
    PhotonView view;
    [SerializeField]
    LocalUI ui = null;
    [SerializeField]
    bool staticEnemy = false;
    public List<Item> itemList = new List<Item>();

    private void Start()
    {
        anim = GetComponent<Animator>();
        start = transform.position;
        view = PhotonView.Get(this);
        ui.Init(maxHp, hp);
        if (!photonView.IsMine) return;
        End = transform.position + new Vector3(Random.Range(-patrolX,patrolX), transform.position.y, 
            Random.Range(-patrolZ,patrolZ));
        destination = End;
        counter = wait;
        hp = maxHp;
    }
    private void Update()
    {
        if (!photonView.IsMine) return;
        if (isDeath) return;

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
    public void Select()
    {
        Debug.Log("Select");
    }


    [PunRPC]
    public void PlayAnimation(string s)
    {
        anim.Play(s);
    }

    [PunRPC]
    void DamageReciver(int dmg)
    {
        if (isDeath) return;
        hp -= dmg;
        view.RPC("SyncronizeStat", RpcTarget.All, hp);
        if(hp<=0)
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
        ui.UpdaBar(hp);
    }
}
