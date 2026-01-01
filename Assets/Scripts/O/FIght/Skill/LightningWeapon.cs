using System.Collections;
using UnityEngine;

public class LightningWeapon : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject lightningPrefab;

    [Header("Stats")]
    private float damageRate = 1f;
    private int strikeCount = 1;
    private float cooldown = 4f;
    private float range = 1f;

    [Header("Spawn Settings")]
    private Camera mainCamera;
    private float spawnMargin = 1f;
    private Transform player;
    private PlayerController playerController;
    private bool isActive = true;
    private bool isEvolved = false;


    [Header("Sound")]
    private string hitSoundName = "Lightning";

    public void SetEvolution(bool evolved)
    {
        isEvolved = evolved;
    }

    private void Start()
    {
        player = transform.parent;
        if (player == null) player = transform;

        playerController = player.GetComponent<PlayerController>();
        mainCamera = Camera.main;

        StartCoroutine(StrikeRoutine());
    }

    public void UpdateStats(AbilityLevelData levelData)
    {
        damageRate = levelData.damageRate;
        strikeCount = levelData.projectileCount;
        cooldown = levelData.cooldown;
        range = levelData.range;
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
            
            return;
        }

        float baseDamage = playerController != null ? playerController.GetAttackPower() : 15f;
        float finalDamage = baseDamage * damageRate;

        for (int i = 0; i < strikeCount; i++)
        {
            Vector3 randomPosition = GetRandomScreenPosition();
            GameObject lightning = Instantiate(lightningPrefab, randomPosition, lightningPrefab.transform.rotation);
            LightningStrike strike = lightning.GetComponent<LightningStrike>();
            if (strike == null)
            {
                strike = lightning.AddComponent<LightningStrike>();
            }

            strike.SetEvolution(isEvolved);
            strike.Initialize(finalDamage, range);
            strike.SetHitSound(hitSoundName); // È¿°úÀ½
        }
    }

    private Vector3 GetRandomScreenPosition()
    {
        if (mainCamera == null) return player.position;

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