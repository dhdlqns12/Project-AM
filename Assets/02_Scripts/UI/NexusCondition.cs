using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NexusCondition : MonoBehaviour
{
    private NexusManager nexusManager;

    [SerializeField] private GameObject playerNexusHPBar;
    [SerializeField] private GameObject enemyNexusHPBar;
    [SerializeField] private TextMeshProUGUI playerNexusHPPercentText;
    [SerializeField] private TextMeshProUGUI enemyNexusHPPercentText;

    private float playerNexusMaxHP;
    private float playerNexusCurHP;
    private float playerNexusHPPercent;

    private float enemyNexusMaxHP;
    private float enemyNexusCurHP;
    private float enemyNexusHPPercent;

    private void Awake()
    {
        if (playerNexusHPBar == null || enemyNexusHPBar == null || playerNexusHPPercentText == null || enemyNexusHPPercentText == null)
        {
            Debug.LogError("Nexus 정보가 연동되지 않음");
        }
    }

    void Update()
    {
        playerNexusMaxHP = NexusManager.Instance.playerNexus.MaxHp;
        playerNexusCurHP = NexusManager.Instance.playerNexus.CurrentHp;

        enemyNexusMaxHP = NexusManager.Instance.enemyNexus.MaxHp;
        enemyNexusCurHP = NexusManager.Instance.enemyNexus.CurrentHp;

        UpdateNexusHP();
        FillAmountUI();
    }

    private void UpdateNexusHP()
    {
        playerNexusHPPercent = ((playerNexusCurHP * 100) / playerNexusMaxHP);
        enemyNexusHPPercent = ((enemyNexusCurHP * 100) / enemyNexusMaxHP);

        playerNexusHPPercentText.text = playerNexusHPPercent.ToString("F1") + " %";
        enemyNexusHPPercentText.text = enemyNexusHPPercent.ToString("F1") + " %";
    }

    private void FillAmountUI()
    {
        playerNexusHPBar.GetComponent<Image>().fillAmount = playerNexusHPPercent / 100f;
        enemyNexusHPBar.GetComponent<Image>().fillAmount = enemyNexusHPPercent / 100f;
    }
}
