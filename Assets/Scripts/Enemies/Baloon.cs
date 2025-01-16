using UnityEngine;

public class Baloon : MonoBehaviour
{
    [SerializeField] private float _speed = 0.2f;
    [SerializeField] private float _rotationSpeed =5;
    [SerializeField] float floatAmplitude = 0.5f;
    [SerializeField] float floatFrequency = 1f;
    [SerializeField] private Transform _targetA;
    [SerializeField] private Transform _targetB;

    private Transform _currentTarget;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        _currentTarget = _targetA;
    }
    void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        float distance = Vector3.Distance(transform.position, _currentTarget.position);

        if (distance < 2f)
        {
            _currentTarget = _currentTarget == _targetA ? _targetB : _targetA;
        }

        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, _currentTarget.position, ref velocity, _speed * Time.deltaTime);
        Vector3 direction = (_currentTarget.position - transform.position).normalized;

        float offsetX = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        float offsetZ = Mathf.Cos(Time.time * (floatFrequency / 2)) * (floatAmplitude / 2);
        targetPosition.x += offsetX;
        targetPosition.z += offsetX;

        transform.position = targetPosition;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
    }
}
