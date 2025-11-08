using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference slideAction;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private bool allowJump = true;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Slide Settings")]
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private float slideSpeed = 15f;
    [SerializeField] private float slideFriction = 7.35f;
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private float cameraSlideOffset = -0.1f;
    [SerializeField] private float heightLerpSpeed = 20f;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float slideCooldown = 1.5f;
    [SerializeField] private float momentumDecayTime = 0.5f;

    private CharacterController characterController;
    private Vector3 velocity;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float slideCooldownCounter;
    private bool wasGroundedLastFrame;
    private bool isSliding;
    private float slideTimer;
    private Vector3 slideDirection;
    private float originalHeight;
    private Vector3 originalCameraLocalPos;
    private bool hasMomentum;
    private Vector3 momentumVelocity;
    private float momentumTimer;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;
        if (cameraRoot != null)
        {
            originalCameraLocalPos = cameraRoot.localPosition;
        }
    }

    private void OnEnable()
    {
        movementAction?.action.Enable();
        jumpAction?.action.Enable();
        slideAction?.action.Enable();
        jumpAction.action.performed += OnJumpPerformed;
        slideAction.action.performed += OnSlidePerformed;
    }

    private void OnDisable()
    {
        movementAction?.action.Disable();
        jumpAction?.action.Disable();
        slideAction?.action.Disable();
        jumpAction.action.performed -= OnJumpPerformed;
        slideAction.action.performed -= OnSlidePerformed;
    }

    private void Update()
    {
        UpdateTimers();
        HandleMovement();
        HandleJump();
        HandleSlide();
        ApplyGravity();
        characterController.Move(velocity * Time.deltaTime);
        wasGroundedLastFrame = characterController.isGrounded;
    }

    private void UpdateTimers()
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
        
        if (slideCooldownCounter > 0) slideCooldownCounter -= Time.deltaTime;

        if (hasMomentum && momentumTimer > 0)
        {
            momentumTimer -= Time.deltaTime;
            if (momentumTimer <= 0)
            {
                hasMomentum = false;
            }
        }
    }

    private void HandleMovement()
    {
        if (isSliding) return;

        Vector2 input = movementAction.action.ReadValue<Vector2>();
        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
        
        if (hasMomentum && momentumTimer > 0)
        {
            if (input.magnitude > 0.1f)
            {
                Vector3 newVelocity = moveDirection * movementSpeed;
                
                float t = 1f - (momentumTimer / momentumDecayTime);
                velocity.x = Mathf.Lerp(momentumVelocity.x, newVelocity.x, t);
                velocity.z = Mathf.Lerp(momentumVelocity.z, newVelocity.z, t);
            }
            else
            {
                float decayFactor = momentumTimer / momentumDecayTime;
                velocity.x = momentumVelocity.x * decayFactor;
                velocity.z = momentumVelocity.z * decayFactor;
            }
        }
        else
        {
            velocity.x = moveDirection.x * movementSpeed;
            velocity.z = moveDirection.z * movementSpeed;
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }

    private void HandleJump()
    {
        if (!allowJump) return;

        float jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);

        bool canJump = characterController.isGrounded || coyoteTimeCounter > 0;
        
        if (jumpBufferCounter > 0 && canJump)
        {
            velocity.y = jumpForce;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            
            hasMomentum = false;
            momentumTimer = 0f;
        }
    }

    private void OnSlidePerformed(InputAction.CallbackContext context)
    {
        Vector3 currentHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        if (characterController.isGrounded && !isSliding && currentHorizontalVelocity.magnitude > 0.1f && slideCooldownCounter <= 0f)
        {
            StartSlide();
        }
    }

    private void HandleSlide()
    {
        if (!isSliding) 
        {
            LerpHeightAndCameraBack();
            return;
        }

        UpdateSlide();
        if (slideTimer <= 0)
        {
            EndSlide();
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        slideCooldownCounter = slideCooldown;

        Vector3 currentHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        slideDirection = currentHorizontalVelocity.normalized;
        
        hasMomentum = false;
        momentumTimer = 0f;
    }

    private void UpdateSlide()
    {
        slideTimer -= Time.deltaTime;

        float currentSlideSpeed = slideSpeed - (slideFriction * (slideDuration - slideTimer));
        currentSlideSpeed = Mathf.Max(currentSlideSpeed, movementSpeed);

        velocity.x = slideDirection.x * currentSlideSpeed;
        velocity.z = slideDirection.z * currentSlideSpeed;

        characterController.height = Mathf.Lerp(characterController.height, slideHeight, heightLerpSpeed * Time.deltaTime);

        if (cameraRoot != null)
        {
            Vector3 targetCameraPos = originalCameraLocalPos + new Vector3(0f, cameraSlideOffset, 0f);
            cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, targetCameraPos, heightLerpSpeed * Time.deltaTime);
        }
    }

    private void EndSlide()
    {
        isSliding = false;
        
        momentumVelocity = new Vector3(velocity.x, 0f, velocity.z);
        hasMomentum = true;
        momentumTimer = momentumDecayTime;
    }

    private void LerpHeightAndCameraBack()
    {
        characterController.height = Mathf.Lerp(characterController.height, originalHeight, heightLerpSpeed * Time.deltaTime);
        if (cameraRoot != null)
        {
            cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, originalCameraLocalPos, heightLerpSpeed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
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