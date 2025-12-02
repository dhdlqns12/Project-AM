using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.Building.Grid
{
    public class GridCell : MonoBehaviour
    {
        public Vector2Int GridCordinate { get; set; }
        public bool Occupied { get; set; }

        public BuildingEntity BuildingEntity { get; set; }

        public void SetCoordinates(Vector2Int cordinate)
        {
            this.GridCordinate = cordinate;
            Occupied = false;
        }


        public Vector2Int GetCoordinates()
        {
            return GridCordinate;
        }

        public void SetBuilding(BuildingEntity building)
        {
            Occupied = true;
            BuildingEntity = building;
            gameObject.GetComponentInChildren<Image>().color = Color.green;

        }

    }
}