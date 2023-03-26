using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    Transform target = null;
    public Camera cam;
    public void INIT(Transform t)
    {
        target = t;
        cam = GetComponentInChildren<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position;
    }
}
