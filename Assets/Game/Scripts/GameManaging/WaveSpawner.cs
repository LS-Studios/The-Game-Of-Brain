using System;
using UnityEngine;
using System.Collections;
using Zenject;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour {

	[Inject]
	private GameManager myGameManager;

	public enum SpawnState { SPAWNING, WAITING, COUNTING };

	public Transform normalEnemy;
	public Transform specialEnemy;
	public Transform bosEnemy;

	public int startCount;
	public int maxCount;

	public float rate;

	private int enemyCountIncrease = 0;

	private int nextWave = 0;
	public int NextWave
	{
		get { return nextWave + 1; }
	}

	public WaveSpawnPointCategories spawnPointCategories;
	public List<WaveSpawnPoint> spawnPoints = new List<WaveSpawnPoint>();

	public float startDelay = 10f;
	public float timeBetweenWaves = 5f;
	private float waveCountdown;
	public float WaveCountdown
	{
		get { return waveCountdown; }
		set { waveCountdown = value; }
	}

	private float searchCountdown = 1f;

	private SpawnState state = SpawnState.COUNTING;
	public SpawnState State
	{
		get { return state; }
	}

	void Start()
	{
		if (spawnPointCategories != null)
        {
			foreach (WaveSpawnPointCategories.Category spawnPointCategory in spawnPointCategories.spawnPointCategorys) {
				foreach (Transform waveSpawnPointTransform in spawnPointCategory.spawnPointsHolder)
				{
					WaveSpawnPoint waveSpawnPoint = waveSpawnPointTransform.GetComponent<WaveSpawnPoint>();
					waveSpawnPoint.debug = spawnPointCategories.debugSpawnPoints;
					spawnPoints.Add(waveSpawnPoint);
				}
            }
        }

		switch (GameInstance.instance.inGameValues.difficulty)
		{
			case GameInstance.InGameValues.Difficulty.Easy:
				rate = 1;
				break;

			case GameInstance.InGameValues.Difficulty.Medium:
				rate = 2;
				break;

			case GameInstance.InGameValues.Difficulty.Hard:
				rate = 3;
				break;

			case GameInstance.InGameValues.Difficulty.Insane:
				rate = 4;
				break;
		}

		waveCountdown = startDelay;
	}

	void Update()
	{
		if (state == SpawnState.WAITING)
		{
			if (!EnemyIsAlive())
			{
				WaveCompleted();
			}
			else
			{
				return;
			}
		}

		if (waveCountdown <= 0)
		{
			if (state != SpawnState.SPAWNING)
			{
				StartCoroutine(SpawnWave());
			}
		}
		else
		{
			waveCountdown -= Time.deltaTime;
		}
	}

	void WaveCompleted()
	{
		if (GetComponent<ChallengeComponent>() != null)
			GetComponent<ChallengeComponent>().AddChalangeProgress();

		state = SpawnState.COUNTING;
		waveCountdown = timeBetweenWaves;

		if ((nextWave+1) % 2 == 0)
			enemyCountIncrease++;

		nextWave++;
	}

	bool EnemyIsAlive()
	{
		searchCountdown -= Time.deltaTime;
		if (searchCountdown <= 0f)
		{
			searchCountdown = 1f;
			if (GameObject.FindGameObjectWithTag("Enemy") == null)
			{
				return false;
			}
		}
		return true;
	}
	IEnumerator SpawnWave()
	{
		myGameManager.survivedWaves++;		
		state = SpawnState.SPAWNING;

		int currentCount = 0;

		if ((nextWave+1) % 10 == 0 && bosEnemy != null)
        {
			//Bos enemy
			for (int i = 0; i < (nextWave + 1) / 10; i++)
			{
				SpawnEnemy(bosEnemy);
			}
		} 
		else if ((nextWave+1) % 5 == 0 && specialEnemy != null)
		{
			//Special enemy
			while (currentCount < (startCount + enemyCountIncrease) * 1.5f && currentCount < maxCount)
			{
				currentCount++;

				SpawnEnemy(specialEnemy);
				yield return new WaitForSeconds(1f / rate);
			}
		} 
		else if (normalEnemy != null)
        {
			//Normal enemy
			while (currentCount < startCount + enemyCountIncrease && currentCount < maxCount)
			{
				currentCount++;

				SpawnEnemy(normalEnemy);
				yield return new WaitForSeconds(1f / rate);
			}
		}

		state = SpawnState.WAITING;

		yield break;
	}

	void SpawnEnemy(Transform _enemy)
	{
		List<WaveSpawnPoint> availableSpawnPoints = spawnPoints.FindAll(spawnPoint => spawnPoint.isAvailable);
		Transform _sp = availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)].transform;

		Transform enemy = Instantiate(_enemy, _sp.position, Quaternion.identity);

		Enemy enemyComponent = enemy.GetComponent<Enemy>();
		HealtComponent enemyHealth = enemy.GetComponent<HealtComponent>();

		float healthToAdd = 0;
		float damageToAdd = 0;

		switch (GameInstance.instance.inGameValues.difficulty)
		{
			case GameInstance.InGameValues.Difficulty.Easy:
				healthToAdd = nextWave * 2;
				damageToAdd = nextWave * 0.2f;
				break;

			case GameInstance.InGameValues.Difficulty.Medium:
				healthToAdd = nextWave * 2.5f;
				damageToAdd = nextWave * 0.4f;
				break;

			case GameInstance.InGameValues.Difficulty.Hard:
				healthToAdd = nextWave * 3;
				damageToAdd = nextWave * 0.6f;
				break;

			case GameInstance.InGameValues.Difficulty.Insane:
				healthToAdd = nextWave * 4;
				damageToAdd = nextWave * 0.8f;
				break;
		}

		enemyComponent.damage += damageToAdd;
		enemyHealth.startHealthAddValue += healthToAdd;
	}
}
