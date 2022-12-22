using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WaveSpawnPoint: MonoBehaviour
{
    public bool isAvailable = true;
    public float checkDistance = 2f;
    public WaveSpawnPointCategories.Category spawnCategory;
    public bool debug;

    [Inject]
    private GameManager myGameManager;

    private void Start() {
        debug = myGameManager.waveSpawner.spawnPointCategories.debugSpawnPoints;

        SetActivity();
        spawnCategory.activityChanged = SetActivity;
    }

    private void SetActivity() {
        spawnCategory = myGameManager.waveSpawner.spawnPointCategories.GetCategory(transform.parent.name);
    }

    void Update()
    {
        if (NotSpawnRange(checkDistance) || !spawnCategory.isActive)
        {       
            isAvailable = false;
            GetComponent<SpriteRenderer>().enabled = false;
        } 
        else
        {
            isAvailable = true;
            GetComponent<SpriteRenderer>().enabled = true;
        }

        if (!debug)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private bool NotSpawnRange(float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D nearObjects in colliders)
        {
            if (nearObjects.transform.tag == "Player")
            {
                return true;
            }
        }

        return false;
    }
}
