using System;
using Newtonsoft.Json;

[Serializable]
public class EnemySpawnerDataJson
{
    [JsonProperty("Index")]
    public int Index;

    [JsonProperty("Spawn_Interval")]
    public float SpawnInterval;

    [JsonProperty("Warrior_Count")]
    public int WarriorCount;

    [JsonProperty("Archer_Count")]
    public int ArcherCount;

    [JsonProperty("Time_Minute")]
    public float TimeMinute;  // 실제로는 초 단위
}