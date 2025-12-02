using UnityEngine;

public class EnemySpawnerData
{
    public int Index { get; private set; }
    public float SpawnInterval { get; private set; }
    public int WarriorCount { get; private set; }
    public int ArcherCount { get; private set; }

    public float TimeSeconds { get; private set; }
    public float TimeMinutes => TimeSeconds / 60f;

    public int UnitLevel { get; private set; }

    public EnemySpawnerData(EnemySpawnerDataJson json)
    {
        Index = json.Index;
        SpawnInterval = Mathf.Max(0.1f, json.SpawnInterval);
        WarriorCount = Mathf.Max(0, json.WarriorCount);
        ArcherCount = Mathf.Max(0, json.ArcherCount);

        TimeSeconds = json.TimeMinute;

        UnitLevel = CalculateUnitLevel(TimeSeconds);
    }

    private int CalculateUnitLevel(float seconds)
    {
        if (seconds >= 600f) return 3;
        if (seconds >= 300f) return 2;
        return 1;
    }
}