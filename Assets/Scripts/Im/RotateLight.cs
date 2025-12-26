
using System.Collections;
using UnityEngine;

public class RotateLight : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float rotateTime = 1f;
    void Start()
    {
        StartCoroutine(RotateLoop());
    }

   
    private IEnumerator RotateLoop()
    {
        while (true)
        {
            yield return RotateForDuration(rotationSpeed, rotateTime);
            yield return RotateForDuration(-rotationSpeed, rotateTime);
        }
    }

    private IEnumerator RotateForDuration(float speed, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.Rotate(0f, 0f, speed * Time.deltaTime);
            yield return null;
        }
    }
}
