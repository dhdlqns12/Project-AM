using TMPro;
using UnityEngine;

public class TextMoney : MonoBehaviour
{
    private StageManager stageManager;

    private TextMeshProUGUI moneyText;

    private void Awake()
    {
        stageManager = StageManager.Instance;

        moneyText = GetComponent<TextMeshProUGUI>();

        if (moneyText == null)
        {
            Debug.Log("moneyText가 연동되지 않음");
        }
    }

    void Update()
    {
        UpdateMoney();
    }

    private void UpdateMoney()
    {
        moneyText.text = stageManager.Gold.ToString();
    }
}
