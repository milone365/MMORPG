using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    float rotSpeed = 2;
    bool autoMove = false;
    [SerializeField]
    float attackRange = 3;
    [SerializeField]
    float automoveSpeed = 2;
    Enemy enemyTarget;
    Timer attackTimer = new Timer();
    [SerializeField]
    float attackDealy = 2;

    public AnimatorSync sync { get; set; }
    public void Tick(Transform follow,float x,float y)
    {
        float delta = Time.deltaTime;
        if(autoMove==false)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, follow.rotation, rotSpeed * delta);
        }
        RightClick();
        if(x!=0|| y!=0)
        {
            autoMove = false;
        }
        if(autoMove)
        {
            if(enemyTarget==null)
            {
                autoMove = false;
                return;
            }
            Vector3 enemyPos = enemyTarget.transform.position;
            float dist = Vector3.Distance(transform.position, enemyPos);
            float velocity = 0;
            if(dist>attackRange)
            {
                velocity = 1;
                transform.position = Vector3.MoveTowards(transform.position, enemyPos,
                automoveSpeed * delta);
            }
            else
            {
                AutoAttack(delta);
            }
            sync.Move(0, velocity);
            Vector3 look = enemyPos- transform.position;
            look.y = 0;
            Quaternion rot = Quaternion.LookRotation(look);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, automoveSpeed * delta);
        }
    }

    void AutoAttack(float delta)
    {
        if(enemyTarget==null || enemyTarget.isDeath)
        {
            autoMove = false;
            enemyTarget = null;
            return;
        }
       if(!attackTimer.timerActive(delta))
        {
            attackTimer.StartTimer(attackDealy);
            sync.PlayAnimation("atk");
            enemyTarget.TakeDamage(GetDamage());
        }
    }

    int GetDamage()
    {
        int val = 1;

        return val;
    }

    void RightClick()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                if(hit.transform.tag==StaticStrings.enemy)
                {
                    autoMove = true;
                    enemyTarget = hit.transform.GetComponent<Enemy>();
                }
            }
        }
    }
}
