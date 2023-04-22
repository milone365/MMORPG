using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BulletFollower : MonoBehaviourPun
{
    PhotonView view;
    public void Follow(Transform target)
    {
        if (view == null) view = PhotonView.Get(this);
        float[] p = WorldManager.instance.VectorConverter(target.position);
        float[] r= WorldManager.instance.VectorConverter(target.rotation.eulerAngles);
        view.RPC("FollowRpc",RpcTarget.All, p, r);
    }

    [PunRPC]
    public void FollowRpc(float[] pos,float[]rot)
    {
        transform.position = new Vector3(pos[0],pos[1],pos[2]);
        transform.rotation = Quaternion.Euler(rot[0],rot[1],rot[2]);
    }
}
