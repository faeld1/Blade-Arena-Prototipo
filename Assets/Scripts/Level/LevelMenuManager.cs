using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    [SerializeField] private string battleSceneName = "BattleScene";

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levels.Count)
            return;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SetLevel(levels[index]);
        }
        SceneManager.LoadScene(battleSceneName);
    }
}
