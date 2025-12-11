
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundScroll : MonoBehaviour
{
    [SerializeField] private float x = 0.01f, y = 0.02f;
    [SerializeField] private RawImage BackImage;

    private Coroutine _scrollcoroutine;

    void Update()
    {
        BackImage.uvRect = new Rect(BackImage.uvRect.position + new Vector2(x, y) * Time.deltaTime,BackImage.uvRect.size);
    }
}
