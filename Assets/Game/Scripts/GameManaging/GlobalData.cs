using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalData", menuName = "Data/GlobalData")]
public class GlobalData : ScriptableObject
{
    [Header("Item prefabs")]
    public GameObject projectile;
    public GameObject grenadePrefab;
    public GameObject hitEffect;
    public ParticleSystem explosion;

    [Header("UI prefabs")]
    public GameObject sliderPrefab;
    public GameObject itemSlot;
    public GameObject equipmentSlot;
    public GameObject placeHolder;
    public GameObject ItemHeader;

    [Header("Enemy prefabs")]
    public GameObject normalZombie;
    public GameObject babyZombie;
    public GameObject smallBosZombie;

    [Header("Other prefabs")]
    public GameObject rewardPrefab;
    public GameObject floatingText;

    [Header("Sprite references")]
    public Sprite bulletSprite;
    public Sprite rocketSprite;
    public Sprite arrowSprite;

    public void CreateExposion(Vector3 position, float damage)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 1.25f);

        foreach (Collider2D nearObject in colliders)
        {
            if (nearObject.GetComponent<IDamageable>() != null && nearObject.tag != "Player")
                nearObject.GetComponent<IDamageable>().TakeDamage(damage, true);
        }

        Camera.main.transform.parent.GetComponent<CameraHandler>().Shake();
    }

    public void CreateGPAddPoint(Vector3 position, int gpReward)
    {
        CreateFloatingText(position, "+" + gpReward.ToString());
        GameInstance.instance.inGameValues.GamePoints += gpReward;
    }

    public void CreateGPSubstractPoint(Vector3 position, int gpPayment)
    {
        CreateFloatingText(position, "-" + gpPayment.ToString());
        GameInstance.instance.inGameValues.GamePoints -= gpPayment;
    }

    public void CreateFloatingText(Vector3 position, string text)
    {
        GameObject pointFloatInstance = Instantiate(floatingText, position, Quaternion.identity);
        pointFloatInstance.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}
