using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        ResourceManager.Init();
        
        
        Debug.Log("유닛 데이터 출력");
        var unitList = ResourceManager.LoadJsonDataList<Unit>("UnitData");
        foreach (var enemy in unitList)
        {
            Debug.Log("enemy name: " + enemy.Unit_Name);
        }
        
        Debug.Log("빌딩 데이터 출력");
        var buildingList = ResourceManager.LoadJsonDataList<Building>("BuildingData");
        foreach (var building in buildingList)
        {
            Debug.Log("Building: " + building.Building_Name);
        }
        
        Debug.Log("경로에 없는 데이터 로드 시도");
        var toError = ResourceManager.LoadJsonDataList<Building>("Where");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
public class Unit
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

// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
public class Building
{
    public string Building_Coordinate { get; set; }
    public int Building_Level { get; set; }
    public string Building_Name { get; set; }
    public string Building_Type { get; set; }
    public int? Gold_Production_Amount { get; set; }
    public int? Gold_Production_Cycle { get; set; }
    public int Index { get; set; }
    public int? Merge_Result { get; set; }
    public string Produced_Unit_Type { get; set; }
    public int? Unit_Production_Cycle { get; set; }
    public double? Unit_Stat_Multiplier { get; set; }
    public int? Units_Per_Cycle { get; set; }
}

