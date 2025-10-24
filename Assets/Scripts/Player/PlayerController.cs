using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference jumpAction;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private bool allowJump = true;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private Vector3 velocity;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private bool wasGroundedLastFrame;
    private Vector3 bufferMoveDir;
    private Vector3 moveDirection;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        movementAction?.action.Enable();
        jumpAction?.action.Enable();
        jumpAction.action.performed += OnJumpPerformed;
        dashAction?.action.Enable();
        dashAction.action.performed += OnDashPerformed;

    }

    void OnDisable()
    {
        movementAction?.action.Disable();
        jumpAction?.action.Disable();
        jumpAction.action.performed -= OnJumpPerformed;
        dashAction?.action.Disable();
        dashAction.action.performed -= OnDashPerformed;


    }

    void Update()
    {
        UpdateTimers();

        if (bufferMoveDir == Vector3.zero)
        {
            HandleMovement();
        }
        HandleJump();
        ApplyGravity();
        characterController.Move(velocity * Time.deltaTime);
        wasGroundedLastFrame = characterController.isGrounded;
    }

    void UpdateTimers()
    {
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;
        
        if (characterController.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else if (wasGroundedLastFrame)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void HandleMovement()
    {
        Vector2 input = movementAction.action.ReadValue<Vector2>();
        moveDirection = transform.right * input.x + transform.forward * input.y;
        velocity.x = moveDirection.x * movementSpeed;
        velocity.z = moveDirection.z * movementSpeed;
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }

    void OnDashPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Attempting to Dash!");
        bufferMoveDir = velocity;
        if (characterController.isGrounded)
        {
            velocity = moveDirection*200;
            StartCoroutine(ReturnToNormal(.06f));
            //dashwhereplyrmoves(default-forward)
        }
        else
        {
            velocity = GetComponentInChildren<PlayerLook>().playerBody.forward * 200;
            StartCoroutine(ReturnToNormal(.06f));
            //dashwhereyoulookinat
        }
        //bla bla bla
    }

    IEnumerator ReturnToNormal(float secondsRemaining)
    {
        
        yield return new WaitForSeconds(secondsRemaining);
        velocity = bufferMoveDir;
        Debug.Log("Bacc to normal");

        bufferMoveDir = Vector3.zero;
    }

    void HandleJump()
    {
        if (!allowJump) return;

        float jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);

        bool canJump = characterController.isGrounded || coyoteTimeCounter > 0;
        
        if (jumpBufferCounter > 0 && canJump)
        {
            velocity.y = jumpForce;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }
}