using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenue : MonoBehaviour
{
    public float scaleFactor = 10;
    public RawImage mapImage;

    public void ScaleMapIn()
    {
        Vector3 newSize = LSUtils.AddVector(mapImage.transform.localScale, new Vector2(scaleFactor, scaleFactor));
        if (newSize.x <= 2)
            mapImage.transform.localScale = newSize;
    }

    public void ScaleMapOut()
    {
        Vector3 newSize = LSUtils.AddVector(mapImage.transform.localScale, new Vector2(-scaleFactor, -scaleFactor));
        if (newSize.x >= 0.3f)
            mapImage.transform.localScale = newSize;
    }
}
