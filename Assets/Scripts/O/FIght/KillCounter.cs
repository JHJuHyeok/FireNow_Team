using UnityEngine;
using TMPro;

public class KillCounter : MonoBehaviour
{
    public static KillCounter Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI killText;
    private int killCount = 0;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // KillText 자동 찾기 (Inspector에서 설정 안 했을 경우)
        if (killText == null)
        {
            killText = GetComponent<TextMeshProUGUI>();
        }

        UpdateKillText();
    }

    public void AddKill()
    {
        killCount++;
        UpdateKillText();
    }

    private void UpdateKillText()
    {
        if (killText != null)
        {
            killText.text = killCount.ToString();
        }
    }

    public int GetKillCount()
    {
        return killCount;
    }

    public void ResetKillCount()
    {
        killCount = 0;
        UpdateKillText();
    }
}