using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float gravity = -9.8f;
    [SerializeField] float groundedGravity = -2.2f;
    [SerializeField] float verticalVelocity = 0f;

    public CharacterController controller;
    void Awake()
    {
        controller.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 inputDirection = new Vector2(horizontalInput, verticalInput);

        if(IsServer)
        {
            MovePlayer(inputDirection);
        }
        else
        {
            MovePlayerRPC(inputDirection);
        }
    }
    
    [Rpc(SendTo.Server)] //Marks the next method as an RPC that runs on the server
    private void MovePlayerRPC(Vector2 movementInput)
    {
        MovePlayer(movementInput);
    }

    private void MovePlayer(Vector2 movemenInput)
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedGravity;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 moveDir = new Vector3(movemenInput.x, 0f, movemenInput.y).normalized;
        Vector3 horizontalMovement = moveDir * moveSpeed;
        Vector3 verticalMovement = Vector3.up * verticalVelocity;
        Vector3 finalMovement = horizontalMovement + verticalMovement;

        controller.Move(finalMovement * Time.deltaTime);
    }
}
