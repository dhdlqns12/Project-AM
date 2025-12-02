using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawnerDataManager : Singleton<EnemySpawnerDataManager>
{
    private List<EnemySpawnerData> spawnerDataList = new List<EnemySpawnerData>();

    protected override void Init()
    {
        LoadEnemySpawnerData();
    }

    private void LoadEnemySpawnerData()
    {
        spawnerDataList.Clear();

        EnemySpawnerDataJson[] jsonArray = ResourceManager.LoadJsonDataList<EnemySpawnerDataJson>("EnemySpawner");

        if (jsonArray == null || jsonArray.Length == 0)
        {
            Debug.LogError("EnemySpawnerData 로드 실패!");
            return;
        }

        foreach (var json in jsonArray)
        {
            EnemySpawnerData data = new EnemySpawnerData(json);
            spawnerDataList.Add(data);
        }

        // 시간(초) 순으로 정렬
        spawnerDataList = spawnerDataList.OrderBy(d => d.TimeSeconds).ToList();

        Debug.Log($"적 스포너 데이터: {spawnerDataList.Count}개");

        foreach (var data in spawnerDataList)
        {
            Debug.Log($"Index {data.Index}: {data.TimeMinutes:F1}분({data.TimeSeconds}초), Lv{data.UnitLevel}, 주기 {data.SpawnInterval}초");
        }
    }

    /// <summary>
    /// 현재 게임 시간(초)에 맞는 스포너 데이터 가져오기
    /// </summary>
    public EnemySpawnerData GetSpawnerDataByTime(float elapsedSeconds)
    {
        EnemySpawnerData result = null;

        foreach (var data in spawnerDataList)
        {
            if (data.TimeSeconds <= elapsedSeconds)
            {
                result = data;
            }
            else
            {
                break;
            }
        }

        return result ?? spawnerDataList[0];
    }

    public EnemySpawnerData GetSpawnerData(int index)
    {
        return spawnerDataList.FirstOrDefault(d => d.Index == index);
    }

    public List<EnemySpawnerData> GetAllSpawnerData()
    {
        return spawnerDataList;
    }
}