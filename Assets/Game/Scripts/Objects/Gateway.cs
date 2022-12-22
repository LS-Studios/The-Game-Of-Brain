using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using UnityEngine.Events;

public class Gateway : MonoBehaviour
{
    public Transform gate;

    public Vector2 closePos;
    public Vector2 openPose;

    public bool isOpened;

    public float speed = 1f;

    public int priceToOpen = 250;

    public SlideButton buyButton;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI openCloseText;

    public UnityEvent openAction;

    [Inject]
    private GameManager gameManager;

    public void Awake()
    {
        priceText.text = priceToOpen + " GP";

        priceText.GetComponentInParent<SlideButton>().manualAction = openAction;
    }

    public void Update()
    {
        if (GameInstance.instance.inGameValues.GamePoints >= priceToOpen)
        {
            buyButton.CanSlide = true;
        } else
        {
            buyButton.CanSlide = false;
        }
    }

    public void ToggleGate()
    {
        switch (isOpened)
        {
            case true:
                CloseGate();
                break;
            case false: 
                OpenGate();
                break;                   
        }
    }

    public void CreateBoughtGPText()
    {
        GameInstance.instance.referenceValues.globalData.CreateGPSubstractPoint(priceText.transform.position, priceToOpen);
        OpenGate();
    }

    public void OpenGate()
    {
        StartCoroutine(OpenLerp());
        isOpened = true;
        openCloseText.text = "Close";
    }

    public void CloseGate()
    {
        StartCoroutine(CloseLerp());
        isOpened = false;
        openCloseText.text = "Open";
    }

    private IEnumerator OpenLerp()
    {
        float procress = 0;

        while (procress <= 1)
        {
            gate.localPosition = Vector2.Lerp(gate.localPosition, openPose, procress);

            procress += Time.deltaTime * 0.05f * speed;

            yield return null;
        }

        gameManager.Scan();
    }

    private IEnumerator CloseLerp()
    {
        float procress = 0;

        while (procress <= 1)
        {
            gate.localPosition = Vector2.Lerp(gate.localPosition, closePos, procress);

            procress += Time.deltaTime * 0.05f * speed;

            yield return null;
        }

        gameManager.Scan();
    }
}
