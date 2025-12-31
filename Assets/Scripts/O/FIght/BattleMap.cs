using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleMap : MonoBehaviour
{
    [SerializeField]
    public SpriteRenderer BackGround;


    [Header("Background Settings")]
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Vector2 backgroundSize = new Vector2(1000, 1000);

    public Vector2 MapSize
    {
        get { return BackGround.size; }
        set
        {
            BackGround.size = value;
        }
    }

    public void init()
    {
        SetupBackground();
    }

    private void SetupBackground()
    {
        if (BackGround == null)
        {
            GameObject bgObject = new GameObject("Background");
            bgObject.transform.SetParent(transform);
            BackGround = bgObject.AddComponent<SpriteRenderer>();
       
        }

        if (backgroundSprite == null)
        {
           
            return;
        }

        BackGround.sprite = backgroundSprite;
        BackGround.drawMode = SpriteDrawMode.Tiled;
        BackGround.tileMode = SpriteTileMode.Continuous;
        BackGround.size = backgroundSize;
        BackGround.sortingLayerName = "Default";
        BackGround.sortingOrder = -10;
        BackGround.transform.position = new Vector3(0, 0, 1f);
        BackGround.transform.localScale = Vector3.one;

     
    }

 
}
