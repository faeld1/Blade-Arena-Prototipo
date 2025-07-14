using UnityEngine;
using TMPro;

public class GoldEarnsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();
        if (text != null)
            text.gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (text == null) return;
        text.text = message;
        text.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (text != null)
            text.gameObject.SetActive(false);
    }
}
