using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("Weapon Prefabs")]
    public GameObject forceFieldSpawnerPrefab;
    public GameObject defenderCenterPrefab;

    //[Header("Player Weapon")]

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

    public void ActivateDefender(AbilityLevelData levelData, int level)
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

            // 데미지 설정 추가
            float damage = levelData.damageRate;
            float range = levelData.range;
            defender.UpdateDefenderStats(damage, range);
        }
    }

    public void ActivateGuardian(AbilityLevelData levelData)
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

            // 진화형 데미지 설정
            float damage = levelData.damageRate;
            float range = levelData.range;
            defender.UpdateDefenderStats(damage, range);
        }
    }
    // 쿠나이 활성화/업그레이드 (ID 6)
    public void ActivateKunai(AbilityLevelData levelData, int level)
    {

        if (playerWeapon != null)
        {
            playerWeapon.SetWeaponLevel(levelData);
        }
    }

    // 방어막 활성화 (ID 4)
    public void ActivateForceField(AbilityLevelData levelData , int level)
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
            // 여기 수정해야함 스탯 확인후
            spawner.ReStart(scale, false, 10, range);
     
        }
    }

    // 중력장 활성화 (ID 7 - 진화)
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
            // 여기 수정해야함 스탯 확인후
            spawner.ReStart(1.5f, true, 10, range);

        }
    }

    // 수호자 활성화 (ID 5)
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

    // 수비수 활성화 (ID 8 - 진화)
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
}