using UnityEngine;

public class SimpleInfiniteBackground : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] tiles; // 9개 타일 직접 할당
    [SerializeField] private float tileSize = 20f;

    private void Update()
    {
        foreach (Transform tile in tiles)
        {
            // 플레이어와 타일의 거리 계산
            Vector2 distance = (Vector2)player.position - (Vector2)tile.position;

            // X축 재배치 (3배 거리로 체크)
            if (distance.x > tileSize * 1.5f)
            {
                tile.position += Vector3.right * tileSize * 3;
            }
            else if (distance.x < -tileSize * 1.5f)
            {
                tile.position += Vector3.left * tileSize * 3;
            }

            // Y축 재배치 (3배 거리로 체크)
            if (distance.y > tileSize * 1.5f)
            {
                tile.position += Vector3.up * tileSize * 3;
            }
            else if (distance.y < -tileSize * 1.5f)
            {
                tile.position += Vector3.down * tileSize * 3;
            }
        }
    }
}