using UnityEngine;
using static Enums;

public class UnitData
{
    public int Index { get; private set; }
    public Team Team { get; private set; }
    public string Name { get; private set; }
    public string JobName { get; private set; } // 적 유닛은 null
    public UnitType Type { get; private set; }
    public int Level { get; private set; }

    public int Attack { get; private set; }
    public float AttackRange { get; private set; }
    public float AttackSpeed { get; private set; }
    public int Defense { get; private set; }
    public int HP { get; private set; }
    public float MoveSpeed { get; private set; }

    /// <summary>
    /// 유닛 데이터 생성자
    /// </summary>
    public UnitData(UnitDataJson json)
    {
        Index = json.Index;
        Team = Index >= 11 ? Team.Enemy : Team.Player;
        Name = json.UnitName;
        JobName = json.UnitJobName;
        Level = json.UnitLevel;
        Type = json.UnitType; 

        // 데이터 검증
        Attack = Mathf.Max(0, json.UnitAttack);
        AttackRange = Mathf.Max(1, json.UnitAttackRange);
        AttackSpeed = Mathf.Max(0.1f, json.UnitAttackSpeed);
        Defense = Mathf.Max(0, json.UnitDefense);
        HP = Mathf.Max(1, json.UnitHP);
        MoveSpeed = Mathf.Max(0.1f, json.UnitMoveSpeed);
    }
}