using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference slideAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference craftAction;
    [SerializeField] private InputActionReference choice1Action;
    [SerializeField] private InputActionReference choice2Action;
    [SerializeField] private InputActionReference choice3Action;

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
    [Header("Dash")]
    [SerializeField] private float dashDuration = 0.06f;
    [SerializeField] private float dashSpeed = 200F;
    [SerializeField] private float dashCooldown = 1.5f;


    [Header("Implants")]
    [SerializeField] private List<BenzoImplantBase> InstalledImplantList;
    private CharacterController characterController;
    private Vector3 velocity;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float slideCooldownCounter;
    private float dashCooldownCounter;

    private bool wasGroundedLastFrame;
    [SerializeField] private bool isDashing = false;

    private float dashTimer;
    private Vector3 bufferDashVelocity;
    private bool isSliding;

    private float slideTimer;
    private Vector3 slideDirection;
    private float originalHeight;
    private Vector3 originalCameraLocalPos;
    private bool hasMomentum;
    private Vector3 momentumVelocity;
    [SerializeField] private float momentumTimer;
    [Header("System")]
    [HideInInspector] public bool IsCraftOpen; 
    [SerializeField] private CraftSystem CraftSystemReference;
    

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;
        if (cameraRoot != null)
        {
            originalCameraLocalPos = cameraRoot.localPosition;
        }
        foreach (BenzoImplantBase c in InstalledImplantList)
            c.IsEquiped = true;
    }

    private void OnEnable()
    {
        movementAction?.action.Enable();
        jumpAction?.action.Enable();
        slideAction?.action.Enable();
        dashAction?.action.Enable();
        craftAction?.action.Enable();
        choice1Action?.action.Enable();
        choice2Action?.action.Enable();
        choice3Action?.action.Enable();
        jumpAction.action.performed += OnJumpPerformed;
        slideAction.action.performed += OnSlidePerformed;
        dashAction.action.performed += OnDashPerformed;
        craftAction.action.performed += OnCraftPerformed;
        choice1Action.action.performed += OnNumpadPressed1;
        choice2Action.action.performed += OnNumpadPressed2;
        choice3Action.action.performed += OnNumpadPressed3;


    }

    private void OnDisable()
    {
        movementAction?.action.Disable();
        jumpAction?.action.Disable();
        slideAction?.action.Disable();
        dashAction?.action.Disable();
        craftAction?.action.Disable();
        choice1Action?.action.Disable();
        choice2Action?.action.Disable();
        choice3Action?.action.Disable();
        jumpAction.action.performed -= OnJumpPerformed;
        slideAction.action.performed -= OnSlidePerformed;
        dashAction.action.performed -= OnDashPerformed;
        craftAction.action.performed -= OnCraftPerformed;
        choice1Action.action.performed -= OnNumpadPressed1;
        choice2Action.action.performed -= OnNumpadPressed2;
        choice3Action.action.performed -= OnNumpadPressed3;

    }

    private void Update()
    {
        UpdateTimers();
        HandleMovement();
        HandleJump();
        HandleSlide();
        HandleDash();
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
        if(dashCooldownCounter>0)
            dashCooldownCounter -= Time.deltaTime;
        
    }
    private void OnNumpadPressed1(InputAction.CallbackContext context)
    {
        Debug.Log("Key 1 pressed");
        CraftSystemReference.TryToCraft(0);
    }
    private void OnNumpadPressed2(InputAction.CallbackContext context)
    {
        Debug.Log("Key 1 pressed");
        CraftSystemReference.TryToCraft(1);
    }
    private void OnNumpadPressed3(InputAction.CallbackContext context)
    {
        Debug.Log("Key 1 pressed");
        CraftSystemReference.TryToCraft(2);
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
    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Begun Dashing!");

        if (!isDashing && dashCooldownCounter <= 0.0f)
        {

            StartDash();
        }
    }
    private void OnCraftPerformed(InputAction.CallbackContext context)
    {
        CraftSystemReference.SwitchActive();
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
    private void HandleDash()
    {
        if(isDashing)
        {
            UpdateDash();
            if(dashTimer<=0)
            EndDash();
        }
    }

    private void UpdateDash()
    {
        dashTimer -= Time.deltaTime;
        if (bufferDashVelocity == Vector3.zero)
        {
            velocity = this.transform.forward * dashSpeed;
        }
        else
        {
            velocity = bufferDashVelocity.normalized * dashSpeed;
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        slideCooldownCounter = slideCooldown;
        Vector3 currentHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        bufferDashVelocity = currentHorizontalVelocity;
        ActivateImplants(AffectedAction.dash);
        hasMomentum = false;
        momentumTimer = 0f;
        
    }
    private void EndDash()
    {
        isDashing = false;
        momentumVelocity = new Vector3(velocity.x, 0f, velocity.z);

        DectivateImplants(AffectedAction.dash);
        velocity = bufferDashVelocity;
        dashCooldownCounter = dashCooldown;
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        slideCooldownCounter = slideCooldown;

        Vector3 currentHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        slideDirection = currentHorizontalVelocity.normalized;
        ActivateImplants(AffectedAction.slide);
        hasMomentum = false;
        momentumTimer = 0f;
    }
    private void ActivateImplants(AffectedAction aa_)
    {
        foreach(BenzoImplantBase im in InstalledImplantList)
        {
            if(im.a_action ==  aa_)
            {
                Debug.Log("Enabled " + im.name);
                im.Use(true);
            }
        }
    }

    private void DectivateImplants(AffectedAction aa_)
    {
        foreach(BenzoImplantBase im in InstalledImplantList)
        {
            if(im.a_action ==  aa_)
            {
                im.Use(false);
            }
        }
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
        DectivateImplants(AffectedAction.slide);
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
    public void InstallImplant(BenzoImplantBase origImplant)
    {
        InstalledImplantList.Add(origImplant);
        origImplant.IsEquiped = true;
    }
}