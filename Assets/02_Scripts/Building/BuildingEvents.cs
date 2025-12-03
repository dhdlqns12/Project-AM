using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02_Scripts.Building
{
    public static class BuildingEvents
    {
        public static event Action<List<BuildingEntity>> OnBuildingConstructed;
        public static event Action<BuildingEntity> OnBuildingConstructedOne;
        public static event Action<BuildingEntity> OnBuildingDestroyed;
        public static event Action<BuildingEntity> OnBuildingRetrieve;

        public static event Action<Vector2Int?> OnShowBuildingRetrieve;

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

        public static void OnShowBuildingRetrieveInvoked(Vector2Int? retrieveGrid)
        {
            OnShowBuildingRetrieve?.Invoke(retrieveGrid);
        }

        public static void OnBuildingRetrieveInvoked(BuildingEntity buildingEntity)
        {
            OnBuildingRetrieve?.Invoke(buildingEntity);
        }


    }
}