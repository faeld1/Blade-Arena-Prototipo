using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;

    private SkillData currentSkill;
    private int index;
    private SkillShopUI shopUI;

    public SkillData CurrentSkill => currentSkill;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Setup(SkillData skill, int idx, SkillShopUI shop)
    {
        currentSkill = skill;
        index = idx;
        shopUI = shop;

        int skillValue = 0;

        if (skill.attackBonus != 0)
            skillValue = skill.attackBonus;
        else if (skill.defenseBonus != 0)
            skillValue = skill.defenseBonus;
        else if (skill.speedBonus != 0)
            skillValue = skill.speedBonus;
        else if (skill.healthBonus != 0)
            skillValue = skill.healthBonus;

        if (iconImage != null)
            iconImage.sprite = skill.icon;

        if (backgroundImage != null)
            backgroundImage.color = SkillUIColor.GetColor(skill.rarity);

        if (nameText != null)
            nameText.text = skill.skillName;

        if(costText != null)
            costText.text = skill.cost.ToString();

        if (descriptionText != null)
        {
            descriptionText.text = $"Increases {skill.description} by {skillValue}";
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnClick()
    {
        if (shopUI != null)
            shopUI.BuySkill(index);
    }
}
