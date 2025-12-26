using DG.Tweening.Core.Easing;
using System.Collections;
using TMPro;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }

    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private float bossSpawnTime = 600f; // 10분
    [SerializeField] private GameObject bossWarningUI;
    [SerializeField] private GameObject topBoundary;
    [SerializeField] private GameObject bottomBoundary;
    [SerializeField] private float boundaryYTop = 4f;
    [SerializeField] private float boundaryYBottom = -4f;

    private GameObject currentBoss;
    private bool isBossFight = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 초기에는 경계선 비활성화
        if (topBoundary != null) topBoundary.SetActive(false);
        if (bottomBoundary != null) bottomBoundary.SetActive(false);
        if (bossWarningUI != null) bossWarningUI.SetActive(false);
    }

    public void StartBossFight()
    {
        if (isBossFight) return;

        StartCoroutine(BossFightSequence());
    }

    private IEnumerator BossFightSequence()
    {
        isBossFight = true;

        // 1. 타이머 정지
        StopTimer();

        // 2. 경고 UI 표시
        if (bossWarningUI != null)
        {
            bossWarningUI.SetActive(true);
            yield return new WaitForSeconds(2f);
            bossWarningUI.SetActive(false);
        }

        // 3. 경계선 활성화
        ActivateBoundaries();

        // 4. 보스 소환
        SpawnBoss();
    }

    private void StopTimer()
    {
        // GameManager의 타이머 정지
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.StopTimer();
        }
    }

    private void ResumeTimer()
    {
        // GameManager의 타이머 재개
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.ResumeTimer();
        }
    }

    private void ActivateBoundaries()
    {
        // 상단 경계선
        if (topBoundary != null)
        {
            topBoundary.SetActive(true);
            topBoundary.transform.position = new Vector3(0, boundaryYTop, 0);
        }

        // 하단 경계선
        if (bottomBoundary != null)
        {
            bottomBoundary.SetActive(true);
            bottomBoundary.transform.position = new Vector3(0, boundaryYBottom, 0);
        }

        // 플레이어 이동 제한 설정
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.SetMovementBounds(boundaryYBottom, boundaryYTop);
        }
    }

    private void DeactivateBoundaries()
    {
        if (topBoundary != null) topBoundary.SetActive(false);
        if (bottomBoundary != null) bottomBoundary.SetActive(false);

        // 플레이어 이동 제한 해제
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.RemoveMovementBounds();
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab != null && bossSpawnPoint != null)
        {
            currentBoss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

            BossEnemy bossScript = currentBoss.GetComponent<BossEnemy>();
            if (bossScript != null)
            {
                bossScript.OnBossDefeated += OnBossDefeated;
            }
        }
    }

    private void OnBossDefeated()
    {
        StartCoroutine(EndBossFight());
    }

    private IEnumerator EndBossFight()
    {
        yield return new WaitForSeconds(1f);

        // 경계선 비활성화
        DeactivateBoundaries();

        // 타이머 재개
        ResumeTimer();

        isBossFight = false;

    }

    public bool IsBossFightActive()
    {
        return isBossFight;
    }
}