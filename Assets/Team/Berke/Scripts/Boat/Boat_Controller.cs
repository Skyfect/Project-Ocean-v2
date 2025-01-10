using Crest;
using Crest.Examples;
using UnityEngine;

public class Boat_Controller : MonoBehaviour
{
    [Header("MOVEMENT & TURN")]
    [SerializeField] private float _maxSpeed = 30;
    [SerializeField] private float _speedReducer = 5f;
    [SerializeField] private float _moveForce = 10f;
    [SerializeField] private float _turnForce = 50f;
    [SerializeField] private Transform _forcePoint;
    private GameObject _wheel;
    private GameObject _forceRod;

    [Header("COMPONNETS")]
    private float _waterCheckRadius = 0.2f;
    private bool _isInWater;
    private bool _isTurn;
    private Vector3 _currentMovement = Vector3.zero;
    private bool _isMoving = false;
    private Rigidbody _rb;
    private Animator _wheelAnimtor;
    private Animator _forceRodAnimtor;
    private BoatAlignNormal _crestBoatAlignNormal;

    private void Awake()
    {
        _crestBoatAlignNormal = GetComponent<BoatAlignNormal>();
        _rb = GetComponent<Rigidbody>();
        _wheel = GameObject.FindWithTag("Wheel");
        _forceRod = GameObject.FindWithTag("Lever");
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

        Vector3 velocity = _rb.velocity;
        Debug.Log("current speed:" + velocity.z);
    }

    [System.Obsolete]
    private void Movement()
    {
        if (_isMoving)
        {
            _crestBoatAlignNormal._enginePower = 0.257F; // CREST BOAT ENGINE POWER CHANGED

            Vector3 moveForce = transform.forward * _moveForce;
            _rb.AddForce(moveForce, ForceMode.Force);
            //_rb.AddForceAtPosition(transform.forward * _moveSpeed, _forcePoint.position, ForceMode.Force);

            if (_rb.velocity.magnitude > _maxSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _maxSpeed;
            }
        }
        else
        {
            _crestBoatAlignNormal._enginePower = 0f;  // CREST BOAT ENGINE POWER RESET

            Vector3 frictionForce = -_rb.velocity.normalized * _speedReducer * _rb.velocity.magnitude;
            _rb.AddForce(frictionForce, ForceMode.Force);

            if (_rb.velocity.magnitude < 0.1f)
            {
                _rb.velocity = Vector3.zero;
            }
        }
    }


    public void Rotate(int direction)
    {
        // Rotate (-1: Left, 1: Right)
        //transform.Rotate(0f, direction * _turnSpeed * Time.deltaTime, 0f);

        Vector3 torque = Vector3.up * direction * _turnForce;
        _rb.AddTorque(torque, ForceMode.Acceleration);
    }

    public void ToggleMovement()
    {
        _isMoving = !_isMoving;
        HandleAnimControl();
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
