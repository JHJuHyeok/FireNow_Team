using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectionManager : MonoBehaviour
{
    [Header("UI References - 기존 LevelUpUI 사용")]
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
        List<AbilityData> selectedAbilities = GetRandomAbilities(availableAbilities);

        // UI 업데이트
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
        Debug.Log($"생성 완료: {ability.id}");

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
                WeaponManager.Instance.ActivateDefender(level);
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
                Debug.LogWarning($"무기 ID {ability.id} 아직 미구현");
                break;
        }
    }

    void ApplyPassive(AbilityData ability, AbilityLevelData levelData, int level)
    {
        //Debug.Log($"패시브 적용: {ability.name}");
        // TODO: 패시브 적용 구현
    }

    List<AbilityData> GetAvailableAbilities()
    {
        List<AbilityData> available = new List<AbilityData>();

        // 현재 보유한 무기/패시브 개수 카운트
        int weaponCount = ownedAbilities.Count(x =>
        {
            AbilityData data = AbilityDatabase.GetAbility(x.id);
            return data != null && (data.type == AbilityType.weapon || data.type == AbilityType.evolution);
        });

        int passiveCount = ownedAbilities.Count(x =>
        {
            AbilityData data = AbilityDatabase.GetAbility(x.id);
            return data != null && data.type == AbilityType.passive;
        });

        // 모든 능력 ID를 순회
        foreach (string abilityId in AllAbilityIds)
        {
            AbilityData ability = AbilityDatabase.GetAbility(abilityId);

            if (ability == null) continue;

            // 진화 무기는 별도 처리
            if (ability.type == AbilityType.evolution)
            {
                if (CanEvolve(ability))
                {
                    available.Add(ability);
                }
                continue;
            }

            // 무기가 이미 6개면 새로운 무기는 추가 안 함
            if (ability.type == AbilityType.weapon)
            {
                PlayerAbility owned = ownedAbilities.Find(x => x.id == ability.id);

                if (owned == null && weaponCount >= 6)
                {
                    continue; // 새로운 무기는 스킵
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
            // 패시브가 이미 6개면 새로운 패시브는 추가 안 함
            else if (ability.type == AbilityType.passive)
            {
                PlayerAbility owned = ownedAbilities.Find(x => x.id == ability.id);

                if (owned == null && passiveCount >= 6)
                {
                    continue; // 새로운 패시브는 스킵
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

    bool CanEvolve(AbilityData evolutionAbility)
    {
        PlayerAbility evolutionOwned = ownedAbilities.Find(x => x.id == evolutionAbility.id);
        if (evolutionOwned != null) return false;

        // 모든 무기 ID를 순회하여 진화 조건 확인
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

    public void SelectAbility(AbilityData ability)
    {
        PlayerAbility owned = ownedAbilities.Find(x => x.id == ability.id);

        int appliedLevel = 1;

        if (owned == null)
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

    void AddToEquipmentPanel(PlayerAbility ability)
    {
        AbilityData abilityData = AbilityDatabase.GetAbility(ability.id);
        if (abilityData == null)
        {
            return;
        }

        bool isWeapon = (abilityData.type == AbilityType.weapon || abilityData.type == AbilityType.evolution);
        Transform parent = isWeapon ? wIconParent : sIconParent;

        if (parent == null)
        {
            return;
        }

        int slotIndex = isWeapon ? weaponSlotIndex : passiveSlotIndex;

        if (slotIndex >= parent.childCount)
        {
            return;
        }

        Transform slotTransform = parent.GetChild(slotIndex);

        if (slotTransform.childCount < 1)
        {
            return;
        }

        Transform iconTransform = slotTransform.GetChild(0);
        Image iconImage = iconTransform.GetComponent<Image>();

        if (iconImage == null)
        {
            return;
        }

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

        if (isWeapon)
        {
            weaponSlotIndex++;
        }
        else
        {
            passiveSlotIndex++;
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