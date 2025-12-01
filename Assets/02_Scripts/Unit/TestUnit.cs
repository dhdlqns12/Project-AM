using UnityEngine;

public class UnitTest : MonoBehaviour
{
    [SerializeField] private GameObject playerUnitPrefab;
    [SerializeField] private GameObject enemyUnitPrefab;

    private void Start()
    {
        // 플레이어 유닛 생성
        SpawnPlayerUnit(1, new Vector3(-5, -0.5f, 0));  // 서툰 전사

        // 적 유닛 생성
        SpawnEnemyUnit(11, new Vector3(5, -0.5f, 0));   // 적 전사
    }

    private void SpawnPlayerUnit(int dataIndex, Vector3 position)
    {
        UnitData data = TestUnitDataManager.Instance.GetUnitData(dataIndex);

        GameObject obj = Instantiate(playerUnitPrefab, position, Quaternion.identity);
        obj.layer = LayerMask.NameToLayer("PlayerUnit");

        PlayerUnit unit = obj.GetComponent<PlayerUnit>();
        unit.Init(data);
    }

    private void SpawnEnemyUnit(int dataIndex, Vector3 position)
    {
        UnitData data = TestUnitDataManager.Instance.GetUnitData(dataIndex);

        GameObject obj = Instantiate(enemyUnitPrefab, position, Quaternion.identity);
        obj.layer = LayerMask.NameToLayer("EnemyUnit");

        EnemyUnit unit = obj.GetComponent<EnemyUnit>();
        unit.Init(data);
    }

    private void Update()
    {
        // 테스트: Space 누르면 플레이어 유닛 스폰
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnPlayerUnit(1, new Vector3(-5, Random.Range(-2f, 2f), 0));
        }

        // 테스트: E 누르면 적 유닛 스폰
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnEnemyUnit(11, new Vector3(5, Random.Range(-2f, 2f), 0));
        }
    }
}