using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private bool isPaused = false;
    public bool IsPaused => isPaused;

    [Header("Wave Settings")]
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private Camera mainCamera;

    [Header("UI")]
    [SerializeField] private Text waveText;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button ContinueBtn;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private TMP_Text timeText;

    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private float bossSpawnTime = 600f; // 10분
    [SerializeField] private GameObject bossWarningUI;
    [SerializeField] private GameObject topBoundary;
    [SerializeField] private GameObject bottomBoundary;
    [SerializeField] private float boundaryYTop = 4f;
    [SerializeField] private float boundaryYBottom = -4f;

    [Header("Battle Settings")]
    [SerializeField] private float battleSpeed = 1f;

    [Header("Time Settings")]
    [SerializeField] private float maxTime = 900f; // 15분

    [Header("Equipment Panel")]
    public Transform wIconParent;
    public Transform sIconParent;

    [Header("Star Sprites")]
    public Sprite emptyStarSprite;
    public Sprite filledStarSprite;
    public Sprite redStarSprite;

    private int weaponSlotIndex = 0;
    private int passiveSlotIndex = 0;
    private int currentWaveIndex = 0;
    private float battleTime = 0f;
    private bool isBattleActive = false;

    // 보스 관련
    private bool bossSpawned = false;
    private bool isBossFight = false;
    private GameObject currentBoss;
    private bool isTimerStopped = false;

    public event Action<int> OnWaveStart;
    public event Action<int> OnWaveComplete;
    public event Action OnBattleWin;
    public event Action OnBattleLose;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Start()
    {
        InitializeBattle();
        StartBattle();

        if (pauseBtn != null)
        {
            pauseBtn.onClick.AddListener(ActivateUI);
        }
        if (ContinueBtn != null)
        {
            ContinueBtn.onClick.AddListener(DeactivateUI);
        }

        // 보스 UI 초기화
        if (topBoundary != null) topBoundary.SetActive(false);
        if (bottomBoundary != null) bottomBoundary.SetActive(false);
        if (bossWarningUI != null) bossWarningUI.SetActive(false);
    }

    private void Update()
    {
        if (isBattleActive)
        {
            // 타이머가 정지되지 않았을 때만 시간 증가
            if (!isTimerStopped)
            {
                battleTime += Time.deltaTime;
                TimeDisplay();
            }

            UpdateWaveSpawning();

            // 보스 소환 체크
            if (!bossSpawned && battleTime >= bossSpawnTime)
            {
                StartBossFight();
            }
        }
    }

    // ===== 보스 관련 메서드 =====

    public void StartBossFight()
    {
        if (bossSpawned || isBossFight) return;

        bossSpawned = true;
        StartCoroutine(BossFightSequence());
    }

    private IEnumerator BossFightSequence()
    {
        isBossFight = true;

        // 1. 타이머 정지
        StopTimer();

        // 2. 모든 웨이브 비활성화
        foreach (var wave in waves)
        {
            if (wave.gameObject.activeSelf)
            {
                wave.gameObject.SetActive(false);
            }
        }

        // 3. 경고 UI 표시
        if (bossWarningUI != null)
        {
            bossWarningUI.SetActive(true);
            yield return new WaitForSeconds(2f);
            bossWarningUI.SetActive(false);
        }

        // 4. 경계선 활성화
        ActivateBoundaries();

        // 5. 보스 소환
        SpawnBoss();
    }

    public void StopTimer()
    {
        isTimerStopped = true;
        Debug.Log("Timer stopped for boss fight");
    }

    public void ResumeTimer()
    {
        isTimerStopped = false;
        Debug.Log("Timer resumed");
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
        if (bossPrefab != null)
        {
            Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : GetRandomSpawnPosition();
            currentBoss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

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

        Debug.Log("Boss defeated! Timer resumed.");
    }

    // ===== 기존 메서드들 =====

    void SetNormalStars(Transform starLinear, int level)
    {
        for (int i = 0; i < starLinear.childCount; i++)
        {
            Transform star = starLinear.GetChild(i);
            Image starImage = star.GetComponent<Image>();

            if (starImage == null) continue;

            star.gameObject.SetActive(true);

            if (i < level)
            {
                starImage.sprite = filledStarSprite;
                starImage.color = Color.white;
            }
            else
            {
                starImage.sprite = emptyStarSprite;
                starImage.color = Color.white;
            }
        }
    }

    void SetEvolutionStars(Transform starLinear)
    {
        for (int i = 0; i < starLinear.childCount; i++)
        {
            Transform star = starLinear.GetChild(i);
            Image starImage = star.GetComponent<Image>();

            if (starImage == null) continue;

            if (i == 2)
            {
                star.gameObject.SetActive(true);
                starImage.sprite = redStarSprite;
                starImage.color = Color.white;
            }
            else
            {
                star.gameObject.SetActive(false);
            }
        }
    }

    private void ActivateUI()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
            isPaused = true;
            Time.timeScale = 0f;
            UpdateEquipmentDisplay();
        }
    }

    private void UpdateEquipmentDisplay()
    {
        AbilitySelectionManager abilityManager = FindObjectOfType<AbilitySelectionManager>();
        if (abilityManager == null) return;

        DisableAllSlots();

        int weaponIndex = 0;
        int passiveIndex = 0;

        foreach (PlayerAbility ability in abilityManager.ownedAbilities)
        {
            AbilityData abilityData = AbilityDatabase.GetAbility(ability.id);
            if (abilityData == null) continue;

            bool isWeapon = (abilityData.type == AbilityType.weapon || abilityData.type == AbilityType.evolution);
            Transform parent = isWeapon ? wIconParent : sIconParent;
            int slotIndex = isWeapon ? weaponIndex : passiveIndex;

            if (parent == null || slotIndex >= 6 || slotIndex >= parent.childCount) continue;

            Transform slotTransform = parent.GetChild(slotIndex);
            if (slotTransform.childCount < 1) continue;

            Transform iconTransform = slotTransform.GetChild(0);
            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage == null) continue;

            Sprite sprite = Resources.Load<Sprite>($"{abilityData.spriteName}");
            if (sprite != null)
            {
                iconImage.sprite = sprite;
                iconImage.enabled = true;
                iconImage.color = Color.white;
            }

            if (isWeapon)
            {
                weaponIndex++;
            }
            else
            {
                passiveIndex++;
            }

            Transform starLinear = slotTransform.Find("bottomBack/StarLinear");

            if (starLinear != null)
            {
                starLinear.gameObject.SetActive(true);

                if (ability.id == "7" || ability.id == "8" || ability.id == "9")
                {
                    SetEvolutionStars(starLinear);
                }
                else
                {
                    SetNormalStars(starLinear, ability.currentLevel);
                }
            }
        }
    }

    void DisableAllSlots()
    {
        if (wIconParent != null)
        {
            for (int i = 0; i < wIconParent.childCount; i++)
            {
                Transform slot = wIconParent.GetChild(i);
                if (slot.childCount > 0)
                {
                    Image img = slot.GetChild(0).GetComponent<Image>();
                    if (img != null) img.enabled = false;
                }
            }
        }

        if (sIconParent != null)
        {
            for (int i = 0; i < sIconParent.childCount; i++)
            {
                Transform slot = sIconParent.GetChild(i);
                if (slot.childCount > 0)
                {
                    Image img = slot.GetChild(0).GetComponent<Image>();
                    if (img != null) img.enabled = false;
                }
            }
        }
    }

    private void DeactivateUI()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
            isPaused = false;
            Time.timeScale = 1f;
        }
    }

    public void InitializeBattle()
    {
        currentWaveIndex = 0;
        battleTime = 0f;
        isBattleActive = false;
        bossSpawned = false;
        isBossFight = false;
        isTimerStopped = false;

        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);
    }

    public void StartBattle()
    {
        isBattleActive = true;
        battleTime = 0f;
        StartCoroutine(BattleSequence());
    }

    private void TimeDisplay()
    {
        int minutes = Mathf.FloorToInt(battleTime / 60f);
        int seconds = Mathf.FloorToInt(battleTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private IEnumerator BattleSequence()
    {
        while (currentWaveIndex < waves.Count)
        {
            WaveData currentWave = waves[currentWaveIndex];
            yield return new WaitUntil(() => battleTime >= currentWave.endTime);
            currentWaveIndex++;
        }

        BattleWin();
    }

    private void UpdateWaveSpawning()
    {
        // 보스전 중에는 웨이브 스폰 안함
        if (isBossFight) return;

        foreach (var wave in waves)
        {
            if (battleTime >= wave.startTime && battleTime <= wave.endTime)
            {
                if (!wave.gameObject.activeSelf)
                {
                    StartWave(wave);
                }
            }
            else if (battleTime > wave.endTime && wave.gameObject.activeSelf)
            {
                EndWave(wave);
            }
        }
    }

    private void StartWave(WaveData wave)
    {
        wave.gameObject.SetActive(true);
        OnWaveStart?.Invoke(waves.IndexOf(wave));

        if (waveText)
        {
            waveText.text = $"Wave {waves.IndexOf(wave) + 1}";
        }

        StartCoroutine(SpawnWaveEnemies(wave));
    }

    private void EndWave(WaveData wave)
    {
        wave.gameObject.SetActive(false);
        OnWaveComplete?.Invoke(waves.IndexOf(wave));
    }

    private IEnumerator SpawnWaveEnemies(WaveData wave)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(wave.enemiesPath);
        if (jsonFile == null) yield break;

        EnemyDatabaseDTO waveEnemies = JsonUtility.FromJson<EnemyDatabaseDTO>(jsonFile.text);

        int spawnedCount = 0;
        float nextSpawnTime = 0f;
        float waveStartTime = battleTime;

        while (spawnedCount < wave.spawnCount && battleTime < wave.endTime)
        {
            if (battleTime - waveStartTime >= nextSpawnTime)
            {
                if (waveEnemies.enemyList.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, waveEnemies.enemyList.Count);
                    EnemyData enemyData = waveEnemies.enemyList[randomIndex];
                    SpawnEnemy(enemyData);
                    spawnedCount++;
                    nextSpawnTime += wave.spawnRate;
                }
            }
            yield return null;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (mainCamera == null) return Vector3.zero;

        float randomX = UnityEngine.Random.Range(0f, 1f);
        float randomY = UnityEngine.Random.Range(0f, 1f);

        Vector3 viewportPosition = new Vector3(randomX, randomY, mainCamera.nearClipPlane + 1f);
        Vector3 worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);
        worldPosition.z = 0f;

        return worldPosition;
    }

    private void SpawnEnemy(EnemyData enemyData)
    {
        string prefabPath = "Prefabs/Enemies/" + enemyData.id;
        GameObject enemyPrefab = Resources.Load<GameObject>(prefabPath);

        if (enemyPrefab == null)
        {
            Debug.LogError($"적 프리팹을 찾을 수 없습니다: {prefabPath}");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Initialize(enemyData);
        }
    }

    private void BattleWin()
    {
        isBattleActive = false;
        OnBattleWin?.Invoke();

        if (victoryPanel)
        {
            victoryPanel.SetActive(true);
        }
    }

    private void BattleLose()
    {
        isBattleActive = false;
        StopAllCoroutines();
        OnBattleLose?.Invoke();

        if (defeatPanel)
        {
            defeatPanel.SetActive(true);
        }
    }

    public void SetBattleSpeed(float speed)
    {
        battleSpeed = Mathf.Clamp(speed, 0.5f, 3f);
        Time.timeScale = battleSpeed;
    }

    public void PauseBattle()
    {
        Time.timeScale = 0f;
        isBattleActive = false;
    }

    public void ResumeBattle()
    {
        Time.timeScale = battleSpeed;
        isBattleActive = true;
    }
}