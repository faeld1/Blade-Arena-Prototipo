using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillShopUI : MonoBehaviour
{
    [SerializeField] private SkillSlotUI[] skillSlots;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Toggle lockToggle;

    [SerializeField] private List<SkillData> allPossibleSkills;

    private bool firstRefresh = false;

    private List<SkillData> currentShopSkills = new();

    private readonly int[,] rarityChances = new int[10, 5]
    {
        {80,20,0,0,0},
        {70,30,0,0,0},
        {55,35,10,0,0},
        {45,40,15,0,0},
        {35,40,25,0,0},
        {25,35,35,5,0},
        {20,30,40,10,0},
        {18,24,35,20,3},
        {15,21,30,28,6},
        {12,18,28,32,10}
    };

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

        if (firstRefresh == true)
            GameManager.Instance.TrySpendGold(2); // <- gasta 2 de gold para atualizar a loja

        firstRefresh = true; // <- marca que a loja foi atualizada pelo menos uma vez
    }

    public void BuySkill(int index)
    {
        if (index >= 0 && index < currentShopSkills.Count)
        {
            var skill = currentShopSkills[index];
            if (GameManager.Instance != null && GameManager.Instance.TrySpendGold(skill.cost))
            {
                SkillManager.Instance.AddSkill(skill);
                Debug.Log("Skill comprada: " + skill.skillName);

                skillSlots[index].Hide(); // <- esconde o slot visualmente

                SkillManager.Instance.skillHUDController.UpdateHUD(); // <- atualiza HUD de skills
            }
            else
            {
                Debug.Log("Gold insuficiente para comprar " + skill.skillName);
            }
        }
    }

    private List<SkillData> GetRandomSkills(int count)
    {
        List<SkillData> result = new List<SkillData>();

        for (int i = 0; i < count; i++)
        {
            SkillData selected = null;
            int attempts = 0;
            while (selected == null && attempts < 20)
            {
                SkillRarity rarity = GetRandomRarity();
                var candidates = allPossibleSkills.FindAll(s => s.rarity == rarity && !SkillManager.Instance.IsSkillMaxLevel(s));
                if (candidates.Count > 0)
                    selected = candidates[Random.Range(0, candidates.Count)];
                attempts++;
            }

            if (selected == null)
                selected = allPossibleSkills[Random.Range(0, allPossibleSkills.Count)];

            result.Add(selected);
        }

        return result;
    }

    private SkillRarity GetRandomRarity()
    {
        int level = Mathf.Clamp(GameManager.Instance ? GameManager.Instance.playerLevel : 1, 1, 10);
        int roll = Random.Range(0, 100);
        int cumulative = 0;
        for (int r = 0; r < 5; r++)
        {
            cumulative += rarityChances[level - 1, r];
            if (roll < cumulative)
                return (SkillRarity)r;
        }
        return SkillRarity.Common;
    }
}