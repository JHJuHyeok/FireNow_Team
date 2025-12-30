using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private bool isPaused = false;
    public bool IsPaused => isPaused;

    [Header("Wave Settings")]
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float evenWaveSpawnMultiplier = 2f;
    [SerializeField] private float normalCameraSize = 5f;
    [SerializeField] private float evenWaveCameraSize = 7f;
    [SerializeField] private float cameraTransitionSpeed = 2f;

    [Header("UI")]
    [SerializeField] private Text waveText;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button ContinueBtn;
    [SerializeField] private Button HomeBtn;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text moneyText;

    [Header("Wave Warning UI")]
    [SerializeField] private GameObject waveWarningUI;
    [SerializeField] private GameObject waveWarningImage;
    [SerializeField] private TMP_Text waveWarningText;
    [SerializeField] private float warningDuration = 2f;

    private float currentMoney = 0;

    [Header("Boss Settings")]
    [SerializeField] private List<GameObject> bossPrefabs;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private GameObject bossWarningUI;
    [SerializeField] private GameObject topBoundary;
    [SerializeField] private GameObject bottomBoundary;
    [SerializeField] private GameObject leftBoundary;   
    [SerializeField] private GameObject rightBoundary;  
    [SerializeField] private float boundaryYTop = 4f;
    [SerializeField] private float boundaryYBottom = -4f;
    [SerializeField] private float boundaryXLeft = -7f;   
    [SerializeField] private float boundaryXRight = 7f;   

    [Header("Battle Settings")]
    [SerializeField] private float battleSpeed = 1f;

    [Header("Time Settings")]
    [SerializeField] private float maxTime = 900f;

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
    private float targetCameraSize;
    private bool isCameraTransitioning = false;

    // 보스 관련
    private bool isBossFight = false;
    private GameObject currentBoss;
    private bool isTimerStopped = false;
    private bool isWaitingForNextWave = false;

    // 웨이브 경고 표시 추적
    private HashSet<int> waveWarningsShown = new HashSet<int>();

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
        if (HomeBtn != null)
        {
            HomeBtn.onClick.AddListener(BackMain);
        }

        if (topBoundary != null) topBoundary.SetActive(false);
        if (bottomBoundary != null) bottomBoundary.SetActive(false);
        if (leftBoundary != null) leftBoundary.SetActive(false);  
        if (rightBoundary != null) rightBoundary.SetActive(false); 
        if (bossWarningUI != null) bossWarningUI.SetActive(false);
        if (waveWarningUI != null) waveWarningUI.SetActive(false);

        if (mainCamera != null)
        {
            targetCameraSize = normalCameraSize;
            mainCamera.orthographicSize = normalCameraSize;
        }
    }

    private void Update()
    {
        if (isBattleActive)
        {
            if (!isTimerStopped)
            {
                battleTime += Time.deltaTime;
                TimeDisplay();
            }

            // 보스전이나 다음 웨이브 대기 중이 아닐 때만
            if (!isBossFight && !isWaitingForNextWave)
            {
                CheckWaveWarnings();
                UpdateWaveSpawning();
            }

            if (isCameraTransitioning && mainCamera != null)
            {
                mainCamera.orthographicSize = Mathf.Lerp(
                    mainCamera.orthographicSize,
                    targetCameraSize,
                    Time.deltaTime * cameraTransitionSpeed
                );

                if (Mathf.Abs(mainCamera.orthographicSize - targetCameraSize) < 0.01f)
                {
                    mainCamera.orthographicSize = targetCameraSize;
                    isCameraTransitioning = false;
                }
            }
        }
    }

    private void CheckWaveWarnings()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            WaveData wave = waves[i];
            int waveNumber = i + 1;

            if (battleTime >= wave.startTime - 3f &&
                battleTime < wave.startTime &&
                !waveWarningsShown.Contains(waveNumber))
            {
                ShowWaveWarning(waveNumber);
                waveWarningsShown.Add(waveNumber);
            }
        }
    }

    private void ShowWaveWarning(int waveNumber)
    {
        StartCoroutine(WaveWarningSequence(waveNumber));
    }

    private IEnumerator WaveWarningSequence(int waveNumber)
    {
        if (waveWarningUI == null) yield break;

        bool isEvenWave = (waveNumber % 2 == 0);

        waveWarningUI.SetActive(true);

        if (waveWarningImage != null)
        {
            Vector3 originalScale = waveWarningImage.transform.localScale;
            float pulseSpeed = isEvenWave ? 5f : 3f;
            float minScale = isEvenWave ? 0.85f : 0.9f;
            float maxScale = isEvenWave ? 1.15f : 1.1f;
            float elapsedTime = 0f;

            while (elapsedTime < warningDuration)
            {
                float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(elapsedTime * pulseSpeed * Mathf.PI) + 1f) / 2f);
                waveWarningImage.transform.localScale = originalScale * scale;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            waveWarningImage.transform.localScale = originalScale;
        }
        else
        {
            yield return new WaitForSeconds(warningDuration);
        }

        waveWarningUI.SetActive(false);
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        moneyText.text = currentMoney.ToString();
    }

    private void BackMain()
    {
        CleanupBattle();

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneWithFx("MainMenu_Scene");
        }
        else
        {
            SceneManager.LoadScene("MainMenu_Scene");
        }
    }

    private void CleanupBattle()
    {
        Time.timeScale = 1f;
        StopAllCoroutines();
    }

    // ===== 보스 관련 메서드 =====

    private void StartBossFight(int waveNumber)
    {
        if (isBossFight) return;

        // 보스 인덱스 계산 (Wave 2 = 0, Wave 4 = 1, Wave 6 = 2)
        int bossIndex = (waveNumber / 2) - 1;

        StartCoroutine(BossFightSequence(bossIndex));
    }

    private IEnumerator BossFightSequence(int bossIndex)
    {
        isBossFight = true;
        isWaitingForNextWave = true;

        StopTimer();

        // 현재 웨이브 비활성화
        if (currentWaveIndex < waves.Count)
        {
            waves[currentWaveIndex].gameObject.SetActive(false);
        }

        if (bossWarningUI != null)
        {
            bossWarningUI.SetActive(true);
            yield return new WaitForSeconds(2f);
            bossWarningUI.SetActive(false);
        }

        ActivateBoundaries();
        SpawnBoss(bossIndex);
    }

    public void StopTimer()
    {
        isTimerStopped = true;
    }

    public void ResumeTimer()
    {
        isTimerStopped = false;
    }

    private void ActivateBoundaries()
    {
        // 상단
        if (topBoundary != null)
        {
            topBoundary.SetActive(true);
            topBoundary.transform.position = new Vector3(0, boundaryYTop, 0);
        }

        // 하단
        if (bottomBoundary != null)
        {
            bottomBoundary.SetActive(true);
            bottomBoundary.transform.position = new Vector3(0, boundaryYBottom, 0);
        }

        // 좌측 추가
        if (leftBoundary != null)
        {
            leftBoundary.SetActive(true);
            leftBoundary.transform.position = new Vector3(boundaryXLeft, 0, 0);
        }

        // 우측 추가
        if (rightBoundary != null)
        {
            rightBoundary.SetActive(true);
            rightBoundary.transform.position = new Vector3(boundaryXRight, 0, 0);
        }

        // 플레이어 이동 제한
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
        if (leftBoundary != null) leftBoundary.SetActive(false);    
        if (rightBoundary != null) rightBoundary.SetActive(false); 

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.RemoveMovementBounds();
        }
    }
    private void SpawnBoss(int bossIndex)
    {
        if (bossPrefabs == null || bossIndex >= bossPrefabs.Count || bossPrefabs[bossIndex] == null)
        {
            Debug.LogError($"보스 프리팹을 찾을 수 없습니다! 인덱스: {bossIndex}");
            // 보스가 없으면 바로 다음 웨이브로
            StartCoroutine(EndBossFight());
            return;
        }

        GameObject bossPrefab = bossPrefabs[bossIndex];
        Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : GetRandomSpawnPosition();
        currentBoss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        BossEnemy bossScript = currentBoss.GetComponent<BossEnemy>();
        if (bossScript != null)
        {
            bossScript.OnBossDefeated += OnBossDefeated;
        }
    }

    private void OnBossDefeated()
    {
        StartCoroutine(EndBossFight());
    }

    private IEnumerator EndBossFight()
    {
        yield return new WaitForSeconds(1f);

        DeactivateBoundaries();
        ResumeTimer();

        isBossFight = false;
        isWaitingForNextWave = false;

        // 다음 웨이브 인덱스 증가
        currentWaveIndex++;

        // 모든 웨이브 완료 시 승리
        if (currentWaveIndex >= waves.Count)
        {
            BattleWin();
        }
    }

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

            Sprite sprite = Resources.Load<Sprite>($"Sprites/Classified/Ability_Icon/{abilityData.spriteName}");
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
        isBossFight = false;
        isTimerStopped = false;
        isWaitingForNextWave = false;
        waveWarningsShown.Clear();

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
        // 웨이브는 보스 클리어로 진행되므로 여기서는 전체 시간만 체크
        yield return new WaitUntil(() => currentWaveIndex >= waves.Count);
        BattleWin();
    }

    private void UpdateWaveSpawning()
    {
        if (currentWaveIndex >= waves.Count) return;

        WaveData currentWave = waves[currentWaveIndex];
        int waveNumber = currentWaveIndex + 1;

        // 웨이브 시작
        if (battleTime >= currentWave.startTime && battleTime <= currentWave.endTime)
        {
            if (!currentWave.gameObject.activeSelf)
            {
                StartWave(currentWave);
            }
        }
        // 웨이브 종료
        else if (battleTime > currentWave.endTime && currentWave.gameObject.activeSelf)
        {
            EndWave(currentWave, waveNumber);
        }
    }

    private void StartWave(WaveData wave)
    {
        wave.gameObject.SetActive(true);
        int waveNumber = currentWaveIndex + 1;
        OnWaveStart?.Invoke(currentWaveIndex);

        if (waveText)
        {
            waveText.text = $"Wave {waveNumber}";
        }

        if (waveNumber % 2 == 0)
        {
            SetCameraSize(evenWaveCameraSize);
        }
        else
        {
            SetCameraSize(normalCameraSize);
        }

        StartCoroutine(SpawnWaveEnemies(wave, waveNumber));
    }

    private void SetCameraSize(float size)
    {
        targetCameraSize = size;
        isCameraTransitioning = true;
    }

    private void EndWave(WaveData wave, int waveNumber)
    {
        wave.gameObject.SetActive(false);
        OnWaveComplete?.Invoke(currentWaveIndex);

        SetCameraSize(normalCameraSize);

        // 짝수 웨이브면 보스 등장
        if (waveNumber % 2 == 0 && waveNumber <= 6)
        {
            StartBossFight(waveNumber);
        }
        else
        {
            // 홀수 웨이브면 다음 웨이브로
            currentWaveIndex++;
        }
    }

    private IEnumerator SpawnWaveEnemies(WaveData wave, int waveNumber)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(wave.enemiesID);
        if (jsonFile == null) yield break;

        EnemyDatabaseDTO waveEnemies;
        try
        {
            waveEnemies = JsonUtility.FromJson<EnemyDatabaseDTO>(jsonFile.text);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON 파싱 에러: {wave.enemiesID}\n{e.Message}");
            yield break;
        }

        if (waveEnemies == null || waveEnemies.enemyList == null || waveEnemies.enemyList.Count == 0)
        {
            yield break;
        }

        int totalSpawnCount = wave.spawnCount;
        if (waveNumber % 2 == 0)
        {
            totalSpawnCount = Mathf.RoundToInt(wave.spawnCount * evenWaveSpawnMultiplier);
        }

        int spawnedCount = 0;
        float nextSpawnTime = 0f;
        float waveStartTime = battleTime;

        while (spawnedCount < totalSpawnCount && battleTime < wave.endTime && !isWaitingForNextWave)
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