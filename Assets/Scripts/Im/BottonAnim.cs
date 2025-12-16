using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BottonAnim : MonoBehaviour
{

    
    private Vector3 _TargetPosition;
  
    public void OnClickBotton()
    {
        
        _TargetPosition = new Vector3(-0.07f, -0.07f, -0.07f);

        transform.DOPunchScale(_TargetPosition, 0.25f, 10, 0);

        
    }
}
