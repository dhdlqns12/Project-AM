using System.Collections.Generic;
using Inventory;
using UnityEditor;
using UnityEngine;
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

        public MouseState CurrentMouseState { get; set; }
        private BuildingEntity buildingEntity;
        private RectTransform previewTransform;
        private Dictionary<Vector2Int, GameObject> gridSlots = new Dictionary<Vector2Int, GameObject>();
        void Awake()
        {
            previewTransform = GetComponent<RectTransform>();
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
        }

        private void SelectCancel()
        {
            CurrentMouseState = MouseState.NonSelected;
            buildingEntity = null;
            gridSlotContainer.SetActive(false);
        }

        public void SetPreview(BuildingEntity building)
        {
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
                    gridSlots[vector].GetComponentInChildren<Image>().color = new Color(0, 1, 0, 0.5f);
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