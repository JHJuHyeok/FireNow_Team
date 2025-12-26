// TiledBackground.cs
using UnityEngine;

public class TiledBackground : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Sprite backgroundSprite; // 배경 스프라이트
    [SerializeField] private Vector2 mapSize = new Vector2(500, 500); // 맵 크기

    private void Start()
    {
        SetupBackground();
    }

    private void SetupBackground()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
        }

        // 스프라이트 설정
        sr.sprite = backgroundSprite;
        sr.drawMode = SpriteDrawMode.Tiled; // 타일 모드
        sr.tileMode = SpriteTileMode.Continuous;
        sr.size = mapSize; // 큰 크기

        // 정렬 (모든 것 뒤에)
        sr.sortingLayerName = "Default";
        sr.sortingOrder = -10;

        // 위치 (중앙)
        transform.position = new Vector3(0, 0, 1f); // Z=1 (뒤에)

  
    }
}