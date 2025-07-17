using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Gold UI")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("XP Button Settings")]
    [SerializeField] private int xpAmount = 5;
    [SerializeField] private int goldCost = 5;
    [SerializeField] private GameObject goldCostParent;
    [SerializeField] private Button xpButton;

    [Header("Skip Countdown Settings")]
    [SerializeField] private Button skipCountdownButton;

    [Header("Active Skill Count")]
    [SerializeField] private TextMeshProUGUI activeSkillCountText;
    [SerializeField] private TextMeshProUGUI desactiveSkillCountText;

    [Header("LevelXP Settings")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI xpText;

    [Header("Countdown Settings")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI livesText;
    [Header("Gold Earns")]
    [SerializeField] private GoldEarnsDisplay goldEarnsDisplay;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (xpButton != null)
            xpButton.onClick.AddListener(BuyXpButtonFuction);

        if (skipCountdownButton != null)
            skipCountdownButton.onClick.AddListener(SkipCountdownFunction);

        if (goldText == null)
            goldText = GetComponent<TextMeshProUGUI>();
        UpdateGold(GameManager.Instance ? GameManager.Instance.GetCurrentGold() : 0);

        ShowSkipCountdownButton(false);
        HideGoldEarns();

    }
    private void Start()
    {
        UpdateActiveSkillCount();
    }
    private void OnEnable()
    {
        GameManager.OnXPChanged += UpdateDisplayLevelXP;
        GameManager.OnLevelUp += LevelUp;
        UpdateDisplayLevelXP(GameManager.Instance ? GameManager.Instance.playerLevel : 0,
                      GameManager.Instance ? GameManager.Instance.playerXP : 0,
                      GameManager.Instance ? GameManager.Instance.xpToNextLevel[Mathf.Clamp(GameManager.Instance.playerLevel, 0, GameManager.Instance.xpToNextLevel.Length - 1)] : 0);

        GameManager.OnGoldChanged += UpdateGold;
    }

    private void OnDisable()
    {
        GameManager.OnXPChanged -= UpdateDisplayLevelXP;
        GameManager.OnLevelUp -= LevelUp;
        GameManager.OnGoldChanged -= UpdateGold;
    }

    private void LevelUp(int level)
    {
        UpdateActiveSkillCount();

        UpdateDisplayLevelXP(level,
                      GameManager.Instance.playerXP,
                      level < GameManager.Instance.xpToNextLevel.Length ? GameManager.Instance.xpToNextLevel[level] : 0);
    }

    private void UpdateDisplayLevelXP(int level, int xp, int toNext)
    {
        if (xpText != null)
        {
            if (toNext <= 0) toNext = 0;
            xpText.text = $"XP {xp} / {toNext}";

            if (level == 10)
            {
                xpText.text = "";
                goldCostParent.SetActive(false);
            }
        }
        if(levelText != null)
        {
            levelText.text = level.ToString();
        }
    }


    private void BuyXpButtonFuction()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BuyXP(xpAmount, goldCost);

            if (GameManager.Instance.playerLevel == 10)
            {
                xpButton.interactable = false;
            }
        }

        UpdateDisplayLevelXP(GameManager.Instance.playerLevel,
                      GameManager.Instance.playerXP,
                      GameManager.Instance.xpToNextLevel[Mathf.Clamp(GameManager.Instance.playerLevel, 0, GameManager.Instance.xpToNextLevel.Length - 1)]);
    }
    public void UpdateActiveSkillCount()
    {
        activeSkillCountText.text = $"{SkillManager.Instance.activeSkills.Count} / {SkillManager.Instance.maxActiveSlots}";
        desactiveSkillCountText.text = $"{SkillManager.Instance.reservedSkills.Count} / 4";
    }

    public void UpdateCountdown(int value)
    {
        if (countdownText != null)
            countdownText.text = value.ToString();
    }

    public void EnableCountdown()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);
    }
    public void UpdateCountdownZero()
    {
        UpdateCountdown(0);
        countdownText.gameObject.SetActive(false);
    }

    public void UpdateRound(int round, int total)
    {
        if (roundText != null)
            roundText.text = $"Round {round} / {total}";
    }

    public void UpdateLives(int value)
    {
        if (livesText != null)
            livesText.text = value.ToString();
    }

    private void SkipCountdownFunction()
    {
        LevelManager.Instance?.SkipCountdown();
    }

    public void ShowSkipCountdownButton(bool show)
    {
        if (skipCountdownButton != null)
            skipCountdownButton.gameObject.SetActive(show);
    }

    public void ShowGoldEarns(string message)
    {
        if (goldEarnsDisplay != null)
            goldEarnsDisplay.ShowMessage(message);
    }

    public void HideGoldEarns()
    {
        if (goldEarnsDisplay != null)
            goldEarnsDisplay.Hide();
    }

    private void UpdateGold(int value)
    {
        if (goldText != null)
            goldText.text = value.ToString();
    }

}
