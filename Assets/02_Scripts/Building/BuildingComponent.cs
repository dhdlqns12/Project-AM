using UnityEngine;

namespace _02_Scripts.Building
{
    public class BuildingComponent : MonoBehaviour
    {
        private BuildingEntity buildingEntity;
        [SerializeField] private GameObject buildingPrefab;

        public void Init(BuildingEntity entity)
        {
            buildingEntity = entity;
            CreateGrid();
        }

        private void CreateGrid()
        {

        }
    }
}