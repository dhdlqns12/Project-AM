using UnityEngine;

namespace _02_Scripts.Building.Grid
{
    public class GridContainer : MonoBehaviour
    {
        [SerializeField] private GameObject gridSlotContainer;
        [SerializeField] private GameObject gridSlotPrefab;
        [SerializeField] private int gridSlotAmount = 25;

        void Awake()
        {
            Init();
        }

        void Init()
        {
            for (int i = 0; i < gridSlotAmount; i++)
            {
                GameObject slot = Instantiate(gridSlotPrefab, gridSlotContainer.transform);
            }
        }
    }
}
