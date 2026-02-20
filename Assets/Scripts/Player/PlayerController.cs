using System.Collections;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Localization")]
    public string acitveLocale;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    [Header("Input")]
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference switchLocaleAction;
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
    [SerializeField] private float slideSpeedBonus = 8f;
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private float cameraSlideOffset = -0.5f;
    [SerializeField] private float heightLerpSpeed = 10f;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float slideBaseCooldown = 5.0f;

    [Header("WallJump settings")]
    [SerializeField] private float wallCheckDistance = 0.6f;
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private float wallSlideSpeed = -1f;
    [SerializeField] private float wallJumpHeight = 3f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallJumpMomentumDecay = 2f;
    [SerializeField] private float wallJumpBaseCooldownTime = 5.0f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    // === FOV CONTROL ===
    [Header("Camera FOV Settings")]
    [SerializeField] private float baseFOV = 60f;              // Базовый FOV из настроек игрока
    [SerializeField] private float fovMultiplier = 2f;         // Множитель: maxFOV = baseFOV * fovMultiplier
    [SerializeField] private float fovSafeZoneSpeed = 5f;      // Ниже этой скорости FOV не меняется
    [SerializeField] private float maxSpeedForFOV = 30f;       // При этой скорости достигается max FOV

    // Private References
    private Camera mainCamera;

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
    private Vector3 previousPosition;
    private float previousSpeed;
    
    // Dash state — заменён bufferMoveDir на флаг
    private bool isDashing;

    // Wall Logic
    private Vector3 wallNormal = Vector3.zero;
    private Vector3 wallJumpMomentum = Vector3.zero;
    private bool hasJumpedFromWall = false;

    // Slide Logic
    private bool isSliding;
    private float slideTimer;
    private Vector3 slideBoost = Vector3.zero;
    private float originalHeight;
    private Vector3 originalCameraLocalPos;

