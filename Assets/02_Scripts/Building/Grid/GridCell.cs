using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace _02_Scripts.Building.Grid
{
    public class GridCell : MonoBehaviour
    {
        public Vector2Int GridCordinate { get; set; }
        public bool Occupied { get; set; }

        public BuildingEntity BuildingEntity { get; set; }
        [SerializeField] private Button retrieveButton;

        private Color originalColor;

        void Start()
        {
            originalColor = gameObject.GetComponentInChildren<Image>().color;
        }

        public void SetCoordinates(Vector2Int cordinate)
        {
            this.GridCordinate = cordinate;
            Occupied = false;
        }

        public void Clear()
        {
            Occupied = false;
            BuildingEntity = null;
            gameObject.GetComponentInChildren<Image>().color= originalColor;
            ShowRetrieveButton(false);
        }


        public Vector2Int GetCoordinates()
        {
            return GridCordinate;
        }

        public void SetBuilding(BuildingEntity building)
        {
            Occupied = true;
            BuildingEntity = building;
            Color color = new Color();
            switch (BuildingEntity.BuildingType)
            {
                case BuildingType.Farm: color = Color.yellow;
                    break;
                case BuildingType.Barracks: color = Color.red;
                    break;
                case BuildingType.Tower: color = Color.green;
                    break;
            }
            gameObject.GetComponentInChildren<Image>().color = color;
        }

        public void ShowRetrieveButton(bool show)
        {
            retrieveButton.gameObject.SetActive(show);
        }

        public void Retrieve()
        {
            if (BuildingEntity == null) return;
            BuildingEvents.OnBuildingRetrieveInvoked(BuildingEntity);
        }
    }
}