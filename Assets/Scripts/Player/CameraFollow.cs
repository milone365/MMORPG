using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform target;
    public Camera cam;
    public Transform arm;
    public void Init(Transform target)
    {
        this.target = target;
    }
    void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position;
    }
}
