using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
      StartCoroutine(GameStartDelay());
    }
    private void Update()
    {
        if (!battleOngoing) return;

        CheckBattleState();
    }

    private IEnumerator GameStartDelay()
    {
        yield return new WaitForSeconds(.5f);
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
}
