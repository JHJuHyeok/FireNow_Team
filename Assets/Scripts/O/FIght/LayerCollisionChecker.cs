using UnityEngine;

public class LayerCollisionChecker : MonoBehaviour
{
    [ContextMenu("Check Layer Collision")]
    void CheckLayerCollision()
    {
        int bulletLayer = LayerMask.NameToLayer("Bullet");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int defaultLayer = LayerMask.NameToLayer("Default");

    }
}