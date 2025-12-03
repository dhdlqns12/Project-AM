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

        [SerializeField] private GraphicRaycaster GridtargetGridRaycaster;
        [SerializeField] private GraphicRaycaster InventorytargetGridRaycaster;
        private EventSystem eventSystem;

        [SerializeField] private Color ok = new Color(0, 1, 0, 0.5f);
        [SerializeField] private Color notOk = new Color(1, 0, 0, 0.3f);

        public MouseState CurrentMouseState { get; set; }
        private BuildingEntity buildingEntity;
        private Dictionary<Vector2Int, GameObject> gridSlots = new Dictionary<Vector2Int, GameObject>();
        private Dictionary<Vector2Int, GameObject> occupied = new Dictionary<Vector2Int, GameObject>();
        private bool canBuild;
        private List<Vector2Int> targetGrid = new List<Vector2Int>();
        private bool canMerge;

        private List<BuildingEntity> buildingPools = new List<BuildingEntity>();
        private int[] mergeTargetInventoryIndexs = new int[2];
        private bool isRotating;
        private bool canRetrieve;
        private Vector2Int? retrieveGrid;

        public event Action<BuildingEntity, List<Vector2Int>> OnBuildingProgress;




        void Start()
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
            var data = ResourceManager.LoadJsonDataList<BuildingData>("BuildingData");
            if (data == null) return;
            if (data.Length == 0) return;
            for (int i = 0; i < data.Length; i++)
            {
                buildingPools.Add(new BuildingEntity(data[i]));
            }
            eventSystem = FindObjectOfType<EventSystem>();
        }

        void Update()
        {
            SetPreviewTransform();
            CheckRetrieve();
            if (Input.GetMouseButtonDown(1))
            {
                SelectCancel();
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (canBuild && !canMerge)
                {
                    OnBuildingProgress?.Invoke(buildingEntity, targetGrid);
                    SelectCancel();
                }else if (canMerge && !canBuild)
                {
                    if (buildingEntity == null) return;
                    if(mergeTargetInventoryIndexs[0] == -1 || mergeTargetInventoryIndexs[1] == -1 ) return;
                    if (buildingEntity.MergeResult == null) return;
                    InventoryEvents.OnBuildingMergedInvoked(mergeTargetInventoryIndexs[0],mergeTargetInventoryIndexs[1], GetBuildingByIndex(buildingEntity.MergeResult));
                    SelectCancel();
                }
                if (canRetrieve)
                {
                    if (retrieveGrid == null) return;
                    BuildingEvents.OnShowBuildingRetrieveInvoked(retrieveGrid);
                }
            }

            if (Input.GetKey(KeyCode.Space))
            {
                if (buildingEntity == null) return;
                // RotatePreview();
            }
        }

        public BuildingEntity GetBuildingByIndex(int? index)
        {
            for (int i = 0; i < buildingPools.Count; i++)
            {
                if (buildingPools[i].Index == index)
                {
                    return new BuildingEntity(buildingPools[i]);
                }
            }
            return null;
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
            CheckMerge();

        }

        private void CheckRetrieve()
        {
            if (buildingEntity != null) return;
            PointerEventData pointerData = new PointerEventData(eventSystem);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            GridtargetGridRaycaster.Raycast(pointerData, results);
            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    GridCell cell = result.gameObject.GetComponent<GridCell>();
                    if (cell != null)
                    {
                        if (cell.Occupied)
                        {
                            canRetrieve = true;
                            retrieveGrid = cell.GetCoordinates();
                        }
                        else
                        {
                            canRetrieve = false;
                            retrieveGrid = null;
                        }
                    }
                }
            }
            else
            {
                canRetrieve = false;
                retrieveGrid = null;
            }
        }

        private void CheckMerge()
        {
            if (buildingEntity == null) return;
            Array.Fill(mergeTargetInventoryIndexs,-1);
            mergeTargetInventoryIndexs[0] = buildingEntity.InventoryIndex;
            PointerEventData pointerData = new PointerEventData(eventSystem);
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            InventorytargetGridRaycaster.Raycast(pointerData, results);
            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
                    if(slot == null) continue;
                    BuildingEntity targetBuilding = slot.BuildingEntity;
                    if (buildingEntity.CanMerge(targetBuilding))
                    {
                        canMerge = true;
                        mergeTargetInventoryIndexs[1] = targetBuilding.InventoryIndex;
                    }
                    else
                    {
                        canMerge = false;
                    }
                }
            }
            else
            {
                canMerge = false;
            }
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
            GridtargetGridRaycaster.Raycast(pointerData, results);

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