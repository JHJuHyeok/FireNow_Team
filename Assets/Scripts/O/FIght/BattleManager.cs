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

    private BattleSoundManager soundManager;

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
    [SerializeField] private Button WinBtn;
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
    [SerializeField] private float boundaryXLeft = -2.5f;
    [SerializeField] private float boundaryXRight = 2.5f;

    [Header("Boss HP Bar")]
    [SerializeField] private GameObject bossHPBarUI;
    [SerializeField] private Slider bossHPSlider;
    [SerializeField] private Image bossHPFillImage;
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private TMP_Text bossHPText;
    [SerializeField] private Color normalHPColor = Color.red;
    [SerializeField] private Color lowHPColor = new Color(0.5f, 0f, 0f);
    [SerializeField] private float lowHPThreshold = 0.3f;

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
    private float currentBossHP;
    private float maxBossHP;

    // 웨이브 경고 표시 추적
    private HashSet<int> waveWarningsShown = new HashSet<int>();

    public event Action<int> OnWaveStart;
    public event Action<int> OnWaveComplete;
    public event Action OnBattleWin;
    public event Action OnBattleLose;
    private bool isProcessingBossDefeat = false;

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

        if (WinBtn != null)
        {
            WinBtn.onClick.AddListener(BackMain);
        }
        // 사운드 매니저 찾기
        soundManager = FindObjectOfType<BattleSoundManager>();

        if (topBoundary != null) topBoundary.SetActive(false);
        if (bottomBoundary != null) bottomBoundary.SetActive(false);
        if (leftBoundary != null) leftBoundary.SetActive(false);
        if (rightBoundary != null) rightBoundary.SetActive(false);
        if (bossWarningUI != null) bossWarningUI.SetActive(false);
        if (waveWarningUI != null) waveWarningUI.SetActive(false);
        if (bossHPBarUI != null) bossHPBarUI.SetActive(false);

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

            // 짝수 웨이브만 경고 표시
            if (waveNumber % 2 != 0) continue; // 홀수는 스킵

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
        // SoundManager 파괴
        if (SoundManager.Instance != null)
        {
            Destroy(SoundManager.Instance.gameObject);
        }
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

        // 모든 경험치 오브 제거
        ExpOrb[] expOrbs = FindObjectsOfType<ExpOrb>();
        foreach (ExpOrb orb in expOrbs)
        {
            if (orb != null)
            {
                Destroy(orb.gameObject);
            }
        }

        // 모든 적 제거 (경험치 드롭 방지)
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
    }
    // ===== 보스 관련 메서드 =====

    private void StartBossFight(int waveNumber)
    {
        if (isBossFight) return;

        int bossIndex = (waveNumber / 2) - 1;

        StartCoroutine(BossFightSequence(bossIndex));
    }

    private IEnumerator BossFightSequence(int bossIndex)
    {
        isBossFight = true;
        isWaitingForNextWave = true;

        StopTimer();

        // Wave 비활성화
        if (currentWaveIndex < waves.Count)
        {
            waves[currentWaveIndex].gameObject.SetActive(false);
        }

        // 모든 적 제거 (경험치는 드롭)
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.Die(); // 경험치 드롭하면서 제거
            }
        }

        // 보스 경고
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




    private void SpawnBoss(int bossIndex)
    {
        if (bossPrefabs == null || bossIndex >= bossPrefabs.Count || bossPrefabs[bossIndex] == null)
        {
            
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
            bossScript.OnHealthChanged += UpdateBossHPBar;

            InitializeBossHPBar(bossScript.MaxHealth, "BOSS");
        }
    }

    private void InitializeBossHPBar(float maxHealth, string bossName)
    {
      
        maxBossHP = maxHealth;
        currentBossHP = maxBossHP;

        if (bossHPSlider != null)
        {
            bossHPSlider.maxValue = maxBossHP;
            bossHPSlider.value = currentBossHP;
  
        }
        
     

        if (bossHPBarUI != null)
        {
            bossHPBarUI.SetActive(true);
      
        }
     

        UpdateBossHPBar(currentBossHP);
    }
    private void UpdateBossHPBar(float currentHP)
    {
        currentBossHP = currentHP;

        if (bossHPSlider != null)
        {
            bossHPSlider.value = currentBossHP;
        }

        if (bossHPText != null)
        {
            bossHPText.text = $"{currentBossHP:F0} / {maxBossHP:F0}";
        }

        if (bossHPFillImage != null)
        {
            float hpRatio = currentBossHP / maxBossHP;
            bossHPFillImage.color = hpRatio <= lowHPThreshold ? lowHPColor : normalHPColor;
        }
    }

    private void OnBossDefeated()
    {
        StartCoroutine(EndBossFight());
    }

    private void EndWave(WaveData wave, int waveNumber)
    {
        wave.gameObject.SetActive(false);
        OnWaveComplete?.Invoke(currentWaveIndex);

        SetCameraSize(normalCameraSize);

      
        // 짝수 웨이브 종료 시 보스 스폰
        if (waveNumber % 2 == 0 && waveNumber <= 6)
        {
   
            StartBossFight(waveNumber);
            // currentWaveIndex는 증가 안 함
        }
        else
        {
        
            currentWaveIndex++;
        }

     
    }

    private IEnumerator EndBossFight()
    {
       
        yield return new WaitForSeconds(1f);

        if (bossHPBarUI != null)
        {
            bossHPBarUI.SetActive(false);
        }

        DeactivateBoundaries();
        ResumeTimer();

        isBossFight = false;
        isWaitingForNextWave = false;

        // 보스 클리어 보상: 랜덤 레벨업
        GiveRandomLevelUp();

        currentWaveIndex++;

       
        if (currentWaveIndex >= waves.Count)
        {
           
            BattleWin();
        }
       

        isProcessingBossDefeat = false;
    }

    private void GiveRandomLevelUp()
    {
        AbilitySelectionManager abilityManager = FindObjectOfType<AbilitySelectionManager>();
        if (abilityManager == null) return;

        // 1. 진화 가능한 무기 찾기 (최우선)
        List<AbilityData> evolvableWeapons = new List<AbilityData>();

        foreach (PlayerAbility playerAbility in abilityManager.ownedAbilities)
        {
            AbilityData abilityData = AbilityDatabase.GetAbility(playerAbility.id);
            if (abilityData == null || abilityData.type != AbilityType.weapon) continue;

            // 최대 레벨이고 진화 가능한지 체크
            if (playerAbility.currentLevel >= abilityData.maxLevel)
            {
                string evolutionId = abilityData.evolution.result;
                if (!string.IsNullOrEmpty(evolutionId))
                {
                    AbilityData evolutionData = AbilityDatabase.GetAbility(evolutionId);
                    if (evolutionData != null && abilityManager.CanEvolve(evolutionData))
                    {
                        evolvableWeapons.Add(evolutionData);
                    }
                }
            }
        }

        // 진화 가능한 무기가 있으면 진화
        if (evolvableWeapons.Count > 0)
        {
            AbilityData selectedEvolution = evolvableWeapons[UnityEngine.Random.Range(0, evolvableWeapons.Count)];

            abilityManager.SelectAbility(selectedEvolution);
            return;
        }

        // 2. 진화 가능한 무기가 없으면 레벨업 가능한 능력 찾기
        List<PlayerAbility> upgradableAbilities = new List<PlayerAbility>();

        foreach (PlayerAbility playerAbility in abilityManager.ownedAbilities)
        {
            AbilityData abilityData = AbilityDatabase.GetAbility(playerAbility.id);
            if (abilityData == null) continue;

            // 최대 레벨이 아니고, 진화 무기가 아닌 것
            if (playerAbility.currentLevel < abilityData.maxLevel && abilityData.type != AbilityType.evolution)
            {
                upgradableAbilities.Add(playerAbility);
            }
        }

        // 레벨업 가능한 능력이 있으면 랜덤 레벨업
        if (upgradableAbilities.Count > 0)
        {
            PlayerAbility selected = upgradableAbilities[UnityEngine.Random.Range(0, upgradableAbilities.Count)];
            AbilityData selectedData = AbilityDatabase.GetAbility(selected.id);

     
            abilityManager.SelectAbility(selectedData);
            return;
        }

        DropBonusExperience();
    }

    private void DropBonusExperience()
    {
        // 보너스 경험치 구슬 생성
        GameObject expOrbPrefab = Resources.Load<GameObject>("Prefabs/ExpOrb");
        if (expOrbPrefab == null) return;

        Vector3 spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        spawnPos.z = 0f;

        for (int i = 0; i < 5; i++) // 큰 경험치 5개
        {
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
            GameObject expOrb = Instantiate(expOrbPrefab, spawnPos + offset, Quaternion.identity);

            ExpOrb orbScript = expOrb.GetComponent<ExpOrb>();
            if (orbScript != null)
            {
                orbScript.SetExpType("big");
            }
        }
    }

    private void UpdateWaveSpawning()
    {
        if (currentWaveIndex >= waves.Count) return;

        WaveData currentWave = waves[currentWaveIndex];
        int waveNumber = currentWaveIndex + 1;

        if (battleTime >= currentWave.startTime && battleTime <= currentWave.endTime)
        {
            if (!currentWave.gameObject.activeSelf)
            {
                StartWave(currentWave);
            }
        }
        else if (battleTime > currentWave.endTime && currentWave.gameObject.activeSelf)
        {
            EndWave(currentWave, waveNumber);
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

                // 수정: 진화 무기 체크
                if (abilityData.type == AbilityType.evolution)
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
        yield return new WaitUntil(() => currentWaveIndex >= waves.Count);
        BattleWin();
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
    [Header("Boss Boundary Visuals")]
    [SerializeField] private GameObject boundaryVisualPrefab; // 라인 렌더러 프리팹
    private List<GameObject> boundaryVisuals = new List<GameObject>();

    private void ActivateBoundaries()
    {
        
        // 기존 충돌 바운더리 활성화
        if (topBoundary != null)
        {
            topBoundary.SetActive(true);
            topBoundary.transform.position = new Vector3(0, boundaryYTop, 0);
        }

        if (bottomBoundary != null)
        {
            bottomBoundary.SetActive(true);
            bottomBoundary.transform.position = new Vector3(0, boundaryYBottom, 0);
        }

        if (leftBoundary != null)
        {
            leftBoundary.SetActive(true);
            leftBoundary.transform.position = new Vector3(boundaryXLeft, 0, 0);
        }

        if (rightBoundary != null)
        {
            rightBoundary.SetActive(true);
            rightBoundary.transform.position = new Vector3(boundaryXRight, 0, 0);
        }

        // 시각적 바운더리 생성
        CreateBoundaryVisuals();

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.SetMovementBounds(boundaryYBottom, boundaryYTop,boundaryXLeft,boundaryXRight);
        }
    }

    private void CreateBoundaryVisuals()
    {
        // 기존 비주얼 제거
        ClearBoundaryVisuals();

        // LineRenderer를 사용한 경계선 생성
        GameObject boundaryObj = new GameObject("BoundaryVisual");
        LineRenderer lineRenderer = boundaryObj.AddComponent<LineRenderer>();

        // LineRenderer 설정
        lineRenderer.positionCount = 5; // 사각형 + 닫기
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // 머티리얼 설정 (빨간색 발광 효과)
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 0f, 0f, 0.8f);
        lineRenderer.endColor = new Color(1f, 0f, 0f, 0.8f);

        // 정렬 레이어 설정
        lineRenderer.sortingLayerName = "UI"; // 또는 원하는 레이어
        lineRenderer.sortingOrder = 100;

        // 경계선 좌표 설정
        Vector3[] positions = new Vector3[5]
        {
        new Vector3(boundaryXLeft, boundaryYTop, 0),      // 좌상
        new Vector3(boundaryXRight, boundaryYTop, 0),     // 우상
        new Vector3(boundaryXRight, boundaryYBottom, 0),  // 우하
        new Vector3(boundaryXLeft, boundaryYBottom, 0),   // 좌하
        new Vector3(boundaryXLeft, boundaryYTop, 0)       // 좌상 (닫기)
        };

        lineRenderer.SetPositions(positions);

        boundaryVisuals.Add(boundaryObj);

        // 깜빡이는 효과 추가 (선택사항)
        StartCoroutine(PulseBoundaryEffect(lineRenderer));
    }

    private IEnumerator PulseBoundaryEffect(LineRenderer lineRenderer)
    {
        float pulseSpeed = 2f;
        Color baseColor = new Color(1f, 0f, 0f, 0.8f);

        while (lineRenderer != null && isBossFight)
        {
            float alpha = Mathf.Lerp(0.4f, 1f, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            Color pulseColor = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

            lineRenderer.startColor = pulseColor;
            lineRenderer.endColor = pulseColor;

            yield return null;
        }
    }

    private void ClearBoundaryVisuals()
    {
        foreach (GameObject visual in boundaryVisuals)
        {
            if (visual != null)
            {
                Destroy(visual);
            }
        }
        boundaryVisuals.Clear();
    }

    private void DeactivateBoundaries()
    {
        if (topBoundary != null) topBoundary.SetActive(false);
        if (bottomBoundary != null) bottomBoundary.SetActive(false);
        if (leftBoundary != null) leftBoundary.SetActive(false);
        if (rightBoundary != null) rightBoundary.SetActive(false);

        // 시각적 바운더리 제거
        ClearBoundaryVisuals();

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.RemoveMovementBounds();
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