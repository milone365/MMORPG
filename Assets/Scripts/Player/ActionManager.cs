using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField]
    float autoMoveSpeed = 3;
    NetworkPlayer player = null;
    Animator anim;
    FollowCamera follow;
    float screenH, screenW;
    PlayerUI playerUi;
    [SerializeField]
    float rotSpeed = 2;
    public List<ActionClass> upActions = new List<ActionClass>()
    {
        new ActionClass(){code=KeyCode.Alpha1 },new ActionClass(){code=KeyCode.Alpha2 },
        new ActionClass(){code=KeyCode.Alpha3 },new ActionClass(){code=KeyCode.Alpha4 },
        new ActionClass(){code=KeyCode.Alpha5 },new ActionClass(){code=KeyCode.Alpha6 }
    };
    public List<ActionClass> downActions = new List<ActionClass>()
    {
        new ActionClass(){code=KeyCode.Z },new ActionClass(){code=KeyCode.X },
        new ActionClass(){code=KeyCode.C },new ActionClass(){code=KeyCode.V },
        new ActionClass(){code=KeyCode.B },new ActionClass(){code=KeyCode.N }
    };

    List<ActionClass> allActions = new List<ActionClass>();
    PhotonView view = null;
    public void INIT(NetworkPlayer player,Animator anim,FollowCamera cam)
    {
        this.player = player;
        this.follow = cam;
        this.anim = anim;
        screenH = Screen.height;
        screenW= Screen.width;
        Cursor.SetCursor(defaultIMG, Vector2.zero, CursorMode.Auto);
        var ui = Resources.Load<PlayerUI>("UIPrefab/PlayerUI");
        playerUi = Instantiate(ui);
        playerUi.INIT(player,this);
        allActions.AddRange(upActions);
        allActions.AddRange(downActions);
        view = PhotonView.Get(this);
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
        foreach(var item in allActions)
        {
            if(Input.GetKeyDown(item.code))
            {
                OnPressButton(item.code,item);
                break;
            }
        }
    }
    public void OnPressButton(KeyCode code,ActionClass action=null)
    {
        var a = action;
        if(a==null)
        {
            a = GetAction(code);
            if (a == null) return;
        }
        if (a.skill == null) return;
        if (PhotonNetwork.IsConnected)
        {
            view.RPC("PlayAnimation", RpcTarget.All, a.skill.animName.ToString());
        }
        else
        {
            anim.Play(a.skill.animName.ToString());
            Debug.Log("not connected");
        }
    }

    ActionClass GetAction(KeyCode code)
    {
        foreach(var a in allActions)
        {
            if(a.code==code)
            {
                return a;
            }
        }
        return null;
    }
    void MouseAction()
    {
        Vector3 mousePos = Input.mousePosition;
        if (!MouseInView(mousePos)) return;

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
            targetEnemy = null;
            return;
        }
        float dist = GetDistance(transform.position, targetEnemy.transform.position);
        float vel = 0;

        if (dist <= meleeAtkDist)
        {
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
        else
        {
            Vector3 p = targetEnemy.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, p,
                autoMoveSpeed * Time.deltaTime);
            vel = 1;
            p.y = transform.position.y;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(p), Time.deltaTime*
               rotSpeed );
        }
        anim.SetFloat("y",vel);
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
    
    bool MouseInView(Vector3 pos)
    {
        if (pos.x > 0 && pos.x < screenW && pos.y > 0 && pos.y < screenH) return true;

       return false;
    }

    public void CompleteAction()
    {
        
    }
}

[System.Serializable]
public class ActionClass
{
    public KeyCode code;
    public Skills skill;
}