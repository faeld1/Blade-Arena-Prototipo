using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Setup")]
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private List<Transform> enemySpawnPoints = new List<Transform>();
    [SerializeField] private float countdownDuration = 15f;
    [SerializeField] private float firstCountdownDuration = 20f;
    [SerializeField] private int startingLives = 3;

    private bool skipCountdownRequested = false;

    private int currentRound = 0;
    private int lives;
    private int currentLevelIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        lives = startingLives;
    }

    public void SkipCountdown()
    {
        skipCountdownRequested = true;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene" && currentLevel != null)
        {
            if (BattleManager.Instance != null)
            {
                playerSpawnPoint = BattleManager.Instance.PlayerStartPosition;
                endPoint = BattleManager.Instance.PlayerEndPosition;
                enemySpawnPoints = BattleManager.Instance.EnemyDestinations;
            }
            StartCoroutine(LevelRoutine());
        }
    }

    public void SetLevel(LevelData data, int index = -1)
    {
        currentLevel = data;
        currentLevelIndex = index;
    }

    private IEnumerator LevelRoutine()
    {
        //GameManager.Instance?.AddGold(5); // starting gold
        bool playerDiedLastRound = true;
        for (currentRound = 0; currentRound < currentLevel.rounds.Count; currentRound++)
        {
            UIManager.Instance?.UpdateRound(currentRound + 1, currentLevel.rounds.Count);
            if (currentRound > 0)
            {
                SkillManager.Instance?.skillShopUI?.RefreshShop(0);
            }
            if (playerDiedLastRound || currentRound == 0)
            {
                SpawnPlayer();
                MovePlayerToEndPoint();
                yield return WaitForPlayerArrival();
                GameManager.Instance.player.GetComponent<Player_Movement>().StopMovement();
                FaceFirstEnemy();
            }
            else
            {
                FaceFirstEnemy();
            }

            playerDiedLastRound = false;

            SpawnEnemies(currentLevel.rounds[currentRound]);
            GameManager.Instance.player.GetComponent<Player_Combat>().SetCurrentTarget(null); // Reset current target
            GameManager.Instance.player.GetComponent<RichAI>().updateRotation = true; // Enable automatic rotation
            float roundCountdown = currentRound == 0 ? firstCountdownDuration : countdownDuration;
            yield return StartCoroutine(CountdownRoutine(roundCountdown));
            GameManager.Instance?.StartBattle();
            GameManager.Instance.player.GetComponent<Player_Movement>().StopFacingTarget();
            yield return new WaitUntil(() => GameManager.Instance.battleOngoing == false);

            bool playerDead = GameManager.Instance.player == null || GameManager.Instance.player.GetComponent<CharacterStats>().isDead;
            if (playerDead) // Caso o Player tenha morrido na rodada
            {
                playerDiedLastRound = true;
                lives--;
                UIManager.Instance?.UpdateLives(lives);
                if (lives <= 0)
                {
                    yield return StartCoroutine(ReturnToMenuAfterDelay(3f));
                    yield break;
                }
                var respawnTimer = StartCoroutine(IntermissionRoutine(false));
                yield return new WaitForSeconds(3f);
                RemoveRemainingEnemies();
                yield return new WaitForSeconds(2f);
                RespawnPlayer();
                yield return WaitForPlayerArrival();
                playerDiedLastRound = false;
                if (currentRound == currentLevel.rounds.Count - 1)
                    currentRound--;
                yield return respawnTimer;
            }
            else //Caso o Player tenha vencido a rodada
            {
                MovePlayerToEndPoint();
                var intermission = StartCoroutine(IntermissionRoutine(true));
                yield return WaitForPlayerArrival();

                GameManager.Instance.player.GetComponent<Player_Movement>().StopMovement();
                FaceFirstEnemy();
                GameManager.Instance.player.GetComponent<CharacterStats>().currentHealth =
                GameManager.Instance.player.GetComponent<CharacterStats>().GetMaxHealth(); // Reset health

                GameManager.Instance.player.GetComponent<Player_Stats>().UpdateHealth(); // Update health bar

                yield return intermission;
            }
        }

        if (lives > 0 && currentLevelIndex >= 0)
        {
            ES3.Save($"LevelUnlocked_{currentLevelIndex + 1}", true);
        }

        yield return StartCoroutine(ReturnToMenuRoutine());
    }

    private IEnumerator CountdownRoutine(float duration)
    {
        UIManager.Instance?.EnableCountdown();
        UIManager.Instance?.ShowSkipCountdownButton(true);
        float timer = duration;
        skipCountdownRequested = false;
        while (timer > 0f)
        {
            if (skipCountdownRequested && timer > 1f)
            {
                timer = 1f;
                skipCountdownRequested = false;
            }
            UIManager.Instance?.UpdateCountdown(Mathf.CeilToInt(timer));
            timer -= Time.deltaTime;
            yield return null;
        }
        UIManager.Instance?.ShowSkipCountdownButton(false);
        UIManager.Instance?.UpdateCountdownZero();
    }

    private IEnumerator IntermissionRoutine(bool won)
    {
        int baseGold = Mathf.Clamp(4 + currentRound, 4, 7);
        int interest = Mathf.Clamp(GameManager.Instance.GetCurrentGold() / 10, 0, 3);
        int winGold = won ? 1 : 0;
        int total = baseGold + interest + winGold;

        string message = $"+{baseGold} DE GOLD BASE\n";
        if (interest > 0) message += $"+{interest} DE JUROS\n";
        if (won) message += "+1 POR VITORIA\n";
        message += $"\nTOTAL: {total} DE GOLD";

        UIManager.Instance?.ShowGoldEarns(message);

        UIManager.Instance?.EnableCountdown();
        float timer = 5f;
        while (timer > 0f)
        {
            UIManager.Instance?.UpdateCountdown(Mathf.CeilToInt(timer));
            timer -= Time.deltaTime;
            yield return null;
        }
        UIManager.Instance?.UpdateCountdownZero();
        UIManager.Instance?.HideGoldEarns();

        GameManager.Instance?.AddGold(total);
    }

    private IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }

    private void SpawnPlayer()
    {
        if (GameManager.Instance.player == null) return;
        var playerObj = GameManager.Instance.player;
        playerObj.SetActive(true);
        IAstarAI agent = playerObj.GetComponent<IAstarAI>();
        if (agent != null)
        {
            agent.Teleport(playerSpawnPoint.position);
        }
        CharacterStats stats = playerObj.GetComponent<CharacterStats>();
        stats.currentHealth = stats.GetMaxHealth();
        stats.isDead = false;
        playerObj.GetComponent<Player_Combat>()?.ResetAttack();
        EnablePlayerHealthBar(playerObj);
    }

    private void RespawnPlayer()
    {
        if (GameManager.Instance.player == null) return;
        var playerObj = GameManager.Instance.player;
        playerObj.SetActive(true);
        IAstarAI agent = playerObj.GetComponent<IAstarAI>();
        if (agent != null)
        {
            agent.Teleport(playerSpawnPoint.position);
            agent.isStopped = false;
            agent.destination = endPoint.position;
            agent.SearchPath();
        }
        CharacterStats stats = playerObj.GetComponent<CharacterStats>();
        stats.currentHealth = stats.GetMaxHealth();
        stats.isDead = false;
        playerObj.GetComponent<Player>().SetIdle();
        playerObj.GetComponent<Player_Movement>().ResumeMovement();
        playerObj.GetComponent<Player_Combat>()?.ResetAttack();
        EnablePlayerHealthBar(playerObj);
    }

    private void EnablePlayerHealthBar(GameObject playerObj)
    {
        var hb = playerObj.GetComponentInChildren<HealthBar>(true);
        if (hb != null)
        {
            hb.gameObject.SetActive(true);
            hb.UpdateHealthUI();
        }
    }

    private IEnumerator RisePlayer(Transform target, Vector3 end)
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 start = target.position;
        while (elapsed < duration)
        {
            target.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.position = end;
    }

    private void SpawnEnemies(RoundData round)
    {
        for (int i = 0; i < round.enemies.Count && i < enemySpawnPoints.Count; i++)
        {
            if (round.enemies[i] == null) continue;
            Transform point = enemySpawnPoints[i];
            GameObject enemyObj = Instantiate(round.enemies[i], point.position, point.rotation);
            if (GameManager.Instance.player != null)
            {
                enemyObj.transform.LookAt(GameManager.Instance.player.transform);
            }
        }
    }

    private void RemoveRemainingEnemies()
    {
        foreach (var enemy in new List<Enemy>(GameManager.Instance.activeEnemies))
        {
            if (enemy != null)
                Destroy(enemy.gameObject);
        }
        GameManager.Instance.activeEnemies.Clear();
    }

    private void MovePlayerToEndPoint()
    {
        if (GameManager.Instance.player == null) return;
        IAstarAI agent = GameManager.Instance.player.GetComponent<IAstarAI>();
        if (agent != null)
        {
            agent.isStopped = false;
            agent.destination = endPoint.position;
            agent.SearchPath();
        }
    }

    private IEnumerator WaitForPlayerArrival()
    {
        if (GameManager.Instance?.player == null)
            yield break;

        IAstarAI agent = GameManager.Instance.player.GetComponent<IAstarAI>();
        if (agent == null)
            yield break;

        yield return new WaitUntil(() => agent.reachedDestination);
    }

    private void FaceFirstEnemy()
    {
        if (GameManager.Instance.player == null) return;
        if (enemySpawnPoints.Count == 0) return;
        Transform lookTarget = enemySpawnPoints[0];
        var movement = GameManager.Instance.player.GetComponent<Player_Movement>();
        IAstarAI agent = GameManager.Instance.player.GetComponent<IAstarAI>();

        agent.updateRotation = false; // Disable automatic rotation
        movement.FaceTarget(lookTarget);
        Debug.Log("FaceFirstEnemy sendo chamado");
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        float timer = 5f;
        UIManager.Instance?.EnableCountdown();
        while (timer > 0f)
        {
            UIManager.Instance?.UpdateCountdown(Mathf.CeilToInt(timer));
            timer -= Time.deltaTime;
            yield return null;
        }
        UIManager.Instance?.UpdateCountdownZero();
        SceneManager.LoadScene("MainMenu");
    }
}
