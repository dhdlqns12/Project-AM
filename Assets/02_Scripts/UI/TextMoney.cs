using UnityEngine;
using TMPro;

public class TextMoney : MonoBehaviour
{
    private StageManager stageManager;

    private TextMeshProUGUI textMoney;

    private void Awake()
    {
        stageManager = StageManager.Instance;

        textMoney = GetComponent<TextMeshProUGUI>();

        if (textMoney == null)
        {
            Debug.LogWarning("Money Text가 연동되지 않음");
        }
    }

    void Update()
    {
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        textMoney.text = stageManager.Gold.ToString();
    }
}
