using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Image itemImage; // ImageContent
    public TMP_Text itemText; // Text (TMP) - 선택사항

    /// <summary>
    /// 아이템 설정
    /// </summary>
    public void Setup(Sprite sprite, string itemName = "", int count = 0)
    {
        // 이미지 설정
        if (itemImage != null && sprite != null)
        {
            itemImage.sprite = sprite;
        }

        // 텍스트 설정 (선택사항)
        if (itemText != null)
        {
            if (count > 0)
            {
                itemText.text = count.ToString();
            }
            else if (!string.IsNullOrEmpty(itemName))
            {
                itemText.text = itemName;
            }
            else
            {
                itemText.text = "";
            }
        }
    }
}