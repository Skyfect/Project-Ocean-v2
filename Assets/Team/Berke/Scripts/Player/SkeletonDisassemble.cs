using UnityEngine;

public class SkeletonDisassemble : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Mesh bakedMesh;
            
    void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false; 
        }

        if (skinnedMeshRenderer == null)
        {
            return;
        }
        DisableSkinnedDeformation();
    }

    private void DisableSkinnedDeformation()
    {
        bakedMesh = new Mesh();

        skinnedMeshRenderer.BakeMesh(bakedMesh);

        GameObject staticMeshObject = new GameObject("StaticMesh");
        staticMeshObject.transform.position = skinnedMeshRenderer.transform.position;
        staticMeshObject.transform.rotation = skinnedMeshRenderer.transform.rotation;
        staticMeshObject.transform.localScale = skinnedMeshRenderer.transform.lossyScale;

        MeshFilter meshFilter = staticMeshObject.AddComponent<MeshFilter>();
        meshFilter.mesh = bakedMesh;

        MeshRenderer meshRenderer = staticMeshObject.AddComponent<MeshRenderer>();
        meshRenderer.materials = skinnedMeshRenderer.materials;

        skinnedMeshRenderer.enabled = false;

        Debug.Log("Statik mesh baþarýyla oluþturuldu.");

        Debug.Log("Mesh Vertex Sayýsý: " + bakedMesh.vertexCount);
        Debug.Log("Mesh Triangle Sayýsý: " + bakedMesh.triangles.Length / 3);
    }
}
