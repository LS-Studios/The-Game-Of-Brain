using Zenject;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AutoShoot : MonoBehaviour
{
    public Transform muzzle;
    public TextMeshProUGUI ammoText;

    private GlobalData globalData;

    public float currentAmmu;

    public bool isShoot = false;
    private float nextTimeToFire = 0f;

    private void Start()
    {
        if (GetComponent<ZenAutoInjecter>() == null)
            gameObject.AddComponent<ZenAutoInjecter>();

        currentAmmu = GetComponent<ItemInfo>().ItemData.weaponData.maxAmmo;
        globalData = GameInstance.instance.referenceValues.globalData;
    }

    void Update()
    {
        if (isShoot && Time.time >= nextTimeToFire && GameInstance.instance.canDoItemAction)
        {
            nextTimeToFire = Time.time + 1f / GetComponent<ItemInfo>().ItemData.weaponData.firerate;

            Shoot();
        }

        if (ammoText != null)
            ammoText.text = currentAmmu.ToString();

        if (currentAmmu <= 0)
            ammoText.gameObject.SetActive(false);
    }

    public void Shoot()
    {
        if (currentAmmu > 0)
        {
            ItemData itemData = GetComponent<ItemInfo>().ItemData;

            AudioManager.instance.Play(itemData.weaponData.attackSounds[Random.Range(0, itemData.weaponData.attackSounds.Length)], transform);

            GameObject bullet = Instantiate(globalData.projectile, muzzle.position, muzzle.rotation);
            bullet.GetComponent<Rigidbody2D>().AddForce(muzzle.up * 20, ForceMode2D.Impulse);

            bullet.GetComponent<Prokectile>().SetWeaponData(itemData, new List<string> { "LMG", "Player" });
            Camera.main.transform.parent.GetComponent<CameraHandler>().Shake();

            currentAmmu -= 1;
        }
        else
        {
            AudioManager.instance.Play("EmptyMag", transform);
        }
    }
}
