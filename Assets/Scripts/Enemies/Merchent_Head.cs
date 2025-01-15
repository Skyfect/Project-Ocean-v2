using UnityEngine;
using UnityEngine.AI;

public class Merchent_Head : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _head;

    [SerializeField] private float _smoothSpeed = 5f;

    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = _head.GetComponent<NavMeshAgent>();
    }
    void Start()
    {
    }

    void Update()
    {
        if (_target != null)
        {
            float distance = Vector2.Distance(_target.position, transform.position);

            if (distance > 2 && distance < 10)
            {
                Vector3 direction = (_target.position - transform.position).normalized;

                Quaternion targetRotation = Quaternion.LookRotation(-direction);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _smoothSpeed);

                Vector3 euler = transform.eulerAngles;
                transform.eulerAngles = new Vector3(0, euler.y, 0);

                _agent.SetDestination(_target.position);
            }

            else
            {
                _agent.speed = 0 ;
            }
        }
    }
}
