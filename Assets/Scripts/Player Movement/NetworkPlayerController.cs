using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] float baseMoveSpeed = 10f; 
    private float currentSpeedMultiplier = 1f;

    [SerializeField] float gravity = -9.8f;
    [SerializeField] float groundedGravity = -2.2f;
    [SerializeField] float verticalVelocity = 0f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    public CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Only the player who owns this character should control and move it
        if (!IsOwner) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 inputDirection = new Vector2(horizontalInput, verticalInput);

        bool jumpPressed = Input.GetKeyDown(jumpKey);

        // Move locally immediately — zero input delay!
        MovePlayer(inputDirection, jumpPressed);
    }

    private void MovePlayer(Vector2 movementInput, bool jumpPressed)
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = groundedGravity;
            if (jumpPressed) verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 moveDir = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

        float effectiveSpeed = baseMoveSpeed * currentSpeedMultiplier;

        Vector3 horizontalMovement = moveDir * effectiveSpeed;
        Vector3 verticalMovement = Vector3.up * verticalVelocity;

        controller.Move((horizontalMovement + verticalMovement) * Time.deltaTime);
    }

    public void ModifySpeed(float modifier)
    {
        currentSpeedMultiplier = modifier;
    }
}