using UnityEngine;
using System;
using Zenject;
using System.Collections;

public class LMG : DestructibleObject
{
    public SpriteRenderer topRenderer;
    public SpriteRenderer baseRenderer;

    public Transform top;

    public float aimRadius = 5f;

    [HideInInspector]
    public GameObject closestEnemy = null;

    public GameObject healthBar;

    [HideInInspector]
    public bool randomRotate = false;

    public ItemData itemData;

    private Vector2 direction;

    public bool isPlaced = false;

    [HideInInspector]
    public CurrentItemHandler currentItemHandler;

    protected void Awake()
    {
        itemData = GetComponent<ItemInfo>().ItemData;

        GetComponent<HealtComponent>().startHealth = itemData.lMGData.maxHealth;

        StartCoroutine(RandomRotate(top));
    }

    protected override void Start()
    {
        base.Start();

        if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraTrap))
        {
            itemData.currentAmmount = 2;
            itemData.maxAmmount = 2;
        }

        destructiveStateSteps = itemData.lMGData.destructiveStateSteps;
    }

    protected override void Update()
    {
        base.Update();

        HealthBar healthBarScript = healthBar.GetComponent<HealthBar>();

        if (isPlaced)
            healthBarScript.SetValue(healthComponent.currentHealth / healthComponent.maxHealth);
        else
            gameObject.SetActive(false);

        if (GetComponent<AutoShoot>().currentAmmu <= 0)
        {
            GetComponent<HealtComponent>().TakeDamageToDie(false);
        }

        AimAtEnemy();
    }

    public GameObject Place(Vector2 position)
    {
        if (!isPlaced)
        {
            Animator animator = transform.parent.parent.parent.GetComponent<Animator>();
            animator.SetTrigger("Attack");

            if (AudioManager.instance != null)
                AudioManager.instance.Play("Place", transform);

            GameObject lmg = Instantiate(gameObject, position, transform.rotation);
            lmg.GetComponent<ItemInfo>().ItemData = itemData;
            lmg.GetComponent<LMG>().isPlaced = true;

            //Update Navmesh
            AstarPath.active.Scan();

            itemData.currentAmmount--;

            //Remove from Inventory
            if (itemData.currentAmmount <= 0)
            {
                currentItemHandler.RemoveItem(gameObject);
                currentItemHandler.UpdateCurrentItem();
            }

            return lmg;
        }

        return null;
    }

    private void AimAtEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(top.position, aimRadius);
        foreach (Collider2D objectInRadius in enemies)
        {
            if (closestEnemy == null && objectInRadius.GetComponent<Enemy>() != null && objectInRadius.GetComponent<HealtComponent>().currentHealth > 0)
            {
                closestEnemy = objectInRadius.gameObject;
            }
        }

        if ((closestEnemy != null && Vector2.Distance(closestEnemy.transform.position, top.position) > aimRadius) || (closestEnemy != null && closestEnemy.GetComponent<HealtComponent>().currentHealth <= 0))
        {
            closestEnemy = null;
        }

        if (closestEnemy != null && isPlaced)
        {
            randomRotate = false;
            GetComponent<AutoShoot>().isShoot = true;
            Vector2 enemyPosition = (Vector2)closestEnemy.transform.position;
            direction = new Vector2(enemyPosition.x - top.position.x, enemyPosition.y - top.position.y);
            top.up = Vector2.Lerp(top.up, direction, 0.1f);
        }
        else
        {
            randomRotate = true;
            GetComponent<AutoShoot>().isShoot = false;
        }
    }

    public IEnumerator RandomRotate(Transform objectToRotate)
    {
        float randomAngle = UnityEngine.Random.Range(-360, 360);

        float value = 0;

        while (value < 0.1 && randomRotate)
        {
            objectToRotate.rotation = Quaternion.Slerp(objectToRotate.rotation, Quaternion.Euler(0, 0, randomAngle), value);

            value += Time.deltaTime * 0.05f;

            yield return null;
        }

        yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));

        StartCoroutine(RandomRotate(objectToRotate));
    }

    public override void DestroyState0()
    {
        topRenderer.sprite = itemData.lMGData.tops[0];
        baseRenderer.sprite = itemData.lMGData.bases[0];
    }
    public override void DestroyState1()
    {
        topRenderer.sprite = itemData.lMGData.tops[1];
        baseRenderer.sprite = itemData.lMGData.bases[1];
    }
    public override void DestroyState2()
    {
        topRenderer.sprite = itemData.lMGData.tops[2];
        baseRenderer.sprite = itemData.lMGData.bases[2];
    }
    public override void DestroyState3()
    {
        topRenderer.sprite = itemData.lMGData.tops[3];
        baseRenderer.sprite = itemData.lMGData.bases[3];
    }
}
