using System;
using _02_Scripts.Building;

namespace Inventory
{
    public static class InventoryEvents
    {
        public static event Action<BuildingEntity> OnBuildingSelected;

        public static event Action<BuildingEntity> OnBuildingSelectCanceled;

        public static event Action<int, int, BuildingEntity> OnBuildingMerged;

        public static void OnBuildingSelectedInvoked(BuildingEntity buildingEntity)
        {
            OnBuildingSelected?.Invoke(buildingEntity);
        }

        public static void OnBuildingSelectCanceledInvoked(BuildingEntity buildingEntity)
        {
            OnBuildingSelectCanceled?.Invoke(buildingEntity);
        }

        public static void OnBuildingMergedInvoked(int target1, int target2, BuildingEntity buildingEntity)
        {
            OnBuildingMerged?.Invoke(target1,target2,buildingEntity);
        }

    }
}