using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private UnitSpawner unitSpawner;
    [SerializeField] private Transform enemySpawnPoint;

    [Header("설정")]
    [SerializeField] private bool autoStart = true;

    private EnemySpawnerData currentSpawnerData;
    private float gameElapsedTime = 0f;
    private Coroutine spawnCoroutine;

    private void Start()
    {
        if (autoStart)
        {
            StartSpawning();
        }
    }

    private void Update()
    {
        gameElapsedTime += Time.deltaTime;

        // 테스트용
        if (Input.GetKeyDown(KeyCode.T))
        {
            gameElapsedTime += 300f;
            Debug.Log($"시간 스킵 현재: {GetGameTimeString()}");
        }

        UpdateSpawnerData();
    }

    public void StartSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        gameElapsedTime = 0f;
        UpdateSpawnerData();
        spawnCoroutine = StartCoroutine(SpawnRoutine());

        Debug.Log("적 스폰 시작");
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private void UpdateSpawnerData()
    {
        EnemySpawnerData newData = EnemySpawnerDataManager.Instance.GetSpawnerDataByTime(gameElapsedTime);

        if (newData != currentSpawnerData)
        {
            currentSpawnerData = newData;

            float minutes = gameElapsedTime / 60f;
            Debug.Log($"스포너 데이터 변경 (시간: {minutes:F1}분, 레벨: {currentSpawnerData.UnitLevel})");

            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = StartCoroutine(SpawnRoutine());
            }
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (currentSpawnerData != null)
            {
                SpawnEnemyWave();
                yield return new WaitForSeconds(currentSpawnerData.SpawnInterval);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private void SpawnEnemyWave()
    {
        if (currentSpawnerData == null || unitSpawner == null) return;

        Vector3 spawnPos = enemySpawnPoint != null ? enemySpawnPoint.position : Vector3.zero;

        // 전사 스폰
        if (currentSpawnerData.WarriorCount > 0)
        {
            unitSpawner.SpawnUnits(Enums.UnitType.Warrior, spawnPos, Team.Enemy, currentSpawnerData.UnitLevel, currentSpawnerData.WarriorCount, 0.3f);
        }

        // 궁수 스폰
        if (currentSpawnerData.ArcherCount > 0)
        {
            unitSpawner.SpawnUnits(Enums.UnitType.Archer, spawnPos, Team.Enemy, currentSpawnerData.UnitLevel, currentSpawnerData.ArcherCount, 0.3f);
        }

        Debug.Log($"적 웨이브! (Lv.{currentSpawnerData.UnitLevel}, 전사: {currentSpawnerData.WarriorCount}, 궁수: {currentSpawnerData.ArcherCount})");
    }

    private int GetEnemyUnitIndex(Enums.UnitType unitType, int level)
    {
        int baseIndex = 11;

        if (unitType == Enums.UnitType.Archer)
        {
            baseIndex = 14;
        }

        return baseIndex + (level - 1);
    }

    public string GetGameTimeString()
    {
        int minutes = Mathf.FloorToInt(gameElapsedTime / 60f);
        int seconds = Mathf.FloorToInt(gameElapsedTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}