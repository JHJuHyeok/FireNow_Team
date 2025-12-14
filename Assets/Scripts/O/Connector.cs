using UnityEngine;
using UnityEngine.UI;

public class Connector : MonoBehaviour
{
    [Header("UI References")]
    public Image lineImage;

    [Header("Sprites")]
    public Sprite lineInactive;  // Talent_ProgressBarBG
    public Sprite lineActive;    // Talent_ProgressBarFG

    public void SetActive(bool active)
    {
        if (lineImage != null)
        {
            lineImage.sprite = active ? lineActive : lineInactive;
        }
    }

 
}