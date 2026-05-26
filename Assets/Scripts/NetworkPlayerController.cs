using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 10f;
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

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 inputDirection = new Vector2(horizontalInput, verticalInput);

        bool jumpPressed = Input.GetKeyDown(jumpKey);

        if (IsServer)
        {
            MovePlayer(inputDirection, jumpPressed);
        }
        else
        {
            MovePlayerRPC(inputDirection, jumpPressed);
        }
    }

    [Rpc(SendTo.Server)] //Marks the next method as an RPC that runs on the server
    private void MovePlayerRPC(Vector2 movementInput, bool jumpPressed)
    {
        MovePlayer(movementInput, jumpPressed);
    }

    private void MovePlayer(Vector2 movementInput, bool jumpPressed)
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = groundedGravity;
            }

            if (jumpPressed)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 moveDir = new Vector3(
            movementInput.x,
            0f,
            movementInput.y
        ).normalized;

        Vector3 horizontalMovement = moveDir * moveSpeed;
        Vector3 verticalMovement = Vector3.up * verticalVelocity;

        Vector3 finalMovement = horizontalMovement + verticalMovement;

        controller.Move(finalMovement * Time.deltaTime);
    }
}
