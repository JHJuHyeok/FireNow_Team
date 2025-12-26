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
            Debug.Log("BackGround SpriteRenderer 자동 생성됨");
        }

        if (backgroundSprite == null)
        {
            Debug.LogError("배경 스프라이트가 할당되지 않았습니다! Map Inspector에서 Background Sprite를 할당하세요.");
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

        Debug.Log($"배경 설정 완료: Sprite={backgroundSprite.name}, Size={backgroundSize}, DrawMode=Tiled");
    }

 
}
