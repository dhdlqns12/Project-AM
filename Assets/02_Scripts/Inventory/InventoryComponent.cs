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
            controlPanel.SetActive(toggle);
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
            BuildingEvents.OnBuildingConstructed += RemoveBuilding;
            InventoryEvents.OnBuildingMerged += MergeBuilding;
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
            buildingData.RemoveAll(building => building.InventoryIndex == target1 || building.InventoryIndex == target2);
            AddBuilding(newBuilding);
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
            BuildingEvents.OnBuildingConstructed -= RemoveBuilding;
            InventoryEvents.OnBuildingMerged -= MergeBuilding;
        }
    }
}