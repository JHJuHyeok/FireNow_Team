using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BottonAnim : MonoBehaviour
{
<<<<<<< HEAD

    
    private Vector3 _TargetPosition;
  
    public void OnClickBotton()
    {
        
=======
    private Vector3 _TargetPosition;
    private Tween _tween; 
    void Start()
    {
        

    }

    public void OnClickBotton()
    {
        _tween?.Kill();
>>>>>>> origin/design/Battle/SkillAnim
        _TargetPosition = new Vector3(-0.07f, -0.07f, -0.07f);

        _tween = transform.DOPunchScale(_TargetPosition, 0.25f, 10, 0);

        
    }
}
