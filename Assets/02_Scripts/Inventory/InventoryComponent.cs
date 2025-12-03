using System;
using System.Collections.Generic;
using _02_Scripts.Building;
using UnityEngine;
using Utils;

namespace Inventory
{
    public class InventoryComponent : MonoBehaviour
    {
        [SerializeField] private GameObject controlPanel;
        [SerializeField] private GameObject inventoryContainer;
        [SerializeField] private GameObject inventorySlotPrefab;
        [SerializeField] private int inventorySlotAmount = 16;
        [SerializeField] private GameObject gachaButtonComponent;

        private List<InventorySlot> inventorySlots = new List<InventorySlot>();
        private List<BuildingEntity> buildingData = new List<BuildingEntity>();

        private GachaBuilding gachaBuilding;
        private bool isAdding;

        void Awake()
        {
            Init();
        }

        public void ToggleInventory(bool toggle)
        {
            if (toggle)
            {
                controlPanel.GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                controlPanel.GetComponent<CanvasGroup>().alpha = 0;
            }
        }

        void Init()
        {
            for (int i = 0; i < inventorySlotAmount; i++)
            {
                GameObject slot = Instantiate(inventorySlotPrefab, inventoryContainer.transform);
                inventorySlots.Add(slot.GetComponent<InventorySlot>());
            }

            if (gachaButtonComponent != null)
            {
                gachaBuilding = gachaButtonComponent.GetComponent<GachaBuilding>();
                gachaBuilding.OnGetBuilding += AddBuilding;
            }
            BuildingEvents.OnBuildingConstructedOne += RemoveBuilding;
            InventoryEvents.OnBuildingMerged += MergeBuilding;
            BuildingEvents.OnBuildingRetrieve += AddBuilding;
        }

        public void AddBuilding(BuildingEntity buildingEntity)
        {
            if (isAdding) return;
            if (buildingData.Count >= inventorySlotAmount) return;

            isAdding = true;

            buildingData.Add(buildingEntity);
            buildingEntity.InventoryIndex = buildingData.Count - 1;

            UpdateInventoryUI();
            isAdding = false;
        }

        private void MergeBuilding(int target1, int target2, BuildingEntity newBuilding)
        {
            if (target1 == target2) return;

            if (target1 < target2)
            {
                buildingData[target2] = newBuilding;
                buildingData.RemoveAt(target1);
            }
            else
            {
                buildingData.RemoveAt(target1);
                buildingData[target2] = newBuilding;
            }
            UpdateInventoryUI();
        }



        private void RemoveBuilding(BuildingEntity buildingToRemove)
        {
            bool removed = buildingData.Remove(buildingToRemove);
            if (removed)
            {
                UpdateInventoryUI();
            }
        }

        public void UpdateInventoryUI()
        {
            for(int i = 0; i < buildingData.Count; i++)
            {
                buildingData[i].InventoryIndex = i;
            }

            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (i < buildingData.Count)
                {
                    inventorySlots[i].SetBuildingEntity(buildingData[i]);
                }
                else
                {
                    inventorySlots[i].Clear();
                }
            }
        }

        void OnDestroy()
        {
            if (gachaBuilding != null)
            {
                gachaBuilding.OnGetBuilding -= AddBuilding;
            }
            BuildingEvents.OnBuildingConstructedOne -= RemoveBuilding;
            InventoryEvents.OnBuildingMerged -= MergeBuilding;
            BuildingEvents.OnBuildingRetrieve -= AddBuilding;
        }
    }
}