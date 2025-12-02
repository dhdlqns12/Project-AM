using _02_Scripts.Building;
using TMPro;
using UnityEngine;
using Utils;

namespace Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        public BuildingEntity BuildingEntity;
        [SerializeField] private TextMeshProUGUI buildingNameText;

        public void SetBuildingEntity(BuildingEntity building)
        {
            this.BuildingEntity = building;
            buildingNameText.text = building.BuildingName;
        }

        public void Clear()
        {
            this.BuildingEntity = null;
            buildingNameText.text = "";
        }

        public void SelectBuilding()
        {
            if (BuildingEntity == null) return;
            InventoryEvents.OnBuildingSelectedInvoked(BuildingEntity);
        }

    }
}