using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private UnitSpawner unitSpawner;
    [SerializeField] private Transform enemySpawnPoint;

    [Header("설정")]
    [SerializeField] private bool autoStart = true;

    [Header("대형 배치")]
    [SerializeField] private float frontLineOffset = -1.5f;  // 앞줄 오프셋
    [SerializeField] private float backLineOffset = -0.5f;  // 뒷줄 오프셋
    [SerializeField] private float unitSpacing = 0.8f;      // 유닛 간 간격 (Y축)

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
            JumpToNextWave();
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
                SpawnWaveFormation();

                yield return new WaitForSeconds(currentSpawnerData.SpawnInterval);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    /// <summary>
    /// 새로운 방식: 전사 먼저 전부 생성 -> 궁수 생성 (긴 간격)
    /// 새로운 방식: 전사 앞줄, 궁수 뒷줄 대형 배치
    /// </summary>
    private void SpawnWaveFormation()
    {
        if (currentSpawnerData == null || unitSpawner == null) return;

        Vector3 basePos = enemySpawnPoint != null ? enemySpawnPoint.position : Vector3.zero;

        if (currentSpawnerData.WarriorCount > 0)
        {
            SpawnLineFormation(Enums.UnitType.Warrior, basePos, frontLineOffset, currentSpawnerData.WarriorCount, currentSpawnerData.UnitLevel);
        }

        if (currentSpawnerData.ArcherCount > 0)
        {
            SpawnLineFormation(Enums.UnitType.Archer, basePos, backLineOffset, currentSpawnerData.ArcherCount, currentSpawnerData.UnitLevel);
        }
    }

    /// <summary>
    /// 일렬로 유닛 배치
    /// </summary>
    private void SpawnLineFormation(Enums.UnitType unitType, Vector3 basePos, float xOffset, int count, int level)
    {
        // Y축 중앙 정렬을 위한 시작 위치 계산
        float totalHeight = (count - 1) * unitSpacing;
        float startY = -totalHeight / 2f;

        for (int i = 0; i < count; i++)
        {
            // 배치 위치 계산
            Vector3 spawnPos = new Vector3(basePos.x + xOffset, basePos.y + startY + (i * unitSpacing), basePos.z);

            // 유닛 생성 (즉시, 한 마리씩)
            unitSpawner.SpawnUnits(unitType, spawnPos, Team.Enemy, level, 1, 0f);
        }

        string typeName = unitType == Enums.UnitType.Warrior ? "전사" : "궁수";
    }

    /// <summary>
    /// 다음 웨이브 Index로 점프(테스트)
    /// </summary>
    private void JumpToNextWave()
    {
        int nextIndex = currentSpawnerData.Index + 1;

        EnemySpawnerData nextWave = EnemySpawnerDataManager.Instance.GetSpawnerData(nextIndex);

        if (nextWave != null)
        {
            gameElapsedTime = nextWave.TimeMinute;
        }
    }
}