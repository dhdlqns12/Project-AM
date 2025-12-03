using System.Collections.Generic;
using UnityEngine;
using _02_Scripts.Building;

public class PlayerSpawner : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private UnitSpawner unitSpawner;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("웨이브 설정")]
    [SerializeField] private float waveInterval = 30f;  // 웨이브 주기 (30초)
    [SerializeField] private bool autoStart = true;
    private float waveTimer = 0f;
    private int waveCount = 0;

    [Header("순차 생성 설정")]
    [SerializeField] private float sequentialSpawnInterval = 0.3f;

    [Header("대형 배치 설정")]
    [SerializeField] private float frontLineOffset = 1.5f;
    [SerializeField] private float backLineOffset = -0.5f;
    [SerializeField] private float unitSpacing = 0.8f;

    private List<BuildingEntity> activeBuildings = new List<BuildingEntity>();

    private void Start()
    {
        if (autoStart)
        {
            waveTimer = 0f;
        }
    }

    private void OnEnable()
    {
        BuildingEvents.OnBuildingConstructed += OnBuildingConstructed;
        BuildingEvents.OnBuildingDestroyed += OnBuildingDestroyed;
    }

    private void OnDisable()
    {
        BuildingEvents.OnBuildingConstructed -= OnBuildingConstructed;
        BuildingEvents.OnBuildingDestroyed -= OnBuildingDestroyed;
    }

    private void Update()
    {
        waveTimer += Time.deltaTime;

        // 테스트용 시간 스킵
        if (Input.GetKeyDown(KeyCode.P))
        {
            waveTimer += waveInterval;
        }

        if (waveTimer >= waveInterval)
        {
            TriggerWave();
            waveTimer = 0f;
        }
    }

    private void TriggerWave()
    {
        waveCount++;

        if (activeBuildings.Count == 0)
        {
            return;
        }

        foreach (var building in activeBuildings)
        {
            if (building != null)
            {
                ProduceUnits(building);
            }
        }
    }

    /// <summary>
    /// 건물 등록
    /// </summary>
    private void OnBuildingConstructed(List<BuildingEntity> buildings)
    {
        if (buildings == null || buildings.Count == 0)
        {
            return;
        }

        foreach (var building in buildings)
        {
            RegisterBuilding(building);
        }
    }

    /// <summary>
    /// 건물 등록
    /// </summary>
    private void RegisterBuilding(BuildingEntity building)
    {
        if (building == null)
        {
            Debug.LogWarning("BuildingEntity가 null");
            return;
        }

        // 유닛 생산 건물인지 확인
        if (!building.ProductionUnitType.HasValue)
        {
            return;
        }

        if (!building.UnitPerCycle.HasValue || building.UnitPerCycle.Value <= 0)
        {
            return;
        }

        if (!activeBuildings.Contains(building))
        {
            activeBuildings.Add(building);
        }
    }

    /// <summary>
    /// 건물 제거
    /// </summary>
    private void OnBuildingDestroyed(BuildingEntity building)
    {
        if (building == null) return;

        if (activeBuildings.Contains(building))
        {
            activeBuildings.Remove(building);
        }
    }

    /// <summary>
    /// 유닛 생산
    /// </summary>
    private void ProduceUnits(BuildingEntity building)
    {
        if (!building.ProductionUnitType.HasValue || !building.UnitPerCycle.HasValue)
            return;

        Enums.UnitType unitType = building.ProductionUnitType.Value;
        int buildingLevel = building.BuildingLevel;
        int unitCount = building.UnitPerCycle.Value;

        Vector3 spawnPosition = playerSpawnPoint != null
            ? playerSpawnPoint.position
            : Vector3.zero;

        SpawnUnitsFormation(unitType, spawnPosition, buildingLevel, unitCount);
    }

    private void SpawnUnitsSequential(Enums.UnitType unitType, Vector3 position, int level, int count)
    {
        unitSpawner.SpawnUnits(
            unitType,
            position,
            Team.Player,
            level,
            count,
            sequentialSpawnInterval
        );
    }


    private void SpawnUnitsFormation(Enums.UnitType unitType, Vector3 position, int level, int count)
    {
        float totalWidth = (count - 1) * unitSpacing;
        float startY = -totalWidth / 2f;
        float xOffset = unitType == Enums.UnitType.Warrior ? frontLineOffset : backLineOffset;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = new Vector3(
                position.x + xOffset,
                position.y + startY + (i * unitSpacing),
                position.z
            );

            unitSpawner.SpawnUnits(
                unitType,
                spawnPos,
                Team.Player,
                level,
                1,
                0f
            );
        }
    }

    /// <summary>
    /// 디버그 UI
    /// </summary>
    private void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.BeginArea(new Rect(10, 10, 350, 200));

        GUILayout.Label($"아군 웨이브 시스템");
        GUILayout.Label($"다음 웨이브까지: {(waveInterval - waveTimer):F1}초");
        GUILayout.Label($"웨이브 #{waveCount}");
        GUILayout.Label($"배치된 건물: {activeBuildings.Count}개");

        GUILayout.Space(10);

        foreach (var building in activeBuildings)
        {
            if (building != null && building.UnitPerCycle.HasValue)
            {
                GUILayout.Label($"{building.BuildingName} (웨이브당 {building.UnitPerCycle}마리)");
            }
        }

        GUILayout.EndArea();
    }

    /// <summary>
    /// 현재 웨이브 시간 정보
    /// </summary>
    public string GetWaveTimeString()
    {
        float remainingTime = waveInterval - waveTimer;
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}

