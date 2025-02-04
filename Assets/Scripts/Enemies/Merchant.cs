using UnityEngine;
using UnityEngine.AI;

public class Merchant : MonoBehaviour
{
    public float stretchAmount = 0.2f;
    public Transform targetPosition;
    public Transform neckBone;

    private Transform[] childBones;
    private Vector3 originalPosition;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        childBones = neckBone.GetComponentsInChildren<Transform>();
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (skinnedMeshRenderer != null)
        {
            Bounds bounds = skinnedMeshRenderer.localBounds;
            bounds.center += new Vector3(5, 5, 5); 
            bounds.extents += new Vector3(5, 5, 5);
            skinnedMeshRenderer.localBounds = bounds;
        }

        originalPosition = neckBone.position;
    }

    void Update()
    {
        float distance = Vector2.Distance(targetPosition.position, neckBone.position);
        if (neckBone != null)
        {
            Vector3 startPosition = neckBone.position;
            Vector3 endPosition = targetPosition.position;

            //neckBone.localPosition = originalPosition + new Vector3(0, 0, stretchAmount);
            if (distance > 2 && distance < 10)
            {
                neckBone.position = Vector3.SmoothDamp(neckBone.position, targetPosition.position, ref velocity, stretchAmount);
                neckBone.rotation = Quaternion.Slerp(neckBone.rotation, targetPosition.rotation, Time.deltaTime * stretchAmount);
            }
            else if (distance <= 2)
            {
                neckBone.position = neckBone.position;
                neckBone.rotation = neckBone.rotation;
            }

            else if (distance >= 10)
            {
                neckBone.position = Vector3.SmoothDamp(neckBone.position, originalPosition, ref velocity, stretchAmount);
                neckBone.rotation = Quaternion.Slerp(neckBone.rotation, Quaternion.identity, Time.deltaTime * stretchAmount);
            }


            foreach (var child in childBones)
            {
                if (child != neckBone)
                {
                    child.position = child.position; 
                }
            }
        }
    }

    public void StretchNeck(Transform newTarget)
    {
        targetPosition = newTarget;
    }

}
