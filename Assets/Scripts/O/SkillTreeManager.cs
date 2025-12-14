using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;

    [Header("Prefabs")]
    public GameObject skillNodePrefab;
    public GameObject connectorPrefab;
    public GameObject specialSkillNodePrefab;

    [Header("References")]
    public Transform contentParent;
    public ScrollRect scrollRect;

    [Header("Column Parents")]
    public Transform leftColumnParent;
    public Transform centerColumnParent;
    public Transform rightColumnParent;

    [Header("기본 스킬 (4가지)")]
    public SkillTypeData[] skillTypes = new SkillTypeData[4];

    [Header("특수 스킬")]
    public SpecialSkillData[] specialSkills;

    [Header("Level Sprites")]
    public Sprite levelBGSprite;
    public Sprite levelFGSprite;
    public Font levelFont;

    [Header("Settings")]
    public int maxLevel = 100;
    public float rowHeight = 200f;
    public float nodeSize = 150f;
    public int specialSkillInterval = 10;

    [Header("진행 상황")]
    public PlayerProgress progress = new PlayerProgress();

    private List<SkillNode> normalSkillNodes = new List<SkillNode>();
    private List<LevelIndicatorData> levelIndicators = new List<LevelIndicatorData>();
    private List<GameObject> specialSkillNodes = new List<GameObject>();
    private List<GameObject> connectors = new List<GameObject>();

    [System.Serializable]
    private class LevelIndicatorData
    {
        public GameObject gameObject;
        public Image backgroundImage;
        public Text levelText;
        public int level;
    }

    void Awake()
    {
        Instance = this;
        LoadProgress();
    }

    void Start()
    {
        SetupColumns();
        GenerateSkillTree();
        UpdateContentSize();

        Canvas.ForceUpdateCanvases();
        ScrollToCurrentLevel();
    }

    void SetupColumns()
    {
        RectTransform contentRect = contentParent.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0f);
        contentRect.anchorMax = new Vector2(0.5f, 0f);
        contentRect.pivot = new Vector2(0.5f, 0f);
        contentRect.anchoredPosition = Vector2.zero;

        if (leftColumnParent == null)
            leftColumnParent = CreateColumn("LeftColumn", -300f);
        if (centerColumnParent == null)
            centerColumnParent = CreateColumn("CenterColumn", 0f);
        if (rightColumnParent == null)
            rightColumnParent = CreateColumn("RightColumn", 300f);
    }

    Transform CreateColumn(string name, float xPosition)
    {
        GameObject column = new GameObject(name);
        column.transform.SetParent(contentParent, false);

        RectTransform rect = column.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(xPosition, 0);
        rect.sizeDelta = new Vector2(200f, 0f);

        return column.transform;
    }

    void GenerateSkillTree()
    {
        float yPosition = 0f;
        int currentLevel = 1;

        // 특수 스킬 레벨 맵핑 
        Dictionary<int, string> specialSkillMap = new Dictionary<int, string>
    {
        { 5, "wulai" },
        { 7, "yundongjianjiang" },
        { 9, "roushiaihaozhe" },
        { 11, "lieshazhe" },
        { 13, "maotouying" },
        { 15, "tunshizhe" },
        { 20, "kuangren" },
        { 25, "tanlanzhe" },
        { 30, "tianyouzhe" },
        { 35, "yanjiuzhe" },
        { 40, "ruodianshipo" },
        { 50, "shenshengyiji" },
        { 60, "zhongbiaojiang" },
        { 65, "jubianxiaoying" },
        { 70, "liangzijiasu" },
        { 75, "cailiaogeming" },
        { 80, "chaochangfanying" },
        { 85, "daixiejiasu" }
    };

        for (int skillIndex = 0; skillIndex < maxLevel * 3; skillIndex++)
        {
            int skillTypeIndex = skillIndex % 4;

            // 1. 일반 스킬 노드
            CreateNormalSkillNode(skillIndex, skillTypeIndex, currentLevel, yPosition);

            yPosition += nodeSize;

            // 2. 레벨 표시 (3개마다)
            if ((skillIndex + 1) % 3 == 0)
            {
                CreateLevelIndicator(currentLevel, yPosition - nodeSize);

                // 3. 특수 스킬 노드 (특정 레벨에만)
                if (specialSkillMap.ContainsKey(currentLevel))
                {
                    string spriteName = specialSkillMap[currentLevel];
                    CreateSpecialSkillNode(currentLevel, spriteName, yPosition - nodeSize);
                }

                currentLevel++;
            }

            // 4. 연결선
            if (skillIndex < maxLevel * 3 - 1)
            {
                float spacing = rowHeight - nodeSize;
                CreateConnectors(skillIndex, currentLevel - 1, yPosition, spacing);
                yPosition += spacing;
            }
        }

       
    }

    void CreateSpecialSkillNode(int level, string spriteName, float yPosition)
    {
        GameObject nodeObj = Instantiate(specialSkillNodePrefab, rightColumnParent);
        RectTransform nodeRect = nodeObj.GetComponent<RectTransform>();

        nodeRect.anchorMin = new Vector2(0.5f, 0f);
        nodeRect.anchorMax = new Vector2(0.5f, 0f);
        nodeRect.pivot = new Vector2(0.5f, 0f);
        nodeRect.sizeDelta = new Vector2(nodeSize, nodeSize);
        nodeRect.anchoredPosition = new Vector2(0, yPosition);
        nodeRect.localScale = Vector3.one;

        // 스프라이트 로드
        Sprite unlockedSprite = Resources.Load<Sprite>($"Resources/Sprites/Classified/Evolution_Tap/{spriteName}");
        Sprite lockedSprite = Resources.Load<Sprite>($"Resources/Sprites/Classified/Evolution_Tap/{spriteName}_gray");

        if (unlockedSprite == null)
        {
       
        }
        if (lockedSprite == null)
        {
         
        }

        SpecialSkillNode specialNode = nodeObj.GetComponent<SpecialSkillNode>();
        if (specialNode != null)
        {
            bool isUnlocked = (level <= progress.currentLevel);

            // 특수 스킬 인덱스 찾기
            int specialIndex = specialSkillNodes.Count;
            bool isPurchased = progress.IsSpecialSkillPurchased(specialIndex);

            // 스프라이트 직접 할당
            if (specialNode.icon != null)
            {
                specialNode.icon.sprite = isUnlocked ? unlockedSprite : lockedSprite;
            }

            specialNode.Initialize(specialIndex, null, isUnlocked, isPurchased);

       
        }

        specialSkillNodes.Add(nodeObj);
    }

    void CreateConnectors(int skillIndex, int currentLevel, float startY, float height)
    {
        bool isActive = skillIndex < progress.GetPurchasedSkillCount();
        CreateConnector(leftColumnParent, startY, height, isActive);

        if (currentLevel > 0 && currentLevel % specialSkillInterval == 0)
        {
            CreateConnector(rightColumnParent, startY, height, isActive);
        }
    }
    void CreateNormalSkillNode(int skillIndex, int skillTypeIndex, int level, float yPosition)
    {
        if (skillTypeIndex >= skillTypes.Length)
        {

            return;
        }

        SkillTypeData skillType = skillTypes[skillTypeIndex];

        GameObject nodeObj = Instantiate(skillNodePrefab, leftColumnParent);
        RectTransform nodeRect = nodeObj.GetComponent<RectTransform>();

        nodeRect.anchorMin = new Vector2(0.5f, 0f);
        nodeRect.anchorMax = new Vector2(0.5f, 0f);
        nodeRect.pivot = new Vector2(0.5f, 0f);
        nodeRect.sizeDelta = new Vector2(nodeSize, nodeSize);
        nodeRect.anchoredPosition = new Vector2(0, yPosition);
        nodeRect.localScale = Vector3.one;

        SkillNode node = nodeObj.GetComponent<SkillNode>();
        if (node != null)
        {
            int maxUnlockedSkills = progress.GetMaxUnlockedSkills();
            bool isUnlocked = (skillIndex < maxUnlockedSkills);
            bool isPurchased = progress.IsSkillPurchased(skillIndex);
            int cost = CalculateCost(skillType, level);

            node.Initialize(skillIndex, skillTypeIndex, skillType, cost, isUnlocked, isPurchased);
            normalSkillNodes.Add(node);
        }
    }

    void CreateLevelIndicator(int level, float yPosition)
    {
        GameObject levelObj = new GameObject($"Level_{level}");
        levelObj.transform.SetParent(centerColumnParent, false);

        RectTransform levelRect = levelObj.AddComponent<RectTransform>();
        levelRect.anchorMin = new Vector2(0.5f, 0f);
        levelRect.anchorMax = new Vector2(0.5f, 0f);
        levelRect.pivot = new Vector2(0.5f, 0.5f);
        levelRect.sizeDelta = new Vector2(80f, 80f);
        levelRect.anchoredPosition = new Vector2(0, yPosition + nodeSize / 2f);
        levelRect.localScale = Vector3.one;

        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(levelObj.transform, false);

        Image bgImage = bgObj.AddComponent<Image>();
        bool isReached = (level <= progress.currentLevel);
        bgImage.sprite = isReached ? levelFGSprite : levelBGSprite;
        bgImage.preserveAspect = true;

        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;

        GameObject textObj = new GameObject("LevelText");
        textObj.transform.SetParent(levelObj.transform, false);

        Text levelText = textObj.AddComponent<Text>();
        levelText.text = level.ToString();

        if (levelFont != null)
        {
            levelText.font = levelFont;
        }

        levelText.fontSize = 36;
        levelText.fontStyle = FontStyle.Bold;
        levelText.alignment = TextAnchor.MiddleCenter;
        levelText.color = Color.white;
        levelText.resizeTextForBestFit = true;
        levelText.resizeTextMinSize = 20;
        levelText.resizeTextMaxSize = 40;

        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        LevelIndicatorData data = new LevelIndicatorData
        {
            gameObject = levelObj,
            backgroundImage = bgImage,
            levelText = levelText,
            level = level
        };
        levelIndicators.Add(data);
    }

    void CreateSpecialSkillNode(int level, float yPosition)
    {
        int specialIndex = (level / specialSkillInterval) - 1;
        if (specialSkills == null || specialIndex < 0 || specialIndex >= specialSkills.Length)
            return;

        SpecialSkillData specialData = specialSkills[specialIndex];
        if (specialData == null)
            return;

        GameObject nodeObj = Instantiate(specialSkillNodePrefab, rightColumnParent);
        RectTransform nodeRect = nodeObj.GetComponent<RectTransform>();

        nodeRect.anchorMin = new Vector2(0.5f, 0f);
        nodeRect.anchorMax = new Vector2(0.5f, 0f);
        nodeRect.pivot = new Vector2(0.5f, 0f);
        nodeRect.sizeDelta = new Vector2(nodeSize, nodeSize);
        nodeRect.anchoredPosition = new Vector2(0, yPosition);
        nodeRect.localScale = Vector3.one;

        SpecialSkillNode specialNode = nodeObj.GetComponent<SpecialSkillNode>();
        if (specialNode != null)
        {
            bool isUnlocked = (level <= progress.currentLevel);
            bool isPurchased = progress.IsSpecialSkillPurchased(specialIndex);

            specialNode.Initialize(specialIndex, specialData, isUnlocked, isPurchased);
        }

        specialSkillNodes.Add(nodeObj);
    }



    void CreateConnector(Transform parent, float yPosition, float height, bool isActive)
    {
        GameObject connector = Instantiate(connectorPrefab, parent);
        RectTransform rect = connector.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.sizeDelta = new Vector2(10f, height);
        rect.anchoredPosition = new Vector2(0, yPosition);
        rect.localScale = Vector3.one;

        Connector connectorScript = connector.GetComponent<Connector>();
        if (connectorScript != null)
        {
            connectorScript.SetActive(isActive);
        }
        else
        {
            Image img = connector.GetComponent<Image>();
            if (img != null)
            {
                img.color = isActive ? new Color(0.3f, 0.8f, 0.3f) : new Color(0.3f, 0.3f, 0.3f);
            }
        }

        connectors.Add(connector);
    }

    void UpdateContentSize()
    {
        RectTransform contentRect = contentParent.GetComponent<RectTransform>();
        float totalHeight = (maxLevel * 3) * rowHeight + 300f;
        contentRect.sizeDelta = new Vector2(800f, totalHeight);
    }

    void ScrollToCurrentLevel()
    {
        if (scrollRect == null) return;

        int purchasedCount = progress.GetPurchasedSkillCount();
        int maxUnlockedSkills = progress.GetMaxUnlockedSkills();

        int targetSkillIndex = Mathf.Min(purchasedCount, maxUnlockedSkills - 1);
        targetSkillIndex = Mathf.Max(0, targetSkillIndex);

        float targetHeight = targetSkillIndex * rowHeight;

        RectTransform contentRect = contentParent.GetComponent<RectTransform>();
        RectTransform viewportRect = scrollRect.viewport;

        float offset = viewportRect.rect.height / 2f - nodeSize / 2f;
        contentRect.anchoredPosition = new Vector2(0, -(targetHeight - offset));


    }

    public void PurchaseSkill(int skillIndex, int skillTypeIndex, int cost, CurrencyType currencyType)
    {
        if (CurrencyManager.Instance == null) return;

        if (progress.IsSkillPurchased(skillIndex))
        {

            return;
        }

        int maxUnlockedSkills = progress.GetMaxUnlockedSkills();
        if (skillIndex >= maxUnlockedSkills)
        {
 
            return;
        }

        int requiredGold = (currencyType == CurrencyType.Gold) ? cost : 0;
        int requiredDNA = (currencyType == CurrencyType.GoldDNA) ? cost : 0;

        if (!CurrencyManager.Instance.SpendCurrency(requiredGold, requiredDNA))
        {
 
            return;
        }

        progress.PurchaseSkill(skillIndex);

        if (PlayerStats.Instance != null)
        {
            SkillTypeData skillType = skillTypes[skillTypeIndex];
            PlayerStats.Instance.ApplySkillUpgrade(skillTypeIndex, skillType.statIncreasePerLevel);
        }

        if (skillIndex >= 0 && skillIndex < normalSkillNodes.Count)
        {
            normalSkillNodes[skillIndex].SetPurchased(true);
        }

        UpdateConnectors();
        UpdateLevelIndicators();
        SaveProgress();

        Canvas.ForceUpdateCanvases();
        ScrollToCurrentLevel();

    }

    void UpdateConnectors()
    {
        int purchasedCount = progress.GetPurchasedSkillCount();

        for (int i = 0; i < connectors.Count; i++)
        {
            if (connectors[i] != null)
            {
                bool isActive = i < purchasedCount;

                Connector connectorScript = connectors[i].GetComponent<Connector>();
                if (connectorScript != null)
                {
                    connectorScript.SetActive(isActive);
                }
                else
                {
                    Image img = connectors[i].GetComponent<Image>();
                    if (img != null)
                    {
                        img.color = isActive ? new Color(0.3f, 0.8f, 0.3f) : new Color(0.3f, 0.3f, 0.3f);
                    }
                }
            }
        }
    }

    void UpdateLevelIndicators()
    {
        foreach (var indicator in levelIndicators)
        {
            bool isReached = (indicator.level <= progress.currentLevel);
            indicator.backgroundImage.sprite = isReached ? levelFGSprite : levelBGSprite;
        }
    }

    public void PurchaseSpecialSkill(int specialSkillIndex)
    {
        if (specialSkillIndex < 0 || specialSkillIndex >= specialSkills.Length)
        {

            return;
        }

        SpecialSkillData specialSkill = specialSkills[specialSkillIndex];

        if (CurrencyManager.Instance == null)
        {
      
            return;
        }

        int requiredGold = (specialSkill.requiredCurrency == CurrencyType.Gold) ? specialSkill.cost : 0;
        int requiredDNA = (specialSkill.requiredCurrency == CurrencyType.GoldDNA) ? specialSkill.cost : 0;

        if (!CurrencyManager.Instance.SpendCurrency(requiredGold, requiredDNA))
        {
         
            return;
        }

        progress.PurchaseSpecialSkill(specialSkillIndex);

        if (specialSkillIndex < specialSkillNodes.Count)
        {
            SpecialSkillNode node = specialSkillNodes[specialSkillIndex].GetComponent<SpecialSkillNode>();
            if (node != null)
            {
                node.SetPurchased(true);
            }
        }


        SaveProgress();
    }

    int CalculateCost(SkillTypeData skillType, int level)
    {
        return Mathf.RoundToInt(skillType.baseCost * Mathf.Pow(skillType.costIncreaseRate, level - 1));
    }

    void SaveProgress()
    {
        string json = JsonUtility.ToJson(progress);
        PlayerPrefs.SetString("SkillProgress", json);
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        if (PlayerPrefs.HasKey("SkillProgress"))
        {
            string json = PlayerPrefs.GetString("SkillProgress");
            progress = JsonUtility.FromJson<PlayerProgress>(json);
        }
        else
        {
            progress = new PlayerProgress();
        }
    }

    public void ResetProgress()
    {
        foreach (Transform child in leftColumnParent)
            Destroy(child.gameObject);
        foreach (Transform child in centerColumnParent)
            Destroy(child.gameObject);
        foreach (Transform child in rightColumnParent)
            Destroy(child.gameObject);

        normalSkillNodes.Clear();
        levelIndicators.Clear();
        specialSkillNodes.Clear();
        connectors.Clear();

        progress = new PlayerProgress();
        PlayerPrefs.DeleteKey("SkillProgress");

        GenerateSkillTree();
        UpdateContentSize();
        ScrollToCurrentLevel();
    }
}