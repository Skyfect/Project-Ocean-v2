using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Gun_Controller : MonoBehaviour
{
    [SerializeField]  private bool isGunVisible = false; 
    private bool isAiming = false;
    private bool isShootting = false;

    [Header("Inputs")]
    [SerializeField] private InputActionAsset _playerControls;
    private InputAction _rightClickAction;
    private InputAction _leftClickAction;

    [Header("Components")]
    [SerializeField] private GameObject gun;
    [SerializeField] private Transform gunInitialPos;
    [SerializeField] private Transform gunDefaultPosition;
    [SerializeField] private Transform gunAimPosition;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private Transform gunBarrel;
    [SerializeField] private float fireRange = 100f;
    [SerializeField] private float gunDisableTimer = 3f;
    

    private void Awake()
    {
        _rightClickAction = _playerControls.FindActionMap("Player").FindAction("RightClick");
        _leftClickAction = _playerControls.FindActionMap("Player").FindAction("LeftClick");
    }
    void Start()
    {
        gun.SetActive(false);

        if(crosshair != null)
            crosshair.SetActive(false);

        InvokeRepeating("GunPosUpdate", 0f, 0.02f);
    }

    private void OnEnable()
    {
        _rightClickAction.Enable();
        _leftClickAction.Enable();

        _rightClickAction.started += AimInput;
        _rightClickAction.canceled += AimInput;

        _leftClickAction.started += ShootInput;
        _leftClickAction.canceled += ShootInput;
    }

    private void OnDisable()
    {
        _leftClickAction.Disable();
        _leftClickAction.Disable();

        _rightClickAction.started -= AimInput;
        _rightClickAction.canceled -= AimInput;

        _leftClickAction.started -= ShootInput;
        _leftClickAction.canceled -= ShootInput;
    }

    private void GunPosUpdate()
    {
        UpdateGunDefaultPosition();
        UpdateGunAimPosition();
        DisabledGun();
    }

    void AimInput(InputAction.CallbackContext contextRight)
    {
        if (contextRight.started)
        {
            if (!isGunVisible)
            {
                gun.SetActive(true); // Show Gun
                isGunVisible = true;
                isAiming = false;
            }
            else
            {
                isAiming = !isAiming; // Aim state change
            }
        }

        if (contextRight.canceled)
        {
            isAiming = false;
        }
    }

    void ShootInput(InputAction.CallbackContext contextLeft)
    {
        // Left Click: Shoot
        if (contextLeft.started && isGunVisible)
        {
            if (!isShootting)
            {
                isShootting = true;
                StartCoroutine(Shooting());
            }
        }

        // When left click is released, weapon returns to default position
        if (contextLeft.canceled && isGunVisible)
        {
            isShootting = false;
            if (!isAiming) 
            {
                ResetGunPosition();
            }
        }
    }


    private void UpdateGunAimPosition()
    {
        if (isGunVisible && isAiming)
        {
            if (crosshair != null) crosshair.SetActive(false); // Cross disable
            gun.transform.position = Vector3.Lerp(gun.transform.position, gunAimPosition.position, Time.deltaTime * 10f);
            gun.transform.rotation = Quaternion.Lerp(gun.transform.rotation, gunAimPosition.rotation, Time.deltaTime * 10f);
        }
    }
    private void UpdateGunDefaultPosition()
    {
        if (isGunVisible && !isAiming)
        {
            if(crosshair != null) crosshair.SetActive(true);  // Cross enable
            gun.transform.position = Vector3.Lerp(gun.transform.position, gunDefaultPosition.position, Time.deltaTime * 10f);
            gun.transform.rotation = Quaternion.Lerp(gun.transform.rotation, gunDefaultPosition.rotation, Time.deltaTime * 10f);
        }
    }

    private void ResetGunPosition()
    {
        isAiming = false; // Aim mod disable
        gun.transform.position = Vector3.Lerp(gun.transform.position, gunDefaultPosition.position, Time.deltaTime * 10f);
        gun.transform.rotation = Quaternion.Lerp(gun.transform.rotation, gunDefaultPosition.rotation, Time.deltaTime * 10f);
    }

    private void DisabledGun()
    {
        if (isGunVisible)
        {
            if (!isAiming && !isShootting)
            {
                if (gunDisableTimer > 0)
                {
                    gunDisableTimer -= Time.deltaTime;
                }
                else
                {
                    if (crosshair != null) crosshair.SetActive(false);
                    gun.SetActive(false);

                    gunDisableTimer = 3;
                    isGunVisible = false;
                }
            }
            else
            {
                gunDisableTimer = 3;
            }
        }
    }

    IEnumerator Shooting()
    {
        while (isShootting)
        {
            Shoot();
            yield return new WaitForSeconds(fireRange);
        }
    }

    void Shoot()
    {
        Ray ray;
        if (isAiming)
        {
            // Camera center postion aim
            ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        }
        else
        {
            // Gun front position
            ray = new Ray(gunBarrel.position, gunBarrel.forward);
        }

        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * fireRange, Color.blue, 0.1f);

        if (Physics.Raycast(ray, out hit, fireRange))
        {
            Debug.Log("Hit: " + hit.collider.name);

            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));

            Destroy(impact, 2f);
        }
    }
}
