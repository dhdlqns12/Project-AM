using System;
using System.Collections.Generic;
using UnityEngine;

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