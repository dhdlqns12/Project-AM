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


        void Awake()
        {
            Init();
        }

        void Init()
        {
            var aaaa = Resources.Load<TextAsset>("Data/BuildingData").text;
            var aaaParse = JsonConvert.DeserializeObject<List<BuildingData>>(aaaa);
            gachaPool = new List<BuildingEntity>();
            for (int i = 0; i < aaaParse.Count; i++)
            {
                if (aaaParse[i].BuildingLevel == BUILDING_LEVEL_CAN_GET_GACHA)
                {
                    gachaPool.Add(new BuildingEntity(aaaParse[i]));
                }

            }

        }

        public void OnClickGachaBuilding()
        {
            int random = UnityEngine.Random.Range(0, gachaPool.Count);
            OnGetBuilding?.Invoke(gachaPool[random]);
        }


    }
}