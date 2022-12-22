using System.Collections;
using System.Collections.Generic;
using Zenject;
using System;
using UnityEngine;
using Pathfinding;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using System.Drawing;
using System.Linq;
using UnityEngine.InputSystem.HID;

[RequireComponent(typeof(HealtComponent))]
public class Enemy : MonoBehaviour
{
    [Header("General values")]
    public EnemyTyp enemyTyp;
    public enum EnemyTyp { Normal, Light, Heavy }

    public float attackRate = 0.5f;

    public float damage = 3f;

    public List<string> targetTags = new List<string>();

    public float seeRange = 8f;

    public bool readyToAttack = false;

    [Header("References")]

    public GameObject head;

    [HideInInspector]
    public Transform target;

    public AIPath aIPathScript;

    public HealthBar healthBar;

    public Animator animator;

    protected float step = 0f;

    protected int spawnedEffects = 0;
    protected bool doDamageDelay = false;

    [HideInInspector]
    public HealtComponent healthComponent;

    protected bool isMove = false;

    public GameObject[] ranPosHolders;
    public List<Transform> ranPosTransforms = new List<Transform>();

    protected virtual void Awake()
    {
        healthComponent = GetComponent<HealtComponent>();

        healthComponent.onDie = Die;

        foreach (GameObject ranPosHolder in ranPosHolders)
        {
            foreach (Transform child in ranPosHolder.transform)
            {
                ranPosTransforms.Add(child);
            }
        }
    }

    protected virtual void Start()
    {
        if (GetComponent<ZenAutoInjecter>() == null)
            gameObject.AddComponent<ZenAutoInjecter>().ContainerSource = ZenAutoInjecter.ContainerSources.SceneContext;

        SetNewTarget();

        GetComponentInChildren<SpriteRenderer>().material.SetFloat("_Fade", 0f);

        StartCoroutine(Apear());
    }

    private void SetNewTarget()
    {
        GameObject closestTagrget = GetClosestTargetInRange(seeRange);

        if (closestTagrget != null)
        {
            readyToAttack = true;
            target = closestTagrget.transform;
            GetComponent<AIDestinationSetter>().target = target;
        } else
        {
            if (target == null)
            {
                if (ranPosTransforms.Count > 0)
                {
                    Transform foundTarget = ranPosTransforms[UnityEngine.Random.Range(0, ranPosTransforms.Count - 1)];

                    while (Vector2.Distance(foundTarget.position, transform.position) < 3f)
                    {
                        foundTarget = ranPosTransforms[UnityEngine.Random.Range(0, ranPosTransforms.Count - 1)];
                    }

                    target = foundTarget;
                    GetComponent<AIDestinationSetter>().target = foundTarget;

                    aIPathScript.endReachedDistance = 0;
                }
            }
            else
            {
                if (aIPathScript.reachedDestination)
                {
                    readyToAttack = false;

                    if (ranPosTransforms.Count > 0)
                    {
                        Transform foundTarget = ranPosTransforms[UnityEngine.Random.Range(0, ranPosTransforms.Count - 1)];

                        while (Vector2.Distance(foundTarget.position, transform.position) < 3f)
                        {
                            foundTarget = ranPosTransforms[UnityEngine.Random.Range(0, ranPosTransforms.Count - 1)];
                        }

                        target = foundTarget;
                        GetComponent<AIDestinationSetter>().target = foundTarget;

                        aIPathScript.endReachedDistance = 0;
                    }

                    aIPathScript.whenCloseToDestination = CloseToDestinationMode.Stop;
                    aIPathScript.endReachedDistance = 0;
                }
            }
        }
    }

    private GameObject GetClosestTragetInWorld()
    {
        List<GameObject> potentialTargets = new List<GameObject>();

        foreach (string tag in targetTags)
        {
            potentialTargets.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }

        return GetClosestTarget(potentialTargets);
    }

    private GameObject GetClosestTargetInRange(float radius)
    {
        Collider2D[] potentialTargetColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        List<GameObject> closeObjects = potentialTargetColliders.Select(col => col.gameObject).ToList();

        List<GameObject> potentialTargets = closeObjects.Where(g => targetTags.Contains(g.tag)).ToList();

        return GetClosestTarget(potentialTargets);
    }

    private GameObject GetClosestTarget(List<GameObject> potentialTargets)
    {
        if (potentialTargets.Contains(gameObject))
            potentialTargets.Remove(gameObject);

        GameObject nearest = null;

        Vector3 nearestPosition = Vector3.zero;
        float nearestDistance = float.PositiveInfinity;
        {
            int instancesCount = potentialTargets.Count;
            int i = 0;
            if (instancesCount > 0)
            {
                nearest = potentialTargets[0];
                nearestPosition = nearest.transform.position;
                nearestDistance = Vector3.Distance(transform.position, nearestPosition);
                i = 1;
            }
            for (; i < instancesCount; i++)
            {
                GameObject next = potentialTargets[i];
                Vector3 nextPosition = next.transform.position;
                float dist = Vector3.Distance(transform.position, nextPosition);
                if (dist < nearestDistance)
                {
                    nearest = next;
                    nearestPosition = next.transform.position;
                    nearestDistance = dist;
                }
            }
        }

        return nearest;
    }

    protected virtual void Update()
    {
        SetNewTarget();

        if (!healthComponent.isDead && target != null)
            LSUtils.RotateObjectTowards(head, target.gameObject);

        aIPathScript.maxSpeed = healthComponent.currentSpeed;

        healthBar.SetValue(healthComponent.currentHealth / healthComponent.maxHealth);

        Animation();
    }

    public virtual void Die()
    {
        foreach(Transform t in transform)
        {
            if (t.GetSiblingIndex() != transform.childCount-1 && t.gameObject != healthComponent.burnPS.gameObject)
            {
                t.gameObject.SetActive(false);
            } else
            {
                t.gameObject.SetActive(true);
            }
        }

        if (GetComponent<Rigidbody2D>() != null)
            GetComponent<RewardComponent>().GiveRewardToPlayer();

        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<BoxCollider2D>());
        Destroy(GetComponent<AIPath>());

        StartCoroutine(Disapear());
    }

    private IEnumerator Apear()
    {
        while (step < 1)
        {
            step += Time.deltaTime;

            Array.ForEach(GetComponentsInChildren<SpriteRenderer>(), renderer => renderer.material.SetFloat("_Fade", step));

            yield return null;
        }
    }

    private IEnumerator Disapear()
    {
        yield return new WaitForSeconds(1f);

        while (step > 0) 
        {
            step -= Time.deltaTime * 0.01f;

            Array.ForEach(GetComponentsInChildren<SpriteRenderer>(), renderer => renderer.material.SetFloat("_Fade", step));

            yield return null;
        }

        Destroy(gameObject);
    }

    private void Animation()
    {
        StartCoroutine(IsMoving(isMove));

        if (isMove)
        {
            animator.SetFloat("Speed", 1);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    private IEnumerator IsMoving(bool checkBool)
    {
        Vector3 lastPos = transform.position;

        yield return new WaitForSeconds(0.2f);

        Vector3 nextPos = transform.position;

        isMove = lastPos != nextPos;
    }
}
