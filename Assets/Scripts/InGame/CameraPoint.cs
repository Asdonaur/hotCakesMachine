using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPoint : MonoBehaviour
{
    public Vector3 v3Offset;
    public float flMultip;

    // Update is called once per frame
    void Update()
    {
        transform.position = (PlayerMovement.scr.transform.position * flMultip) + v3Offset;
    }
}
