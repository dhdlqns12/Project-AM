using System;
using _02_Scripts.Building;

namespace Inventory
{
    public static class InventoryEvents
    {
        public static event Action<BuildingEntity> OnBuildingSelected;

        public static event Action<BuildingEntity> OnBuildingSelectCanceled;

        public static void OnBuildingSelectedInvoked(BuildingEntity buildingEntity)
        {
            OnBuildingSelected?.Invoke(buildingEntity);
        }

        public static void OnBuildingSelectCanceledInvoked(BuildingEntity buildingEntity)
        {
            OnBuildingSelectCanceled?.Invoke(buildingEntity);
        }

    }
}