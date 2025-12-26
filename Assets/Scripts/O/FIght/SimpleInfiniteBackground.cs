using UnityEngine;

public class SimpleInfiniteBackground : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] tiles; // 4개 타일 직접 할당
    [SerializeField] private float tileSize = 20f;

    private void Update()
    {
        foreach (Transform tile in tiles)
        {
            // 플레이어와 타일의 거리 계산
            Vector2 distance = (Vector2)player.position - (Vector2)tile.position;

            // X축 재배치
            if (distance.x > tileSize)
            {
                tile.position += Vector3.right * tileSize * 2;
            }
            else if (distance.x < -tileSize)
            {
                tile.position += Vector3.left * tileSize * 2;
            }

            // Y축 재배치
            if (distance.y > tileSize)
            {
                tile.position += Vector3.up * tileSize * 2;
            }
            else if (distance.y < -tileSize)
            {
                tile.position += Vector3.down * tileSize * 2;
            }
        }
    }
}