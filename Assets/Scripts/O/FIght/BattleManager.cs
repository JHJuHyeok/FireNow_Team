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

    [Header("Battle Settings")]
    [SerializeField] private float battleSpeed = 1f;

    [Header("Time Settings")]
    [SerializeField] private float maxTime = 900f; // 15분



    private int currentWaveIndex = 0;
    private float battleTime = 0f;
    private bool isBattleActive = false;

    public event Action<int> OnWaveStart;
    public event Action<int> OnWaveComplete;
    public event Action OnBattleWin;
    public event Action OnBattleLose;

    // 싱글톤 인스턴스 생성
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

        // 메인 카메라가 설정되지 않았으면 자동으로 찾기
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    // 초기화
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

    }

    // 전투 시간 업데이트 및 웨이브 스폰 체크
    private void Update()
    {
        if (isBattleActive)
        {
            battleTime += Time.deltaTime;
            UpdateWaveSpawning();
            TimeDisplay();
        }
    }


    private void ActivateUI()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
            isPaused = true;
            Time.timeScale = 0f;

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



  
    // 전투 초기 설정
    public void InitializeBattle()
    {
        currentWaveIndex = 0;
        battleTime = 0f;
        isBattleActive = false;

        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);
    }

    // 전투 시작
    public void StartBattle()
    {
        isBattleActive = true;
        battleTime = 0f;
        StartCoroutine(BattleSequence());
    }


    private void TimeDisplay()
    {
        int minutes = Mathf.FloorToInt(battleTime / 60f);
        int seconds = Mathf.FloorToInt(battleTime  % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    // 전투 진행 시퀀스 (모든 웨이브가 끝날 때까지 진행)
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

    // 각 웨이브의 시작/종료 시간 체크 및 활성화
    private void UpdateWaveSpawning()
    {
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

    // 웨이브 시작 처리
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

    // 웨이브 종료 처리
    private void EndWave(WaveData wave)
    {
        wave.gameObject.SetActive(false);
        OnWaveComplete?.Invoke(waves.IndexOf(wave));
    }

    // 웨이브의 적들을 spawnRate 간격으로 spawnCount만큼 생성
    private IEnumerator SpawnWaveEnemies(WaveData wave)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(wave.enemiesPath);
        if (jsonFile == null) yield break;

        EnemyDatabase waveEnemies = JsonUtility.FromJson<EnemyDatabase>(jsonFile.text);

        int spawnedCount = 0;
        float nextSpawnTime = 0f;
        float waveStartTime = battleTime;

        while (spawnedCount < wave.spawnCount && battleTime < wave.endTime)
        {
            if (battleTime - waveStartTime >= nextSpawnTime)
            {
                if (waveEnemies.list.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, waveEnemies.list.Count);
                    EnemyData enemyData = waveEnemies.list[randomIndex];

                    SpawnEnemy(enemyData);
                    spawnedCount++;
                    nextSpawnTime += wave.spawnRate;
                }
            }

            yield return null;
        }
    }

    // 화면 내 랜덤 위치 반환
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

    // EnemyData를 기반으로 적 생성
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

        // Enemy 스크립트 초기화
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Initialize(enemyData);
        }

    
    }
    // 전투 승리 처리
    private void BattleWin()
    {
        isBattleActive = false;
        OnBattleWin?.Invoke();

        if (victoryPanel)
        {
            victoryPanel.SetActive(true);
        }
    }

    // 전투 패배 처리
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

    // 전투 속도 설정 (0.5배 ~ 3배)
    public void SetBattleSpeed(float speed)
    {
        battleSpeed = Mathf.Clamp(speed, 0.5f, 3f);
        Time.timeScale = battleSpeed;
    }

    // 전투 일시정지
    public void PauseBattle()
    {
        Time.timeScale = 0f;
        isBattleActive = false;
    }

    // 전투 재개
    public void ResumeBattle()
    {
        Time.timeScale = battleSpeed;
        isBattleActive = true;
    }
}