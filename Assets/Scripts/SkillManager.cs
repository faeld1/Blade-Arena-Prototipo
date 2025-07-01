using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    public SkillShopUI skillShopUI; // referencie no Inspetor

    public List<SkillInstance> activeSkills = new List<SkillInstance>(); 
    public List<SkillInstance> reservedSkills = new List<SkillInstance>();

    public SkillHUDController skillHUDController { get; private set; }

    public int maxActiveSlots = 3;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        skillHUDController = GetComponent<SkillHUDController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            skillShopUI.gameObject.SetActive(true);
            skillShopUI.RefreshShop();
        }
    }
    public void AddSkill(SkillData skill)
    {
        var existing = activeSkills.Find(s => s.data == skill);

        if (existing != null)
        {
            if (!existing.IsMaxLevel)
            {
                existing.level++;
                ReapplyBonuses();
                skillHUDController.UpdateHUD();
            }
            return;
        }

        if (activeSkills.Count < maxActiveSlots)
            activeSkills.Add(new SkillInstance(skill));
        else
            reservedSkills.Add(new SkillInstance(skill));

        ReapplyBonuses();
        skillHUDController.UpdateHUD(); // AQUI
    }

    public void MoveSkill(SkillInstance skill, bool moveToActive)
    {
        if (moveToActive)
        {
            if (activeSkills.Count >= maxActiveSlots) return;
            reservedSkills.Remove(skill);
            activeSkills.Add(skill);
        }
        else
        {
            activeSkills.Remove(skill);
            reservedSkills.Add(skill);
        }

        ReapplyBonuses();
        skillHUDController.UpdateHUD(); // AQUI
    }


    public int GetSkillLevel(SkillData skill)
    {
        var existing = activeSkills.Find(s => s.data == skill);
        return existing?.level ?? 0;
    }

    public bool IsSkillMaxLevel(SkillData skill)
    {
        var existing = activeSkills.Find(s => s.data == skill);
        return existing != null && existing.IsMaxLevel;
    }

    public void ReapplyBonuses()
    {
        var playerStats = GameManager.Instance?.player?.GetComponent<Player_Stats>();
        if (playerStats == null) return;

        int totalAtk = 0, totalDef = 0, totalHp = 0;

        foreach (var skill in activeSkills)
        {
            totalAtk += skill.data.attackBonus * skill.level;
            totalDef += skill.data.defenseBonus * skill.level;
            totalHp += skill.data.healthBonus * skill.level;
        }

        playerStats.SetBonuses(totalAtk, totalDef, totalHp);
    }

    public void CloseShop()
    {
        skillShopUI.gameObject.SetActive(false);
    }
}
