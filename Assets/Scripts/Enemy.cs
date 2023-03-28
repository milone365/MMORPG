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

}
