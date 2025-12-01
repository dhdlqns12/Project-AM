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

        private int lastInventorySlotIndex = -1;
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
        }

        public void AddBuilding(BuildingEntity buildingEntity)
        {
            if (lastInventorySlotIndex >= inventorySlotAmount - 1 ) return;
            inventorySlots[lastInventorySlotIndex + 1].SetBuildingEntity(buildingEntity);
            lastInventorySlotIndex++;
            buildingData.Add(buildingEntity);
            UpdateInventoryUI();
        }

        public void UpdateInventoryUI()
        {
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
            gachaBuilding.OnGetBuilding -= AddBuilding;
        }

    }
}
