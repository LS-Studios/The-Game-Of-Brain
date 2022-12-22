using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenueItemScript : MonoBehaviour
{
    public Color hoverColor;
    public Color baseColor;
    public Image backGround;
    private GameObject text;

    public GameObject characterHolder;
    public int weaponValue = 0;

    private Transform weapon;
    void Start()
    {
        text = transform.GetChild(1).gameObject;
        text.SetActive(false);

        backGround = transform.GetChild(0).GetComponent<Image>();
        backGround.color = baseColor;

        characterHolder = GameObject.FindGameObjectWithTag("Player").gameObject;

        weapon = characterHolder.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(weaponValue);
    }

    private void Update()
    {
        //WeaponBase currentWeapon = FindObjectOfType<currentItemHandler>().currentItemHandler.GetComponent<WeaponBase>();

        //if (currentWeapon.weapondata != null)
        //{
        //    text.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + currentWeapon.weapondata.damage;
        //    text.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = currentWeapon.currentClipSize + "/" + currentWeapon.currentAmmu;
        //    text.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + currentWeapon.level;
        //} else
        //{
        //    text.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + currentWeapon.grenadedata.damage;
        //    text.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + currentWeapon.currentGrenadeAmmount;
        //    text.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + currentWeapon.level;
        //}
    }
    public void Select()
    {
        backGround.color = hoverColor;
        text.SetActive(true);
        CurrentItemHandler currentWeapon = characterHolder.transform.GetChild(1).GetChild(1).GetComponent<CurrentItemHandler>();
        //currentWeapon.currentWeaponNumber = weaponValue;
    }
    public void Deselect()
    {
        backGround.color = baseColor;
        text.SetActive(false);
    }
}
