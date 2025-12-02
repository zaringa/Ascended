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
    [SerializeField] private InputActionReference slideAction;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float dashSpeed = 200.0f;
    [SerializeField] private float jumpSpeedBoostFactor = 0.3f; // Сколько горизонт. скорости добавляется к высоте прыжка (в метрах). 0 = выкл.
[SerializeField] private float airControlSpeed = 0.5f;     // Насколько медленно меняется направление в воздухе (0 = невозможно, 1 = нормальный контроль)       // 0 = никакого контроля в воздухе, 1 = полный контроль как на земле


    [Header("Jump")]
    [SerializeField] private bool allowJump = true;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Slide")]
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private float slideSpeed = 15f;
    [SerializeField] private float slideFriction = 7.35f;
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private float cameraSlideOffset = -0.1f;
    [SerializeField] private float heightLerpSpeed = 20f;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float slideCooldown = 1.5f;
    [SerializeField] private float momentumDecayTime = 0.5f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private Vector3 velocity;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float slideCooldownCounter;
    private bool wasGroundedLastFrame;
    private Vector3 bufferMoveDir;
    private Vector3 moveDirection;

    private bool isSliding;
    private float slideTimer;
    private Vector3 slideDirection;
    private float originalHeight;
    private Vector3 originalCameraLocalPos;
    private bool hasMomentum;
    private Vector3 momentumVelocity;
    private float momentumTimer;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;
        if (cameraRoot != null)
        {
            originalCameraLocalPos = cameraRoot.localPosition;
        }
    }

    void OnEnable()
    {
        movementAction?.action.Enable();
        jumpAction?.action.Enable();
        jumpAction.action.performed += OnJumpPerformed;
        dashAction?.action.Enable();
        dashAction.action.performed += OnDashPerformed;
        slideAction?.action.Enable();
        slideAction.action.performed += OnSlidePerformed;
    }

    void OnDisable()
    {
        movementAction?.action.Disable();
        jumpAction?.action.Disable();
        jumpAction.action.performed -= OnJumpPerformed;
        dashAction?.action.Disable();
        dashAction.action.performed -= OnDashPerformed;
        slideAction?.action.Disable();
        slideAction.action.performed -= OnSlidePerformed;
    }

    void Update()
    {
        UpdateTimers();

        if (bufferMoveDir == Vector3.zero)
        {
            HandleMovement();
        }
        HandleJump();
        HandleSlide();
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

    void HandleMovement()
    {
        Vector2 input = movementAction.action.ReadValue<Vector2>();
        Vector3 inputDir = transform.right * input.x + transform.forward * input.y;

        // Горизонтальная скорость (x/z)
        Vector3 currentHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        // Управление на земле — полное
        if (characterController.isGrounded || isSliding)
        {
            float targetSpeed = isSliding ? Mathf.Max(slideSpeed, movementSpeed) : movementSpeed;
            Vector3 targetVelocity = inputDir * targetSpeed;
            velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, 10f * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, 10f * Time.deltaTime);
        }
        else
        {
            // В воздухе — сохраняем текущую скорость, но медленно изменяем её при вводе
            if (input.magnitude > 0.1f)
            {
                // Целевая скорость — то, куда хочешь двигаться
                Vector3 targetVelocity = inputDir * movementSpeed;

                // Медленное изменение текущей скорости к целевой
                velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, airControlSpeed * 0.5f * Time.deltaTime); // 0.5f — делаем ещё медленнее
                velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, airControlSpeed * 0.5f * Time.deltaTime);
            }
            // Если нет ввода — скорость остаётся без изменений
        }

        // Обработка импульса после слайда
        if (hasMomentum && momentumTimer > 0)
        {
            float decayFactor = momentumTimer / momentumDecayTime;
            Vector3 momentumContribution = momentumVelocity * decayFactor;

            if (input.magnitude > 0.1f)
            {
                // Смешиваем импульс и ввод
                Vector3 blendedTarget = Vector3.Lerp(momentumContribution, inputDir * movementSpeed, 0.3f); // 30% от ввода
                velocity.x = Mathf.Lerp(velocity.x, blendedTarget.x, airControlSpeed * 0.3f * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, blendedTarget.z, airControlSpeed * 0.3f * Time.deltaTime);
            }
            else
            {
                // Без ввода — просто затухание импульса
                velocity.x = Mathf.Lerp(velocity.x, momentumContribution.x, 0.3f * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, momentumContribution.z, 0.3f * Time.deltaTime);
            }
        }
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }

    void OnDashPerformed(InputAction.CallbackContext context)
    {
        bufferMoveDir = velocity;
        if (moveDirection == Vector3.zero)
        {
            velocity = GetComponentInChildren<PlayerLook>().playerBody.forward * dashSpeed;
        }
        else
        {
            velocity = moveDirection * 200;
        }
        StartCoroutine(ReturnToNormal(.06f));
    }
    IEnumerator ReturnToNormal(float secondsRemaining)
    {
        
        yield return new WaitForSeconds(secondsRemaining);
        velocity = bufferMoveDir;

        bufferMoveDir = Vector3.zero;
    }

    void HandleJump()
    {
        if (!allowJump) return;

        // Базовая сила прыжка
        float baseJumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // ➕ Добавка от горизонтальной скорости (только при прыжке с земли/слайда)
        float speedBoost = 0f;
        if (characterController.isGrounded || isSliding)
        {
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
            float speed = horizontalVelocity.magnitude;
            speedBoost = speed * jumpSpeedBoostFactor;
        }

        float totalJumpForce = baseJumpForce + speedBoost;

        bool canJump = characterController.isGrounded || coyoteTimeCounter > 0;

        if (jumpBufferCounter > 0 && canJump)
        {
            velocity.y = totalJumpForce;
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
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
        momentumVelocity = currentHorizontalVelocity; // ← Сохраняем не только направление, но и модуль!
        
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

    void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            // Применяем небольшую "прилипательную" силу для стабильности
            velocity.y = -2f; // можно вынести как [SerializeField] groundedStickForce
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }
}