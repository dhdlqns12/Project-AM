using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace _02_Scripts.Building
{
    public class BuildingEntity
    {
        public int Index;
        public string BuildingName;
        public int BuildingLevel;
        public List<Vector2Int> BuildingCoordinates;
        public Vector2Int CenterCoordinate;
        public float? UnitProductionCycle;
        public Enums.UnitType? ProductionUnitType;
        public int? UnitPerCycle;
        public float? GoldProductionCycle;
        public float? GoldProductionAmount;
        public float? UnitStatMultiplier;
        public int? MergeResult;
        public int InventoryIndex;
        public BuildingType BuildingType;

        private const int MAX_BUILDING_LEVEL = 3;

        public BuildingEntity(BuildingData buildingData)
        {
            Index = buildingData.Index;
            BuildingName = buildingData.BuildingName;
            BuildingLevel = buildingData.BuildingLevel;
            BuildingCoordinates = GetBuildingCoordinates(buildingData.BuildingCoordinateString);
            CenterCoordinate = new Vector2Int(0, 0);
            UnitProductionCycle = buildingData.UnitProductionCycle;
            ProductionUnitType = buildingData.ProductionUnitType;
            UnitPerCycle = buildingData.UnitPerCycle;
            GoldProductionCycle = buildingData.GoldProductionCycle;
            GoldProductionAmount = buildingData.GoldProductionAmount;
            UnitStatMultiplier = buildingData.UnitStatMultiplier;
            MergeResult = buildingData.MergeResult;
            BuildingType = buildingData.BuildingType;
        }

        public BuildingEntity(BuildingEntity buildingEntity)
        {
            Index = buildingEntity.Index;
            BuildingName = buildingEntity.BuildingName;
            BuildingLevel = buildingEntity.BuildingLevel;
            BuildingCoordinates = buildingEntity.BuildingCoordinates;
            CenterCoordinate = new Vector2Int(0, 0);
            UnitProductionCycle = buildingEntity.UnitProductionCycle;
            ProductionUnitType = buildingEntity.ProductionUnitType;
            UnitPerCycle = buildingEntity.UnitPerCycle;
            GoldProductionCycle = buildingEntity.GoldProductionCycle;
            GoldProductionAmount = buildingEntity.GoldProductionAmount;
            UnitStatMultiplier = buildingEntity.UnitStatMultiplier;
            MergeResult = buildingEntity.MergeResult;
            BuildingType = buildingEntity.BuildingType;
        }

        public bool CanMerge(BuildingEntity buildingEntity)
        {
            if (BuildingLevel == MAX_BUILDING_LEVEL) return false;
            if(buildingEntity == null) return false;
            if(buildingEntity.BuildingType != this.BuildingType) return false;
            if(buildingEntity.BuildingLevel != this.BuildingLevel) return false;
            if(buildingEntity.InventoryIndex == this.InventoryIndex) return false;
            return true;
        }

        private List<Vector2Int> GetBuildingCoordinates(string buildingCoordinateString)
        {
            var coordinates = new List<Vector2Int>();

            if (string.IsNullOrWhiteSpace(buildingCoordinateString) ||
                buildingCoordinateString.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return coordinates;
            }

            string[] pairs = buildingCoordinateString.Split(')');

            foreach (string pair in pairs)
            {
                if (string.IsNullOrWhiteSpace(pair))
                {
                    continue;
                }

                string cleanedPair = pair.Replace("(", "").Replace(",", " ").Trim();

                string[] nums = cleanedPair.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (nums.Length == 2 &&
                    int.TryParse(nums[0], out int x) &&
                    int.TryParse(nums[1], out int y))
                {
                    coordinates.Add(new Vector2Int(x, y));
                }

            }

            return coordinates;
        }

    }
}