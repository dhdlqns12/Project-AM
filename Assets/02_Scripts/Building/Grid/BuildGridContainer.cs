using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace _02_Scripts.Building.Grid
{
    public class BuildGridContainer : MonoBehaviour
    {
        [SerializeField] private GameObject gridSlotContainer;
        [SerializeField] private GameObject gridSlotPrefab;
        [SerializeField] private int gridSlotAmount = 5;

        [SerializeField] private GameObject previewGridComponent;

        private Dictionary<Vector2Int, GridCell> gridCells = new Dictionary<Vector2Int, GridCell>();
        private BuildingPreviewComponent previewComponent;
        private List<BuildingEntity> buildings = new List<BuildingEntity>();

        private Dictionary<float?, float?> goldGains = new Dictionary<float?, float?>();
        private Dictionary<float?, float> goldProductionTimers = new Dictionary<float?, float>();
        private Dictionary<BuildingEntity, List<GridCell>>buildingCells = new Dictionary<BuildingEntity, List<GridCell>>();

        private BuildingEntity retrieveTargetBuilding = null;

        void Awake()
        {
            Init();
        }

        void Update()
        {
            if (goldGains.Count > 0)
            {
                GainGold();
            }
        }

        void Init()
        {
            for (int i = 0; i < gridSlotAmount; i++)
            {
                for (int j = 0; j < gridSlotAmount; j++)
                {
                    GameObject slot = Instantiate(gridSlotPrefab, gridSlotContainer.transform);
                    GridCell cell = slot.GetComponentInChildren<GridCell>();
                    Vector2Int vector2Int = new Vector2Int(i, j);
                    cell.SetCoordinates(vector2Int);
                    gridCells[vector2Int] = cell;
                }

            }

            if (previewGridComponent != null)
            {
                previewComponent = previewGridComponent.GetComponent<BuildingPreviewComponent>();
                previewComponent.OnBuildingProgress += Build;
            }

            BuildingEvents.OnShowBuildingRetrieve += ShowRetrieveButton;
            BuildingEvents.OnBuildingRetrieve+= Retrieve;
        }

        private void ShowRetrieveButton(Vector2Int? vector2Int)
        {
            retrieveTargetBuilding = null;
            foreach (var cellList in buildingCells.Values)
            {
                foreach (var cell in cellList)
                {
                    cell.ShowRetrieveButton(false);
                }
            }
            if (!vector2Int.HasValue) return;
            BuildingEntity targetBuilding = null;
            foreach (var pair in buildingCells)
            {
                if (pair.Value.Any(cell => cell.GetCoordinates() == vector2Int.Value))
                {
                    targetBuilding = pair.Key;
                    break;
                }
            }

            if (targetBuilding != null)
            {
                List<GridCell> cellsToShow = buildingCells[targetBuilding];
                foreach (var cell in cellsToShow)
                {
                    cell.ShowRetrieveButton(true);
                }
                retrieveTargetBuilding = targetBuilding;
            }
        }

        private void GainGold()
        {
            List<float?> productionCycles = goldProductionTimers.Keys.ToList();

            foreach (float cycle in productionCycles)
            {
                goldProductionTimers[cycle] += Time.deltaTime;
                if (goldProductionTimers[cycle] >= cycle)
                {
                    int cyclesPassed = (int)(goldProductionTimers[cycle] / cycle);
                    float? totalAmountPerCycle = goldGains[cycle];

                    if (totalAmountPerCycle.HasValue)
                    {
                        float goldToGain = cyclesPassed * totalAmountPerCycle.Value;
                        StageManager.Instance.IncreaseGold((int)goldToGain);
                    }
                    goldProductionTimers[cycle] -= cyclesPassed * cycle;
                }
            }
        }
        private void Build(BuildingEntity buildingEntity, List<Vector2Int> targetGrid)
        {
            if (buildingEntity == null) return;

            bool canBuild = true;
            for (int i = 0; i < targetGrid.Count; i++)
            {
                Vector2Int vector2 = targetGrid[i];
                if (gridCells.ContainsKey(vector2))
                {
                    if (gridCells[vector2].Occupied)
                    {
                        canBuild = false;
                        break;
                    }
                }
            }

            if (!canBuild) return;
            List<GridCell> list = new List<GridCell>();
            for (int i = 0; i < targetGrid.Count; i++)
            {
                Vector2Int vector2 = targetGrid[i];
                if (gridCells.ContainsKey(vector2))
                {
                    gridCells[vector2].SetBuilding(buildingEntity);
                    buildings.Add(buildingEntity);
                    list.Add(gridCells[vector2]);
                }
            }
            buildingCells[buildingEntity] = list;

            if (buildingEntity.BuildingType == BuildingType.Farm)
            {
                if (buildingEntity.GoldProductionCycle == null) return;
                if (buildingEntity.GoldProductionAmount == null) return;
                if (!goldGains.ContainsKey(buildingEntity.GoldProductionCycle))
                {
                    goldGains.Add(buildingEntity.GoldProductionCycle, buildingEntity.GoldProductionAmount);
                }
                else
                {
                    goldGains[buildingEntity.GoldProductionCycle] += buildingEntity.GoldProductionAmount;
                }
                if (!goldProductionTimers.ContainsKey(buildingEntity.GoldProductionCycle))
                {
                    goldProductionTimers.Add(buildingEntity.GoldProductionCycle, 0f);
                }
            }

            BuildingEvents.OnBuildingConstructedInvoked(buildings);
            BuildingEvents.OnBuildingDestroyedOneInvoked(buildingEntity);
        }

        private void Retrieve(BuildingEntity buildingEntity)
        {
            if(buildingEntity == null) return;
            List<GridCell> grids = buildingCells[buildingEntity];
            for (int i = 0; i < grids.Count; i++)
            {
                grids[i].Clear();
            }

            if (buildingEntity.BuildingType == BuildingType.Farm)
            {
                if (buildingEntity.GoldProductionCycle.HasValue && buildingEntity.GoldProductionAmount.HasValue)
                {
                    float? cycle = buildingEntity.GoldProductionCycle;
                    float? amount = buildingEntity.GoldProductionAmount;
                    if (goldGains.ContainsKey(cycle))
                    {
                        goldGains[cycle] -= amount;
                        if (goldGains[cycle] <= 0)
                        {
                            goldGains.Remove(cycle);
                            goldProductionTimers.Remove(cycle);
                        }
                    }
                }

                buildingCells.Remove(buildingEntity);
            }
        }



        void OnDestroy()
        {
            BuildingEvents.OnShowBuildingRetrieve -= ShowRetrieveButton;
            BuildingEvents.OnBuildingRetrieve -= Retrieve;
        }


    }
}
