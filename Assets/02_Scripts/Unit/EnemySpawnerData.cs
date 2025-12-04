using UnityEngine;

public class EnemySpawnerData
{
    public int Index { get; private set; }
    public float SpawnInterval { get; private set; }
    public int WarriorCount { get; private set; }
    public int ArcherCount { get; private set; }

    public float TimeMinute { get; private set; }

    public int UnitLevel { get; private set; }

    public EnemySpawnerData(EnemySpawnerDataJson json)
    {
        Index = json.Index;
        SpawnInterval = Mathf.Max(0.1f, json.SpawnInterval);
        WarriorCount = Mathf.Max(0, json.WarriorCount);
        ArcherCount = Mathf.Max(0, json.ArcherCount);

        TimeMinute = json.TimeMinute;
    }

    /// <summary>
    /// 전체 웨이브 데이터를 기준으로 레벨 계산
    /// </summary>
    public void CalculateLevel(System.Collections.Generic.List<EnemySpawnerData> allWaves)
    {
        UnitLevel = 1;  // 기본 레벨

        int waveIndex = allWaves.IndexOf(this);

        if (waveIndex >= 0)
        {
            UnitLevel = waveIndex + 1;
            UnitLevel = Mathf.Clamp(UnitLevel, 1, 3);  // 최대 레벨 3
        }
    }
}