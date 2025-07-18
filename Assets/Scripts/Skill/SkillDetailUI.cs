using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SkillDetailUI : MonoBehaviour
{
    public static SkillDetailUI Instance;

    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject skillPanel;

    [Header("Skill UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private Button sellButton;
    [SerializeField] private TextMeshProUGUI sellPriceText;

    private Image backgroundImage;

    private SkillInstance currentSkill;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (sellButton != null)
            sellButton.onClick.AddListener(SellCurrentSkill);

        backgroundImage = skillPanel.GetComponent<Image>();
    }

    private void Start()
    {
        Hide();
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

    private void Update()
    {
        if (skillPanel != null && skillPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            RectTransform rect = skillPanel.GetComponent<RectTransform>();
            if (!RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, null))
            {
                // If clicking on another skill, it will reopen via its own handler
                PointerEventData data = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(data, results);
                foreach (var r in results)
                {
                    if (r.gameObject.GetComponentInParent<SkillHudSlotUI>() != null)
                        return;
                }
                Debug.Log("Clicked outside skill panel, hiding detail UI.");
                Hide();
            }
        }
    }

    private void UpdateUI(SkillInstance skill)
    {
        if (iconImage != null) iconImage.sprite = skill.data.icon;
        if (nameText != null) nameText.text = skill.data.skillName;

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

        if (sellPriceText != null)
            sellPriceText.text = (skill.data.cost * skill.level).ToString();

        if (backgroundImage != null)
            backgroundImage.color = SkillUIColor.GetColor(skill.data.rarity);

        if (stars != null)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                    stars[i].SetActive(i < skill.level);

                if (skill.level == stars.Length)
                {
                    stars[i].GetComponent<Image>().color = new Color(1f, 0.85f, 0.05f); // Highlight last star if max level
                }
                else
                {
                    stars[i].GetComponent<Image>().color = Color.white; // Default color for other stars
                }
            }
        }
    }
}
