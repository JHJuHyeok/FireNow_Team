using UnityEngine;
using UnityEngine.UI;

public class InfiniteBackground : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private float scrollSpeed = 0.1f;

    private Vector2 savedOffset;
    private Vector3 lastPlayerPosition;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<RawImage>();
        }

        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
    }

    private void LateUpdate()
    {
        if (player == null || backgroundImage == null) return;

        // 플레이어의 월드 좌표 이동량 계산
        Vector3 movement = player.position - lastPlayerPosition;

        // UV 오프셋 이동 
        // -= 에서 += 로 변경
        savedOffset.x += movement.x * scrollSpeed;  
        savedOffset.y += movement.y * scrollSpeed;  

        backgroundImage.uvRect = new Rect(savedOffset.x, savedOffset.y, 1, 1);

        // 현재 위치 저장
        lastPlayerPosition = player.position;
    }
}