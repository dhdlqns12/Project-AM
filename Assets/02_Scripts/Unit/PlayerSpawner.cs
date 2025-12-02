using System.Collections.Generic;
using UnityEngine;
using _02_Scripts.Building;

public class PlayerSpawner : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private UnitSpawner unitSpawner;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("생성 설정")]
    [SerializeField] private bool useFormationSpawn = false;

    [Header("순차 생성 설정")]
    [SerializeField] private float sequentialSpawnInterval = 0.3f;

    [Header("대형 배치 설정")]
    [SerializeField] private float frontLineOffset = -0.5f;
    [SerializeField] private float backLineOffset = 1.5f;
    [SerializeField] private float unitSpacing = 0.8f;


    // 배치된 건물들과 생산 타이머 관리
    private Dictionary<BuildingEntity, float> buildingTimers = new Dictionary<BuildingEntity, float>();

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
        UpdateBuildingProduction();
    }

    /// <summary>
    /// 건물 생성 시 - 타이머 등록
    /// </summary>
    private void OnBuildingConstructed(List<BuildingEntity> buildings)
    {
        if (buildings == null || buildings.Count == 0)
        {
            Debug.LogError("BuildingEntity 리스트가 비었습니다!");
            return;
        }

        Debug.Log($"건물 생성 감지: {buildings.Count}개");

        foreach (var building in buildings)
        {
            RegisterBuilding(building);
        }
    }

    /// <summary>
    /// 건물 등록 및 즉시 첫 유닛 생성
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
            Debug.Log($"{building.BuildingName}: 유닛 생산 건물 아님");
            return;
        }

        if (!building.UnitPerCycle.HasValue || building.UnitPerCycle.Value <= 0)
        {
            Debug.Log($"{building.BuildingName}: 생성할 유닛 개수 없음");
            return;
        }

        if (!building.UnitProductionCycle.HasValue || building.UnitProductionCycle.Value <= 0)
        {
            Debug.Log($"{building.BuildingName}: 생산 주기 없음");
            return;
        }

        if (!buildingTimers.ContainsKey(building))
        {
            buildingTimers[building] = 0f;  // 처음엔 0초 (즉시 생산)
            Debug.Log($"건물 등록: {building.BuildingName} (Lv.{building.BuildingLevel}, 주기: {building.UnitProductionCycle}초)");
        }
    }

    /// <summary>
    /// 건물 파괴 시 - 타이머 제거
    /// </summary>
    private void OnBuildingDestroyed(BuildingEntity building)
    {
        if (building == null) return;

        if (buildingTimers.ContainsKey(building))
        {
            buildingTimers.Remove(building);
            Debug.Log($"건물 제거: {building.BuildingName}");
        }
    }

    /// <summary>
    /// 모든 건물의 생산 타이머 업데이트
    /// </summary>
    private void UpdateBuildingProduction()
    {
        // Dictionary를 순회하면서 타이머 업데이트
        List<BuildingEntity> buildingsToUpdate = new List<BuildingEntity>(buildingTimers.Keys);

        foreach (var building in buildingsToUpdate)
        {
            if (building == null || !building.UnitProductionCycle.HasValue) continue;

            // 타이머 증가
            buildingTimers[building] += Time.deltaTime;

            // 생산 주기 도달 시 유닛 생성
            if (buildingTimers[building] >= building.UnitProductionCycle.Value)
            {
                ProduceUnits(building);
                buildingTimers[building] = 0f;  // 타이머 리셋
            }
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

        Debug.Log($"{building.BuildingName} 유닛 생산: {unitType} Lv.{buildingLevel} x{unitCount}");

        if (useFormationSpawn)
        {
            SpawnUnitsFormation(unitType, spawnPosition, buildingLevel, unitCount);
        }
        else
        {
            SpawnUnitsSequential(unitType, spawnPosition, buildingLevel, unitCount);
        }
    }


    /// <summary>
    /// 순차 생성
    /// </summary>
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

    /// <summary>
    /// 대형 배치
    /// </summary>
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
    /// 디버그용: 현재 등록된 건물 정보
    /// </summary>
    private void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 500));
        GUILayout.Label($"생산 중인 건물: {buildingTimers.Count}개");

        foreach (var kvp in buildingTimers)
        {
            BuildingEntity building = kvp.Key;
            float timer = kvp.Value;

            if (building != null && building.UnitProductionCycle.HasValue)
            {
                float cycle = building.UnitProductionCycle.Value;
                GUILayout.Label($"{building.BuildingName}: {timer:F1}/{cycle:F1}초");
            }
        }

        GUILayout.EndArea();
    }
}
