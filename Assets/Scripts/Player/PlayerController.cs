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
    [SerializeField] private CooldownBarUI dashCooldownBarUI;
    [SerializeField] private CooldownBarUI slideCooldownBarUI;
    [SerializeField] private CooldownBarUI wallJumpCooldownBarUI;

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
    [SerializeField] private float dashBaseCooldown = 5.0f;

    [Header("Slide settings")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideSpeed = 12f;
    [SerializeField] private float slideFriction = 10f;
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private float cameraSlideOffset = -0.5f;
    [SerializeField] private float heightLerpSpeed = 10f;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float slideBaseCooldown = 5.0f;

    [Header("Rebound settings")]
    [SerializeField] private float wallCheckDistance = 0.6f;
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private float wallSlideSpeed = -1f;
    [SerializeField] private float wallJumpHeight = 3f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallJumpMomentumDecay = 2f;
    [SerializeField] private float wallJumpBaseCooldownTime = 5.0f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    // Cooldown Systems
    private CooldownSystem dashCooldownSystem;
    private CooldownSystem slideCooldownSystem;
    private CooldownSystem wallJumpCooldownSystem;

    // Private Variables
    private CharacterController characterController;
    private Vector3 velocity;
    
    // Timers
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    
    // States
    private bool wasGroundedLastFrame;
    private Vector3 moveDirection;
    
    // Dash & General Momentum
    private Vector3 bufferMoveDir;
    private bool hasMomentum;
    private Vector3 momentumVelocity;
    private float momentumTimer;

    // Wall Logic
    private Vector3 wallNormal = Vector3.zero;
    private Vector3 wallJumpMomentum = Vector3.zero;
    private bool hasJumpedFromWall = false;

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

        // Initialize Cooldown Systems
        dashCooldownSystem = new CooldownSystem(dashBaseCooldown);
        slideCooldownSystem = new CooldownSystem(slideBaseCooldown);
        wallJumpCooldownSystem = new CooldownSystem(wallJumpBaseCooldownTime);

        dashCooldownBarUI.cooldownSystem = dashCooldownSystem;
        slideCooldownBarUI.cooldownSystem = slideCooldownSystem;
        wallJumpCooldownBarUI.cooldownSystem = wallJumpCooldownSystem;
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
        // Update cooldown systems
        dashCooldownSystem.UpdateCooldown(Time.deltaTime);
        slideCooldownSystem.UpdateCooldown(Time.deltaTime);
        wallJumpCooldownSystem.UpdateCooldown(Time.deltaTime);

        dashCooldownBarUI.cooldownSystem = dashCooldownSystem;
        slideCooldownBarUI.cooldownSystem = slideCooldownSystem;
        wallJumpCooldownBarUI.cooldownSystem = wallJumpCooldownSystem;

        UpdateTimers();
        HandleWallCheck();

        if (bufferMoveDir == Vector3.zero)
        {
            HandleMovement();
        }

        HandleJump();
        HandleSlide();
        ApplyGravity();

        wasGroundedLastFrame = characterController.isGrounded;

        Vector3 displacement = velocity * Time.deltaTime;
        CollisionFlags collisionFlags = characterController.Move(displacement);

        if ((collisionFlags & CollisionFlags.Above) != 0)
        {
            if (velocity.y > 0)
            {
                velocity.y = 0;
            }
        }
    }

    void ApplyGravity()
    {
        bool isSlidingOnWall = wallNormal != Vector3.zero && !characterController.isGrounded;

        // Пропускаем гравитацию, если сейчас деш
        if (bufferMoveDir != Vector3.zero) return;

        // 1. На земле
        if (characterController.isGrounded && velocity.y <= 0)
        {
            velocity.y = -2f; // Прижимаем к земле
            // Сбрасываем флаг отскока при приземлении
            hasJumpedFromWall = false;
        }
        // 2. На стене (Wall Slide) - если не в рывке и не в кулдауне отскока
        else if (isSlidingOnWall && !wallJumpCooldownSystem.IsOnCooldown && !hasJumpedFromWall)
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

    // Добавляем метод для обработки столкновений
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Проверяем, столкнулись ли мы сверху
        if (Vector3.Angle(hit.normal, Vector3.down) < 45f) // угол между нормалью и вектором вниз
        {
            if (velocity.y > 0) // если движемся вверх
            {
                velocity.y = 0; // обнуляем вертикальную скорость
            }
        }
        // Проверяем, столкнулись ли мы снизу (например, при прыжке под потолок)
        else if (Vector3.Angle(hit.normal, Vector3.up) < 45f)
        {
            if (velocity.y < 0) // если движемся вниз
            {
                velocity.y = 0; // обнуляем вертикальную скорость
            }
        }
    }

    void UpdateTimers()
    {
        // Jump Buffering
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        // Coyote Time
        if (characterController.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            // Сбрасываем инерцию от стены, если коснулись земли
            wallJumpMomentum = Vector3.zero;
            wallNormal = Vector3.zero;
            // Сбрасываем флаг отскока при приземлении
            hasJumpedFromWall = false;
        }
        else if (wasGroundedLastFrame)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

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

    // --- WALL CHECK LOGIC ---
    void HandleWallCheck()
    {
        wallNormal = Vector3.zero;

        // Мы не ищем стену, если мы на земле (чтобы не липнуть к плинтусам)
        if (characterController.isGrounded) return;

        // Не проверяем стену, если уже использовали отскок после прыжка
        if (hasJumpedFromWall) return;

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
                if (!wallJumpCooldownSystem.IsOnCooldown)
                {
                    wallNormal = hit.normal;
                }
            }
        }
    }

    // --- MOVEMENT LOGIC ---
        void HandleMovement()
    {
        Vector2 input = movementAction.action.ReadValue<Vector2>();
        moveDirection = transform.right * input.x + transform.forward * input.y;

        // Приоритет 1: Инерция от Wall Jump (Script 2)
        if (wallJumpMomentum.magnitude > 0.1f)
        {
            // Вычисляем, как давно произошёл отскок (через оставшееся время импульса)
            float timeSinceJump = wallJumpMomentumDecay - Vector3.Distance(wallJumpMomentum, Vector3.zero) / wallJumpMomentumDecay;
            float normalizedTime = timeSinceJump / wallJumpMomentumDecay;
            
            // Чем дольше прошло, тем больше контроль (от 0% до 100% за wallJumpMomentumDecay секунд)
            float controlFactor = normalizedTime;
            controlFactor = Mathf.Clamp01(controlFactor); // Ограничиваем от 0 до 1

            // Применяем ввод с плавающим контролем
            if (input.magnitude > 0.1f)
            {
                Vector3 inputWorldDir = moveDirection.normalized;
                Vector3 targetVelocity = inputWorldDir * movementSpeed;

                // Плавное изменение текущей скорости к целевой, с изменяющимся коэффициентом
                float lerpSpeed = 4f * controlFactor; // Чем дольше прошло, тем больше влияние ввода
                velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, lerpSpeed * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, lerpSpeed * Time.deltaTime);
            }
            else
            {
                // Без ввода: плавный переход от импульса к нулю
                velocity.x = Mathf.Lerp(velocity.x, 0f, (1f - controlFactor) * 3f * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, 0f, (1f - controlFactor) * 3f * Time.deltaTime);
            }
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
            // Горизонтальная скорость (x/z)
            Vector3 currentHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

            // Управление на земле — с ускорением и замедлением
            if (characterController.isGrounded && !isSliding)
            {
                // Если есть ввод направления
                if (input.magnitude > 0.1f)
                {
                    Vector3 targetVelocity = moveDirection * movementSpeed;

                    // Плавное изменение скорости к целевой (ускорение)
                    velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, 10f * Time.deltaTime);
                    velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, 10f * Time.deltaTime);
                }
                else
                {
                    // Нет ввода — плавное замедление (скольжение)
                    velocity.x = Mathf.Lerp(velocity.x, 0f, 8f * Time.deltaTime);
                    velocity.z = Mathf.Lerp(velocity.z, 0f, 8f * Time.deltaTime);
                }
            }
            else
            {
                // В воздухе — изменение направления с ограниченной скоростью
                if (input.magnitude > 0.1f)
                {
                    Vector3 inputWorldDir = moveDirection.normalized;
                    Vector3 targetVelocity = inputWorldDir * movementSpeed;

                    // Плавное изменение текущей скорости к целевой (меньший коэффициент для "тяжести" в воздухе)
                    velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, 4f * Time.deltaTime);
                    velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, 4f * Time.deltaTime);
                }
                // Если нет ввода в воздухе, скорость остается неизменной (или можно добавить большее затухание)
            }
        }
    }

    // --- JUMP LOGIC ---
    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
    }

    void HandleJump()
    {
        if (!allowJump) return;

        bool isWallSliding = wallNormal != Vector3.zero && !characterController.isGrounded && !hasJumpedFromWall;
        bool canGroundJump = characterController.isGrounded || coyoteTimeCounter > 0;

        if (jumpBufferCounter > 0)
        {
            // 1. WALL JUMP (Приоритет выше, если мы в воздухе у стены)
            if (isWallSliding && !wallJumpCooldownSystem.IsOnCooldown)
            {
                wallJumpCooldownSystem.ActivateCooldown();
                // Текущая горизонтальная скорость перед отскоком
                Vector3 currentHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
                
                // Отражаем горизонтальную скорость относительно нормали стены
                Vector3 reflectedHorizontalVelocity = Vector3.Reflect(currentHorizontalVelocity, wallNormal);
                
                // Вертикальная сила (только вверх)
                float wallJumpVForce = Mathf.Sqrt(wallJumpHeight * -2f * gravity);
                
                // Применяем силу отскока к отраженной горизонтальной скорости
                Vector3 wallJumpHorizontalVelocity = reflectedHorizontalVelocity.normalized * wallJumpForce;
                
                // Итоговая скорость: горизонтальная сила отскока + вертикальная сила прыжка
                velocity = new Vector3(wallJumpHorizontalVelocity.x, wallJumpVForce, wallJumpHorizontalVelocity.z);

                // Горизонтальный импульс (для последующего контроля)
                wallJumpMomentum = wallJumpHorizontalVelocity;

                // Устанавливаем флаг, что отскок был использован
                hasJumpedFromWall = true;

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
                float baseJumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);

                float totalJumpForce = baseJumpForce;
                velocity.y = totalJumpForce;
                
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;

                // Прыжок прерывает подкат
                if (isSliding) EndSlide();
            }
        }
    }

    void OnDashPerformed(InputAction.CallbackContext context)
    {
        // Проверяем кулдаун деша
        if (dashCooldownSystem.IsOnCooldown) return; 
        dashCooldownSystem.ActivateCooldown();

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
        if (slideCooldownSystem.IsOnCooldown) return;
        slideCooldownSystem.ActivateCooldown();

        Vector3 currentHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        
        // Подкат возможен только на земле, если есть скорость и прошел кулдаун
        if (characterController.isGrounded && !isSliding && currentHorizontalVelocity.magnitude > 0.1f && slideCooldownSystem.IsOnCooldown)
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
    
    void OnCollisionEnter(Collision collision)
    {
        if (bufferMoveDir != Vector3.zero)
            velocity = Vector3.zero;
    }
}