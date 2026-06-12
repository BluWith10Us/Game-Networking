using TMPro;
using UnityEngine;

public class ResultHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;

    private void Awake()
    {
        ShowResult();
    }

    void ShowResult()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        resultText.text = GameManager.Instance.GetResult();
    }
}
