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

    // --- Jump & Wall Jump ---
    [Header("Jump")]
    [SerializeField] private bool allowJump = true;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallJumpForce = 7f; // Сила горизонтального отскока
    [SerializeField] private float wallCheckDistance = 0.6f; // Дистанция проверки стены (должна быть > радиуса контроллера)
    [SerializeField] private string wallTag = "Wall"; // ТЕГ, который должен быть назначен стенам
    [SerializeField] private float wallSlideSpeed = -1f;

    // --- Gravity ---
    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    // --- Приватные переменные состояния ---
    private CharacterController characterController;
    private Vector3 velocity;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private bool wasGroundedLastFrame;
    private Vector3 wallNormal = Vector3.zero; // Нормаль обнаруженной стены

    // НОВЫЕ ПОЛЯ ДЛЯ ПЛАВНОГО ОТСКОКА
    private Vector3 wallJumpMomentum = Vector3.zero; // Текущий импульс отскока
    [SerializeField] private float momentumDecay = 10f; // Скорость ослабления импульса

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
        
        // Перемещение персонажа
        characterController.Move(velocity * Time.deltaTime);
        wasGroundedLastFrame = characterController.isGrounded;
    }

    void UpdateTimers()
    {
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;
        
        if (characterController.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            wallNormal = Vector3.zero; // Сброс нормали при касании земли
        }
        else if (wasGroundedLastFrame)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    // --- ОБНАРУЖЕНИЕ СТЕНЫ ПО ТЕГУ ---
    void HandleWallCheck()
    {
        wallNormal = Vector3.zero;

        if (characterController.isGrounded) return;

        RaycastHit hit;
        Vector3 origin = transform.position;
        float radius = characterController.radius;
        
        // Определяем направление проверки (используем направление взгляда, если нет движения)
        Vector3 checkDirection = transform.forward; 

        Vector2 input = movementAction.action.ReadValue<Vector2>();
        if (input.magnitude > 0.1f)
        {
             // Если игрок движется, проверяем в направлении его движения
             Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
             checkDirection = moveDirection.normalized;
        }

        // SphereCast БЕЗ LayerMask (проверяет все)
        // Physics.AllLayers (по умолчанию)
        if (Physics.SphereCast(origin, radius, checkDirection, out hit, wallCheckDistance)) 
        {
            // *** ПРОВЕРКА ПО ТЕГУ ***
            if (!hit.collider.CompareTag(wallTag)) 
            {
                return;
            }
            
            // Проверка: Стена должна быть вертикальной
            // Угол > 45 и < 135 гарантирует вертикальную стену, не пол и не потолок
            if (Vector3.Angle(Vector3.up, hit.normal) > 45f && Vector3.Angle(Vector3.up, hit.normal) < 135f)
            {
                wallNormal = hit.normal;
                
                // Сбрасываем горизонтальную скорость для "прилипания"
                velocity.x = 0;
                velocity.z = 0;
            }
        }
    }

    void HandleMovement()
    {
        Vector2 input = movementAction.action.ReadValue<Vector2>();
        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
        
        // 1. Управление (Movement)
        // Если есть активный импульс от стены, игрок не может сразу пересилить его
        float controlModifier = (wallJumpMomentum.magnitude > 0.1f) ? 0.2f : 1f; // Снижаем контроль при отскоке
        
        velocity.x = Mathf.Lerp(velocity.x, moveDirection.x * movementSpeed, controlModifier);
        velocity.z = Mathf.Lerp(velocity.z, moveDirection.z * movementSpeed, controlModifier);


        // 2. Применение и ослабление Импульса (Momentum)
        if (wallJumpMomentum.magnitude > 0.01f)
        {
            // Применяем импульс к текущей горизонтальной скорости
            velocity.x += wallJumpMomentum.x * Time.deltaTime;
            velocity.z += wallJumpMomentum.z * Time.deltaTime;

            // Ослабляем импульс со временем (плавное затухание)
            wallJumpMomentum = Vector3.Lerp(wallJumpMomentum, Vector3.zero, momentumDecay * Time.deltaTime);
        } 
        // Если импульс почти нулевой, убедимся, что его нет
        else if (wallJumpMomentum.magnitude != 0)
        {
            wallJumpMomentum = Vector3.zero;
        }
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }

    void HandleJump()
    {
        if (!allowJump) return;

        // *** РАСЧЕТ ОДИН РАЗ В НАЧАЛЕ ***
        float jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);

        bool canJump = characterController.isGrounded || coyoteTimeCounter > 0;
        bool canWallJump = wallNormal != Vector3.zero && !characterController.isGrounded;

        if (jumpBufferCounter > 0)
        {
            if (canWallJump)
            {
                // ЛОГИКА ОТСКОКА ОТ СТЕНЫ

                // Применяем горизонтальный импульс
                wallJumpMomentum = wallNormal * wallJumpForce;

                // Устанавливаем вертикальную скорость
                velocity.y = jumpForce;

                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
                wallNormal = Vector3.zero;
            }
            else if (canJump)
            {
                // ЛОГИКА ОБЫЧНОГО ПРЫЖКА
                velocity.y = jumpForce; // <-- jumpForce доступен здесь
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
            }
        }
    }
    
    void ApplyGravity()
    {
        // Условие скольжения по стене: есть нормаль стены И игрок не на земле
        bool isSlidingOnWall = wallNormal != Vector3.zero && !characterController.isGrounded;

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else if (isSlidingOnWall)
        {
            // Применяем гравитацию, чтобы тянуть игрока вниз
            velocity.y += gravity * Time.deltaTime; 
            
            // *** КОРРЕКЦИЯ: Использование Clamp ***
            // Ограничиваем вертикальную скорость: 
            // 1. Минимум: wallSlideSpeed (например, -1f)
            // 2. Максимум: 0f (запрет движения вверх вдоль стены)
            velocity.y = Mathf.Clamp(velocity.y, wallSlideSpeed, 0f); 
        }
        else
        {
            // Применяем стандартную гравитацию
            velocity.y += gravity * Time.deltaTime;
        }
    }
    
    // --- ВИЗУАЛИЗАЦИЯ ДЛЯ ОТЛАДКИ ---
    private void OnDrawGizmos()
    {
        // Проверяем, что компонент и игра запущены
        if (Application.isPlaying && characterController != null && wallNormal != Vector3.zero)
        {
            // Рисуем зеленую линию, показывающую нормаль (направление отскока)
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + wallNormal * 1.5f);
        }
        else if (characterController != null && !characterController.isGrounded)
        {
            // Визуализируем дистанцию, на которой ищется стена (желтая сфера)
            Gizmos.color = Color.yellow;
            Vector3 checkDirection = transform.forward;
            Vector2 input = movementAction.action.ReadValue<Vector2>();
            if (input.magnitude > 0.1f)
            {
                 Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
                 checkDirection = moveDirection.normalized;
            }

            Vector3 origin = transform.position;
            float radius = characterController.radius;
            Gizmos.DrawWireSphere(origin + checkDirection * wallCheckDistance, radius);
        }
    }
}