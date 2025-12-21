using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    // 활성화된 능력과 현재 레벨 추적
    private Dictionary<string, MonoBehaviour> activeAbilities = new Dictionary<string, MonoBehaviour>();
    private Dictionary<string, int> abilityLevels = new Dictionary<string, int>();

    private void Start()
    {
        AbilityDatabase.Initialize();
    }

    private void Update()
    {
        // 테스트용 키 입력
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AcquireAbility("shuriken");
            //Debug.Log("Q키: 수리검 획득/레벨업");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            AcquireAbility("defender");
            //Debug.Log("W키: 수호자 획득/레벨업");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            AcquireAbility("forcefield");
            //Debug.Log("E키: 보호막 획득/레벨업");
        }
    }

    /// <summary>
    /// 능력 획득 (처음 획득 또는 레벨업)
    /// </summary>
    public void AcquireAbility(string abilityId)
    {
        AbilityData data = AbilityDatabase.GetAbility(abilityId);
        if (data == null)
        {
            //Debug.LogError($"능력을 찾을 수 없습니다: {abilityId}");
            return;
        }

        // 이미 보유한 능력이면 레벨업
        if (activeAbilities.ContainsKey(abilityId))
        {
            UpgradeAbility(abilityId);
            return;
        }

        // 처음 획득 (레벨 0)
        abilityLevels[abilityId] = 0;

        // 능력 타입별 처리
        switch (data.type)
        {
            case AbilityType.weapon:
                ActivateWeapon(data);
                break;

            case AbilityType.passive:
                ApplyPassiveAbility(data, 0);
                break;

            case AbilityType.evolution:
                // 진화형 무기 처리
                ActivateWeapon(data);
                break;
        }

        //Debug.Log($"능력 획득: {data.name} (레벨 1)");
    }

    /// <summary>
    /// 무기 타입 능력 활성화
    /// </summary>
    private void ActivateWeapon(AbilityData data)
    {
        AbilityLevelData levelData = data.levels[0]; // 첫 레벨 데이터

        switch (data.id)
        {
            case "defender":
            case "수호자": // id에 따라 케이스 추가
                ActivateDefender(data, levelData);
                break;

            case "forcefield":
            case "보호막":
                ActivateForceField(data, levelData);
                break;

            case "shuriken":
            case "수리검":
                ActivateShuriken(data, levelData);
                break;

            default:
                //Debug.LogWarning($"처리되지 않은 무기 타입: {data.id}");
                break;
        }
    }

    private void ActivateDefender(AbilityData data, AbilityLevelData levelData)
    {
        Defendersenter defender = GetComponent<Defendersenter>();
        if (defender == null)
        {
            //Debug.LogError("Defendersenter 컴포넌트가 없습니다!");
            return;
        }

        defender.defenderCount = levelData.projectileCount; // projectileCount를 개수로 사용
        defender.revolutionSpeed = levelData.speed;
        defender.spinradius = levelData.range;
        defender.enabled = true;

        activeAbilities[data.id] = defender;
    }

    private void ActivateForceField(AbilityData data, AbilityLevelData levelData)
    {
        ForceFieldSponer forceField = GetComponent<ForceFieldSponer>();
        if (forceField == null)
        {
            //Debug.LogError("ForceFieldSponer 컴포넌트가 없습니다!");
            return;
        }

        forceField.enabled = true;
        forceField.Restart(levelData.range, false);

        activeAbilities[data.id] = forceField;
    }

    private void ActivateShuriken(AbilityData data, AbilityLevelData levelData)
    {
        ShurikenShooter shooter = GetComponent<ShurikenShooter>();
        if (shooter == null)
        {
            //Debug.LogError("ShurikenShooter 컴포넌트가 없습니다!");
            return;
        }

        // ShurikenData 설정
        ShurikenData shurikenData = new ShurikenData
        {
            id = data.id,
            damage = levelData.damage,
            speed = levelData.speed,
            lifeTime = levelData.duration,
            pierceCount = 1
        };

        shooter.SetShurikenData(shurikenData);
        shooter.UpgradeShootCount(levelData.projectileCount);
        shooter.UpgradeShootSpeed(levelData.cooldown);
        shooter.enabled = true;

        activeAbilities[data.id] = shooter;
    }

    /// <summary>
    /// 패시브 능력 적용
    /// </summary>
    private void ApplyPassiveAbility(AbilityData data, int level)
    {
        if (level >= data.levels.Count)
        {
            //Debug.LogWarning($"레벨 {level}의 데이터가 없습니다.");
            return;
        }

        AbilityLevelData levelData = data.levels[level];
        PlayerController playerController = GetComponent<PlayerController>();

        if (playerController == null) return;

        // 패시브 스탯 적용 (누적)
        // playerController.maxHP += levelData.maxHPIncrease;
        // playerController.moveSpeed += levelData.speedIncrease;
        // 등등...

        //Debug.Log($"패시브 능력 적용: {data.name} - {levelData.description}");
    }

    /// <summary>
    /// 능력 레벨업
    /// </summary>
    public void UpgradeAbility(string abilityId)
    {
        if (!activeAbilities.ContainsKey(abilityId))
        {
            //Debug.LogWarning($"보유하지 않은 능력입니다: {abilityId}");
            return;
        }

        AbilityData data = AbilityDatabase.GetAbility(abilityId);
        int currentLevel = abilityLevels[abilityId];
        int nextLevel = currentLevel + 1;

        // 최대 레벨 체크
        if (nextLevel >= data.maxLevel)
        {
            //Debug.LogWarning($"{data.name}은(는) 이미 최대 레벨입니다.");
            return;
        }

        abilityLevels[abilityId] = nextLevel;
        AbilityLevelData levelData = data.levels[nextLevel];
        MonoBehaviour ability = activeAbilities[abilityId];

        switch (data.type)
        {
            case AbilityType.weapon:
                UpgradeWeapon(data, ability, levelData);
                break;

            case AbilityType.passive:
                ApplyPassiveAbility(data, nextLevel);
                break;
        }

        //Debug.Log($"능력 레벨업: {data.name} (레벨 {nextLevel + 1}) - {levelData.description}");
    }

    private void UpgradeWeapon(AbilityData data, MonoBehaviour ability, AbilityLevelData levelData)
    {
        switch (data.id)
        {
            case "defender":
            case "수호자":
                Defendersenter defender = ability as Defendersenter;
                defender.SetDefenderCount(levelData.projectileCount);
                defender.revolutionSpeed = levelData.speed;
                defender.spinradius = levelData.range;
                break;

            case "forcefield":
            case "보호막":
                ForceFieldSponer forceField = ability as ForceFieldSponer;
                forceField.Restart(levelData.range, false);
                break;

            case "shuriken":
            case "수리검":
                ShurikenShooter shooter = ability as ShurikenShooter;

                // ShurikenData 업데이트
                ShurikenData newData = new ShurikenData
                {
                    id = data.id,
                    damage = levelData.damage,
                    speed = levelData.speed,
                    lifeTime = levelData.duration,
                    pierceCount = 1
                };
                shooter.SetShurikenData(newData);
                shooter.UpgradeShootCount(levelData.projectileCount);
                shooter.UpgradeShootSpeed(levelData.cooldown);
                break;
        }
    }

    /// <summary>
    /// 능력 진화 체크 및 적용
    /// </summary>
    public void CheckEvolution(string weaponId)
    {
        AbilityData weaponData = AbilityDatabase.GetAbility(weaponId);
        if (weaponData == null || weaponData.evolution == null) return;

        // 필요한 패시브 아이템을 보유하고 있는지 체크
        if (!HasAbility(weaponData.evolution.requireItem)) return;

        // 진화된 무기로 교체
        DeactivateAbility(weaponId);
        AcquireAbility(weaponData.evolution.result);

        //Debug.Log($"능력 진화: {weaponData.name} → {weaponData.evolution.result}");
    }

    public bool HasAbility(string abilityId)
    {
        return activeAbilities.ContainsKey(abilityId);
    }

    public int GetAbilityLevel(string abilityId)
    {
        if (abilityLevels.ContainsKey(abilityId))
            return abilityLevels[abilityId];
        return -1;
    }

    private void DeactivateAbility(string abilityId)
    {
        if (!activeAbilities.ContainsKey(abilityId)) return;

        MonoBehaviour ability = activeAbilities[abilityId];
        ability.enabled = false;

        activeAbilities.Remove(abilityId);
        abilityLevels.Remove(abilityId);
    }
}