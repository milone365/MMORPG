using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : Entity
{
    public string userName = "Tom";
    Rigidbody rb;
    [SerializeField]
    float speed = 5;
    [SerializeField]
    float multipler = 15;
    [SerializeField]
    float sensitiv = 1;
    [SerializeField]
    KeyCode lockRotKey = KeyCode.M;
    bool lockRot;
    FollowCamera follow;   
    ActionManager actionManager;
    [SerializeField]
    LayerMask floor;
    [SerializeField]
    float floorDist = 0.25f;
    float gravityForce = -9.81f;
    [SerializeField]
    float weight = 1;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        actionManager = GetComponent<ActionManager>();

        if (!photonView.IsMine)
        {
            actionManager.enabled = false;
            return;
        }

        WorldController.instance.playerList.Add(transform);
        var camPref = Resources.Load<FollowCamera>(Helper.follower);
        follow = Instantiate(camPref);
        follow.INIT(transform);
        actionManager.INIT(this, anim, follow);
        onDeathEvent = () =>
          {
              actionManager.playerUi.deathPanel.SetActive(true);
          };
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        bool onGround = OnGround();
        float gravity = 0;
        if(!OnGround())
        {
            gravity = gravityForce * Time.deltaTime * weight;
        }
        rb.velocity = new Vector3(rb.velocity.x, gravity, rb.velocity.z);
        Movement(Time.deltaTime);
        actionManager.TICK();
    }

    void Movement(float delta)
    {
        float x = Input.GetAxis(Helper.horizontal);
        float y = Input.GetAxis(Helper.vertical);
        float velocity = speed;
        if(Input.GetKey(KeyCode.LeftShift))
        {
            velocity = speed * 2;
            anim.SetBool(Helper.sprint, true);
        }
        else
        {
            anim.SetBool(Helper.sprint, false);
        }
        if (Input.GetKeyDown(lockRotKey))
        {
            lockRot = !lockRot;
        }
        if (lockRot == false)
        {
            float mouseX = Input.GetAxisRaw(Helper.mousex);
            Vector3 rot = follow.transform.eulerAngles;
            follow.transform.rotation = Quaternion.Euler(rot.x, rot.y + mouseX * sensitiv, rot.z);
            if(!isDeath && !actionManager.autoattack)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, follow.transform.rotation, 2 * Time.deltaTime);
            }
        }

        if(!isDeath)
        {
            Vector3 move = (transform.right * x) + (transform.forward * y) * delta * velocity * multipler;
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
            anim.SetFloat("x", x);
            anim.SetFloat("y", y);
            if (x != 0 || y != 0)
            {
                actionManager.autoattack = false;
            }
        }
    }

    bool OnGround()
    {
        Vector3 dir = transform.position + (Vector3.down*floorDist);
        Debug.DrawLine(transform.position,dir,Color.blue);
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,floor))
        {
            if(hit.transform!=null)
            {
                return true;
            }
        }
        return false;
    }
   
}
