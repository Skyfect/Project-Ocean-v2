using UnityEngine;

public class Skeleton_Ragdoll : MonoBehaviour
{
    public Transform root;
    public void ApplyRagdoll(Transform root, float expForce, Vector3 pos, float expRange)
    {
        foreach (Transform child in root)
        {
            if (root.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddExplosionForce(expForce, pos, expRange);
            }
            ApplyRagdoll(child, expForce, pos, expRange);
        }
    }
}
