using System.Collections;
using UnityEngine;

public class Boxshake : MonoBehaviour
{

    
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float rotateTime = 0.1f;

    [SerializeField] private float startTiltAngel = 5f;

    [SerializeField] private GameObject childToDisable;
    [SerializeField] private GameObject[] objectsToEnable;

    private Quaternion _originalRot;
    void Start()
    {
        foreach (var obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        _originalRot = transform.localRotation;
        StartCoroutine(RotateSequence());
    }

    private IEnumerator RotateSequence()
    {
        transform.rotation = _originalRot * Quaternion.Euler(0,0,startTiltAngel);

        for (int i = 0; i < 2; i++)
        {
            yield return Shake(rotateTime);
        }

        transform.rotation = _originalRot;

        if(childToDisable != null)
        {
            childToDisable.SetActive(false);
        }

        foreach (var obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }    
        }
    }
    private IEnumerator Shake(float Time)
    {
        yield return RotateForDuration(rotationSpeed, rotateTime);
        yield return RotateForDuration(-rotationSpeed, rotateTime);
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
