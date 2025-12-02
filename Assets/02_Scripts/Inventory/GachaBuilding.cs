using System;
using System.Collections.Generic;
using _02_Scripts.Building;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

namespace Inventory
{
    public class GachaBuilding : MonoBehaviour
    {

        public event Action<BuildingEntity> OnGetBuilding;
        private List<BuildingEntity> gachaPool;
        private const int BUILDING_LEVEL_CAN_GET_GACHA = 1;
        private const int GACHA_COST = 100;


        void Awake()
        {
            Init();
        }

        void Init()
        {
            var data = ResourceManager.LoadJsonDataList<BuildingData>("BuildingData");
            gachaPool = new List<BuildingEntity>();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].BuildingLevel == BUILDING_LEVEL_CAN_GET_GACHA)
                {
                    gachaPool.Add(new BuildingEntity(data[i]));
                }
            }
            StageManager.Instance.IncreaseGold(GACHA_COST * 100);
        }

        public void OnClickGachaBuilding()
        {
            if (StageManager.Instance.Gold < GACHA_COST) return;
            int random = UnityEngine.Random.Range(0, gachaPool.Count);
            OnGetBuilding?.Invoke(new BuildingEntity(gachaPool[random]));
            StageManager.Instance.ConsumeGold(GACHA_COST);
        }


    }
}