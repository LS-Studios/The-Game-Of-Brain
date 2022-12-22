using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class InfoBar : MonoBehaviour
{
    public Slider slider;

    public TextMeshProUGUI AmmuText;

    [HideInInspector]
    public WeaponBase weaponBase;

    public CurrentItemHandler currentItemHandler;

    public void SetHealtValue(float value)
    {
        slider.value = value/100;
    }

    void Update()
    {
        if (currentItemHandler.CurrentItem != null)
        {
            weaponBase = currentItemHandler.CurrentItem.GetComponent<WeaponBase>();

            LMG lmg = currentItemHandler.CurrentItem.GetComponent<LMG>();

            if (weaponBase != null)
            {
                if (weaponBase.itemData.dataTyp == ItemData.DataTyp.Grenadedata)
                    AmmuText.text = "" + weaponBase.itemData.currentAmmount;
                else if (weaponBase.itemData.weaponData.weaponTyp == ItemData.WeaponData.WeaponTyp.Particel)
                {
                    if (weaponBase.itemData.weaponData.currentClipAmmo > 0)
                        AmmuText.text = weaponBase.itemData.weaponData.currentClipAmmo.ToString("F2") + " / " + weaponBase.itemData.weaponData.currentAmmo.ToString("F2");
                    else
                        AmmuText.text = "0,00 / " + weaponBase.itemData.weaponData.currentAmmo;
                }
                else if (weaponBase.itemData.weaponData.weaponTyp == ItemData.WeaponData.WeaponTyp.Projectile)
                    AmmuText.text = weaponBase.itemData.weaponData.currentClipAmmo + " / " + weaponBase.itemData.weaponData.currentAmmo;
                else if (weaponBase.itemData.weaponData.weaponTyp == ItemData.WeaponData.WeaponTyp.NoAmmu)
                    AmmuText.text = "";
                else
                    AmmuText.text = "";
            } else if (lmg != null)
            {
                AmmuText.text = lmg.itemData.currentAmmount + " / " + lmg.itemData.maxAmmount;
            }
        } else
        {
            AmmuText.text = "";
        }
    }
}
