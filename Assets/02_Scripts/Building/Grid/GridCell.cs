using UnityEngine;

namespace _02_Scripts.Building.Grid
{
    public class GridCell : MonoBehaviour
    {
        private Vector2Int gridCordinate;
        public bool Occupied { get; set; }

        public void SetCoordinates(Vector2Int cordinate)
        {
            this.gridCordinate = cordinate;
            Occupied = false;
        }


        public Vector2Int GetCoordinates()
        {
            return gridCordinate;
        }

    }
}