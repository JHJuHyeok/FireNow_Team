using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BottonAnim : MonoBehaviour
{
    private Vector3 _TargetPosition;
    private Tween _tween; 
    void Start()
    {
        

    }

    public void OnClickBotton()
    {
        _tween?.Kill();
        _TargetPosition = new Vector3(-0.07f, -0.07f, -0.07f);

        _tween = transform.DOPunchScale(_TargetPosition, 0.25f, 10, 0);

        
    }
}
