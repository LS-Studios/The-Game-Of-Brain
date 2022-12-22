using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIPlacementHandler : MonoBehaviour
{
    public GameObject movementJoyStick;
    public GameObject aimJoyStick;
    public GameObject shootBtn;
    public GameObject reloadBtn;
    public GameObject punchBtn;
    public GameObject runBtn;
    public GameObject grenadeBtn;

    private SettingsManager settings;

    void Start()
    {
        settings = GameInstance.instance.settingValues.settings;

        PlaceUI();
    }

    public void PlaceUI()
    {
        settings = GameInstance.instance.settingValues.settings;

        RectTransform movementJoyStickRect = movementJoyStick.GetComponent<RectTransform>();
        movementJoyStickRect.anchorMin = settings.movementJoyStickPlacement.anchor.Item1;
        movementJoyStickRect.anchorMax = settings.movementJoyStickPlacement.anchor.Item2;
        movementJoyStickRect.anchoredPosition = settings.movementJoyStickPlacement.pos;

        RectTransform aimJoyStickRect = aimJoyStick.GetComponent<RectTransform>();
        aimJoyStickRect.anchorMin = settings.aimJoyStickPlacement.anchor.Item1;
        aimJoyStickRect.anchorMax = settings.aimJoyStickPlacement.anchor.Item2;
        aimJoyStickRect.anchoredPosition = settings.aimJoyStickPlacement.pos;

        RectTransform shootBtnRect = shootBtn.GetComponent<RectTransform>();
        shootBtnRect.anchorMin = settings.shootBtnPlacement.anchor.Item1;
        shootBtnRect.anchorMax = settings.shootBtnPlacement.anchor.Item2;
        shootBtnRect.anchoredPosition = settings.shootBtnPlacement.pos;

        RectTransform reloadBtnRect = reloadBtn.GetComponent<RectTransform>();
        reloadBtnRect.anchorMin = settings.reloadBtnPlacement.anchor.Item1;
        reloadBtnRect.anchorMax = settings.reloadBtnPlacement.anchor.Item2;
        reloadBtnRect.anchoredPosition = settings.reloadBtnPlacement.pos;

        RectTransform punchBtnRect = punchBtn.GetComponent<RectTransform>();
        punchBtnRect.anchorMin = settings.punchBtnPlacement.anchor.Item1;
        punchBtnRect.anchorMax = settings.punchBtnPlacement.anchor.Item2;
        punchBtnRect.anchoredPosition = settings.punchBtnPlacement.pos;

        RectTransform runBtnRect = runBtn.GetComponent<RectTransform>();
        runBtnRect.anchorMin = settings.runBtnPlacement.anchor.Item1;
        runBtnRect.anchorMax = settings.runBtnPlacement.anchor.Item2;
        runBtnRect.anchoredPosition = settings.runBtnPlacement.pos;

        RectTransform grenadeBtnRect = grenadeBtn.GetComponent<RectTransform>();
        grenadeBtnRect.anchorMin = settings.grenadeBtnPlacement.anchor.Item1;
        grenadeBtnRect.anchorMax = settings.grenadeBtnPlacement.anchor.Item2;
        grenadeBtnRect.anchoredPosition = settings.grenadeBtnPlacement.pos;
    }

    public void PlaceSwitchedUI()
    {
        settings = GameInstance.instance.settingValues.settings;
        settings.SetSwitchedPosition();

        PlaceUI();
    }

    public void PlaceBackToDefaultUI()
    {
        settings = GameInstance.instance.settingValues.settings;
        settings.SetDefaultPositions();

        RectTransform movementJoyStickRect = movementJoyStick.GetComponent<RectTransform>();
        movementJoyStickRect.anchorMin = settings.movementJoyStickPlacement.anchor.Item1;
        movementJoyStickRect.anchorMax = settings.movementJoyStickPlacement.anchor.Item2;
        movementJoyStickRect.anchoredPosition = settings.movementJoyStickPlacement.pos;

        RectTransform aimJoyStickRect = aimJoyStick.GetComponent<RectTransform>();
        aimJoyStickRect.anchorMin = settings.aimJoyStickPlacement.anchor.Item1;
        aimJoyStickRect.anchorMax = settings.aimJoyStickPlacement.anchor.Item2;
        aimJoyStickRect.anchoredPosition = settings.aimJoyStickPlacement.pos;

        RectTransform shootBtnRect = shootBtn.GetComponent<RectTransform>();
        shootBtnRect.anchorMin = settings.shootBtnPlacement.anchor.Item1;
        shootBtnRect.anchorMax = settings.shootBtnPlacement.anchor.Item2;
        shootBtnRect.anchoredPosition = settings.shootBtnPlacement.pos;

        RectTransform reloadBtnRect = reloadBtn.GetComponent<RectTransform>();
        reloadBtnRect.anchorMin = settings.reloadBtnPlacement.anchor.Item1;
        reloadBtnRect.anchorMax = settings.reloadBtnPlacement.anchor.Item2;
        reloadBtnRect.anchoredPosition = settings.reloadBtnPlacement.pos;

        RectTransform punchBtnRect = punchBtn.GetComponent<RectTransform>();
        punchBtnRect.anchorMin = settings.punchBtnPlacement.anchor.Item1;
        punchBtnRect.anchorMax = settings.punchBtnPlacement.anchor.Item2;
        punchBtnRect.anchoredPosition = settings.punchBtnPlacement.pos;

        RectTransform runBtnRect = runBtn.GetComponent<RectTransform>();
        runBtnRect.anchorMin = settings.runBtnPlacement.anchor.Item1;
        runBtnRect.anchorMax = settings.runBtnPlacement.anchor.Item2;
        runBtnRect.anchoredPosition = settings.runBtnPlacement.pos;

        RectTransform grenadeBtnRect = grenadeBtn.GetComponent<RectTransform>();
        grenadeBtnRect.anchorMin = settings.grenadeBtnPlacement.anchor.Item1;
        grenadeBtnRect.anchorMax = settings.grenadeBtnPlacement.anchor.Item2;
        grenadeBtnRect.anchoredPosition = settings.grenadeBtnPlacement.pos;
    }
}