// anim
    private Vector3 hipsOffset;

    public float CurrentSpeed
    {
        get
        {
            // Получаем фактическое перемещение за кадр
            Vector3 currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
            // Убираем вертикальную составляющую, так как нас интересует только горизонтальная скорость
            currentVelocity.y = 0;
            
            float currentSpeed = currentVelocity.magnitude;
            
            // Сглаживаем скорость от кадра к кадру
            if (Time.deltaTime > 0)
            {
                currentSpeed = Mathf.Lerp(previousSpeed, currentSpeed, Time.deltaTime * 10f);
            }
            
            previousSpeed = currentSpeed;
            return currentSpeed;
        }
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;

        if (cameraRoot != null)
        {
            originalCameraLocalPos = cameraRoot.localPosition;

            // Try to get Camera — directly or in children
            if (!cameraRoot.TryGetComponent<Camera>(out mainCamera))
            {
                mainCamera = cameraRoot.GetComponentInChildren<Camera>();
            }

            // Если не нашли — попытка на самом Player (на случай, если cameraRoot не задан)
            if (mainCamera == null && !TryGetComponent<Camera>(out mainCamera))
            {
                mainCamera = GetComponentInChildren<Camera>();
            }

            if (mainCamera != null)
            {
                // Инициализируем FOV
                mainCamera.fieldOfView = baseFOV;
            }
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
        if (movementAction?.action != null)
        {
            movementAction.action.Enable();
        }
        if (jumpAction?.action != null)
        {
            jumpAction.action.Enable();
            jumpAction.action.performed += OnJumpPerformed;
        }
        if (dashAction?.action != null)
        {
            dashAction.action.Enable();
            dashAction.action.performed += OnDashPerformed;
        }
        if (slideAction?.action != null)
        {
            slideAction.action.Enable();
            slideAction.action.performed += OnSlidePerformed;
        }
        if (switchLocaleAction?.action != null)
        {
            switchLocaleAction.action.Enable();
            switchLocaleAction.action.performed += OnLocaleSwitch;
        }
    }

    void OnDisable()
    {
        if (movementAction?.action != null)
        {
            movementAction.action.Disable();
        }
        if (jumpAction?.action != null)
        {
            jumpAction.action.performed -= OnJumpPerformed;
            jumpAction.action.Disable();
        }
        if (dashAction?.action != null)
        {
            dashAction.action.performed -= OnDashPerformed;
            dashAction.action.Disable();
        }
        if (slideAction?.action != null)
        {
            slideAction.action.performed -= OnSlidePerformed;
            slideAction.action.Disable();
        }
        if (switchLocaleAction?.action != null)
        {
            switchLocaleAction.action.performed -= OnLocaleSwitch;
            switchLocaleAction.action.Disable();
        }
    }

    void Update()
    {
        // Обновляем предыдущую позицию в начале кадра
        previousPosition = transform.position;

        // Update cooldown systems
        dashCooldownSystem.UpdateCooldown(Time.deltaTime);
        slideCooldownSystem.UpdateCooldown(Time.deltaTime);
        wallJumpCooldownSystem.UpdateCooldown(Time.deltaTime);

        // Fallback: Direct keyboard input for actions if InputActionReferences are not set
        if (jumpAction?.action == null && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        if (dashAction?.action == null && Keyboard.current != null && Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            OnDashPerformed(default);
        }
        if (slideAction?.action == null && Keyboard.current != null && Keyboard.current.leftCtrlKey.wasPressedThisFrame)
        {
            OnSlidePerformed(default);
        }
        if (switchLocaleAction?.action == null && Keyboard.current != null && Keyboard.current.leftAltKey.wasPressedThisFrame)
        {
            Debug.Log("Tab key pressed");
            OnLocaleSwitch(default);
        }
        UpdateTimers();
        HandleWallCheck();
        _animator.SetFloat("Sped",CurrentSpeed);

        if (!isDashing)
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

        // === FOV UPDATE ===
        UpdateCameraFOV();
    }

    void UpdateCameraFOV()
    {
        if (mainCamera == null) return;

        float speed = CurrentSpeed;

        float fov;
        if (speed <= fovSafeZoneSpeed)
        {
            fov = baseFOV;
        }
        else
        {
            float clampedSpeed = Mathf.Clamp(speed, fovSafeZoneSpeed, maxSpeedForFOV);
            float t = (clampedSpeed - fovSafeZoneSpeed) / (maxSpeedForFOV - fovSafeZoneSpeed);
            fov = Mathf.Lerp(baseFOV, baseFOV * fovMultiplier, t);
        }

        // Плавное изменение FOV
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, fov, 10f * Time.deltaTime);
    }

    void ApplyGravity()
    {
        bool isSlidingOnWall = wallNormal != Vector3.zero && !characterController.isGrounded;

        // Пропускаем гравитацию, если сейчас деш
        if (isDashing) return;

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
        else if (coyoteTimeCounter > 0)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // General Momentum Decay (Dash exit / Slide exit)
        // Не используется, так как hasMomentum не устанавливается в true
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
        Vector2 input = Vector2.zero;
        if (movementAction?.action != null)
        {
            input = movementAction.action.ReadValue<Vector2>();
        }
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
        Vector2 input = Vector2.zero;
        if (movementAction?.action != null)
        {
            input = movementAction.action.ReadValue<Vector2>();
        }
        else
        {
            // Fallback: Direct keyboard input if InputActionReference is not set
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                    input.y = 1;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                    input.y = -1;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                    input.x = -1;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                    input.x = 1;
            }
        }

        // --- Изменения для слайда ---
        if (isSliding)
        {
            // Принудительно устанавливаем "движение вперед", как при зажатой W
            input.y = 1f;
            // Блокируем движение назад (S)
            if (input.y < 0f) input.y = 1f;
        }
        // ----------------------------

        moveDirection = transform.right * input.x + transform.forward * input.y;

        // Приоритет 0: Инерция от слайда (если есть)
        if (slideBoost.magnitude > 0.1f)
        {
            // Применяем буст к скорости
            velocity += slideBoost * Time.deltaTime;

            // Если ввод направлен против движения, уменьшаем буст быстрее
            Vector3 inputWorldDir = transform.right * input.x + transform.forward * input.y;

            if (Vector3.Dot(inputWorldDir.normalized, slideBoost.normalized) < -0.5f)
            {
                slideBoost = Vector3.Lerp(slideBoost, Vector3.zero, 5f * Time.deltaTime);
            }
        }
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
        // Не используется, так как hasMomentum не устанавливается в true
        // Приоритет 3: Обычное движение
        else
        {
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
        SwitchLocale();

        
    }
    private void OnLocaleSwitch(InputAction.CallbackContext context)
    {
        SwitchLocale();
    }
    void SwitchLocale()
    {
        switch(acitveLocale)
        {
            case "en":
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                break;
            case "ru":
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
                break;
        }

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
                if (slideBoost.magnitude > 0.1f) slideBoost = Vector3.zero;
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

        isDashing = true;

        velocity = Vector3.zero;

        Vector3 dashDirection;

        if(characterController.isGrounded)
        {
            dashDirection = moveDirection.normalized;
        }
        else
        {
            Vector2 inputVector = Vector2.zero;
            if (movementAction?.action != null)
            {
                inputVector = movementAction.action.ReadValue<Vector2>();
            }

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
        Vector2 input = Vector2.zero;
        if (movementAction?.action != null)
        {
            input = movementAction.action.ReadValue<Vector2>();
        }
        Vector3 expectedVelocity = transform.right * input.x + transform.forward * input.y;

        velocity = expectedVelocity * movementSpeed;

        // Сбрасываем флаг деша
        isDashing = false;
    }

    // --- SLIDE LOGIC (Script 1) ---
    private void OnSlidePerformed(InputAction.CallbackContext context)
    {
        if (slideCooldownSystem.IsOnCooldown) return;

        Vector3 currentHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        
        // Подкат возможен только на земле, если есть скорость и прошел кулдаун
        if (characterController.isGrounded && !isSliding)
        {
            slideCooldownSystem.ActivateCooldown();
            StartSlide();
        }
    }

    private void HandleSlide()
    {
        if (!isSliding)
        {
            // Плавное затухание слайд-буста на земле
            if (characterController.isGrounded && slideBoost.magnitude > 0.1f)
            {
                slideBoost = Vector3.Lerp(slideBoost, Vector3.zero, 2f * Time.deltaTime);
            }
            
            LerpHeightAndCameraBack();
            return;
        }

        UpdateSlide();
        if (slideTimer <= 0 || IsBlockedAbove())
        {
            EndSlide();
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;

        // Получаем направление взгляда камеры, проецируем его на горизонтальную плоскость
        Vector3 lookDir = cameraRoot.transform.forward;
        lookDir.y = 0f; // проецируем на ground plane
        if (lookDir.magnitude < 0.01f) // если почти вертикальный взгляд (вверх/вниз), используем forward персонажа как fallback
        {
            lookDir = transform.forward;
            lookDir.y = 0f;
        }
        lookDir.Normalize();

        // Формируем slideBoost в направлении взгляда (по горизонтали)
        slideBoost = lookDir * slideSpeedBonus;

        // Немедленно применяем буст к скорости (для мгновенного эффекта)
        velocity += slideBoost;

        // Устанавливаем высоту и камеру
        characterController.height = slideHeight;
        if (cameraRoot != null)
        {
            Vector3 targetCameraPos = originalCameraLocalPos + new Vector3(0f, cameraSlideOffset, 0f);
            cameraRoot.localPosition = targetCameraPos; // мгновенная установка, либо плавная — как у вас сейчас
        }

        // Отключаем другие виды инерции
        // hasMomentum = false; — не используется
        // momentumTimer = 0f; — не используется
    }

    private void UpdateSlide()
    {
        if (!(slideTimer < 0.1f && IsBlockedAbove()))
        {
            slideTimer -= Time.deltaTime;
        }

        // Плавное уменьшение высоты персонажа во время слайда
        characterController.height = Mathf.Lerp(characterController.height, slideHeight, heightLerpSpeed * Time.deltaTime);

        if (cameraRoot != null)
        {
            Vector3 targetCameraPos = originalCameraLocalPos + new Vector3(0f, cameraSlideOffset, 0f);
            cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, targetCameraPos, heightLerpSpeed * Time.deltaTime);
        }
    }

    private bool IsBlockedAbove()
    {
        Vector3 rayStart = transform.position + Vector3.up * (originalHeight - slideHeight) / 2f;
        float rayDistance = originalHeight / 2f + 0.1f;
        
        if (Physics.Raycast(rayStart, Vector3.up, rayDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }

    private void EndSlide()
    {
        // Только завершаем слайд, если можем встать
        if (!IsBlockedAbove())
        {
            isSliding = false;
        }
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

    public void AddExternalImpulse(Vector3 impulse)
    {
        velocity += impulse;
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
        isDashing = false;
        wallJumpMomentum = Vector3.zero;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;

        // Сброс инерции от других механик
        isDashing = false;
        wallJumpMomentum = Vector3.zero;

        // --- ИСПРАВЛЕНИЕ ПРЫЖКА ---
        // Принудительно обнуляем таймеры, чтобы койот-тайм не сработал
        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
        
        // Говорим контроллеру, что в прошлом кадре мы НЕ были на земле.
        // Это предотвратит активацию койота в следующем кадре Update.
        wasGroundedLastFrame = false; 
    }
}