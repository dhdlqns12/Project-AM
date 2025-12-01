using _02_Scripts.Building;
using TMPro;
using UnityEngine;

namespace Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        private BuildingEntity buildingEntity;
        [SerializeField] private TextMeshProUGUI buildingNameText;

        public void SetBuildingEntity(BuildingEntity building)
        {
            this.buildingEntity = building;
            buildingNameText.text = building.BuildingName;
        }

        public void Clear()
        {
            this.buildingEntity = null;
            buildingNameText.text = "";
        }

    }
}