using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("Weapon Prefabs")]
    public GameObject forceFieldSpawnerPrefab;
    public GameObject defenderCenterPrefab;
    public GameObject kunaiWeaponPrefab;

    private GameObject currentForceField;
    private GameObject currentDefender;
    private KunaiWeapon currentKunai;

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

    // 쿠나이 활성화/업그레이드 (ID 6)
    public void ActivateKunai(AbilityLevelData levelData, int level)
    {
        if (currentKunai == null)
        {
            GameObject kunaiObj = Instantiate(kunaiWeaponPrefab, transform.position, Quaternion.identity, transform);
            currentKunai = kunaiObj.GetComponent<KunaiWeapon>();
     
        }

        if (currentKunai != null)
        {
            currentKunai.SetLevel(level, levelData);
        }
    }

    // 방어막 활성화 (ID 4)
    public void ActivateForceField(int level)
    {
        if (currentForceField == null)
        {
            currentForceField = Instantiate(forceFieldSpawnerPrefab, transform.position, Quaternion.identity, transform);
       
        }

        ForceFieldSponer spawner = currentForceField.GetComponent<ForceFieldSponer>();
        if (spawner != null)
        {
            float scale = 0.5f + (level * 0.1f);
            spawner.Restart(scale, false);
     
        }
    }

    // 중력장 활성화 (ID 7 - 진화)
    public void ActivateGravityField()
    {
        if (currentForceField == null)
        {
            currentForceField = Instantiate(forceFieldSpawnerPrefab, transform.position, Quaternion.identity, transform);
        }

        ForceFieldSponer spawner = currentForceField.GetComponent<ForceFieldSponer>();
        if (spawner != null)
        {
            spawner.Restart(1.5f, true);
    
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