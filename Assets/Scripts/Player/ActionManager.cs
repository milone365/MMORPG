using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
public class ActionManager : MonoBehaviourPun
{
    [SerializeField]
    Texture2D defaultIMG = null;
    [SerializeField]
    Texture2D swordIMG = null,bagIMG=null;
    Transform target = null;
    [SerializeField]
    float counter = 0;
    [SerializeField]
    float attackDealy = 1.7f;
    [SerializeField]
    float meleeAtkDist = 1.5f;
    public bool autoattack = false;
    Enemy targetEnemy;
    [SerializeField]
    float minZoom = 10, maxZoom = 120;
    [SerializeField]
    float zoomSpeed = 10;

    NetworkPlayer player = null;
    NavMeshAgent agent;
    Animator anim;
    FollowCamera follow;
    
    public void INIT(NetworkPlayer player,NavMeshAgent agent,Animator anim,FollowCamera cam)
    {
        this.player = player;
        this.follow = cam;
        this.agent = agent;
        this.anim = anim;
        Cursor.SetCursor(defaultIMG, Vector2.zero, CursorMode.Auto);
        
    }

    public void TICK()
    {
        KeyBoard();
        MouseAction();
        if (autoattack)
        {
            AutoAttack();
        }
    }

    void KeyBoard()
    {

    }
    void MouseAction()
    {
        Ray ray = follow.cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool leftDown = Input.GetMouseButtonDown(0);
        bool rightDown = Input.GetMouseButtonDown(1);
        float scroll = Input.GetAxisRaw(Helper.scroll);
        if (scroll != 0)
        {
            float current = follow.cam.fieldOfView;
            current += scroll * zoomSpeed;
            current = Mathf.Clamp(current, minZoom, maxZoom);
            follow.cam.fieldOfView = current;
        }
        if (Physics.Raycast(ray, out hit))
        {
            Texture2D t = defaultIMG;
            if (leftDown)
            {
                Selectable selectable = hit.transform.GetComponent<Selectable>();
                if (selectable != null)
                {
                    if (hit.transform != this.transform)
                    {
                        target = hit.transform;
                        selectable.Select();
                    }
                }
            }
            if (hit.transform.tag == Helper.enemy)
            {
                t = swordIMG;
                Enemy e = hit.transform.GetComponent<Enemy>();
                if (e == null) return;
                if (e.isDeath)
                {
                    if(e.itemList.Count>0)
                        t = bagIMG;
                }
                if (rightDown)
                    {
                    
                        if (e.isDeath)
                        {
                            foreach (var item in e.itemList)
                            {
                                Debug.Log(item.name);
                                Debug.Log(item.description);
                                Debug.Log("cost: " + item.cost);
                            }
                            //test
                            e.itemList.Clear();
                            return;
                        }
                        targetEnemy = e;
                        Selectable selectable = hit.transform.GetComponent<Selectable>();
                        if (selectable != null)
                        {
                            if (hit.transform != this.transform)
                            {
                                target = hit.transform;
                                selectable.Select();
                            }
                        }
                        autoattack = true;
                        agent.enabled = true;
                        agent.SetDestination(hit.transform.position);
                    
                    }
            }

            Cursor.SetCursor(t, Vector2.zero, CursorMode.Auto);
        }
    }

    void AutoAttack()
    {
        if (targetEnemy == null || targetEnemy.isDeath)
        {
            autoattack = false;
            agent.enabled = false;
            targetEnemy = null;
            return;
        }
        float dist = GetDistance(transform.position, targetEnemy.transform.position);
        if (dist <= meleeAtkDist)
        {
            agent.SetDestination(transform.position);
            counter -= Time.deltaTime;
            if (counter <= 0)
            {
                counter = attackDealy;
                anim.Play(Helper.unarmedMelee);
                if (PhotonNetwork.IsConnected)
                {
                    var view = PhotonView.Get(this);
                    view.RPC("PlayAnimation", RpcTarget.All, Helper.unarmedMelee);
                }
                targetEnemy.TakeDamage(GetAttackPower());
            }
        }
        anim.SetFloat("y", agent.velocity.magnitude);
    }
    float GetDistance(Vector3 t1, Vector3 t2)
    {
        t1.y = 0;
        t2.y = 0;
        return Vector3.Distance(t1, t2);
    }

    int GetAttackPower()
    {
        return 1;
    }
}
