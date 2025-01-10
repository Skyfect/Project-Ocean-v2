using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 10f;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _animHorizontal, _animVertical;
    private bool _isCrouch;

    private float nextUpdateTime = 0f;
    private float updateInterval = 0.05f;

    [Header("Jump")]
    [SerializeField] private float _jumpForce= 5;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded;

    [Header("Cling")]
    [SerializeField] private float _interactDistance = 3.0f;
    [SerializeField] private LayerMask _interactableLayer;
    private bool _isClinged = false;
    private bool _isInTransition = false;
    private Transform _clingedObject;

    [Header("Look Sensitivy")]
    [SerializeField] private float _mouseSensitivy = 2f;
    [SerializeField] private float _upDownRange = 80f;
    private float _verticalRotation;

    [Header("Inputs Customisation")]
    [SerializeField]
    private InputActionAsset _playerControls;

    private InputAction _walkAction;
    private InputAction _runAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _crouchAction;
    private InputAction _clingAction;

    private Vector2 _walkInput;
    private Vector2 _lookInput;

    
    [Header("Default Values")]
    private float _initialWalkSpeed;
    private float _initialRunSpeed;
    private float _initialCrouchSpeed;
    private float _initialGravity;
    private float _initialJump;
    private Vector3 _currentMovement = Vector3.zero;

    [Header("Components")]
    [SerializeField] private Transform _headTransform;
    private Animator _anim;
    private Rigidbody _rb;
    private NetworkConnection _networkConnection;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _networkConnection = FindFirstObjectByType<NetworkConnection>();

        _walkAction = _playerControls.FindActionMap("Player").FindAction("Walk");
        _runAction = _playerControls.FindActionMap("Player").FindAction("Run");
        _lookAction = _playerControls.FindActionMap("Player").FindAction("Look");
        _jumpAction = _playerControls.FindActionMap("Player").FindAction("Jump");
        _crouchAction = _playerControls.FindActionMap("Player").FindAction("Crouch");
        _clingAction = _playerControls.FindActionMap("Player").FindAction("Cling");

        _walkAction.performed += context => _walkInput = context.ReadValue<Vector2>();
        _walkAction.canceled += context => _walkInput = Vector2.zero;

        _lookAction.performed += context => _lookInput = context.ReadValue<Vector2>();
        _lookAction.canceled += context => _lookInput = Vector2.zero;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    void Start()
    {
        GetInitialParameters();
        InvokeRepeating("MovementUpdate", 0f, 0.02f);
    }


    private void OnEnable()
    {
        _walkAction.Enable();
        _runAction.Enable();
        _lookAction.Enable();
        _jumpAction.Enable();
        _crouchAction.Enable();
        _crouchAction.started += HandleCrouch;
        _crouchAction.canceled += HandleCrouch;
        _clingAction.Enable();
    }

    private void OnDisable()
    {
        _walkAction.Disable();
        _runAction.Disable();
        _lookAction.Disable();
        _jumpAction.Disable();
        _crouchAction.Disable();
        _crouchAction.started -= HandleCrouch;
        _crouchAction.canceled -= HandleCrouch;
        _clingAction.Disable();
    }



    [System.Obsolete]
    private void MovementUpdate()
    {
        HandleMovement();
        HandleRotation();
        HandleClingInteraction();
        CrouchMovement();
        MovementAnimsControl();
    }

    [System.Obsolete]
    private void HandleMovement()
    {
        float runInput = _runAction.ReadValue<float>() > 0 ? _runSpeed : 1f;

        float verticalSpeed = _walkInput.y * _walkSpeed * runInput;
        float horizontalSpeed = _walkInput.x * _walkSpeed * runInput;

        RunAnim(runInput);

        Vector3 horizontalMovement = new Vector3(horizontalSpeed, 0, verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;

        HandleGravityAndJump();

        _currentMovement.x = horizontalMovement.x;
        _currentMovement.z = horizontalMovement.z;

        _rb.linearVelocity = (_currentMovement);

        if (_networkConnection != null && Time.time >= nextUpdateTime)
        {
            _networkConnection.SendPositionUpdate(transform.position);
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    private void MovementAnimsControl()
    {
        if (_isCrouch)
            return;

        Vector2 movementInput = _walkAction.ReadValue<Vector2>();

        if (movementInput.x < 0 && movementInput.y > 0) // L Diagonal
        {
            _animHorizontal = -0.3f;
            _animVertical = 0.3f;
            _anim.SetFloat("horizontal", _animHorizontal);
            _anim.SetFloat("vertical", _animVertical);
        }
        if (movementInput.x > 0 && movementInput.y > 0)  // R Diagonal
        {
            _animHorizontal = 0.3f;
            _animVertical = 0.3f;
            _anim.SetFloat("horizontal", _animHorizontal);
            _anim.SetFloat("vertical", _animVertical);
        }
        if (movementInput.x > 0 && movementInput.y == 0)  // R Strafe
        {
            _animHorizontal = 0.3f;
            _animVertical = 0F;
            _anim.SetFloat("horizontal", _animHorizontal);
            _anim.SetFloat("vertical", _animVertical);
        }
        if (movementInput.x < 0 && movementInput.y == 0)  // L Strafe
        {
            _animHorizontal = -0.3f;
            _animVertical = 0F;
            _anim.SetFloat("horizontal", _animHorizontal);
            _anim.SetFloat("vertical", _animVertical);
        }
        if (movementInput.x == 0 && movementInput.y < 0)  // Backward
        {
            _animHorizontal = 0f;
            _animVertical = -0.3F;
            _anim.SetFloat("horizontal", _animHorizontal);
            _anim.SetFloat("vertical", _animVertical);
        }
    }

    private void RunAnim(float runSpeed)
    {
        if (runSpeed != 1) // run
        {
            _animHorizontal = 0;
            _animVertical = 1;
            _anim.SetFloat("horizontal", _animHorizontal * runSpeed);
            _anim.SetFloat("vertical", _animVertical * runSpeed);
        }
            

        else  // walk
        {
            _animHorizontal = 0;
            _animVertical = 0.3f;
            _anim.SetFloat("horizontal", _animHorizontal);
            _anim.SetFloat("vertical", _animVertical);
        }
    }

    private void HandleRotation()
    {
        if (_headTransform == null)
            return;

        float mouseXRotation = _lookInput.x * _mouseSensitivy;
        transform.Rotate(0, mouseXRotation, 0);

        _verticalRotation -= _lookInput.y * _mouseSensitivy;
        _verticalRotation = Mathf.Clamp(_verticalRotation, - _upDownRange, _upDownRange);

        _headTransform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
    }

    private void HandleGravityAndJump()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundLayer);

        if (_isGrounded)
        {
            _currentMovement.y = -0.5f;

            if (_jumpAction.triggered)
            {
                _currentMovement.y = _jumpForce;
            }
           
        }
        else   // Jump
        {
            _animHorizontal = 0f;
            _animVertical = 0.2f;
            _anim.SetFloat("horizontal", _animHorizontal);
            _anim.SetFloat("vertical", _animVertical);

            _currentMovement.y -= _gravity * Time.deltaTime;
        }
    }

    private void HandleCrouch(InputAction.CallbackContext context)
    {
        if (_isClinged)
            return;

        if (context.started)
        {
            _walkSpeed = _crouchSpeed;
            _runSpeed = 0;

            _isCrouch = true;
        }
        else if (context.canceled)
        {
            SetInitialParameters();
            
            _anim.SetBool("crouch", false);
            _isCrouch = false;
        }
    }

    [System.Obsolete]
    private void CrouchMovement()
    {
        if (!_isCrouch || _isClinged) return;

        Vector2 movementInput = _walkAction.ReadValue<Vector2>();

        if (movementInput.y == 0)  // Crouch
        {
            _animHorizontal = 0f;
            _animVertical = 0f;
            _anim.SetFloat("horizontal", _animHorizontal);
            _anim.SetFloat("vertical", _animVertical);
        }
        if (movementInput.y > 0) // Crouch walk
        {
            _animHorizontal = 0f;
            _animVertical = 0.1f;
            _anim.SetFloat("horizontal", _animHorizontal);
            _anim.SetFloat("vertical", _animVertical);
        } 
        if (movementInput.y < 0)   // Crouch backwalk
        {
            _animHorizontal = 0f;
            _animVertical = -0.1f;
            _anim.SetFloat("horizontal", _animHorizontal);
            _anim.SetFloat("vertical", _animVertical);
        }
    }

    private void HandleClingInteraction()
    {
        if (_isInTransition)
            return;

        if (_isClinged && _clingAction.triggered)
        {
            DetachFromObject();
            return;
        }

        Ray ray = new Ray(_headTransform.position, _headTransform.forward);

        Debug.DrawRay(ray.origin, ray.direction * _interactDistance, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactableLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);

            if (_clingAction.triggered && !_isClinged)
            {
                ClingToObject(hit.transform);
            }
        }
    }

    void ClingToObject(Transform targetPos)
    {
        _clingedObject = targetPos;

        _isInTransition = true;
        _isClinged = true;

        ResetParameters();

        transform.SetParent(_clingedObject);

        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
            
        Invoke(nameof(ResetTransition), 0.1f);
    }

    void DetachFromObject()
    {
        _isInTransition = true;
        _isClinged = false;

        transform.SetParent(null);

        SetInitialParameters();

        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
        }
    }

    void ResetTransition()
    {
        _isInTransition = false; 
    }

    private void GetInitialParameters()
    {
        _initialWalkSpeed = _walkSpeed;
        _initialRunSpeed = _runSpeed;
        _initialCrouchSpeed = _crouchSpeed;
        _initialGravity = _gravity;
        _initialJump = _jumpForce;
    }

    private void SetInitialParameters()
    {
        _walkSpeed = _initialWalkSpeed;
        _runSpeed = _initialRunSpeed;
        _crouchSpeed = _initialCrouchSpeed;
        _gravity = _initialGravity;
        _jumpForce = _initialJump;
    }

    private void ResetParameters()
    {
        _walkSpeed = 0;
        _runSpeed = 0;
        _crouchSpeed = 0;
        _gravity = 0;
        _jumpForce = 0;
    }
}
