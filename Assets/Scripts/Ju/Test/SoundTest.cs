using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlaySound("hit");
    }
}
