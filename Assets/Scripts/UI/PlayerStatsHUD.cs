using TMPro;
using UnityEngine;

public class PlayerStatsHUD : MonoBehaviour
{
    public static PlayerStatsHUD Instance;

    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI maxHealthText;

    private Player_Stats playerStats;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playerStats = GameManager.Instance?.player?.GetComponent<Player_Stats>();
        UpdateStats();
    }

    public void UpdateStats()
    {
        if (playerStats == null)
            playerStats = GameManager.Instance?.player?.GetComponent<Player_Stats>();
        if (playerStats == null) return;

        if (damageText != null)
            damageText.text = playerStats.GetBaseDamage().ToString("F0");
        if (attackSpeedText != null)
            attackSpeedText.text = playerStats.offense.attackSpeed.GetValue().ToString("F1");
        if (maxHealthText != null)
            maxHealthText.text = playerStats.GetMaxHealth().ToString("F0");
    }
}
