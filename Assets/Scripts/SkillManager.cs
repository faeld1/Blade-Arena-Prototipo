using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    public SkillShopUI skillShopUI; // referencie no Inspetor

    [SerializeField] private Button openShopButton; // opcional, se quiser um botao para abrir a loja

    public List<SkillInstance> activeSkills = new List<SkillInstance>(); 
    public List<SkillInstance> reservedSkills = new List<SkillInstance>();

    public SkillHUDController skillHUDController { get; private set; }

    public int maxActiveSlots = 3;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        skillHUDController = GetComponent<SkillHUDController>();
        
        openShopButton?.onClick.AddListener(OpenCloseShop);
    }

    private void Start()
    {
        CloseShop(); // opcional, para iniciar a loja fechada
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OpenCloseShop();
        }
    }
    public void AddSkill(SkillData skill)
    {
        // procura em ambas as listas para evitar duplicacao
        var existing = activeSkills.Find(s => s.data == skill);
        if (existing == null)
            existing = reservedSkills.Find(s => s.data == skill);

        // se ja possuir, apenas aumenta o nivel e reaplica bonus
        if (existing != null)
        {
            if (!existing.IsMaxLevel)
            {
                existing.level++;
                ReapplyBonuses();
            }
            skillHUDController.UpdateHUD();
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

            // remove possveis duplicatas
            reservedSkills.Remove(skill);
            activeSkills.Remove(skill);

            activeSkills.Add(skill);
        }
        else
        {
            activeSkills.Remove(skill);
            reservedSkills.Remove(skill);

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

        int totalAtk = 0, totalHp = 0, totalSpeed = 0;

        foreach (var skill in activeSkills)
        {
            totalAtk += skill.data.attackBonus * skill.level;
            totalHp += skill.data.healthBonus * skill.level;
            totalSpeed += skill.data.speedBonus * skill.level;
        }

        playerStats.SetBonuses(totalAtk, totalHp, totalSpeed);
    }

    public void OpenCloseShop()
    {
        if (skillShopUI.gameObject.activeSelf)
        {
            CloseShop();
        }
        else
        {
            OpenShop();
        }
    }

    public void CloseShop()
    {
        skillShopUI.gameObject.SetActive(false);
        openShopButton.gameObject.SetActive(true); // opcional, se quiser esconder o botao ao fechar a loja
    }
    public void OpenShop()
    {
        skillShopUI.gameObject.SetActive(true);
        openShopButton.gameObject.SetActive(false); // opcional, se quiser esconder o botao ao abrir a loja
    }
}
