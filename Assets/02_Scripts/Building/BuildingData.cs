using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace _02_Scripts.Building
{
    [Serializable]
    public class BuildingData
    {
        [JsonProperty("Index")]
        public int Index;

        [JsonProperty("Building_Name")]
        public string BuildingName;

        [JsonProperty("Building_Type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public BuildingType BuildingType;

        [JsonProperty("Building_Level")]
        public int BuildingLevel;

        [JsonProperty("Building_Coordinate")]
        public string? BuildingCoordinateString;

        [JsonProperty("Unit_Production_Cycle")]
        public float? UnitProductionCycle;

        [JsonProperty("Produced_Unit_Type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductionUnitType? ProductionUnitType;

        [JsonProperty("Units_Per_Cycle")]
        public int? UnitPerCycle;

        [JsonProperty("Gold_Production_Cycle")]
        public float? GoldProductionCycle;

        [JsonProperty("Gold_Production_Amount")]
        public float? GoldProductionAmount;

        [JsonProperty("Unit_Stat_Multiplier")]
        public float? UnitStatMultiplier;

        [JsonProperty("Merge_Result")]
        public int? MergeResult;
    }

    public enum BuildingType
    {
        Barracks, Tower, Farm
    }

    public enum ProductionUnitType
    {
        Warrior, Archer
    }
}