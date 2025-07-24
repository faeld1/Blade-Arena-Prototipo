using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AdminManager : MonoBehaviour
{
    [SerializeField] private Button gameSpeedBtn;
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private TextMeshProUGUI gameSpeedText;

    [SerializeField] private RectTransform adminPainelShowPosition;
    [SerializeField] private RectTransform adminPainelHidePosition;

    private RectTransform rectTransform;
    private readonly float[] timeScaleOptions = new float[] { 1f, 2f, 3f };
    private int currentSpeedIndex = 0;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (gameSpeedBtn != null)
        {
            gameSpeedBtn.onClick.AddListener(ToggleGameSpeed);
            if (gameSpeedText == null)
                gameSpeedText = gameSpeedBtn.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (mainMenuBtn != null)
            mainMenuBtn.onClick.AddListener(MainMenuBtn);

        UpdateGameSpeedText();
    }

    private void Start()
    {
        StartCoroutine(UIManager.Instance.MoveUI(rectTransform, adminPainelShowPosition, adminPainelHidePosition, 0f));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (rectTransform.anchoredPosition == adminPainelShowPosition.anchoredPosition)
            {
                StartCoroutine(UIManager.Instance.MoveUI(rectTransform, adminPainelShowPosition, adminPainelHidePosition, 0f));
            }
            else
            {
                
                StartCoroutine(UIManager.Instance.MoveUI(rectTransform, adminPainelHidePosition, adminPainelShowPosition, 0f));
            }
        }
    }

    public void GameSpeed(float timeScale)
    {
        Time.timeScale = timeScale;
        if (gameSpeedText != null)
            gameSpeedText.text = $"{timeScale}x";
    }

    private void ToggleGameSpeed()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % timeScaleOptions.Length;
        GameSpeed(timeScaleOptions[currentSpeedIndex]);
    }

    private void MainMenuBtn()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateGameSpeedText()
    {
        GameSpeed(timeScaleOptions[currentSpeedIndex]);
    }
}
