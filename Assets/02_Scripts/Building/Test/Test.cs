using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

namespace _02_Scripts.Building.Test
{
    public class Test : MonoBehaviour
    {
        void Start()
        {
            var aaaa = Resources.Load<TextAsset>("Data/BuildingData").text;
            var aaaParse = JsonConvert.DeserializeObject<List<BuildingData>>(aaaa);

            for (int i = 0; i < aaaParse.Count; i++)
            {
                var a = aaaParse[i];
                var b = new BuildingEntity(a);
                // Debug.Log(b.ToDebugString());

            }
        }
    }
}
