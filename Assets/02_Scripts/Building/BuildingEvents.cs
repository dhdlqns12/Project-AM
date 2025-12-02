using System;

namespace _02_Scripts.Building
{
    public static class BuildingEvents
    {
        public static event Action<BuildingEntity> OnBuildingConstructed;
        public static event Action<BuildingEntity> OnBuildingDestroyed;

        public static void OnBuildingConstructedInvoked(BuildingEntity buildingEntity)
        {
            OnBuildingConstructed?.Invoke(buildingEntity);
        }

        public static void OnBuildingDestroyedInvoked(BuildingEntity buildingEntity)
        {
            OnBuildingDestroyed?.Invoke(buildingEntity);
        }

    }
}