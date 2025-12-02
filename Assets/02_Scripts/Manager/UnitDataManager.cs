using System.Collections.Generic;
using UnityEngine;

public class UnitDataManager : Singleton<UnitDataManager>
{
    private Dictionary<int, UnitData> unitDataDictionary = new Dictionary<int, UnitData>();

    protected override void Init()
    {
        LoadPlayerUnitData();
        LoadEnemyUnitData();
    }

    private void LoadPlayerUnitData()
    {
        unitDataDictionary.Clear();

        UnitDataJson[] jsonArray = ResourceManager.LoadJsonDataList<UnitDataJson>("UnitData");

        if (jsonArray == null || jsonArray.Length == 0)
        {
            Debug.LogError("UnitData 로드 실패!");
            return;
        }

        foreach (var json in jsonArray)
        {
            UnitData data = new UnitData(json);
            unitDataDictionary.Add(data.Index, data);
        }

        Debug.Log($"플레이어 유닛: {unitDataDictionary.Count}개");
    }

    private void LoadEnemyUnitData()
    {
        UnitDataJson[] jsonArray = ResourceManager.LoadJsonDataList<UnitDataJson>("EnemyData");

        if (jsonArray == null || jsonArray.Length == 0)
        {
            Debug.LogError("EnemyUnitData 로드 실패!");
            return;
        }

        foreach (var json in jsonArray)
        {
            UnitData data = new UnitData(json);
            unitDataDictionary.Add(data.Index, data);
        }

        Debug.Log($"적 유닛: {jsonArray.Length}개");
    }


    /// <summary>
    /// Index로 유닛 데이터 가져오기
    /// </summary>
    public UnitData GetUnitData(int index)
    {
        if (unitDataDictionary.TryGetValue(index, out UnitData data))
            return data;

        Debug.LogError($"유닛 데이터 없음: Index {index}");
        return null;
    }
}
