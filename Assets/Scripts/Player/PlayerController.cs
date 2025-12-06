using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference slideAction;

    [Header("Movement settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float momentumDecayTime = .05f;
    
    [Header("Jump settings")]
    [SerializeField] private bool allowJump = true;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Dash settings")]
    [SerializeField] private float dashSpeed = 200.0f;
    [SerializeField] private float dashDuration = .06f;

    [Header("Slide settings")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideSpeed = 12f;
    [SerializeField] private float slideFriction = 10f;
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private float cameraSlideOffset = -0.5f;
    [SerializeField] private float heightLerpSpeed = 10f;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float slideCooldown = 1.0f;

    [Header("Rebound settings")]
    [SerializeField] private float wallCheckDistance = 0.6f;
    [SerializeField] private string wallTag = "Wall"; // Убедитесь, что у стен есть этот тег!
    [SerializeField] private float wallSlideSpeed = -1f; // Скорость сползания
    [SerializeField] private float wallJumpHeight = 3f;      // Высота отскока
    [SerializeField] private float wallJumpForce = 10f;      // Сила отталкивания от стены
    [SerializeField] private float wallJumpMomentumDecay = 2f; // Скорость затухания импульса от стены
    [SerializeField] private float wallJumpCooldownTime = 0.2f; // Время, когда нельзя прилипнуть к стене после отскока
    [SerializeField] private float airControlDuringWallJump = 0.5f; // Управляемость в воздухе после отскока

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    // Private Variables
    private CharacterController characterController;
    private Vector3 velocity;
    
    // Timers
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float slideCooldownCounter;
    private float wallJumpCooldownTimer;
    
    // States
    private bool wasGroundedLastFrame;
    private Vector3 moveDirection;
    
    // Dash & General Momentum
    private Vector3 bufferMoveDir; // Сохраняет вектор движения до рывка
    private bool hasMomentum;
    private Vector3 momentumVelocity;
    private float momentumTimer;

    // Wall Logic
    private Vector3 wallNormal = Vector3.zero;
    private Vector3 wallJumpMomentum = Vector3.zero;

    // Slide Logic
    private bool isSliding;
    private float slideTimer;
    private Vector3 slideDirection;
    private float originalHeight;
    private Vector3 originalCameraLocalPos;

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
        HandleWallCheck();

        // Если мы не в состоянии рывка (Dash), управляем движением
        if (bufferMoveDir == Vector3.zero)
        {
            HandleMovement();
        }

        HandleJump();
        HandleSlide(); // Логика подката (только на земле)
        ApplyGravity();

        characterController.Move(velocity * Time.deltaTime);
        wasGroundedLastFrame = characterController.isGrounded;
    }

    void UpdateTimers()
    {
        // Jump Buffering
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        // Wall Jump Cooldown
        if (wallJumpCooldownTimer > 0) wallJumpCooldownTimer -= Time.deltaTime;

        // Coyote Time
        if (characterController.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            // Сбрасываем инерцию от стены, если коснулись земли
            wallJumpMomentum = Vector3.zero;
            wallNormal = Vector3.zero;
        }
        else if (wasGroundedLastFrame)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Slide Cooldown
        if (slideCooldownCounter > 0) slideCooldownCounter -= Time.deltaTime;

        // General Momentum Decay (Dash exit / Slide exit)
        if (hasMomentum && momentumTimer > 0)
        {
            momentumTimer -= Time.deltaTime;
            if (momentumTimer <= 0)
            {
                hasMomentum = false;
            }
        }
    }

    // --- WALL CHECK LOGIC (Script 2) ---
    void HandleWallCheck()
    {
        wallNormal = Vector3.zero;

        // Мы не ищем стену, если мы на земле (чтобы не липнуть к плинтусам)
        if (characterController.isGrounded) return;

        RaycastHit hit;
        Vector3 origin = transform.position;
        float radius = characterController.radius; // Используем радиус контроллера

        // Направление проверки зависит от ввода игрока
        Vector3 checkDirection = transform.forward;
        Vector2 input = movementAction.action.ReadValue<Vector2>();
        if (input.magnitude > 0.1f)
        {
            Vector3 inputDir = transform.right * input.x + transform.forward * input.y;
            checkDirection = inputDir.normalized;
        }

        if (Physics.SphereCast(origin, radius, checkDirection, out hit, wallCheckDistance))
        {
            // Проверяем тег (если нужен)
            if (!string.IsNullOrEmpty(wallTag) && !hit.collider.CompareTag(wallTag))
            {
                return;
            }

            // Проверяем угол (чтобы это была стена, а не пол или потолок)
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle > 70f && angle < 110f) // Слегка расширил углы
            {
                // Если кулдаун после отскока прошел, фиксируем стену
                if (wallJumpCooldownTimer <= 0)
                {
                    wallNormal = hit.normal;
                }
            }
        }
    }

    // --- MOVEMENT LOGIC (Merged) ---
    void HandleMovement()
    {
        Vector2 input = movementAction.action.ReadValue<Vector2>();
        moveDirection = transform.right * input.x + transform.forward * input.y;

        // Приоритет 1: Инерция от Wall Jump (Script 2)
        if (wallJumpMomentum.magnitude > 0.1f)
        {
            // Горизонтальная скорость определяется импульсом от стены
            velocity.x = wallJumpMomentum.x;
            velocity.z = wallJumpMomentum.z;

            // Добавляем немного контроля в воздухе
            velocity.x += moveDirection.x * movementSpeed * airControlDuringWallJump;
            velocity.z += moveDirection.z * movementSpeed * airControlDuringWallJump;

            // Затухание импульса стены
            wallJumpMomentum = Vector3.MoveTowards(wallJumpMomentum, Vector3.zero, wallJumpMomentumDecay * Time.deltaTime);
        }
        // Приоритет 2: Инерция после Dash или Slide (Script 1)
        else if (hasMomentum && momentumTimer > 0)
        {
            if (input.magnitude > 0.1f)
            {
                // Игрок пытается двигаться, плавно переходим от инерции к вводу
                Vector3 targetVelocity = moveDirection * movementSpeed;
                float t = 1f - (momentumTimer / momentumDecayTime);
                velocity.x = Mathf.Lerp(momentumVelocity.x, targetVelocity.x, t);
                velocity.z = Mathf.Lerp(momentumVelocity.z, targetVelocity.z, t);
            }
            else
            {
                // Игрок не жмет кнопки, просто замедляем инерцию
                float decayFactor = momentumTimer / momentumDecayTime;
                velocity.x = momentumVelocity.x * decayFactor;
                velocity.z = momentumVelocity.z * decayFactor;
            }
        }
        // Приоритет 3: Обычное движение
        else
        {
            velocity.x = moveDirection.x * movementSpeed;
            velocity.z = moveDirection.z * movementSpeed;
        }
    }

    // --- JUMP LOGIC (Merged) ---
    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }

    void HandleJump()
    {
        if (!allowJump) return;

        bool isWallSliding = wallNormal != Vector3.zero && !characterController.isGrounded;
        bool canGroundJump = characterController.isGrounded || coyoteTimeCounter > 0;

        if (jumpBufferCounter > 0)
        {
            // 1. WALL JUMP (Приоритет выше, если мы в воздухе у стены)
            if (isWallSliding)
            {
                // Вектор отскока: от стены + вверх
                Vector3 wallJumpDir = (wallNormal + Vector3.up).normalized;
                
                // Вертикальная сила
                float wallJumpVForce = Mathf.Sqrt(wallJumpHeight * -2f * gravity);
                velocity.y = wallJumpVForce;

                // Горизонтальный импульс
                wallJumpMomentum = wallNormal * wallJumpForce;

                // Кулдаун, чтобы сразу не прилипнуть обратно
                wallJumpCooldownTimer = wallJumpCooldownTime;

                // Сброс
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
                wallNormal = Vector3.zero; 
                
                // Если был подкат, прерываем его
                if (isSliding) EndSlide();
            }
            // 2. GROUND JUMP
            else if (canGroundJump)
            {
                float jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);
                velocity.y = jumpForce;
                
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;

                // Прыжок прерывает подкат
                if (isSliding) EndSlide();
            }
        }
    }

    void OnDashPerformed(InputAction.CallbackContext context)
    {
        bufferMoveDir = velocity;

        Vector3 dashDirection;

        if(characterController.isGrounded)
        {
            dashDirection = moveDirection.normalized;
        }
        else
        {
            Vector2 inputVector = movementAction.action.ReadValue<Vector2>();
            
            if (inputVector.magnitude == 0)
            {
                dashDirection = cameraRoot.transform.forward;
            }
            else
            {
                Vector3 inputWorldDirection = inputVector.x * cameraRoot.transform.right + inputVector.y * cameraRoot.transform.forward;
                dashDirection = inputWorldDirection.normalized;
            }
            
            velocity.y = 0;
        }

        velocity = dashDirection.normalized * dashSpeed;

        StartCoroutine(ReturnToNormal(dashDuration));
    }

    IEnumerator ReturnToNormal(float dashDuration)
    {
        yield return new WaitForSeconds(dashDuration);

        // Возвращаем горизонтальную скорость до состояния до деша
        velocity = new Vector3(bufferMoveDir.x, 0, bufferMoveDir.z);

        // Сбрасываем флаг деша
        bufferMoveDir = Vector3.zero;
    }

    // --- SLIDE LOGIC (Script 1) ---
    private void OnSlidePerformed(InputAction.CallbackContext context)
    {
        Vector3 currentHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        
        // Подкат возможен только на земле, если есть скорость и прошел кулдаун
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

        // Фиксируем направление подката
        Vector3 currentHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        slideDirection = currentHorizontalVelocity.normalized;

        // Отключаем инерцию бега, включаем логику подката
        hasMomentum = false;
        momentumTimer = 0f;
    }

    private void UpdateSlide()
    {
        slideTimer -= Time.deltaTime;

        // Расчет скорости подката с трением
        float currentSlideSpeed = slideSpeed - (slideFriction * (slideDuration - slideTimer));
        currentSlideSpeed = Mathf.Max(currentSlideSpeed, movementSpeed * 0.5f); // Не замедляться до 0 совсем

        velocity.x = slideDirection.x * currentSlideSpeed;
        velocity.z = slideDirection.z * currentSlideSpeed;

        // Изменение высоты и камеры
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

        // Переносим скорость подката в инерцию
        momentumVelocity = new Vector3(velocity.x, 0f, velocity.z);
        hasMomentum = true;
        momentumTimer = momentumDecayTime;
    }

    private void LerpHeightAndCameraBack()
    {
        // Плавный возврат высоты и камеры
        if (Mathf.Abs(characterController.height - originalHeight) > 0.01f)
        {
            characterController.height = Mathf.Lerp(characterController.height, originalHeight, heightLerpSpeed * Time.deltaTime);
        }

        if (cameraRoot != null && Vector3.Distance(cameraRoot.localPosition, originalCameraLocalPos) > 0.01f)
        {
            cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, originalCameraLocalPos, heightLerpSpeed * Time.deltaTime);
        }
    }

    // --- GRAVITY LOGIC (Merged) ---
    void ApplyGravity()
    {
        bool isSlidingOnWall = wallNormal != Vector3.zero && !characterController.isGrounded;
        
        // Пропускаем гравитацию, если сейчас деш
        if (bufferMoveDir != Vector3.zero) return;

        // 1. На земле
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Прижимаем к земле
        }
        // 2. На стене (Wall Slide) - если не в рывке и не в кулдауне отскока
        else if (isSlidingOnWall && wallJumpCooldownTimer <= 0)
        {
            velocity.y += gravity * Time.deltaTime;
            // Ограничиваем скорость падения (скольжение)
            if (velocity.y < wallSlideSpeed)
            {
                velocity.y = wallSlideSpeed;
            }
        }
        // 3. В воздухе
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (bufferMoveDir != Vector3.zero)
            velocity = Vector3.zero;
    }
}