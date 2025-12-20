using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtlasCallTest : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Start()
    {
        button.image.sprite = AtlasManager.GetSprite("Equip_Tap_Atlas", "Equip_Tap_Atlas_41");
    }
}
