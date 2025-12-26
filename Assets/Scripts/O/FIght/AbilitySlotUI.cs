//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//[System.Serializable]
//public class AbilityPanel : MonoBehaviour
//{
//    [Header("UI Elements")]
//    public Image abiliIcon;           // AbiliIcon
//    public TMP_Text nameText;         // NameText
//    public TMP_Text exText;           // ExText (설명)
//    public GameObject starLinear;     // StarLinear (별 표시)
//    public Image[] stars;             // FirstStarOne ~ Five
//    public GameObject evol;           // Evol (진화 표시)
//    public TMP_Text evolText;         // EvolText
//    public Image evolImage;           // EvolImage
//    public Button button;             // 버튼 (패널 자체)

//    private AbilityData currentAbility;
//    private int displayLevel;
//    private AbilitySelectionManager manager;

//    public void Setup(AbilityData ability, int level, AbilitySelectionManager selectionManager)
//    {
//        currentAbility = ability;
//        displayLevel = level;
//        manager = selectionManager;

//        // 아이콘
//        Sprite sprite = Resources.Load<Sprite>($"Sprites/Ability/{ability.spriteName}");
//        if (sprite != null && abiliIcon != null)
//        {
//            abiliIcon.sprite = sprite;
//        }

//        // 이름
//        if (nameText != null)
//        {
//            nameText.text = ability.name;
//        }

//        // 설명
//        if (exText != null && level <= ability.levels.Count)
//        {
//            exText.text = ability.levels[level - 1].description;
//        }

//        // 별 표시
//        if (ability.type == AbilityType.evolution)
//        {
//            // 진화 무기
//            if (starLinear != null) starLinear.SetActive(false);
//            if (evol != null) evol.SetActive(true);

//            string requirement = manager.GetEvolutionRequirement(ability);
//            if (evolText != null)
//            {
//                evolText.text = requirement;
//            }
//        }
//        else
//        {
//            // 일반 무기/패시브
//            if (starLinear != null) starLinear.SetActive(true);
//            if (evol != null) evol.SetActive(false);

//            // 별 개수 표시
//            for (int i = 0; i < stars.Length; i++)
//            {
//                if (stars[i] != null)
//                {
//                    stars[i].gameObject.SetActive(i < level);
//                }
//            }
//        }

//        // 버튼 이벤트
//        if (button != null)
//        {
//            button.onClick.RemoveAllListeners();
//            button.onClick.AddListener(() => manager.SelectAbility(currentAbility));
//        }
//    }
//}