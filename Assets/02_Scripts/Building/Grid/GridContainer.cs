using System.Collections.Generic;
using UnityEngine;

namespace _02_Scripts.Building.Grid
{
    public class GridContainer : MonoBehaviour
    {
        [SerializeField] private GameObject gridSlotContainer;
        [SerializeField] private GameObject gridSlotPrefab;
        [SerializeField] private int gridSlotAmount = 5;

        private Dictionary<Vector2Int, GridCell> gridCells = new Dictionary<Vector2Int, GridCell>();

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
        }
    }
}
