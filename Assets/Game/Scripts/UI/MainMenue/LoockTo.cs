using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoockTo : MonoBehaviour
{
    public GameObject downLeft;
    public GameObject downRight;
    public GameObject upLeft;
    public GameObject upRight;

    void Awake()
    {
        downLeft.SetActive(false);
        downRight.SetActive(false);
        upLeft.SetActive(false);
        upRight.SetActive(false);
    }

    void Update()
    {
        Vector2 relativeMousePos;
        Vector2 mousePos = new Vector2(0, 0);
        if (Mouse.current != null)
            mousePos = Mouse.current.position.ReadValue();
        else if (Touchscreen.current != null)
            mousePos = Touchscreen.current.touches[0].position.ReadValue();

        RectTransform thisRect = GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRect, mousePos, Camera.main, out relativeMousePos);
        relativeMousePos += thisRect.sizeDelta / 2;

        //Cursor is on right side of pivot
        if (relativeMousePos.x > 0)
        {
            //Cursor it above pivot
            if (relativeMousePos.y > 0)
            {
                downLeft.SetActive(false);
                downRight.SetActive(false);
                upLeft.SetActive(false);
                upRight.SetActive(true);
            }
            //Cursor it below pivot
            else
            {
                downLeft.SetActive(false);
                downRight.SetActive(true);
                upLeft.SetActive(false);
                upRight.SetActive(false);
            }
        } 
        //Cursor is on left side of pivot
        else
        {
            //Cursor it above pivot
            if (relativeMousePos.y > 0)
            {
                downLeft.SetActive(false);
                downRight.SetActive(false);
                upLeft.SetActive(true);
                upRight.SetActive(false);
            }
            //Cursor it below pivot
            else
            {
                downLeft.SetActive(true);
                downRight.SetActive(false);
                upLeft.SetActive(false);
                upRight.SetActive(false);
            }
        }
    }
}
