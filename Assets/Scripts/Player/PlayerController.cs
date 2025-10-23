using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- Input ---
    [Header("Input")]
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference jumpAction;

    // --- Movement ---
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;

    // --- Jump & Coyote/Buffer ---
    [Header("Jump")]
    [SerializeField] private bool allowJump = true;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    // --- Wall Jump Settings ---
    [Header("Wall Jump Settings")]
    [SerializeField] private float wallCheckDistance = 0.6f;
    [SerializeField] private string wallTag = "Wall";        
    [SerializeField] private float wallSlideSpeed = -1f;      

    [Header("Wall Jump Forces")]
    [SerializeField] private float wallJumpHeight = 3f;      // Новая высота прыжка при отскоке
    [SerializeField] private float wallJumpForce = 20f;      // Горизонтальный толчок (импульс)
    [SerializeField] private float momentumDecay = 10f;       // Скорость затухания горизонтального импульса
    [SerializeField] private float wallJumpCooldownTime = 0.15f; // Время игнорирования ограничений стены
    
    // Управляемость в воздухе во время отскока (0.95 по требованию пользователя)
    [SerializeField] private float airControlDuringWallJump = 0.95f; 

    // --- Gravity ---
    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    // --- Приватные переменные состояния ---
    private CharacterController characterController;
    private Vector3 velocity;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private bool wasGroundedLastFrame;

    private Vector3 wallNormal = Vector3.zero;       
    private Vector3 wallJumpMomentum = Vector3.zero; 
    private float wallJumpCooldownTimer; // Таймер для игнорирования ограничений

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        movementAction?.action.Enable();
        jumpAction?.action.Enable();
        jumpAction.action.performed += OnJumpPerformed;
    }

    void OnDisable()
    {
        movementAction?.action.Disable();
        jumpAction?.action.Disable();
        jumpAction.action.performed -= OnJumpPerformed;
    }

    void Update()
    {
        UpdateTimers();
        HandleWallCheck();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        
        characterController.Move(velocity * Time.deltaTime);
        wasGroundedLastFrame = characterController.isGrounded;
    }

    void UpdateTimers()
    {
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;
        if (wallJumpCooldownTimer > 0) wallJumpCooldownTimer -= Time.deltaTime; // Уменьшаем кулдаун
        
        if (characterController.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            wallNormal = Vector3.zero; 
            wallJumpMomentum = Vector3.zero; 
        }
        else if (wasGroundedLastFrame)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void HandleWallCheck()
    {
        wallNormal = Vector3.zero;

        if (characterController.isGrounded) return;

        RaycastHit hit;
        Vector3 origin = transform.position;
        float radius = characterController.radius;
        
        Vector3 checkDirection = transform.forward; 
        Vector2 input = movementAction.action.ReadValue<Vector2>();
        
        if (input.magnitude > 0.1f)
        {
             Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
             checkDirection = moveDirection.normalized;
        }

        if (Physics.SphereCast(origin, radius, checkDirection, out hit, wallCheckDistance)) 
        {
            if (!hit.collider.CompareTag(wallTag)) 
            {
                return;
            }
            
            if (Vector3.Angle(Vector3.up, hit.normal) > 45f && Vector3.Angle(Vector3.up, hit.normal) < 135f)
            {
                // Стена обнаружена, но не сбрасываем wallNormal, если активен кулдаун
                if (wallJumpCooldownTimer <= 0)
                {
                    wallNormal = hit.normal;
                }
            }
        }
    }

    void HandleMovement()
    {
        Vector2 input = movementAction.action.ReadValue<Vector2>();
        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
        
        // 1. Обработка импульса от Wall Jump
        if (wallJumpMomentum.magnitude > 0.01f)
        {
            // Горизонтальная скорость игрока СТАНОВИТСЯ импульсом отскока
            velocity.x = wallJumpMomentum.x;
            velocity.z = wallJumpMomentum.z;

            // КОРРЕКЦИЯ ДВИЖЕНИЯ В ВОЗДУХЕ (по требованию)
            velocity.x += moveDirection.x * movementSpeed * airControlDuringWallJump;
            velocity.z += moveDirection.z * movementSpeed * airControlDuringWallJump;

            // Ослабляем импульс со временем
            wallJumpMomentum = Vector3.MoveTowards(wallJumpMomentum, Vector3.zero, momentumDecay * Time.deltaTime);
        } 
        else
        {
            // 2. Обычное движение
            wallJumpMomentum = Vector3.zero;
            velocity.x = moveDirection.x * movementSpeed;
            velocity.z = moveDirection.z * movementSpeed;
        }
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }

    void HandleJump()
    {
        if (!allowJump) return;

        // Расчет силы для обычного прыжка
        float jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);
        // Расчет силы для вертикального отскока (для плавного, но мощного вертикального импульса)
        float wallJumpVerticalImpulse = Mathf.Sqrt(wallJumpHeight * -2f * gravity);

        bool canJump = characterController.isGrounded || coyoteTimeCounter > 0;
        bool canWallJump = wallNormal != Vector3.zero && !characterController.isGrounded;
        
        if (jumpBufferCounter > 0)
        {
            if (canWallJump)
            {
                // Применяем ГОРИЗОНТАЛЬНЫЙ ИМПУЛЬС
                wallJumpMomentum = wallNormal * wallJumpForce; 
                
                // Устанавливаем ВЕРТИКАЛЬНУЮ СКОРОСТЬ
                velocity.y = wallJumpVerticalImpulse; 

                // *** АКТИВИРУЕМ КУЛДАУН ***
                wallJumpCooldownTimer = wallJumpCooldownTime; 
                
                // Сброс состояния
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
                wallNormal = Vector3.zero; // Сброс, чтобы отключить wallSlide
                
                Debug.Log("WALL JUMP!");
            }
            else if (canJump)
            {
                // ЛОГИКА ОБЫЧНОГО ПРЫЖКА
                velocity.y = jumpForce;
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
            }
        }
    }

    void ApplyGravity()
    {
        bool isSlidingOnWall = wallNormal != Vector3.zero && !characterController.isGrounded;
        bool isWallJumpCooldownActive = wallJumpCooldownTimer > 0;

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        // Ограничения и скольжение работают только если мы на стене И НЕ в кулдауне
        else if (isSlidingOnWall && !isWallJumpCooldownActive) 
        {
            // Применяем гравитацию, чтобы тянуть вниз
            velocity.y += gravity * Time.deltaTime; 
            
            // ОГРАНИЧЕНИЕ СКОРОСТИ
            velocity.y = Mathf.Clamp(velocity.y, wallSlideSpeed, 0f); 
            
            // Сбрасываем горизонтальное движение, если оно не вызвано импульсом отскока
            if (wallJumpMomentum.magnitude < 0.01f)
            {
                 velocity.x = 0;
                 velocity.z = 0;
            }
        }
        else
        {
            // Стандартная гравитация (включая прыжок и кулдаун отскока)
            velocity.y += gravity * Time.deltaTime;
        }
    }
    
    // --- ВИЗУАЛИЗАЦИЯ ДЛЯ ОТЛАДКИ (GIZMOS) ---
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && characterController != null && wallNormal != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + wallNormal * 1.5f);
        }
    }
}