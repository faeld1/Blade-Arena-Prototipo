﻿using UnityEngine;
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
            int level = SkillManager.Instance.GetSkillLevel(skill);
            descriptionText.text =
                $"Nível {level + 1} → ATK +{skill.attackBonus} | DEF +{skill.defenseBonus} | HP +{skill.healthBonus}";
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
