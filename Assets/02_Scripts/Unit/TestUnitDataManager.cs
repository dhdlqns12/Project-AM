using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestUnitDataManager : MonoBehaviour
{
    private static TestUnitDataManager instance;
    public static TestUnitDataManager Instance => instance;

    private Dictionary<int, UnitData> unitDataDictionary = new Dictionary<int, UnitData>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CreateDummyData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 테스트용 더미 데이터 생성
    /// </summary>
    private void CreateDummyData()
    {
        // 서툰 전사
        AddDummyUnit(1, "서툰 전사", "전사", Enums.UnitType.Warrior, 1,
            attack: 10, attackRange: 1, attackSpeed: 1.5f,
            defense: 5, hp: 100, moveSpeed: 1.0f);

        // 숙련된 전사
        AddDummyUnit(2, "숙련된 전사", "전사", Enums.UnitType.Warrior, 2,
            attack: 12, attackRange: 1, attackSpeed: 1.5f,
            defense: 6, hp: 120, moveSpeed: 1.0f);

        // 정예 전사
        AddDummyUnit(3, "정예 전사", "전사", Enums.UnitType.Warrior, 3,
            attack: 15, attackRange: 2, attackSpeed: 1.5f,
            defense: 8, hp: 150, moveSpeed: 1.0f);

        // 서툰 궁수
        AddDummyUnit(4, "서툰 궁수", "궁수", Enums.UnitType.Archer, 1,
            attack: 15, attackRange: 3, attackSpeed: 1.0f,
            defense: 2, hp: 50, moveSpeed: 1.2f);

        // 숙련된 궁수
        AddDummyUnit(5, "숙련된 궁수", "궁수", Enums.UnitType.Archer, 2,
            attack: 18, attackRange: 3, attackSpeed: 1.0f,
            defense: 2, hp: 60, moveSpeed: 1.2f);

        // 정예 궁수
        AddDummyUnit(6, "정예 궁수", "궁수", Enums.UnitType.Archer, 3,
            attack: 23, attackRange: 5, attackSpeed: 1.0f,
            defense: 3, hp: 75, moveSpeed: 1.2f);

        Debug.Log($"Loaded {unitDataDictionary.Count} dummy unit data");
    }

    private void AddDummyUnit(int index, string name, string jobName, Enums.UnitType type, int level,
        int attack, int attackRange, float attackSpeed, int defense, int hp, float moveSpeed)
    {
        var json = new UnitDataJson
        {
            Index = index,
            UnitName = name,
            UnitJobName = jobName,
            UnitType = type,
            UnitLevel = level,
            UnitAttack = attack,
            UnitAttackRange = attackRange,
            UnitAttackSpeed = attackSpeed,
            UnitDefense = defense,
            UnitHP = hp,
            UnitMoveSpeed = moveSpeed
        };

        var data = new UnitData(json);
        unitDataDictionary.Add(index, data);
    }

    public UnitData GetUnitData(int index)
    {
        if (unitDataDictionary.TryGetValue(index, out UnitData data))
            return data;

        Debug.LogError($"❌ [TEST] Unit data not found: Index {index}");
        return null;
    }

    public List<UnitData> GetUnitsByType(Enums.UnitType type)
    {
        return unitDataDictionary.Values.Where(u => u.Type == type).ToList();
    }

    public List<UnitData> GetUnitsByJob(string jobName)
    {
        return unitDataDictionary.Values.Where(u => u.JobName == jobName).ToList();
    }

    public UnitData GetNextLevelUnit(int currentIndex)
    {
        var currentUnit = GetUnitData(currentIndex);
        if (currentUnit == null) return null;

        return unitDataDictionary.Values.FirstOrDefault(u => u.JobName == currentUnit.JobName && u.Level == currentUnit.Level + 1);
    }
}
