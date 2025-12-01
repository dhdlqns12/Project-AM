using System.Collections;
using UnityEngine;


/// <summary>
/// 유닛 생성 담당 (건물/스포너로부터 요청 받음)
/// </summary>
public class UnitSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerUnitPrefab;
    [SerializeField] private GameObject enemyUnitPrefab;

    [Header("스폰 설정")]
    [SerializeField] private Transform playerSpawnPoint;  // 플레이어 넥서스 중앙
    [SerializeField] private Transform enemySpawnPoint;   // 적 넥서스 중앙

    [Header("스폰 간격 설정")]
    [SerializeField] private float spawnInterval;

    /// <summary>
    /// 유닛 생성 요청 (외부에서 호출)
    /// </summary>
    /// <param name="unitType">유닛 타입 (Warrior/Archer)</param>
    /// <param name="spawnPosition">생성 위치</param>
    /// <param name="team">팀 구분 (Player/Enemy)</param>
    /// <param name="statMultiplier">스탯 배율 (건물 레벨에 따라)</param>
    /// <param name="count">생성 개수</param>
    public void SpawnUnits(Enums.UnitType unitType, Vector3 spawnPosition, Team team, float statMultiplier, int count, float mSpawnInterval)
    {
        StartCoroutine(SpawnUnitsWithDelay(unitType, spawnPosition, team, statMultiplier, count, mSpawnInterval));
    }


    /// <summary>
    /// 시간차를 두고 유닛 생성
    /// </summary>
    private IEnumerator SpawnUnitsWithDelay(Enums.UnitType unitType, Vector3 spawnPosition, Team team, float statMultiplier, int count, float mSpawnInterval)
    {
        for (int i = 0; i < count; i++)
        {
            // 랜덤 오프셋 (겹치지 않게)
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.3f, 0.3f), 0);
            Vector3 finalPosition = spawnPosition + offset;

            // 유닛 생성
            SpawnUnit(unitType, finalPosition, team, statMultiplier);

            // 다음 유닛까지 대기 (마지막 유닛은 대기 안 함)
            if (i < count - 1)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    /// <summary>
    /// 단일 유닛 생성
    /// </summary>
    private void SpawnUnit(Enums.UnitType unitType, Vector3 position, Team team, float statMultiplier)
    {
        // 1. 유닛 데이터 찾기 (타입 + 팀으로)
        UnitData baseData = FindUnitData(unitType, team);

        if (baseData == null)
        {
            Debug.LogError($"유닛 데이터 없음: {unitType}, {team}");
            return;
        }

        // 2. 프리팹 선택
        GameObject prefab = team == Team.Player ? playerUnitPrefab : enemyUnitPrefab;

        // 3. 유닛 생성
        GameObject unitObj = Instantiate(prefab, position, Quaternion.identity);
        unitObj.layer = team == Team.Player ? LayerMask.NameToLayer("PlayerUnit") : LayerMask.NameToLayer("EnemyUnit");

        // 4. 유닛 초기화 (스탯 배율 적용)
        UnitBase unit = unitObj.GetComponent<UnitBase>();

        if (unit != null)
        {
            // 스탯 배율 적용된 데이터 생성
            UnitData scaledData = ApplyStatMultiplier(baseData, statMultiplier);
            unit.Init(scaledData);

            Debug.Log($"유닛 생성: {scaledData.Name} (배율: {statMultiplier}x)");
        }
        else
        {
            Debug.LogError("UnitBase 컴포넌트 없음!");
            Destroy(unitObj);
        }
    }

    /// <summary>
    /// 타입과 팀으로 유닛 데이터 찾기
    /// </summary>
    private UnitData FindUnitData(Enums.UnitType unitType, Team team)
    {
        // 플레이어: Index 1~6
        // 적: Index 11~16
        int baseIndex = team == Team.Player ? 1 : 11;

        // Warrior: 1, 11
        // Archer: 4, 14
        if (unitType == Enums.UnitType.Archer)
        {
            baseIndex += 3;
        }

        return UnitDataManager.Instance.GetUnitData(baseIndex);
    }

    /// <summary>
    /// 스탯 배율 적용 (건물 레벨에 따라)
    /// </summary>
    private UnitData ApplyStatMultiplier(UnitData baseData, float multiplier)
    {
        // 원본 데이터 복사 후 스탯만 배율 적용
        var multipliedStat = new UnitDataJson
        {
            Index = baseData.Index,
            UnitName = baseData.Name,
            UnitJobName = baseData.JobName,
            UnitType = baseData.Type,
            UnitLevel = baseData.Level,

            //스탯에 배율 적용
            UnitAttack = Mathf.RoundToInt(baseData.Attack * multiplier),
            UnitAttackRange = baseData.AttackRange,  // 사거리는 고정
            UnitAttackSpeed = baseData.AttackSpeed,  // 공속도 고정
            UnitDefense = Mathf.RoundToInt(baseData.Defense * multiplier),
            UnitHP = Mathf.RoundToInt(baseData.HP * multiplier),
            UnitMoveSpeed = baseData.MoveSpeed  // 이속도 고정
        };

        return new UnitData(multipliedStat);
    }

    /// <summary>
    /// 기본 넥서스 위치에서 스폰 (편의 메서드)
    /// </summary>
    public void SpawnFromNexus(Enums.UnitType unitType, Team team, float statMultiplier, int count)
    {
        Vector3 spawnPos = team == Team.Player ? playerSpawnPoint.position : enemySpawnPoint.position;
        SpawnUnits(unitType, spawnPos, team, statMultiplier, count, 0.2f);
    }
}

