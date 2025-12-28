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


    // 벽돌
    public GameObject brickPrefab; // 벽돌 발사체 프리팹
    // 드릴
    public GameObject drillPrefab;       
    // 로켓
    public GameObject rocketPrefab;     
    // 번개
    public GameObject lightningPrefab;
    // 두리안
    public GameObject durianPrefab;

 
    private GameObject currentBrickWeapon; 
    private GameObject currentDrillWeapon;    
    private GameObject currentRocketWeapon;   
    private GameObject currentLightningWeapon;
    private GameObject currentDurianWeapon;




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
        if (currentBrickWeapon == null)
        {
            // 빈 GameObject로 무기 매니저 생성
            currentBrickWeapon = new GameObject("BrickWeapon");
            currentBrickWeapon.transform.SetParent(transform);
            currentBrickWeapon.transform.localPosition = Vector3.zero;

            BrickWeapon brickWeapon = currentBrickWeapon.AddComponent<BrickWeapon>();
            brickWeapon.brickPrefab = brickPrefab;
        }

        BrickWeapon weapon = currentBrickWeapon.GetComponent<BrickWeapon>();
        if (weapon != null)
        {
            weapon.UpdateStats(levelData);
            Debug.Log($"벽돌 활성화 - 레벨: {level}, 데미지: {levelData.damageRate}");
        }
    }

    // ========== 드릴샷 (Weapon_DrillShot) ==========
    public void ActivateDrillShot(AbilityLevelData levelData, int level)
    {
        if (currentDrillWeapon == null)
        {
            currentDrillWeapon = new GameObject("DrillWeapon");
            currentDrillWeapon.transform.SetParent(transform);
            currentDrillWeapon.transform.localPosition = Vector3.zero;

            DrillWeapon drillWeapon = currentDrillWeapon.AddComponent<DrillWeapon>();
            drillWeapon.drillPrefab = drillPrefab;
        }

        DrillWeapon weapon = currentDrillWeapon.GetComponent<DrillWeapon>();
        if (weapon != null)
        {
            weapon.UpdateStats(levelData);
            Debug.Log($"드릴샷 활성화 - 레벨: {level}, 발사체 수: {levelData.projectileCount}");
        }
    }

    // ========== 로켓 (Weapon_Rocket) ==========
    public void ActivateRocket(AbilityLevelData levelData, int level)
    {
        if (currentRocketWeapon == null)
        {
            currentRocketWeapon = new GameObject("RocketWeapon");
            currentRocketWeapon.transform.SetParent(transform);
            currentRocketWeapon.transform.localPosition = Vector3.zero;

            RocketWeapon rocketWeapon = currentRocketWeapon.AddComponent<RocketWeapon>();
            rocketWeapon.rocketPrefab = rocketPrefab;
        }

        RocketWeapon weapon = currentRocketWeapon.GetComponent<RocketWeapon>();
        if (weapon != null)
        {
            weapon.UpdateStats(levelData);
            Debug.Log($"로켓 활성화 - 레벨: {level}, 속도: {levelData.speed}");
        }
    }

    // ========== 번개 (Weapon_Electronic) ==========
    public void ActivateElectronic(AbilityLevelData levelData, int level)
    {
        if (currentLightningWeapon == null)
        {
            currentLightningWeapon = new GameObject("LightningWeapon");
            currentLightningWeapon.transform.SetParent(transform);
            currentLightningWeapon.transform.localPosition = Vector3.zero;

            LightningWeapon lightningWeapon = currentLightningWeapon.AddComponent<LightningWeapon>();
            lightningWeapon.lightningPrefab = lightningPrefab;
        }

        LightningWeapon weapon = currentLightningWeapon.GetComponent<LightningWeapon>();
        if (weapon != null)
        {
            weapon.UpdateStats(levelData);
            Debug.Log($"번개 활성화 - 레벨: {level}, 범위: {levelData.range}");
        }
    }

    // ========== 두리안 (Weapon_Durian) ==========
    public void ActivateDurian(AbilityLevelData levelData, int level)
    {
        if (currentDurianWeapon == null)
        {
            currentDurianWeapon = new GameObject("DurianWeapon");
            currentDurianWeapon.transform.SetParent(transform);
            currentDurianWeapon.transform.localPosition = Vector3.zero;

            DurianWeapon durianWeapon = currentDurianWeapon.AddComponent<DurianWeapon>();
            durianWeapon.durianPrefab = durianPrefab;
        }

        DurianWeapon weapon = currentDurianWeapon.GetComponent<DurianWeapon>();
        if (weapon != null)
        {
            weapon.UpdateStats(levelData);
            Debug.Log($"두리안 활성화 - 레벨: {level}, 데미지: {levelData.damageRate}");
        }
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