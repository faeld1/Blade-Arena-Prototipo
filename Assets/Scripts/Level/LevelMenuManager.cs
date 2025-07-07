using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    [SerializeField] private List<Button> levelButtons = new List<Button>();
    [SerializeField] private string battleSceneName = "BattleScene";

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        for (int i = 0; i < levelButtons.Count && i < levels.Count; i++)
        {
            int index = i;
            bool unlocked = ES3.Load<bool>($"LevelUnlocked_{index}", index == 0);
            levelButtons[i].interactable = unlocked;
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => LoadLevel(index));
        }
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levels.Count)
            return;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SetLevel(levels[index], index);
        }
        SceneManager.LoadScene(battleSceneName);
    }
}
