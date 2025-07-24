using UnityEngine;
using UnityEngine.UI;

public class AdminManager : MonoBehaviour
{
    [SerializeField] private Button gameSpeedBtn;
    [SerializeField] private Button mainMenuBtn;

    [SerializeField] private RectTransform adminPainelShowPosition;
    [SerializeField] private RectTransform adminPainelHidePosition;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
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
}
