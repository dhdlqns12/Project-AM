using System.Collections.Generic;
using _02_Scripts.Building;
using UnityEngine;
using Utils;

namespace Inventory
{
    public class InventoryComponent : MonoBehaviour
    {
        [SerializeField] private GameObject controlPanel;
        [SerializeField] private GameObject inventorySlotPrefab;
        [SerializeField] private int inventorySlotAmount = 16;
        [SerializeField] private GameObject gachaButtonComponent;

        private List<InventorySlot> inventorySlots = new List<InventorySlot>();
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
            controlPanel.SetActive(false);
            for (int i = 0; i < inventorySlotAmount; i++)
            {
                GameObject slot = Instantiate(inventorySlotPrefab, controlPanel.transform);
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
            if (lastInventorySlotIndex >= inventorySlotAmount) return;
            inventorySlots[lastInventorySlotIndex + 1].AddBuildingEntity(buildingEntity);
            Debug.Log($"{buildingEntity.ToDebugString()}");
            UpdateInventoryUI();
        }

        public void UpdateInventoryUI()
        {

        }

        void OnDestroy()
        {
            gachaBuilding.OnGetBuilding -= AddBuilding;
        }

    }
}
