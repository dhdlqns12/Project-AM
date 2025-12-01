using _02_Scripts.Building;
using UnityEngine;

namespace Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        private BuildingEntity buildingEntity;

        public void AddBuildingEntity(BuildingEntity buildingEntity)
        {
            this.buildingEntity = buildingEntity;
        }

    }
}