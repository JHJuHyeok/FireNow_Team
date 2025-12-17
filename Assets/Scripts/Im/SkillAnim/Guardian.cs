using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 0.5f;
    [SerializeField] private float revolutionSpeed = 1f;
    [SerializeField] private GameObject revolutionCenter;
    private Tween _spintween;
    private Tween _revolutiontween;
    void Start()
    {
        Spin();
        revolution();
    }

    private void Spin()
    {
        _spintween?.Kill();
        _spintween = transform.DORotate(new Vector3(0, 0, 360), spinSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);

    }

    private void revolution()
    {
        _revolutiontween?.Kill();
        _revolutiontween = revolutionCenter.transform.DOLocalRotate(new Vector3(0, 0, -360), revolutionSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);

    }


}
