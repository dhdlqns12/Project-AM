using System;
using System.Collections.Generic;
using _02_Scripts.Building.Grid;
using Inventory;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace _02_Scripts.Building
{
    public class BuildingPreviewComponent : MonoBehaviour
    {

        [SerializeField] private GameObject buildingGridPrefab;
        [SerializeField] private Canvas buildingCanvas;

        [SerializeField] private RectTransform gridContainerWrapper;
        [SerializeField] private GameObject gridSlotContainer;
        [SerializeField] private GameObject gridSlotPrefab;
        [SerializeField] private int gridSlotAmount = 5;

        [SerializeField] private GraphicRaycaster targetGridRaycaster;
        [SerializeField] private EventSystem eventSystem;

        [SerializeField] private Color ok = new Color(0, 1, 0, 0.5f);
        [SerializeField] private Color notOk = new Color(1, 0, 0, 0.3f);

        public MouseState CurrentMouseState { get; set; }
        private BuildingEntity buildingEntity;
        private Dictionary<Vector2Int, GameObject> gridSlots = new Dictionary<Vector2Int, GameObject>();
        private Dictionary<Vector2Int, GameObject> occupied = new Dictionary<Vector2Int, GameObject>();
        private bool canBuild;
        private List<Vector2Int> targetGrid = new List<Vector2Int>();


        public event Action<BuildingEntity, List<Vector2Int>> OnBuildingProgress;



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
                    Vector2Int gridSlotPosition = new Vector2Int(i, j);
                    gridSlots[gridSlotPosition] = slot;
                }

            }

            SelectCancel();
            InventoryEvents.OnBuildingSelected += SetPreview;
        }

        void Update()
        {
            SetPreviewTransform();
            if (Input.GetMouseButtonDown(1))
            {
                SelectCancel();
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (!canBuild) return;
                OnBuildingProgress?.Invoke(buildingEntity, targetGrid);
            }


        }

        private void SetPreviewTransform()
        {
            if (CurrentMouseState == MouseState.NonSelected) return;
            if (!gameObject.activeInHierarchy) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                buildingCanvas.transform as RectTransform,
                Input.mousePosition,
                buildingCanvas.worldCamera,
                out Vector2 localPoint);
            gridContainerWrapper.anchoredPosition = localPoint;
            CheckCanBuild();
        }

        private void CheckCanBuild()
        {
            if (buildingEntity == null) return;

            targetGrid.Clear();
            foreach (var slot in occupied)
            {
                GameObject previewSlot = slot.Value;
                GridCell targetSlot = ShootRayFromSlot(previewSlot);
                if (targetSlot != null)
                {
                    if (targetSlot.Occupied)
                    {
                        previewSlot.GetComponentInChildren<Image>().color = notOk;
                        canBuild = false;
                    }
                    else
                    {
                        previewSlot.GetComponentInChildren<Image>().color = ok;
                        canBuild = true;
                        targetGrid.Add(targetSlot.GetCoordinates());
                    }
                }
                else
                {
                    previewSlot.GetComponentInChildren<Image>().color = ok;
                    canBuild = false;
                }
            }

            if (targetGrid.Count == occupied.Count)
            {
                canBuild = true;
            }
            else
            {
                canBuild = false;
            }

        }

        private GridCell ShootRayFromSlot(GameObject slot)
        {
            PointerEventData pointerData = new PointerEventData(eventSystem);
            pointerData.position = RectTransformUtility.WorldToScreenPoint(buildingCanvas.worldCamera, slot.transform.position);
            List<RaycastResult> results = new List<RaycastResult>();
            targetGridRaycaster.Raycast(pointerData, results);

            List<Vector2Int> targetGrid = new List<Vector2Int>();
            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    GridCell cell = result.gameObject.GetComponent<GridCell>();
                    if (cell != null)
                    {
                        targetGrid.Add(cell.GetCoordinates());
                        return cell;
                    }
                }
            }
            return null;
        }

        private void SelectCancel()
        {
            CurrentMouseState = MouseState.NonSelected;
            buildingEntity = null;
            gridSlotContainer.SetActive(false);
        }

        public void SetPreview(BuildingEntity building)
        {
            occupied.Clear();
            buildingEntity = building;
            CurrentMouseState = MouseState.Selected;
            gridSlotContainer.SetActive(true);
            if (buildingEntity == null) return;
            var vectorList = buildingEntity.BuildingCoordinates;
            foreach (GameObject slot in gridSlots.Values)
            {
                slot.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
            }

            foreach (Vector2Int vector in vectorList)
            {
                if (gridSlots.ContainsKey(vector))
                {
                    gridSlots[vector].GetComponentInChildren<Image>().color = ok;
                    occupied.Add(vector, gridSlots[vector]);
                }
            }
        }

        void OnDestroy()
        {
            InventoryEvents.OnBuildingSelected -= SetPreview;
        }

        public enum MouseState
        {
            Selected, NonSelected
        }
    }
}