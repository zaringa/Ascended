using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // ================= HEADER: INPUT =================
    [Header("Input")]
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference slideAction;

    // ================= HEADER: MOVEMENT & MOMENTUM =================
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float momentumDecayTime = 0.5f; // Время затухания инерции (увеличил для импульсов)

    // ================= HEADER: JUMP =================
    [Header("Jump Settings")]
    [SerializeField] private bool allowJump = true;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    // ================= HEADER: DASH =================
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 50.0f; // 200 слишком много для CharacterController, 50 комфортнее
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;

    // ================= HEADER: SLIDE =================
    [Header("Slide Settings")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideSpeed = 12f;
    [SerializeField] private float slideFriction = 10f;
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private float cameraSlideOffset = -0.5f;
    [SerializeField] private float heightLerpSpeed = 10f;
    [SerializeField] private Transform cameraRoot; // Сюда вешаем объект камеры или pivot
    [SerializeField] private float slideCooldown = 1.0f;

    // ================= HEADER: WALL MECHANICS =================
    [Header("Wall Mechanics")]
    [SerializeField] private float wallCheckDistance = 0.6f;
    [SerializeField] private string wallTag = "Wall"; 
    [SerializeField] private float wallSlideSpeed = -1f;
    [SerializeField] private float wallJumpHeight = 3f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallJumpMomentumDecay = 1.5f; 
    [SerializeField] private float wallJumpCooldownTime = 0.2f;

    // ================= HEADER: GRAVITY =================
    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    // --- Private Variables ---
    private CharacterController characterController;
    private Vector3 velocity;
    
    // Timers
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float slideCooldownCounter;
    private float wallJumpCooldownTimer;
    private float dashCooldownCounter;
    
    // States
    private bool wasGroundedLastFrame;
    private Vector3 moveDirection;
    
    // Momentum System
    private Vector3 bufferMoveDir; // Флаг и хранилище скорости для деша
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

        // Блокируем управление движением во время деша
        if (bufferMoveDir == Vector3.zero)
        {
            HandleMovement();
        }

        HandleJump();
        HandleSlide(); 
        ApplyGravity();

        wasGroundedLastFrame = characterController.isGrounded;

        // Финальное применение движения
        CollisionFlags flags = characterController.Move(velocity * Time.deltaTime);
        HandleCollisionFlags(flags);
    }

    // Обработка ударов головой о потолок
    void HandleCollisionFlags(CollisionFlags flags)
    {
        if ((flags & CollisionFlags.Above) != 0)
        {
            if (velocity.y > 0) velocity.y = 0;
        }
    }

    void UpdateTimers()
    {
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;
        if (wallJumpCooldownTimer > 0) wallJumpCooldownTimer -= Time.deltaTime;
        if (dashCooldownCounter > 0) dashCooldownCounter -= Time.deltaTime;
        if (slideCooldownCounter > 0) slideCooldownCounter -= Time.deltaTime;

        if (characterController.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            wallJumpMomentum = Vector3.zero;
            wallNormal = Vector3.zero;
            hasJumpedFromWall = false;
        }
        else if (wasGroundedLastFrame)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Затухание общей инерции (включая внешний импульс)
        if (hasMomentum && momentumTimer > 0)
        {
            momentumTimer -= Time.deltaTime;
            if (momentumTimer <= 0)
            {
                hasMomentum = false;
            }
        }
    }

    // --- НОВОЕ: Внешний импульс (из Скрипта 3) ---
    /// <summary>
    /// Позволяет отбросить игрока (взрыв, джамп-пад, отдача оружия)
    /// </summary>
    public void AddExternalImpulse(Vector3 impulse)
    {
        // 1. Применяем импульс к текущей скорости (сразу работает для Y)
        velocity += impulse;

        // 2. Чтобы импульс по X/Z не исчез в следующем кадре из-за HandleMovement,
        // мы "скармливаем" его системе инерции.
        if (Mathf.Abs(impulse.x) > 1f || Mathf.Abs(impulse.z) > 1f)
        {
            hasMomentum = true;
            momentumTimer = momentumDecayTime; // Можно настроить отдельное время затухания
            momentumVelocity = new Vector3(velocity.x, 0f, velocity.z);
        }
    }

    // --- LOGIC: MOVEMENT ---
    void HandleMovement()
    {
        Vector2 input = movementAction.action.ReadValue<Vector2>();
        moveDirection = transform.right * input.x + transform.forward * input.y;

        // Приоритет 1: Отскок от стены
        if (wallJumpMomentum.magnitude > 0.1f)
        {
            float timeSinceJump = wallJumpMomentumDecay - Vector3.Distance(wallJumpMomentum, Vector3.zero) / wallJumpMomentumDecay;
            float controlFactor = Mathf.Clamp01(timeSinceJump / wallJumpMomentumDecay);

            if (input.magnitude > 0.1f)
            {
                Vector3 targetVel = moveDirection * movementSpeed;
                velocity.x = Mathf.Lerp(velocity.x, targetVel.x, 4f * controlFactor * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, targetVel.z, 4f * controlFactor * Time.deltaTime);
            }
            else
            {
                velocity.x = Mathf.Lerp(velocity.x, 0f, (1f - controlFactor) * 3f * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, 0f, (1f - controlFactor) * 3f * Time.deltaTime);
            }
            
            // Затухание импульса от стены
            wallJumpMomentum = Vector3.MoveTowards(wallJumpMomentum, Vector3.zero, wallJumpMomentumDecay * Time.deltaTime);
        }
        // Приоритет 2: Инерция (после деша, подката или ВНЕШНЕГО ИМПУЛЬСА)
        else if (hasMomentum && momentumTimer > 0)
        {
            // Если игрок нажимает кнопки движения, мы плавно перехватываем контроль
            if (input.magnitude > 0.1f)
            {
                Vector3 targetVelocity = moveDirection * movementSpeed;
                float t = 1f - (momentumTimer / momentumDecayTime); // t растет от 0 до 1
                velocity.x = Mathf.Lerp(momentumVelocity.x, targetVelocity.x, t);
                velocity.z = Mathf.Lerp(momentumVelocity.z, targetVelocity.z, t);
            }
            else
            {
                // Если ввода нет, просто замедляем импульс (трение)
                float decayFactor = momentumTimer / momentumDecayTime;
                velocity.x = momentumVelocity.x * decayFactor;
                velocity.z = momentumVelocity.z * decayFactor;
            }
        }
        // Приоритет 3: Обычный бег
        else
        {
            if (characterController.isGrounded && !isSliding)
            {
                Vector3 targetVelocity = (input.magnitude > 0.1f) ? moveDirection * movementSpeed : Vector3.zero;
                float lerpRate = (input.magnitude > 0.1f) ? 10f : 8f; // Разгон vs Торможение
                
                velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, lerpRate * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, lerpRate * Time.deltaTime);
            }
            else
            {
                // В воздухе управление чуть более инертное
                Vector3 targetVelocity = moveDirection * movementSpeed;
                velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, 4f * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, 4f * Time.deltaTime);
            }
        }
    }

    // --- LOGIC: DASH ---
    void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (dashCooldownCounter > 0) return;

        Debug.Log("Attempting to Dash!"); // Лог из нового скрипта

        bufferMoveDir = velocity; // Сохраняем вектор чтобы помнить, что мы в деше

        Vector3 dashDirection;
        // Если игрок на земле - деш по направлению движения
        if (characterController.isGrounded)
        {
            dashDirection = (moveDirection == Vector3.zero) ? transform.forward : moveDirection.normalized;
        }
        // Если в воздухе - деш туда, куда смотрит камера
        else
        {
            Vector2 inputVector = movementAction.action.ReadValue<Vector2>();
            if (inputVector.magnitude == 0 && cameraRoot != null)
            {
                dashDirection = cameraRoot.forward;
            }
            else if (cameraRoot != null)
            {
                // Комбинируем ввод с направлением камеры
                dashDirection = (cameraRoot.right * inputVector.x + cameraRoot.forward * inputVector.y).normalized;
            }
            else
            {
                dashDirection = transform.forward;
            }
            velocity.y = 0; // Отключаем гравитацию для прямого полета
        }

        velocity = dashDirection * dashSpeed;
        dashCooldownCounter = dashCooldown;

        StartCoroutine(ReturnToNormal(dashDuration));
    }

    IEnumerator ReturnToNormal(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Переходим в состояние инерции после деша
        momentumVelocity = new Vector3(velocity.x, 0f, velocity.z);
        hasMomentum = true;
        momentumTimer = momentumDecayTime;

        bufferMoveDir = Vector3.zero; // Снимаем флаг деша
        Debug.Log("Back to normal"); // Лог из нового скрипта
    }

    // --- LOGIC: JUMP ---
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
            if (isWallSliding)
            {
                // Wall Jump logic
                Vector3 reflectDir = Vector3.Reflect(new Vector3(velocity.x, 0, velocity.z), wallNormal).normalized;
                Vector3 jumpDir = (wallNormal + reflectDir).normalized; // Смесь нормали и отражения
                
                float vForce = Mathf.Sqrt(wallJumpHeight * -2f * gravity);
                
                // Применяем силу
                wallJumpMomentum = jumpDir * wallJumpForce;
                velocity = new Vector3(wallJumpMomentum.x, vForce, wallJumpMomentum.z);

                hasJumpedFromWall = true;
                wallJumpCooldownTimer = wallJumpCooldownTime;
                jumpBufferCounter = 0f;
                wallNormal = Vector3.zero;
                if (isSliding) EndSlide();
            }
            else if (canGroundJump)
            {
                // Ground Jump logic
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
                if (isSliding) EndSlide();
            }
        }
    }

    // --- LOGIC: SLIDE (Ground Only) ---
    private void OnSlidePerformed(InputAction.CallbackContext context)
    {
        Vector3 hVel = new Vector3(velocity.x, 0f, velocity.z);
        if (characterController.isGrounded && !isSliding && hVel.magnitude > 0.1f && slideCooldownCounter <= 0f)
        {
            StartSlide();
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        slideCooldownCounter = slideCooldown;
        slideDirection = new Vector3(velocity.x, 0f, velocity.z).normalized;
        hasMomentum = false; // Сбрасываем обычную инерцию, используем логику подката
    }

    private void HandleSlide()
    {
        if (!isSliding) { LerpHeightAndCameraBack(); return; }

        slideTimer -= Time.deltaTime;
        if (slideTimer <= 0) { EndSlide(); return; }

        // Трение при подкате
        float speed = slideSpeed - (slideFriction * (slideDuration - slideTimer));
        speed = Mathf.Max(speed, movementSpeed * 0.5f);
        
        velocity.x = slideDirection.x * speed;
        velocity.z = slideDirection.z * speed;

        characterController.height = Mathf.Lerp(characterController.height, slideHeight, heightLerpSpeed * Time.deltaTime);
        if (cameraRoot != null)
        {
            Vector3 targetPos = originalCameraLocalPos + Vector3.up * cameraSlideOffset;
            cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, targetPos, heightLerpSpeed * Time.deltaTime);
        }
    }

    private void EndSlide()
    {
        isSliding = false;
        // Сохраняем скорость выхода из подката как инерцию
        momentumVelocity = new Vector3(velocity.x, 0f, velocity.z);
        hasMomentum = true;
        momentumTimer = momentumDecayTime;
    }

    private void LerpHeightAndCameraBack()
    {
        if (Mathf.Abs(characterController.height - originalHeight) > 0.01f)
            characterController.height = Mathf.Lerp(characterController.height, originalHeight, heightLerpSpeed * Time.deltaTime);
        
        if (cameraRoot != null && Vector3.Distance(cameraRoot.localPosition, originalCameraLocalPos) > 0.01f)
            cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, originalCameraLocalPos, heightLerpSpeed * Time.deltaTime);
    }

    // --- LOGIC: WALL CHECK & GRAVITY ---
    void HandleWallCheck()
    {
        wallNormal = Vector3.zero;
        if (characterController.isGrounded || hasJumpedFromWall) return;

        Vector3 input = new Vector3(movementAction.action.ReadValue<Vector2>().x, 0, movementAction.action.ReadValue<Vector2>().y);
        Vector3 checkDir = (input.magnitude > 0.1f) ? (transform.right * input.x + transform.forward * input.z).normalized : transform.forward;

        if (Physics.SphereCast(transform.position, characterController.radius, checkDir, out RaycastHit hit, wallCheckDistance))
        {
            if (!string.IsNullOrEmpty(wallTag) && !hit.collider.CompareTag(wallTag)) return;
            
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle > 70f && angle < 110f && wallJumpCooldownTimer <= 0)
            {
                wallNormal = hit.normal;
            }
        }
    }

    void ApplyGravity()
    {
        // Если деш активен - гравитацию отключаем
        if (bufferMoveDir != Vector3.zero) return;

        bool onWall = wallNormal != Vector3.zero && !characterController.isGrounded && !hasJumpedFromWall;

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else if (onWall && velocity.y < 0) // Скольжение по стене только вниз
        {
            velocity.y += gravity * Time.deltaTime;
            if (velocity.y < wallSlideSpeed) velocity.y = wallSlideSpeed;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }
}