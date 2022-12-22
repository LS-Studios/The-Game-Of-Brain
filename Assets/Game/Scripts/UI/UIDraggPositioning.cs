using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class UIDraggPositioning : MonoBehaviour, IDragHandler
{
    public enum UIPosTyp { MoveStick, AimStick, ShootBtn, ReloadBtn, PunchBtn, RunBtn, GrenadeBtn }
    public UIPosTyp uiPosTyp;

    public Canvas canvas;

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        SettingsManager settings = GameInstance.instance.settingValues.settings;

        switch (uiPosTyp)
        {
            case UIPosTyp.MoveStick:
                settings.movementJoyStickPlacement.pos = rectTransform.anchoredPosition;
                break;

            case UIPosTyp.AimStick:
                settings.aimJoyStickPlacement.pos = rectTransform.anchoredPosition;
                break;

            case UIPosTyp.ShootBtn:
                settings.shootBtnPlacement.pos = rectTransform.anchoredPosition;
                break;

            case UIPosTyp.ReloadBtn:
                settings.reloadBtnPlacement.pos = rectTransform.anchoredPosition;
                break;

            case UIPosTyp.PunchBtn:
                settings.punchBtnPlacement.pos = rectTransform.anchoredPosition;
                break;

            case UIPosTyp.RunBtn:
                settings.runBtnPlacement.pos = rectTransform.anchoredPosition;
                break;

            case UIPosTyp.GrenadeBtn:
                settings.grenadeBtnPlacement.pos = rectTransform.anchoredPosition;
                break;
        }
    }
}
