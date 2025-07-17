using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillDetailUI : MonoBehaviour
{
    public static SkillDetailUI Instance;

    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject skillPanel;

    [Header("Skill UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button backgroundButton;

    private SkillInstance currentSkill;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (sellButton != null)
            sellButton.onClick.AddListener(SellCurrentSkill);
        if (backgroundButton != null)
            backgroundButton.onClick.AddListener(Hide);
    }

    public void Show(SkillInstance skill)
    {
        currentSkill = skill;
        if (playerPanel != null) playerPanel.SetActive(false);
        if (skillPanel != null) skillPanel.SetActive(true);
        UpdateUI(skill);
    }

    public void Hide()
    {
        if (skillPanel != null) skillPanel.SetActive(false);
        if (playerPanel != null) playerPanel.SetActive(true);
        currentSkill = null;
    }

    private void SellCurrentSkill()
    {
        if (currentSkill == null) return;
        SkillManager.Instance?.SellSkill(currentSkill);
        Hide();
    }

    private void UpdateUI(SkillInstance skill)
    {
        if (iconImage != null) iconImage.sprite = skill.data.icon;
        if (nameText != null) nameText.text = skill.data.skillName;
        if (levelText != null) levelText.text = "Lv. " + skill.level;

        int value = 0;
        if (skill.data.attackBonus != 0)
            value = skill.data.attackBonus * skill.level;
        else if (skill.data.defenseBonus != 0)
            value = skill.data.defenseBonus * skill.level;
        else if (skill.data.speedBonus != 0)
            value = skill.data.speedBonus * skill.level;
        else if (skill.data.healthBonus != 0)
            value = skill.data.healthBonus * skill.level;

        if (descriptionText != null)
            descriptionText.text = $"Increases {skill.data.description} by {value}";

        if (stars != null)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                    stars[i].SetActive(i < skill.level);
            }
        }
    }
}
