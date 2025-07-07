using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    [SerializeField] private bool autoStart = false;
    [SerializeField] private Transform playerDestination;
    [SerializeField] private List<Transform> enemyDestinations = new List<Transform>();
    [SerializeField] private float countdownDuration = 5f;

    private void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    private void Start()
    {
        if (autoStart)
            StartCoroutine(BattleRoutine());
    }

    public void BeginBattle()
    {
        StartCoroutine(BattleRoutine());
    }

    private IEnumerator BattleRoutine()
    {
        MoveUnitsToPositions();

        float timer = countdownDuration;
        while (timer > 0f)
        {
            UIManager.Instance?.UpdateCountdown(Mathf.CeilToInt(timer));
            timer -= Time.deltaTime;
            yield return null;
        }

        GameManager.Instance.player.GetComponent<Player>().SetManualEnemyTarget();
        GameManager.Instance.player.GetComponent<Player_Combat>().ResetCurrentTarget();
        // UIManager.Instance?.UpdateCountdown(0);
        UIManager.Instance?.UpdateCountdownZero();
        GameManager.Instance?.StartBattle();
    }


    private void MoveUnitsToPositions()
    {
        if (GameManager.Instance == null)
            return;

        if (playerDestination != null && GameManager.Instance.player != null)
        {
            var playerAgent = GameManager.Instance.player.GetComponent<NavMeshAgent>();
            if (playerAgent != null)
            {
                playerAgent.isStopped = false;
                playerAgent.SetDestination(playerDestination.position);
            }
        }

        var enemies = GameManager.Instance.activeEnemies;
        for (int i = 0; i < enemies.Count && i < enemyDestinations.Count; i++)
        {
            var enemy = enemies[i];
            var dest = enemyDestinations[i];
            if (enemy != null && dest != null)
            {
                enemy.agent.isStopped = false;
                enemy.agent.SetDestination(dest.position);
            }
        }
    }
}

