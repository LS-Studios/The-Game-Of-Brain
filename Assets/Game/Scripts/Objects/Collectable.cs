using System.Collections;
using Zenject;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public enum CollectableTyp {Ammo, Health, GamepointItem}
    public CollectableTyp collectableTyp;

    public GameInstance.InGameValues.Rarity.RarityTyp rarityTyp;

    public float ammountToAdd;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.AddComponent<ZenAutoInjecter>().ContainerSource = ZenAutoInjecter.ContainerSources.SceneContext;

        GetComponent<SpriteRenderer>().material.SetColor("_Color", GameInstance.instance.inGameValues.GetRarity(rarityTyp).rarityColor);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            CurrentItemHandler currentItemHandler = collision.gameObject.GetComponentInChildren<CurrentItemHandler>();

            switch (collectableTyp)
            {
                case CollectableTyp.Ammo:
                    foreach (GameObject g in currentItemHandler.useableItems)
                    {
                        if (g.GetComponent<WeaponBase>() != null)
                        {
                            g.SetActive(true);
                            WeaponBase weaponBase = g.GetComponent<WeaponBase>();

                            if (weaponBase.itemData != null)
                                weaponBase.SetCurrentAmmo(weaponBase.itemData.weaponData.maxClipAmmo, weaponBase.itemData.weaponData.maxAmmo);
                            else
                                weaponBase.SetCurrentAmmo(0, weaponBase.itemData.maxAmmount);

                            g.SetActive(false);

                            currentItemHandler.UpdateCurrentItem();
                        }
                    }
                    break;
                case CollectableTyp.Health:
                    if (collision.gameObject.GetComponent<PlayerHandler>() != null)
                        collision.gameObject.GetComponent<HealtComponent>().AddHealth(ammountToAdd);
                    break;

                case CollectableTyp.GamepointItem:
                    
                    if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraGP))
                    {
                        ammountToAdd = Mathf.RoundToInt(ammountToAdd*1.5f);
                    }
                    GameInstance.instance.inGameValues.GamePoints += (int)ammountToAdd;
                    GameInstance.instance.referenceValues.globalData.CreateGPAddPoint(transform.position, (int)ammountToAdd);
                    break;
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator stopMoving()
    {
        yield return new WaitForSeconds(.25f);

        rb.isKinematic = true;
    }
}
