using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointFloat : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LSUtils.ScaleToNull(gameObject));
    } 
}
