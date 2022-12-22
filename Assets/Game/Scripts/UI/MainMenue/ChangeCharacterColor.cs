using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChangeCharacterColor : MonoBehaviour
{
    public void ChangeSkinColor(Image colorImage) {
        GameInstance.instance.playerValues.playerColors[0] = colorImage.color;
    }

    public void ChangeHairColor(Image colorImage)
    {
        GameInstance.instance.playerValues.playerColors[1] = colorImage.color;
    }

    public void ChangeEysColor(Image colorImage)
    {
        GameInstance.instance.playerValues.playerColors[2] = colorImage.color;
    }

    public void ChangeClothTopColor(Image colorImage)
    {
        GameInstance.instance.playerValues.playerColors[3] = colorImage.color;
    }

    public void ChangeClothBottomColor(Image colorImage)
    {
        GameInstance.instance.playerValues.playerColors[4] = colorImage.color;
    }
}
