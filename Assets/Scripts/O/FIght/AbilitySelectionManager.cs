using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject levelUpCanvas; // LevelUpCanvas

    [Header("Ability Panels (6개)")]
    public List<AbilityPanel> abilityPanels; // AbilityPanelFirst, Second, Third 등

    [Header("Equipment Panel")]
    public Transform wIconParent; // WIcon 부모 (무기 아이콘들)
    public Transform sIconParent; // SIcon 부모 (패시브 아이콘들)
    public GameObject iconPrefab; // 아이콘 Prefab

    [Header("Player State")]
    public List<PlayerAbility> ownedAbilities = new List<PlayerAbility>();
    private Dictionary<string, GameObject> equipmentIcons = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> EquipmentIcons => equipmentIcons;
    private int weaponSlotIndex = 0;
    private int passiveSlotIndex = 0;

    // 모든 능력 ID 목록
    private static readonly string[] AllAbilityIds = new string[]
    {
        // 무기
        "Weapon_Brick",
        "Weapon_DrillShot",
        "Weapon_Durian",
        "Weapon_Electronic",
        "Weapon_Guardian",
        "Weapon_Kunai",
        "Weapon_Rocket",
        "Weapon_Shiled",
        // 패시브
        "Passive_EnergyCube",
        "Passive_EnergyDrink",
        "Passive_Exoskeleton",
        "Passive_FitnessGuide",
        "Passive_Fuel",
        "Passive_NinzaScroll",
        "Passive_Thruster",
        // 진화
        "Evolution_Brick",
        "Evolution_DrillShot",
        "Evolution_Durian",
        "Evolution_Electronic",
        "Evolution_Guardian",
        "Evolution_Kunai",
        "Evolution_Rocket",
        "Evolution_Shiled"
    };

    void Start()
    {
        AbilityDatabase.Initialize();

        // 모든 슬롯 비활성화
        DisableAllSlots();

        InitializeWithKunai();
    }

    void DisableAllSlots()
    {
        // 무기 슬롯 6개 전부 비활성화
        if (wIconParent != null)
        {
            for (int i = 0; i < wIconParent.childCount; i++)
            {
                Transform slot = wIconParent.GetChild(i);
                if (slot.childCount > 0)
                {
                    Image img = slot.GetChild(0).GetComponent<Image>();
                    if (img != null)
                    {
                        img.enabled = false;
                    }
                }
            }
        }

        // 패시브 슬롯 6개 전부 비활성화
        if (sIconParent != null)
        {
            for (int i = 0; i < sIconParent.childCount; i++)
            {
                Transform slot = sIconParent.GetChild(i);
                if (slot.childCount > 0)
                {
                    Image img = slot.GetChild(0).GetComponent<Image>();
                    if (img != null)
                    {
                        img.enabled = false;
                    }
                }
            }
        }
    }

    void InitializeWithKunai()
    {
        // 쿠나이(id 6) 기본 장착 - 1레벨
        PlayerAbility kunai = new PlayerAbility
        {
            id = "Weapon_Kunai",
            currentLevel = 1
        };
        ownedAbilities.Add(kunai);

        // 장비창에 쿠나이 표시
        AddToEquipmentPanel(kunai);

        // 실제 게임에 쿠나이 적용
        ApplyAbilityToGame("Weapon_Kunai", 1);
    }

    public void ShowAbilitySelection()
    {
        if (levelUpCanvas != null)
        {
            levelUpCanvas.SetActive(true);
        }

        List<AbilityData> availableAbilities = GetAvailableAbilities();

        //  남은 선택지 개수별 처리 
        if (availableAbilities.Count == 0)
        {
     
            ShowRewardSelection();
            return;
        }
        else if (availableAbilities.Count == 1)
        {
           
            ShowSingleAbility(availableAbilities[0]);
            return;
        }
        else if (availableAbilities.Count == 2)
        {
      
            ShowTwoAbilities(availableAbilities);
            return;
        }
     

        // 일반적인 경우 (3개 이상)
        List<AbilityData> selectedAbilities = GetRandomAbilities(availableAbilities);

        for (int i = 0; i < abilityPanels.Count; i++)
        {
            if (i < selectedAbilities.Count)
            {
                AbilityData ability = selectedAbilities[i];
                PlayerAbility playerAbility = ownedAbilities.Find(x => x.id == ability.id);
                int nextLevel = playerAbility != null ? playerAbility.currentLevel + 1 : 1;

                abilityPanels[i].Setup(ability, nextLevel, this);
                abilityPanels[i].gameObject.SetActive(true);
            }
            else
            {
                abilityPanels[i].gameObject.SetActive(false);
            }
        }
    }

    // 1개 남았을 때 - 가운데만 표시
    void ShowSingleAbility(AbilityData ability)
    {
        for (int i = 0; i < abilityPanels.Count; i++)
        {
            if (i == 2) // 가운데 패널
            {
                PlayerAbility playerAbility = ownedAbilities.Find(x => x.id == ability.id);
                int nextLevel = playerAbility != null ? playerAbility.currentLevel + 1 : 1;

                abilityPanels[i].Setup(ability, nextLevel, this);
                abilityPanels[i].gameObject.SetActive(true);
            }
            else
            {
                abilityPanels[i].gameObject.SetActive(false);
            }
        }
    }

    // 2개 남았을 때 - 좌우만 표시
    void ShowTwoAbilities(List<AbilityData> abilities)
    {
        int[] positions = { 0, 5 }; // 좌우 인덱스

        for (int i = 0; i < abilityPanels.Count; i++)
        {
            if (i == positions[0] || i == positions[1])
            {
                int abilityIndex = i == positions[0] ? 0 : 1;
                AbilityData ability = abilities[abilityIndex];
                PlayerAbility playerAbility = ownedAbilities.Find(x => x.id == ability.id);
                int nextLevel = playerAbility != null ? playerAbility.currentLevel + 1 : 1;

                abilityPanels[i].Setup(ability, nextLevel, this);
                abilityPanels[i].gameObject.SetActive(true);
            }
            else
            {
                abilityPanels[i].gameObject.SetActive(false);
            }
        }
    }

    // 모든 능력 획득 시 - 체력 회복 vs 골드 획득
    void ShowRewardSelection()
    {
        int[] positions = { 0, 5 }; // 좌우 인덱스

        for (int i = 0; i < abilityPanels.Count; i++)
        {
            if (i == positions[0]) // 체력 회복
            {
                // 여기서 이름, 설명, 아이콘 이름을 원하는 대로 변경
                abilityPanels[i].SetupReward(
                    "체력 회복",                    // 이름 (원하는 텍스트로 변경)
                    "즉시 최대 체력의 30% 회복",      // 설명 (원하는 텍스트로 변경)
                    "heal",                         // 리워드 타입 (변경 X)
                    this
                );
                abilityPanels[i].gameObject.SetActive(true);
            }
            else if (i == positions[1]) // 골드 획득
            {
                // 여기서 이름, 설명, 아이콘 이름을 원하는 대로 변경
                abilityPanels[i].SetupReward(
                    "골드 획득",                    // 이름 (원하는 텍스트로 변경)
                    "골드 300을 획득합니다",         // 설명 (원하는 텍스트로 변경)
                    "gold",                        // 리워드 타입 (변경 X)
                    this
                );
                abilityPanels[i].gameObject.SetActive(true);
            }
            else
            {
                abilityPanels[i].gameObject.SetActive(false);
            }
        }
    }

    // 보상 선택 처리
    public void SelectReward(string rewardType)
    {
        if (rewardType == "heal")
        {
            // 체력 30% 회복
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                // PlayerController의 Heal 메서드 사용
                // baseAmount는 실제로 사용되지 않고 내부에서 30% 계산함
                player.Heal(0f);
           
            }
        }
        else if (rewardType == "gold")
        {
            // 골드 획득
            int goldAmount = 100;

      
        }

        if (levelUpCanvas != null)
        {
            levelUpCanvas.SetActive(false);
        }
        Time.timeScale = 1f;
    }
    void ApplyAbilityToGame(string abilityId, int level)
    {
        AbilityData ability = AbilityDatabase.GetAbility(abilityId);
        if (ability == null) return;

        AbilityLevelData levelData = ability.levels[level - 1];

        if (ability.type == AbilityType.weapon || ability.type == AbilityType.evolution)
        {
            ApplyWeapon(ability, levelData, level);
        }
        else if (ability.type == AbilityType.passive)
        {
            ApplyPassive(ability, levelData, level);
        }
    }

    void ApplyWeapon(AbilityData ability, AbilityLevelData levelData, int level)
    {
        if (WeaponManager.Instance == null)
        {
            return;
        }


        switch (ability.id)
        {
            // 일반 무기
            case "Weapon_Kunai":
                WeaponManager.Instance.ActivateKunai(levelData, level);
                break;

            case "Weapon_DrillShot":
                WeaponManager.Instance.ActivateDrillShot(levelData, level);
                break;

            case "Weapon_Brick":
                WeaponManager.Instance.ActivateBrick(levelData, level);
                break;

            case "Weapon_Shiled":
                WeaponManager.Instance.ActivateForceField(levelData, level);
                break;

            case "Weapon_Guardian":
                WeaponManager.Instance.ActivateDefender(levelData, level);
                break;

            case "Weapon_Durian":
                WeaponManager.Instance.ActivateDurian(levelData, level);
                break;

            case "Weapon_Electronic":
                WeaponManager.Instance.ActivateElectronic(levelData, level);
                break;

            case "Weapon_Rocket":
                WeaponManager.Instance.ActivateRocket(levelData, level);
                break;

            // 진화 무기
            case "Evolution_Kunai":
                WeaponManager.Instance.ActivateGhostKunai(levelData);
                break;

            case "Evolution_Brick":
                WeaponManager.Instance.ActivateDumbbell(levelData, level);
                break;

            case "Evolution_Shiled":
                WeaponManager.Instance.ActivateGravityField(levelData);
                break;

            case "Evolution_Guardian":
                WeaponManager.Instance.ActivateGuardian();
                break;

            case "Evolution_DrillShot":
                WeaponManager.Instance.ActivateDrillShotEvolution(levelData, level);
                break;

            case "Evolution_Durian":
                WeaponManager.Instance.ActivateDurianEvolution(levelData, level);
                break;

            case "Evolution_Electronic":
                WeaponManager.Instance.ActivateElectronicEvolution(levelData, level);
                break;

            case "Evolution_Rocket":
                WeaponManager.Instance.ActivateRocketEvolution(levelData, level);
                break;

            default:
       
                break;
        }
    }
    public bool CanEvolve(AbilityData evolutionAbility)
    {
        PlayerAbility evolutionOwned = ownedAbilities.Find(x => x.id == evolutionAbility.id);
        if (evolutionOwned != null) return false;

        foreach (string weaponId in AllAbilityIds)
        {
            AbilityData weapon = AbilityDatabase.GetAbility(weaponId);

            if (weapon == null || weapon.type != AbilityType.weapon) continue;
            if (weapon.evolution.result != evolutionAbility.id) continue;

            PlayerAbility weaponOwned = ownedAbilities.Find(x => x.id == weapon.id);
            if (weaponOwned == null || weaponOwned.currentLevel < weapon.maxLevel) continue;

            if (!string.IsNullOrEmpty(weapon.evolution.requireItem))
            {
                PlayerAbility passiveOwned = ownedAbilities.Find(x => x.id == weapon.evolution.requireItem);
                if (passiveOwned == null || passiveOwned.currentLevel < 1) continue;
            }

            return true;
        }

        return false;
    }
    void ApplyPassive(AbilityData ability, AbilityLevelData levelData, int level)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        BattleStat stat = player.GetBattleStat();

      
        // 가산 보너스 (고정값 증가)
        stat.maxHP.additive += levelData.maxHPIncrease;
        //stat.attack.additive += levelData.damageRate - 1f;
        stat.moveSpeed.additive += levelData.speedIncrease;
        stat.getHPWithMeat.additive += levelData.healHPIncrease;

        // 승산 보너스 (퍼센트 증가)
        stat.getExp.multiplier += levelData.getEXPIncrease;
        stat.range.multiplier += levelData.rangeIncrease;
        stat.projectileSpeed.multiplier += levelData.speedIncrease;
        stat.cooldown.multiplier -= levelData.cooldownDecrease;
        stat.duration.multiplier += levelData.durationIncrease;

        stat.Refresh();
        player.RefreshStats();

    }
    List<AbilityData> GetAvailableAbilities()
    {
        List<AbilityData> available = new List<AbilityData>();

        int weaponCount = ownedAbilities.Count(x =>
        {
            AbilityData data = AbilityDatabase.GetAbility(x.id);
            return data != null && data.type == AbilityType.weapon;
        });

        int passiveCount = ownedAbilities.Count(x =>
        {
            AbilityData data = AbilityDatabase.GetAbility(x.id);
            return data != null && data.type == AbilityType.passive;
        });

        foreach (string abilityId in AllAbilityIds)
        {
            AbilityData ability = AbilityDatabase.GetAbility(abilityId);

            if (ability == null) continue;

            // 진화 무기는 별도 처리
            if (ability.type == AbilityType.evolution)
            {
                // 이미 진화했으면 추가 안 함
                PlayerAbility evolutionOwned = ownedAbilities.Find(x => x.id == ability.id);
                if (evolutionOwned != null) continue; // 이미 보유 중이면 스킵

                if (CanEvolve(ability))
                {
                    available.Add(ability);
                }
                continue;
            }

            // 무기
            if (ability.type == AbilityType.weapon)
            {
                // 이 무기가 진화했는지 확인
                if (HasEvolved(ability.id))
                {
                    continue; // 진화했으면 원본 무기는 선택지에 안 나옴
                }

                PlayerAbility owned = ownedAbilities.Find(x => x.id == ability.id);

                if (owned == null && weaponCount >= 6)
                {
                    continue;
                }

                if (owned == null)
                {
                    available.Add(ability);
                }
                else if (owned.currentLevel < ability.maxLevel)
                {
                    available.Add(ability);
                }
            }
            // 패시브
            else if (ability.type == AbilityType.passive)
            {
                PlayerAbility owned = ownedAbilities.Find(x => x.id == ability.id);

                if (owned == null && passiveCount >= 6)
                {
                    continue;
                }

                if (owned == null)
                {
                    available.Add(ability);
                }
                else if (owned.currentLevel < ability.maxLevel)
                {
                    available.Add(ability);
                }
            }
        }




        return available;
    }

    //  이 무기가 진화했는지 확인
    private bool HasEvolved(string weaponId)
    {
        AbilityData weapon = AbilityDatabase.GetAbility(weaponId);
        if (weapon == null || weapon.type != AbilityType.weapon) return false;

        // 이 무기의 진화 결과물이 있는지 확인
        if (!string.IsNullOrEmpty(weapon.evolution.result))
        {
            PlayerAbility evolutionOwned = ownedAbilities.Find(x => x.id == weapon.evolution.result);
            return evolutionOwned != null; // 진화 무기를 보유하고 있으면 true
        }

        return false;
    }
    List<AbilityData> GetRandomAbilities(List<AbilityData> available)
    {
        List<AbilityData> passives = available.Where(x => x.type == AbilityType.passive).ToList();
        List<AbilityData> weapons = available.Where(x => x.type == AbilityType.weapon || x.type == AbilityType.evolution).ToList();

        List<AbilityData> selected = new List<AbilityData>();

        // 패시브 3개 선택
        int passiveCount = Mathf.Min(3, passives.Count);
        for (int i = 0; i < passiveCount; i++)
        {
            if (passives.Count > 0)
            {
                int randomIndex = Random.Range(0, passives.Count);
                selected.Add(passives[randomIndex]);
                passives.RemoveAt(randomIndex);
            }
        }

        // 무기 3개 선택
        int weaponCount = Mathf.Min(3, weapons.Count);
        for (int i = 0; i < weaponCount; i++)
        {
            if (weapons.Count > 0)
            {
                int randomIndex = Random.Range(0, weapons.Count);
                selected.Add(weapons[randomIndex]);
                weapons.RemoveAt(randomIndex);
            }
        }

        // LINQ로 섞기
        return selected.OrderBy(x => Random.value).ToList();
    }

    // AbilitySelectionManager.cs의 SelectAbility 메서드에서
    public void SelectAbility(AbilityData ability)
    {
        PlayerAbility owned = ownedAbilities.Find(x => x.id == ability.id);

        int appliedLevel = 1;

        if (owned == null)
        {
            if (ability.type == AbilityType.evolution)
            {
                string baseWeaponId = FindBaseWeaponForEvolution(ability.id);
                if (!string.IsNullOrEmpty(baseWeaponId))
                {
                    PlayerAbility baseWeapon = ownedAbilities.Find(x => x.id == baseWeaponId);
                    if (baseWeapon != null)
                    {
                        //  원본 무기의 인덱스 저장
                        int baseWeaponIndex = ownedAbilities.IndexOf(baseWeapon);

                        // 원본 무기 제거
                        ownedAbilities.Remove(baseWeapon);

                        // 진화 무기를 원본 무기가 있던 위치에 삽입
                        PlayerAbility newEvolution = new PlayerAbility
                        {
                            id = ability.id,
                            currentLevel = 1
                        };
                        ownedAbilities.Insert(baseWeaponIndex, newEvolution);

                        ReplaceEquipmentIcon(baseWeapon.id, ability);
                        appliedLevel = 1;
                    }
                }
            }
            else
            {
                PlayerAbility newAbility = new PlayerAbility
                {
                    id = ability.id,
                    currentLevel = 1
                };
                ownedAbilities.Add(newAbility);
                appliedLevel = 1;
                AddToEquipmentPanel(newAbility);
            }
        }
        else
        {
            owned.currentLevel++;
            appliedLevel = owned.currentLevel;
            UpdateEquipmentPanel(owned);
        }

        ApplyAbilityToGame(ability.id, appliedLevel);

        if (levelUpCanvas != null)
        {
            levelUpCanvas.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    //  새로운 메서드: 원본 무기 슬롯의 아이콘을 진화 무기로 교체
    private void ReplaceEquipmentIcon(string baseWeaponId, AbilityData evolutionAbility)
    {
        if (!equipmentIcons.ContainsKey(baseWeaponId)) return;

        GameObject baseSlot = equipmentIcons[baseWeaponId];
        if (baseSlot == null) return;

        Transform iconTransform = baseSlot.transform.GetChild(0);
        Image iconImage = iconTransform.GetComponent<Image>();
        if (iconImage == null) return;

        // 진화 무기 스프라이트로 교체
        Sprite sprite = Resources.Load<Sprite>($"Sprites/Classified/Ability_Icon/{evolutionAbility.spriteName}");
        if (sprite != null)
        {
            iconImage.sprite = sprite;
            iconImage.enabled = true;
            iconImage.color = Color.white;
        }

        // equipmentIcons 딕셔너리 업데이트
        equipmentIcons.Remove(baseWeaponId);
        equipmentIcons[evolutionAbility.id] = baseSlot;
    }

    void AddToEquipmentPanel(PlayerAbility ability)
    {
        AbilityData abilityData = AbilityDatabase.GetAbility(ability.id);
        if (abilityData == null) return;

        bool isWeapon = (abilityData.type == AbilityType.weapon || abilityData.type == AbilityType.evolution);
        Transform parent = isWeapon ? wIconParent : sIconParent;

        if (parent == null) return;

        int slotIndex = isWeapon ? weaponSlotIndex : passiveSlotIndex;

        if (slotIndex >= parent.childCount) return;

        Transform slotTransform = parent.GetChild(slotIndex);
        if (slotTransform.childCount < 1) return;

        Transform iconTransform = slotTransform.GetChild(0);
        Image iconImage = iconTransform.GetComponent<Image>();
        if (iconImage == null) return;

        Sprite sprite = Resources.Load<Sprite>($"Sprites/Classified/Ability_Icon/{abilityData.spriteName}");
        if (sprite != null)
        {
            iconImage.sprite = sprite;
            iconImage.enabled = true;
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        equipmentIcons[ability.id] = slotTransform.gameObject;

        // 새 무기/패시브만 인덱스 증가
        if (isWeapon)
        {
            weaponSlotIndex++;
        }
        else
        {
            passiveSlotIndex++;
        }
    }

    // 진화 무기의 원본 무기 ID 찾기
    private string FindBaseWeaponForEvolution(string evolutionId)
    {
        foreach (string weaponId in AllAbilityIds)
        {
            AbilityData weapon = AbilityDatabase.GetAbility(weaponId);

            if (weapon == null || weapon.type != AbilityType.weapon) continue;
            if (weapon.evolution.result == evolutionId)
            {
                return weapon.id;
            }
        }

        return null;
    }

    // 장비창에서 제거
    private void RemoveFromEquipmentPanel(PlayerAbility ability)
    {
        if (equipmentIcons.ContainsKey(ability.id))
        {
            GameObject iconSlot = equipmentIcons[ability.id];
            if (iconSlot != null)
            {
                Transform iconTransform = iconSlot.transform.GetChild(0);
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.enabled = false;
                }
            }
            equipmentIcons.Remove(ability.id);
        }
    }
  

    void UpdateEquipmentPanel(PlayerAbility ability)
    {
        // 레벨 표시 업데이트 (필요시)
    }

    public string GetEvolutionRequirement(AbilityData evolutionAbility)
    {
        if (evolutionAbility.type != AbilityType.evolution) return "";

        foreach (string weaponId in AllAbilityIds)
        {
            AbilityData weapon = AbilityDatabase.GetAbility(weaponId);

            if (weapon == null || weapon.type != AbilityType.weapon) continue;
            if (weapon.evolution.result != evolutionAbility.id) continue;

            if (!string.IsNullOrEmpty(weapon.evolution.requireItem))
            {
                AbilityData passive = AbilityDatabase.GetAbility(weapon.evolution.requireItem);
                if (passive != null)
                {
                    return $"{weapon.name} Lv.{weapon.maxLevel} + {passive.name} Lv.1";
                }
            }
        }

        return "";
    }
}