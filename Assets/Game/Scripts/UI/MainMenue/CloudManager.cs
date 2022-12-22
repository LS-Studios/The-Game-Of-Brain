using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    private RectTransform myTranform;
    private Vector2 startPos;
    public float speed = 30f;
    private bool stopWorking = false;

    void Start()
    {
        myTranform = GetComponent<RectTransform>();
        startPos = myTranform.anchoredPosition;
    }

    void Update()
    {
        myTranform.anchoredPosition = new Vector2(myTranform.anchoredPosition.x + (Time.deltaTime * speed), myTranform.anchoredPosition.y);

        if (myTranform.anchoredPosition.x >= -200 && !stopWorking)
        {
            GameObject newCloud = Instantiate(gameObject, transform.parent);
            newCloud.GetComponent<RectTransform>().anchoredPosition = new Vector2(myTranform.anchoredPosition.x - 5976, myTranform.anchoredPosition.y);
            stopWorking = true;
        }
        if (myTranform.anchoredPosition.x >= 5900)
        {
            Destroy(gameObject);
        }
    }
}
