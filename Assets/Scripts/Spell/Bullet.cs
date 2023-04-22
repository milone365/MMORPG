using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : Spell
{
    Transform target;
    string targetName;
    [SerializeField]
    float moveSpeed = 2;
    [SerializeField]
    Transform child = null;
    BulletFollower follower;
    public override void Initialize(Skill skill, Entity owner = null, Entity target = null)
    {
        base.Initialize(skill, owner, target);
        if (target is Player)
        {
            targetName = StaticStrings.player;
        }
        else
        {
            targetName = StaticStrings.enemy;
        }
        transform.position = owner.spawnPoint.position;
        transform.rotation = owner.spawnPoint.rotation;
        this.target = target.transform;
        var g= Photon.Pun.PhotonNetwork.Instantiate(skill.effectName, child.position, child.rotation);
        follower = g.GetComponent<BulletFollower>();
    }

    void Update()
    {
        if (target == null) return;
        Vector3 dir = target.position - transform.position;
        Quaternion rot= Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, moveSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime);
        if(follower!=null)
        {
            follower.Follow(child);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag==targetName)
        {
            Entity entity = other.GetComponent<Entity>();
            if(entity!=null)
            {
                entity.TakeDamage(spellPower);
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if(follower!=null)
        Photon.Pun.PhotonNetwork.Destroy(follower.gameObject);
    }

}
