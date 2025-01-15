using UnityEngine;

public class Merchent_Head : MonoBehaviour
{
    [SerializeField] private Transform _target;
    void Start()
    {
        
    }

    void Update()
    {
        transform.LookAt(-_target.position);
    }
}
