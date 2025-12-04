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
    [SerializeField] private GameObject unitPool;

    [Header("스폰 설정")]
    [SerializeField] private Transform playerSpawnPoint;  // 플레이어 넥서스 중앙(테스트 용 나중에 삭제)
    [SerializeField] private Transform enemySpawnPoint; // 테스트용 나중에 삭제

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
    /// <param name="mSpawnInterval">스폰 간격 설정</param>
    public void SpawnUnits(Enums.UnitType unitType, Vector3 spawnPosition, Team team, int level, int count, float mSpawnInterval = 0f)
    {
        StartCoroutine(SpawnUnitsWithDelay(unitType, spawnPosition, team, level, count, mSpawnInterval));
    }

    /// <summary>
    /// 시간차를 두고 유닛 생성
    /// </summary>
    private IEnumerator SpawnUnitsWithDelay(Enums.UnitType unitType, Vector3 spawnPosition, Team team, int level, int count, float interval)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.3f, 0.3f), 0);
            Vector3 finalPosition = spawnPosition + offset;

            SpawnUnit(unitType, finalPosition, team, level);

            if (i < count - 1)
            {
                yield return new WaitForSeconds(interval);
            }
        }
    }

    /// <summary>
    /// 단일 유닛 생성
    /// </summary>
    private void SpawnUnit(Enums.UnitType unitType, Vector3 position, Team team, int level)
    {
        UnitData unitData = FindUnitData(unitType, team, level);

        if (unitData == null)
        {
            Debug.LogError($"유닛 데이터 없음: {unitType}, {team}, Lv.{level}");
            return;
        }

        Debug.Log($"유닛 데이터 찾음: {unitData.Name}, Lv.{unitData.Level}, HP:{unitData.HP}, ATK:{unitData.Attack}");

        GameObject prefab = team == Team.Player ? playerUnitPrefab : enemyUnitPrefab;

        GameObject unitObj = Instantiate(prefab, position, Quaternion.identity, unitPool.transform);
        unitObj.layer = team == Team.Player ? LayerMask.NameToLayer("PlayerUnit") : LayerMask.NameToLayer("EnemyUnit");

        if (unitObj.TryGetComponent<UnitBase>(out UnitBase unit))
        {
            unit.Init(unitData);
        }
        else
        {
            Debug.LogError("UnitBase 컴포넌트 없음!");
            Destroy(unitObj);
        }
    }

    /// <summary>
    /// 타입, 팀, 레벨로 유닛 데이터 찾기
    /// </summary>
    private UnitData FindUnitData(Enums.UnitType unitType, Team team, int level)
    {
        int baseIndex = team == Team.Player ? 1 : 11;

        if (unitType == Enums.UnitType.Archer)
        {
            baseIndex += 3;
        }

        int finalIndex = baseIndex + (level - 1);

        Debug.Log($"FindUnitData: {unitType}, {team}, Lv.{level} -> Index {finalIndex}");

        return UnitDataManager.Instance.GetUnitData(finalIndex);
    }

    /// <summary>
    /// 기본 넥서스 위치에서 스폰 (편의 메서드)
    /// </summary>
    public void SpawnFromNexus(Enums.UnitType unitType, Team team, float statMultiplier, int count)
    {
        if (playerSpawnPoint == null)
        {
            Debug.LogError("playerSpawnPoint가 설정되지 않았습니다!");
            return;
        }

        Vector3 spawnPos = playerSpawnPoint.position;
        SpawnUnits(unitType, spawnPos, team, 1, count);  // Lv1으로 고정
    }
}

