using System;
using System.Collections.Generic;

namespace _02_Scripts.Building
{
    public static class BuildingEvents
    {
        public static event Action<List<BuildingEntity>> OnBuildingConstructed;
        public static event Action<BuildingEntity> OnBuildingConstructedOne;
        public static event Action<BuildingEntity> OnBuildingDestroyed;

        public static void OnBuildingConstructedInvoked(List<BuildingEntity> list)
        {
            OnBuildingConstructed?.Invoke(list);
        }
        public static void OnBuildingDestroyedOneInvoked(BuildingEntity buildingEntity)
        {
            OnBuildingConstructedOne?.Invoke(buildingEntity);
        }
        public static void OnBuildingDestroyedInvoked(BuildingEntity buildingEntity)
        {
            OnBuildingDestroyed?.Invoke(buildingEntity);
        }

    }
}