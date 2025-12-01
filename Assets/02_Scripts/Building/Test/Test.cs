using System.Collections;
using System.Collections.Generic;
using Building.Data;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

public class Test : MonoBehaviour
{
    void Start()
    {
        var aaaa = Resources.Load<TextAsset>("Data/BuildingData").text;
        var aaaParse = JsonConvert.DeserializeObject<List<BuildingEntity>>(aaaa);

        Debug.Log(aaaParse.ToDebugString());
    }
}
