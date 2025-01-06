using Crest;
using UnityEngine;

public class Boat_Controller : MonoBehaviour
{
    [Header("MOVEMENT & TURN")]
    [SerializeField] private float _maxSpeed = 30;
    [SerializeField] private float _speedReducer = 5f;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _turnSpeed = 50f;
    [SerializeField] private Transform _forcePoint;
    [SerializeField] private GameObject _wheel;
    [SerializeField] private GameObject _forceRod;

    [Header("SWING")]
    [SerializeField] private float swayAmplitude = 1f;
    [SerializeField] private float swayFrequency = 1f;
    private Vector3 swayAxis = new Vector3(1, 0, 1);

    [Header("GRAVITY")]
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private Transform _waterCheck;
    [SerializeField] private LayerMask _waterLayer;

    [Header("COMPONNETS")]
    private float _waterCheckRadius = 0.2f;
    private bool _isInWater;
    private bool _isTurn;
    private Vector3 _currentMovement = Vector3.zero;
    private bool _isMoving = false;
    private Rigidbody _rb;
    private Animator _wheelAnimtor;
    private Animator _forceRodAnimtor;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _wheelAnimtor = _wheel.GetComponent<Animator>();
        _forceRodAnimtor = _forceRod.GetComponent <Animator>();
    }

    private void Start()
    {
        _rb.centerOfMass = new Vector3(0,-1,0);
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        Movement();
        HandleGravityAndTouchWater();
        Swing();

        Vector3 velocity = _rb.velocity;
        //Debug.Log("current speed:" + velocity.z);
    }

    [System.Obsolete]
    private void Movement()
    {
        if (_isMoving)
        {
            //Vector3 moveForce = transform.forward * _moveSpeed;
            //_rb.AddForce(moveForce, ForceMode.Force);
            _rb.AddForceAtPosition(transform.forward * _moveSpeed, _forcePoint.position, ForceMode.Force);

            if (_rb.velocity.magnitude > _maxSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _maxSpeed;
            }
        }
        else
        {
            Vector3 frictionForce = -_rb.velocity.normalized * _speedReducer * _rb.velocity.magnitude;
            _rb.AddForce(frictionForce, ForceMode.Force);

            if (_rb.velocity.magnitude < 0.1f)
            {
                _rb.velocity = Vector3.zero;
            }
        }
    }

    private void Swing()
    {
        if (_rb)
        {
            float swayAngle = (Mathf.PerlinNoise(Time.time * swayFrequency, 0f) - 0.5f) * 2 * swayAmplitude;
            Quaternion swayRotation = Quaternion.Euler(swayAxis * swayAngle);
            _rb.MoveRotation(Quaternion.Lerp(_rb.rotation, _rb.rotation * swayRotation, Time.fixedDeltaTime));
        }

        //if (_isMoving && _isTurn) _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        //else if(_isMoving && !_isTurn) _rb.constraints = RigidbodyConstraints.FreezeRotationY;
        //else if(!_isMoving && _isTurn) _rb.constraints = RigidbodyConstraints.FreezeRotationY;
        //else _rb.constraints = RigidbodyConstraints.FreezeRotationY;
    }

    public void Rotate(int direction)
    {
        // Rotate (-1: Left, 1: Right)
        transform.Rotate(0f, direction * _turnSpeed * Time.deltaTime, 0f);
    }

    public void ToggleMovement()
    {
        _isMoving = !_isMoving;
        HandleAnimControl();
    }

    private void HandleGravityAndTouchWater()
    {
        _isInWater = Physics.CheckSphere(_waterCheck.position, _waterCheckRadius, _waterLayer);

        if (_isInWater)
        {
            _currentMovement.y = +0.5f;

        }
        else
        {
            _currentMovement.y -= _gravity * Time.deltaTime;
        }
    }

    private void HandleAnimControl()
    {
        if (_isMoving) _forceRodAnimtor.SetBool("Force", true);
        else _forceRodAnimtor.SetBool("Force", false);
    }

    public void TurnRightAnim()
    {
        _wheelAnimtor.speed = 1;
        _wheelAnimtor.SetBool("Left", false);
        _wheelAnimtor.SetBool("Right", true);
    }
    public void TurnLefttAnim()
    {
        _wheelAnimtor.speed = 1;
        _wheelAnimtor.SetBool("Right", false);
        _wheelAnimtor.SetBool("Left", true);
    }

    public void StopAnim()
    {
        _wheelAnimtor.speed = 0;
        _isTurn = false;
    }
}
