using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryComponent : MonoBehaviour
    {
        [SerializeField] private GameObject inventorySlotPanel;
        [SerializeField] private GameObject inventorySlotPrefab;
        [SerializeField] private int inventorySlotAmount = 16;

        private List<InventorySlot> inventorySlots;
        void Awake()
        {
            Init();
        }

        void Init()
        {
            inventorySlotPanel.SetActive(false);
            for (int i = 0; i < inventorySlotAmount; i++)
            {
                GameObject slot = Instantiate(inventorySlotPrefab, inventorySlotPanel.transform);
                inventorySlots.Add(slot.GetComponent<InventorySlot>());
            }
        }

    }
}
