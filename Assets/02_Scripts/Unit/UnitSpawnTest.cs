using UnityEngine;

public class UnitSpawnTest : MonoBehaviour
{
    [SerializeField] private UnitSpawner spawner;

    private void Update()
    {
        // Space: 전사 5마리 시간차 생성
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spawner.SpawnFromNexus(Enums.UnitType.Warrior, Team.Player, 1.0f, 5);
        }
    }
}