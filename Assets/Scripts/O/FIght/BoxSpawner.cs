using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private int maxBoxCount = 10; // 필드에 동시에 존재할 최대 박스 수
    [SerializeField] private float spawnInterval = 5f; // 스폰 간격

    [Header("스폰 범위 설정")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float spawnRangeMultiplier = 1.5f; // 카메라 범위의 1.5배

    [Header("스폰 위치 제한")]
    [SerializeField] private float minDistanceFromPlayer = 5f; // 플레이어로부터 최소 거리

    private List<GameObject> spawnedBoxes = new List<GameObject>();
    private Transform player;
    private bool isSpawning = true;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 스폰 시작
        StartCoroutine(SpawnBoxRoutine());
    }

    private IEnumerator SpawnBoxRoutine()
    {
        while (isSpawning)
        {
            // 리스트에서 파괴된 박스 제거
            spawnedBoxes.RemoveAll(box => box == null);

            // 최대 개수보다 적으면 스폰
            if (spawnedBoxes.Count < maxBoxCount)
            {
                SpawnBox();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnBox()
    {
        if (boxPrefab == null)
        {
          
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject box = Instantiate(boxPrefab, spawnPosition, Quaternion.identity);
        spawnedBoxes.Add(box);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPos = Vector3.zero;
        int maxAttempts = 30; // 최대 시도 횟수
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            // 카메라 범위 내 랜덤 위치
            float cameraHeight = mainCamera.orthographicSize * 2f * spawnRangeMultiplier;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            float randomX = Random.Range(-cameraWidth / 2f, cameraWidth / 2f);
            float randomY = Random.Range(-cameraHeight / 2f, cameraHeight / 2f);

            spawnPos = new Vector3(
                mainCamera.transform.position.x + randomX,
                mainCamera.transform.position.y + randomY,
                0f
            );

            // 플레이어와의 거리 체크
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(spawnPos, player.position);
                if (distanceToPlayer >= minDistanceFromPlayer)
                {
                    // 적절한 위치 찾음
                    break;
                }
            }
            else
            {
                // 플레이어가 없으면 그냥 스폰
                break;
            }

            attempts++;
        }

        return spawnPos;
    }

    // 특정 범위 내에 스폰하는 버전
    private Vector3 GetRandomSpawnPositionInRange(float minX, float maxX, float minY, float maxY)
    {
        Vector3 spawnPos = Vector3.zero;
        int maxAttempts = 30;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            spawnPos = new Vector3(randomX, randomY, 0f);

            // 플레이어와의 거리 체크
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(spawnPos, player.position);
                if (distanceToPlayer >= minDistanceFromPlayer)
                {
                    break;
                }
            }
            else
            {
                break;
            }

            attempts++;
        }

        return spawnPos;
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    public void ResumeSpawning()
    {
        isSpawning = true;
        StartCoroutine(SpawnBoxRoutine());
    }

    // 모든 박스 제거
    public void ClearAllBoxes()
    {
        foreach (GameObject box in spawnedBoxes)
        {
            if (box != null)
            {
                Destroy(box);
            }
        }
        spawnedBoxes.Clear();
    }

    private void OnDrawGizmos()
    {
        if (mainCamera == null) return;

        // 스폰 범위 시각화
        float cameraHeight = mainCamera.orthographicSize * 2f * spawnRangeMultiplier;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            mainCamera.transform.position,
            new Vector3(cameraWidth, cameraHeight, 0f)
        );

        // 플레이어 주변 제외 범위
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }
}