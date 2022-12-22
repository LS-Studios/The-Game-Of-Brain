using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Test : MonoBehaviour
{
    [Inject]
    CurrentItemHandler currentItemHandler;

    void Start()
    {
        float d = 5;
        print(d.ToString("F2"));
    }
}
