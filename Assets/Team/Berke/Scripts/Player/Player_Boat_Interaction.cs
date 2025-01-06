using System.Collections.Generic;
using UnityEngine;

public class Player_Boat_Interaction : MonoBehaviour
{
    [SerializeField] private float interactionRange = 5f; 
    [SerializeField] private Camera camera;
    [SerializeField] private Transform wheel; 
    [SerializeField] private Transform boat; 
    [SerializeField] private Material _selectedMaterial;

    private List<MeshRenderer> rendererLists = new List<MeshRenderer>();
    private Dictionary<MeshRenderer, Material> defaultMaterials = new Dictionary<MeshRenderer, Material>();
    private bool isSteering = false; 

    private Vector2 lastMousePosition; 
    private Renderer lastRenderer; 
    private Color originalColor;
    private Boat_Controller boatController;
    private Rigidbody _rb;
    private Animator _anim;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        boatController = FindFirstObjectByType<Boat_Controller>();
    }

    void Update()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            SelectMaterial(hit);

            if (hit.collider.CompareTag("Wheel"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (isSteering) ReleaseWheel();

                    else GrabWheel();
                }
            }

            if (hit.collider.CompareTag("Lever"))
            {
                if (Input.GetKeyDown(KeyCode.T)) boatController.ToggleMovement();
            }
        }
        else
        {
            ResetMaterials();
        }

        BoatTurnControl();
    }

    private void SelectMaterial(RaycastHit hit)
    {
        var selection = hit.transform;

        if (hit.collider.gameObject.CompareTag("Lever") || hit.collider.gameObject.CompareTag("Wheel"))
        {
            var selectionRenderer = hit.collider.gameObject.GetComponent<MeshRenderer>();

            if (!defaultMaterials.ContainsKey(selectionRenderer))
                defaultMaterials[selectionRenderer] = selectionRenderer.sharedMaterial;

            if (!rendererLists.Contains(selectionRenderer))
                rendererLists.Add(selectionRenderer);

            selectionRenderer.sharedMaterial = _selectedMaterial;
        }
    }
    private void ResetMaterials()
    {
        foreach (var renderer in rendererLists)
        {
            if (renderer != null && defaultMaterials.ContainsKey(renderer))
            {
                renderer.sharedMaterial = defaultMaterials[renderer];
            }
        }

        // Clear material lists
        rendererLists.Clear();
        defaultMaterials.Clear();
    }

    public void BoatTurnControl()
    {
        if (isSteering)
        {
            if (Input.GetKey(KeyCode.A))
            {
                RotateShip(-1);
                boatController.TurnLefttAnim();
            }
            if (Input.GetKey(KeyCode.D))
            {
                RotateShip(1);
                boatController.TurnRightAnim();
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                boatController.StopAnim();
            }
        }
    }

    public void SetShipController(Boat_Controller controller)
    {
        boatController = controller;
    }

    private void GrabWheel()
    {
        isSteering = true;

        transform.SetParent(boat);
        GetComponent<Rigidbody>().isKinematic = true;

        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = false; 
                                
        lastMousePosition = Input.mousePosition;

        _anim.enabled = false;
    }

    private void ReleaseWheel()
    {
        isSteering = false;

        transform.SetParent(null); 
        GetComponent<Rigidbody>().isKinematic = false;

        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;

        _anim.enabled = true;
    }

    public void RotateShip(int direction)
    {
        boatController.Rotate(direction);

        _anim.enabled = false;
    }

}
