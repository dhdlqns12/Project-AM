using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var list = ResourceManager.LoadJsonDataList<Root>("Data/EnemyData");
        foreach (var enemy in list)
        {
            Debug.Log("enemy name: " + enemy.Unit_Name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
public class Root
{
    public int Grow_Minute { get; set; }
    public int Index { get; set; }
    public int Unit_Attack { get; set; }
    public int Unit_Attackrange { get; set; }
    public double Unit_Attackspeed { get; set; }
    public int Unit_Defense { get; set; }
    public int Unit_HP { get; set; }
    public int Unit_Level { get; set; }
    public double Unit_Movespeed { get; set; }
    public string Unit_Name { get; set; }
    public string Unit_Type { get; set; }
}

