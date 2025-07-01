using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SkillShopUI : MonoBehaviour
{
    [SerializeField] private SkillSlotUI[] skillSlots;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Toggle lockToggle;

    [SerializeField] private List<SkillData> allPossibleSkills;

    private List<SkillData> currentShopSkills = new();

    private void Start()
    {
        refreshButton.onClick.AddListener(RefreshShop);
        RefreshShop();
    }

    public void RefreshShop()
    {
        if (lockToggle != null && lockToggle.isOn) return;

        currentShopSkills = GetRandomSkills(5);

        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].gameObject.SetActive(true); // <- reativa caso tenha sido escondido antes
            skillSlots[i].Setup(currentShopSkills[i], i, this);
        }
    }

    public void BuySkill(int index)
    {
        if (index >= 0 && index < currentShopSkills.Count)
        {
            SkillManager.Instance.AddSkill(currentShopSkills[index]);
            Debug.Log("Skill comprada: " + currentShopSkills[index].skillName);

            skillSlots[index].Hide(); // <- esconde o slot visualmente

            SkillManager.Instance.skillHUDController.UpdateHUD(); // <- atualiza HUD de skills
        }
    }

    private List<SkillData> GetRandomSkills(int count)
    {
        List<SkillData> result = new List<SkillData>();

        for (int i = 0; i < count; i++)
        {
            SkillData selected = allPossibleSkills[Random.Range(0, allPossibleSkills.Count)];

            // Garante que não apareça na loja se já estiver no nível 5
            if (SkillManager.Instance.IsSkillMaxLevel(selected))
            {
                i--;
                continue;
            }

            result.Add(selected);
        }

        return result;
    }
}