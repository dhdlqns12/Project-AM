using System.Collections.Generic;
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

        void Awake()
        {
            Init();
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
            for (int i = 0; i < targetGrid.Count; i++)
            {
                Vector2Int vector2 = targetGrid[i];
                if (gridCells.ContainsKey(vector2))
                {
                    gridCells[vector2].SetBuilding(buildingEntity);
                    buildings.Add(buildingEntity);
                }
            }
            BuildingEvents.OnBuildingConstructedInvoked(buildings);
            BuildingEvents.OnBuildingDestroyedOneInvoked(buildingEntity);
        }


    }
}
