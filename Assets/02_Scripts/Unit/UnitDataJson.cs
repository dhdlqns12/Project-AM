using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static Enums;

[Serializable]
public class UnitDataJson
{
    [JsonProperty("Index")]
    public int Index;

    [JsonProperty("Unit_Attack")]
    public int UnitAttack;

    [JsonProperty("Unit_Attackrange")]
    public float UnitAttackRange;

    [JsonProperty("Unit_Attackspeed")]
    public float UnitAttackSpeed;

    [JsonProperty("Unit_Defense")]
    public int UnitDefense;

    [JsonProperty("Unit_HP")]
    public int UnitHP;

    [JsonProperty("Unit_Jobname")]
    public string UnitJobName; // 적 유닛은 null

    [JsonProperty("Unit_Level")]
    public int UnitLevel;

    [JsonProperty("Unit_Movespeed")]
    public float UnitMoveSpeed;

    [JsonProperty("Unit_Name")]
    public string UnitName;

    [JsonProperty("Unit_Type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public UnitType UnitType;
}