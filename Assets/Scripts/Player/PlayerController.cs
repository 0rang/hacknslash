using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Attack Params")]
    public float attackTime;
    [Tooltip("Gives the centre of the hit sphere as a float offset to the front of the Player's Transform")]
    public float hitSphereCentre;
    public float hitSphereRadius;

    [Header("Child Objects")]
    public Transform cameraTransform;
    public Transform shadowTransform;

    [Header("Physics Params")]
    public float acceleration;
    [Range(0f, 1f)]
    public float damping;
    public float gravity;
    public float jumpThrust;

    [Header("Camera Params")]
    public float vertCamSensitivity;
    [Tooltip("controls both player and camera rotation horizontally")]
    public float rotationSpeed;

    [Header("Gizmo Draw Toggles")]
    public bool drawAttackHitSphere;

    [Header("Other")]
    [SerializeField]
    private Vector3 velocity;

    private CharacterController controller;

    // TODO: Move Input Sytem code to a separate script with public properties for use in other objects
    [Header("Input Actions")]
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpInput;
    private bool fireInput;
    private bool firePressed;
    private bool fire2Input;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerCameraRotation();

        CamToFloorTeleport();

        // TODO: Make Attack() only get called on the frame the fireInput is pressed down
        if (firePressed)
        {
            Debug.Log("Attack Called");
            Attack();
            firePressed = false;
        }

        if (fire2Input)
        {
            CamToFloorTeleport();
        }
    }

    private void FixedUpdate()
    {
        MoveCharacterController();
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void GetLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    
    public void GetJumpInput(InputAction.CallbackContext context)
    {
        jumpInput = context.ReadValueAsButton();
    }

    public void GetFireInput(InputAction.CallbackContext context)
    {
        Debug.Log("FireInput Event Called");
        fireInput = context.ReadValueAsButton();
        firePressed = context.performed;
        if(context.performed == true)
        {
            Debug.Log("performed true");
        }
    }

    public void GetFire2Input(InputAction.CallbackContext context)
    {
        fire2Input = context.ReadValueAsButton();
    }

    /// <summary>
    /// uses <see cref="lookInput"/> to control Cinemachine virtual camera by rotating child transform which the vcam follows. 
    /// rotates cam according to <see cref="vertCamSensitivity"/>
    /// rotates Player (and hence the camera via its child) according to 
    /// </summary>
    private void PlayerCameraRotation()
    {
        transform.Rotate(0, lookInput.x * rotationSpeed * Time.deltaTime, 0);
        cameraTransform.RotateAround(cameraTransform.position, transform.right, -lookInput.y * vertCamSensitivity * Time.deltaTime);
    }

    private void CamToFloorTeleport()
    {
        RaycastHit hit;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (fire2Input && Physics.Raycast(ray, out hit) && hit.collider.name == "Ground")
        {
            controller.enabled = false;
            transform.position = new Vector3(hit.point.x, hit.point.y + 1, hit.point.z);
            controller.enabled = true;
        }
    }

    /// <summary>
    /// Uses the public physics parameters to control ground and jump movement via the CharacterController Component
    /// Excludes rotation as that is tied 
    /// </summary>
    private void MoveCharacterController()
    {
        Vector2 groundAccel = moveInput * Time.fixedDeltaTime * acceleration;
        Vector3 forwardMotion = transform.forward * groundAccel.y;
        Vector3 strafingMotion = transform.right * groundAccel.x;
        velocity += forwardMotion;
        velocity += strafingMotion;
        velocity -= velocity * damping;
        if (controller.isGrounded)
        {
            velocity = new Vector3(velocity.x, -1f, velocity.z);
            if (jumpInput)
            {
                Debug.Log("Hello I'm Jumping uWu");
                velocity += new Vector3(0, jumpThrust, 0);
            }
        }
        else
        {
            velocity += new Vector3(0, -gravity * Time.fixedDeltaTime, 0);
        }

        controller.Move(velocity);
    }

    /// <summary>
    /// Checks for any enemy colliders overlapping with a sphere generated in front of the player. 
    /// Calls <see cref="EnemyDamage.TakeDamage"/> in those objects
    /// </summary>
    private void Attack()
    {
        Collider[] enemiesHit = Physics.OverlapSphere(
            transform.position + transform.forward * hitSphereCentre,
            hitSphereRadius,
            LayerMask.GetMask("Enemy") // does this exclude or include the enemy layer? 
            );

        foreach (Collider col in enemiesHit)
        {
            Debug.Log("Found a collider! name: " + col.name);
            // TODO: code flow goes into else plz fix
            EnemyDamage enemy = col.gameObject.GetComponent<EnemyDamage>();
            if (enemy != null)
            {
                Debug.Log("Calling TakeDamage in attacked Object");
                enemy.TakeDamage();
            }
            else
            {
                Debug.LogError(col.name + ": object with layer \"Enemy\" does not have EnemyDamage Behaviour");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {

        if (drawAttackHitSphere)
        {
            Gizmos.DrawSphere(transform.position + transform.forward * hitSphereCentre, hitSphereRadius);
        }
    }
}
