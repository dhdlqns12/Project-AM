using UnityEngine;

public enum UnitType
{
    Warrior,
    Archer
}

public class UnitData
{
    public int Index { get; private set; }
    public string Name { get; private set; }
    public string JobName { get; private set; }
    public UnitType Type { get; private set; }
    public int Level { get; private set; }

    public int Attack { get; private set; }
    public int AttackRange { get; private set; }
    public float AttackSpeed { get; private set; }
    public int Defense { get; private set; }
    public int HP { get; private set; }
    public float MoveSpeed { get; private set; }

    /// <summary>
    /// 유닛 데이터 생성자
    /// </summary>
    /// <param name="json"></param>
    /// <exception cref="System.Exception"></exception>
    public UnitData(UnitDataJson json)
    {
        Index = json.Index;
        Name = json.Unit_Name;
        JobName = json.Unit_Jobname;
        Level = json.Unit_Level;

        Type = json.Unit_Type switch
        {
            "Warrior" => UnitType.Warrior,
            "Archer" => UnitType.Archer,
            _ => throw new System.Exception($"Unknown unit type: {json.Unit_Type}")
        };

        // 데이터 검증
        Attack = Mathf.Max(0, json.Unit_Attack);
        AttackRange = Mathf.Max(1, json.Unit_Attackrange);
        AttackSpeed = Mathf.Max(0.1f, json.Unit_Attackspeed);
        Defense = Mathf.Max(0, json.Unit_Defense);
        HP = Mathf.Max(1, json.Unit_HP);
        MoveSpeed = Mathf.Max(0.1f, json.Unit_Movespeed);
    }
}