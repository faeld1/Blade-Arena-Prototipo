using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int currentGold = 0;
    public static event Action<int> OnGoldChanged;

    [Header("Level Settings")]
    public int playerLevel = 3;
    public int playerXP = 0;
    public int[] xpToNextLevel = new int[] { 0, 0, 1, 5, 15, 20, 25, 35, 40, 45 };
    public static event Action<int> OnLevelUp;
    public static event Action<int, int, int> OnXPChanged; // level, xp, toNext
    [Header("Referências")]
    public GameObject player;
    public List<Enemy> activeEnemies = new List<Enemy>();

    public bool battleOngoing = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        // Battle is started externally by the BattleManager
    }

    private void Update()
    {
        if (!battleOngoing) return;

        CheckBattleState();
    }

    public void StartBattle()
    {
        battleOngoing = true;
        Debug.Log("BATALHA INICIADA!");
    }
    public void RegisterEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    private void CheckBattleState()
    {
        if (player == null || player.GetComponent<CharacterStats>().isDead)
        {
            battleOngoing = false;
            Debug.Log("DERROTA!");
            return;
        }
        // Vitória se todos os inimigos estiverem mortos
        bool allDead = true;
        foreach (var enemy in activeEnemies)
        {
            if (!enemy.stats.isDead)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            battleOngoing = false;
            Debug.Log("VITÓRIA!");
        }
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
    }

    public bool TrySpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            OnGoldChanged?.Invoke(currentGold);
            return true;
        }
        return false;
    }

    public void AddXP(int amount)
    {
        playerXP += amount;
        TryLevelUp();
        OnXPChanged?.Invoke(playerLevel, playerXP, GetXPToNextLevel());
    }

    public bool BuyXP(int amount, int goldCost)
    {
        if (TrySpendGold(goldCost))
        {
            AddXP(amount);
            return true;
        }
        return false;
    }

    private void TryLevelUp()
    {
        while (playerLevel < 10 && playerXP >= GetXPToNextLevel())
        {
            playerXP -= GetXPToNextLevel();
            playerLevel++;
            SkillManager.Instance.maxActiveSlots = playerLevel;
            SkillManager.Instance.skillHUDController.UpdateHUD();
            OnLevelUp?.Invoke(playerLevel);
        }
    }

    private int GetXPToNextLevel()
    {
        if (playerLevel <= 0 || playerLevel >= xpToNextLevel.Length)
            return 0;
        return xpToNextLevel[playerLevel];
    }

    public int GetCurrentGold() => currentGold;


}
