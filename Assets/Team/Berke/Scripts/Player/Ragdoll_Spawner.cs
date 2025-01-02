using UnityEngine;

public class Ragdoll_Spawner : MonoBehaviour
{
    [SerializeField] private float _explosionForce, _explosionRange;
    [SerializeField] private Transform _transform;
    [SerializeField] private Transform _ragdollObject;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            SpawnRagdollObject();
        }
    }

    private void SpawnRagdollObject()
    {
        Transform transform = Instantiate(_ragdollObject, _transform.position, _transform.rotation);

        if (transform.TryGetComponent<Skeleton_Ragdoll>(out Skeleton_Ragdoll ragdoll))
        {
            ragdoll.ApplyRagdoll(ragdoll.root, _explosionForce, _transform.position,  _explosionRange);
        }

        Destroy(gameObject);
    }
}
