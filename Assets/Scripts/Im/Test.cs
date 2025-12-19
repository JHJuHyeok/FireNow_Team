using UnityEngine;

public class Test : MonoBehaviour
{
    ForceFieldSponer forceFieldSponer;

    private void Awake()
    {
        forceFieldSponer = GetComponent<ForceFieldSponer>();
    }
    public void testingbutton()
    {
        forceFieldSponer.Restart(0.7f, true);
    }
}
