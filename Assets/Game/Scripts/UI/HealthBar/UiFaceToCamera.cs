using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFaceToCamera : MonoBehaviour
{
    public Transform cam;

    void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void Update()
    {
        transform.rotation = cam.rotation;
    }
}
