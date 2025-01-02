using System.Collections.Generic;
using UnityEngine;

public class SkeletonFall : MonoBehaviour
{
    [SerializeField] private Transform rootBone;
    [SerializeField] private Material customMaterial;

    private Rigidbody[] rigidbodies; 
    private Animator animator; 
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    private void Awake()
    {
        if (rootBone == null)
        {
            Debug.LogError("Root bone is not assigned!");
            return;
        }

        animator = GetComponent<Animator>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        rigidbodies = rootBone.GetComponentsInChildren<Rigidbody>();
    }

    void Start()
    {
        //foreach (var rb in rigidbodies)
        //{
        //    rb.isKinematic = true;
        //}
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            ActivateRagdoll();
        }
    }

    public void ActivateRagdoll()
    {
        if (animator != null)
        {
            animator.enabled = false;
        }

        foreach (var smr in skinnedMeshRenderers)
        {
            smr.enabled = false; 
        }

        foreach (Transform child in rootBone)
        {
            if (child.GetComponent<Rigidbody>() == null)
            {
                child.gameObject.AddComponent<Rigidbody>();
            }

            if (child.GetComponent<BoxCollider>() == null)
            {
                child.gameObject.AddComponent<BoxCollider>();
            }

            if (child.GetComponent<MeshRenderer>() == null)
            {
                var meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
                if (customMaterial != null)
                {
                    meshRenderer.material = customMaterial; 
                }
            }

            if (child.GetComponent<MeshFilter>() == null)
            {
                var meshFilter = child.gameObject.AddComponent<MeshFilter>();
                if (child.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    meshFilter.mesh = child.GetComponent<SkinnedMeshRenderer>().sharedMesh; 
                }
            }
        }
    }
}
