using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Setup")]
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private List<Transform> enemySpawnPoints = new List<Transform>();
    [SerializeField] private float countdownDuration = 5f;
    [SerializeField] private int startingLives = 3;

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
        bool playerDiedLastRound = true;
        for (currentRound = 0; currentRound < currentLevel.rounds.Count; currentRound++)
        {
            if (playerDiedLastRound || currentRound == 0)
            {
                SpawnPlayer();
                MovePlayerToEndPoint();
                yield return new WaitUntil(() =>
                    Vector3.Distance(GameManager.Instance.player.transform.position, endPoint.position) < 0.1f);
                FaceFirstEnemy();
                Debug.Log("FaceFirstEnemy sendo chamado no primeiro If");
                GameManager.Instance.player.GetComponent<Player_Movement>().StopMovement();
            }
            else
            {
                FaceFirstEnemy();
                Debug.Log("FaceFirstEnemy sendo chamado no else");
            }

            playerDiedLastRound = false;

            SpawnEnemies(currentLevel.rounds[currentRound]);
            UIManager.Instance?.UpdateRound(currentRound + 1, currentLevel.rounds.Count);
            yield return StartCoroutine(CountdownRoutine());
            GameManager.Instance?.StartBattle();
            yield return new WaitUntil(() => GameManager.Instance.battleOngoing == false);

            bool playerDead = GameManager.Instance.player == null || GameManager.Instance.player.GetComponent<CharacterStats>().isDead;
            if (playerDead)
            {
                playerDiedLastRound = true;
                lives--;
                UIManager.Instance?.UpdateLives(lives);
                if (lives <= 0) yield break;
                RemoveRemainingEnemies();
                yield return new WaitForSeconds(5f);
                RespawnPlayer();
                yield return new WaitUntil(() =>
                    Vector3.Distance(GameManager.Instance.player.transform.position, endPoint.position) < 0.1f);
                playerDiedLastRound = false;
            }
            else
            {
                MovePlayerToEndPoint();
                yield return new WaitUntil(() =>
                    Vector3.Distance(GameManager.Instance.player.transform.position, endPoint.position) < 0.1f);
                FaceFirstEnemy();
                Debug.Log("FaceFirstEnemy sendo chamado no else final");
                GameManager.Instance.player.GetComponent<Player_Movement>().StopMovement();
                GameManager.Instance.player.GetComponent<CharacterStats>().currentHealth =
                    GameManager.Instance.player.GetComponent<CharacterStats>().GetMaxHealth(); // Reset health

                GameManager.Instance.player.GetComponent<Player_Stats>().UpdateHealth(); // Update health bar
                
                yield return new WaitForSeconds(5f);
            }
        }

        if (lives > 0 && currentLevelIndex >= 0)
        {
            ES3.Save($"LevelUnlocked_{currentLevelIndex + 1}", true);
        }

        yield return StartCoroutine(ReturnToMenuRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        UIManager.Instance?.EnableCountdown();
        float timer = countdownDuration;
        while (timer > 0f)
        {
            UIManager.Instance?.UpdateCountdown(Mathf.CeilToInt(timer));
            timer -= Time.deltaTime;
            yield return null;
        }
        UIManager.Instance?.UpdateCountdownZero();
    }

    private void SpawnPlayer()
    {
        if (GameManager.Instance.player == null) return;
        var playerObj = GameManager.Instance.player;
        playerObj.SetActive(true);
        NavMeshAgent agent = playerObj.GetComponent<NavMeshAgent>();
        agent.Warp(playerSpawnPoint.position);
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
        NavMeshAgent agent = playerObj.GetComponent<NavMeshAgent>();
        agent.Warp(playerSpawnPoint.position);
        agent.isStopped = false;
        agent.SetDestination(endPoint.position);
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
        NavMeshAgent agent = GameManager.Instance.player.GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.SetDestination(endPoint.position);
    }

    private void FaceFirstEnemy()
    {
        if (GameManager.Instance.player == null) return;
        if (enemySpawnPoints.Count == 0) return;
        Transform lookTarget = enemySpawnPoints[0];
        Vector3 direction = lookTarget.position - GameManager.Instance.player.transform.position;
        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 currentEuler = GameManager.Instance.player.transform.rotation.eulerAngles;
        GameManager.Instance.player.transform.rotation =
            Quaternion.Euler(currentEuler.x, lookRotation.eulerAngles.y, currentEuler.z);
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
