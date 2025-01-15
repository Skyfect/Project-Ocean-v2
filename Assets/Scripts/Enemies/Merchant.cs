using UnityEngine;

public class Merchant : MonoBehaviour
{
    public float stretchAmount = 0.2f; 

    public Transform neckBone;
    private Transform[] childBones;

    private Vector3 originalPosition; 

    public Transform targetPosition;
    private Vector3 velocity = Vector3.zero;

    public bool isMoving = false;

    void Start()
    {
        childBones = neckBone.GetComponentsInChildren<Transform>();

        if (neckBone == null)
        {
            Debug.LogError("Boyun kemiði bulunamadý!");

            return;
        }

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
        float distance = Vector2.Distance(targetPosition.position, transform.position);
        if (neckBone != null)
        {
            //neckBone.localPosition = originalPosition + new Vector3(0, 0, stretchAmount);
            if(distance > 2 && distance < 10 && isMoving)
            {
                transform.position = transform.position + transform.up * 0.02f;
                neckBone.position = Vector3.SmoothDamp(neckBone.position, targetPosition.position, ref velocity, stretchAmount);
                neckBone.rotation = Quaternion.Slerp(neckBone.rotation, targetPosition.rotation, Time.deltaTime * stretchAmount);
            }
            else if(distance <= 2)
                isMoving = false;

            else if (distance >= 10)
            {
                isMoving = true;

                neckBone.position = Vector3.SmoothDamp( neckBone.position, originalPosition, ref velocity, stretchAmount
                );

                neckBone.rotation = Quaternion.Slerp(neckBone.rotation, Quaternion.identity,Time.deltaTime * stretchAmount
                );
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
