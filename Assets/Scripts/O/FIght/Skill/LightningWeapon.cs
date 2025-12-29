using System.Collections;
using UnityEngine;

public class LightningWeapon : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject lightningPrefab;

    [Header("Stats")]
    private float damageRate = 1f;
    private int strikeCount = 1;
    private float cooldown = 1f;
    private float range = 1f;

    [Header("Spawn Settings")]
    private Camera mainCamera;
    private float spawnMargin = 1f; // 화면 가장자리 여백

    private Transform player;
    private bool isActive = true;

    private void Start()
    {
        player = transform.parent;
        if (player == null) player = transform;

        mainCamera = Camera.main;

        StartCoroutine(StrikeRoutine());
    }

    public void UpdateStats(AbilityLevelData levelData)
    {
        damageRate = levelData.damageRate;
        strikeCount = levelData.projectileCount;
        cooldown = levelData.cooldown;
        range = levelData.range;

        Debug.Log($"번개 스탯 업데이트 - 데미지: {damageRate}, 번갯불 개수: {strikeCount}, 범위: {range}");
    }

    private IEnumerator StrikeRoutine()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(cooldown);
            StrikeLightning();
        }
    }

    private void StrikeLightning()
    {
        if (lightningPrefab == null)
        {
            Debug.LogError("번개 프리팹이 할당되지 않았습니다!");
            return;
        }

        for (int i = 0; i < strikeCount; i++)
        {
            Vector3 randomPosition = GetRandomScreenPosition();

            GameObject lightning = Instantiate(lightningPrefab, randomPosition, Quaternion.identity);
            LightningStrike strike = lightning.GetComponent<LightningStrike>();

            if (strike == null)
            {
                strike = lightning.AddComponent<LightningStrike>();
            }

            strike.Initialize(damageRate, range);
        }
    }

    private Vector3 GetRandomScreenPosition()
    {
        if (mainCamera == null) return player.position;

        // 화면 범위 내 랜덤 위치
        float randomX = Random.Range(spawnMargin, Screen.width - spawnMargin);
        float randomY = Random.Range(spawnMargin, Screen.height - spawnMargin);

        Vector3 screenPosition = new Vector3(randomX, randomY, mainCamera.nearClipPlane + 10f);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        return worldPosition;
    }

    private void OnDestroy()
    {
        isActive = false;
        StopAllCoroutines();
    }
}