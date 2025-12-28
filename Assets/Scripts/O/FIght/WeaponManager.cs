using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("Weapon Prefabs")]
    public GameObject forceFieldSpawnerPrefab;
    public GameObject defenderCenterPrefab;

    private PlayerWeapon playerWeapon;

    private GameObject currentForceField;
    private GameObject currentDefender;

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
    }

    // ========== 쿠나이 (Weapon_Kunai) ==========
    public void ActivateKunai(AbilityLevelData levelData, int level)
    {
        if (playerWeapon != null)
        {
            playerWeapon.SetWeaponLevel(levelData);
        }
    }

    // ========== 방어막 (Weapon_Shiled) ==========
    public void ActivateForceField(AbilityLevelData levelData, int level)
    {
        if (currentForceField == null)
        {
            currentForceField = Instantiate(forceFieldSpawnerPrefab, transform.position, Quaternion.identity, transform);
        }

        float damage = levelData.damageRate;
        float range = levelData.range;

        ForceFieldSponer spawner = currentForceField.GetComponent<ForceFieldSponer>();
        if (spawner != null)
        {
            float scale = 0.5f + (level * 0.1f);
            spawner.ReStart(scale, false, 10, range);
        }
    }

    // ========== 수호자 (Weapon_Guardian) ==========
    public void ActivateDefender(int level)
    {
        if (currentDefender == null)
        {
            currentDefender = Instantiate(defenderCenterPrefab, transform.position, Quaternion.identity, transform);
        }

        Defendersenter defender = currentDefender.GetComponent<Defendersenter>();
        if (defender != null)
        {
            defender.defenderCount = level + 1;
            defender.SetDefenderCount(defender.defenderCount);
            defender.SetEvolutution(false);
        }
    }

    // ========== 중력장 (Evolution_Shiled - 방어막 진화) ==========
    public void ActivateGravityField(AbilityLevelData levelData)
    {
        if (currentForceField == null)
        {
            currentForceField = Instantiate(forceFieldSpawnerPrefab, transform.position, Quaternion.identity, transform);
        }

        float damage = levelData.damageRate;
        float range = levelData.range;

        ForceFieldSponer spawner = currentForceField.GetComponent<ForceFieldSponer>();
        if (spawner != null)
        {
            spawner.ReStart(1.5f, true, 10, range);
        }
    }

    // ========== 수비수 (Evolution_Guardian - 수호자 진화) ==========
    public void ActivateGuardian()
    {
        if (currentDefender == null)
        {
            currentDefender = Instantiate(defenderCenterPrefab, transform.position, Quaternion.identity, transform);
        }

        Defendersenter defender = currentDefender.GetComponent<Defendersenter>();
        if (defender != null)
        {
            defender.defenderCount = 6;
            defender.SetDefenderCount(6);
            defender.SetEvolutution(true);
        }
    }

    // ========== 벽돌 (Weapon_Brick) ==========
    public void ActivateBrick(AbilityLevelData levelData, int level)
    {
        Debug.Log($"벽돌 활성화 - 레벨: {level}, 데미지: {levelData.damageRate}");
        // TODO: 벽돌 무기 구현
    }

    // ========== 드릴샷 (Weapon_DrillShot) ==========
    public void ActivateDrillShot(AbilityLevelData levelData, int level)
    {
        Debug.Log($"드릴샷 활성화 - 레벨: {level}, 발사체 수: {levelData.projectileCount}");
        // TODO: 드릴샷 무기 구현
    }

    // ========== 두리안 (Weapon_Durian) ==========
    public void ActivateDurian(AbilityLevelData levelData, int level)
    {
        Debug.Log($"두리안 활성화 - 레벨: {level}, 데미지: {levelData.damageRate}");
        // TODO: 두리안 무기 구현
    }

    // ========== 전자기 (Weapon_Electronic) ==========
    public void ActivateElectronic(AbilityLevelData levelData, int level)
    {
        Debug.Log($"전자기 활성화 - 레벨: {level}, 범위: {levelData.range}");
        // TODO: 전자기 무기 구현
    }

    // ========== 로켓 (Weapon_Rocket) ==========
    public void ActivateRocket(AbilityLevelData levelData, int level)
    {
        Debug.Log($"로켓 활성화 - 레벨: {level}, 속도: {levelData.speed}");
        // TODO: 로켓 무기 구현
    }

    // ========== 유령 쿠나이 (Evolution_Kunai - 쿠나이 진화) ==========
    public void ActivateGhostKunai(AbilityLevelData levelData)
    {
        Debug.Log($"유령 쿠나이 진화 - 데미지: {levelData.damageRate}");
        // TODO: 유령 쿠나이 구현
    }

    // ========== 덤벨 (Evolution_Brick - 벽돌 진화) ==========
    public void ActivateDumbbell(AbilityLevelData levelData, int level)
    {
        Debug.Log($"덤벨 진화 - 데미지: {levelData.damageRate}, 발사체 수: {levelData.projectileCount}");
        // TODO: 덤벨 구현
    }

    // ========== 드릴샷 진화 (Evolution_DrillShot) ==========
    public void ActivateDrillShotEvolution(AbilityLevelData levelData, int level)
    {
        Debug.Log($"드릴샷 진화 - 데미지: {levelData.damageRate}");
        // TODO: 드릴샷 진화 구현
    }

    // ========== 두리안 진화 (Evolution_Durian) ==========
    public void ActivateDurianEvolution(AbilityLevelData levelData, int level)
    {
        Debug.Log($"두리안 진화 - 데미지: {levelData.damageRate}");
        // TODO: 두리안 진화 구현
    }

    // ========== 전자기 진화 (Evolution_Electronic) ==========
    public void ActivateElectronicEvolution(AbilityLevelData levelData, int level)
    {
        Debug.Log($"전자기 진화 - 범위: {levelData.range}");
        // TODO: 전자기 진화 구현
    }

    // ========== 로켓 진화 (Evolution_Rocket) ==========
    public void ActivateRocketEvolution(AbilityLevelData levelData, int level)
    {
        Debug.Log($"로켓 진화 - 속도: {levelData.speed}");
        // TODO: 로켓 진화 구현
    }
}