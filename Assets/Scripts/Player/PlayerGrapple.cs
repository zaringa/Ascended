using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerGrapple : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference grappleAction;

    [Header("Detection Settings")]
    [SerializeField] private Camera cam;
    [Tooltip("Слои стен/пола. Точки зацепа сюда НЕ включать.")]
    [SerializeField] private LayerMask obstacleLayers; 
    [Tooltip("Угол обзора (половина конуса).")]
    [SerializeField] private float detectionAngle = 15f; 

    [Header("Visuals")]
    [SerializeField] private float ropeWidth = 0.05f;
    [SerializeField] private Material ropeMaterial;
    [SerializeField] private Transform handPosition;

    [Header("Pull Physics")]
    [Tooltip("Как быстро набирается скорость")]
    public float pullAcceleration = 150f; // Увеличил, чтобы быстрее разгоняться
    [Tooltip("Максимальная скорость полета")]
    public float maxPullSpeed = 50f;      // Скорость полета к точке
    [Tooltip("На каком расстоянии от точки крюк отцепляется")]
    public float stopDistance = 1.5f;     

    [Header("Finish Momentum")]
    [Tooltip("Множитель итоговой скорости (1 = честная физика, >1 = выстреливает сильнее)")]
    public float exitMomentumMultiplier = 1.0f;
    public float postGrappleCooldown = 0.35f;

    // --- DEBUG INFO ---
    [Header("Debug Info")]
    [SerializeField] private int totalPointsOnLevel; 
    [SerializeField] private string currentTargetName; 

    private CharacterController controller;
    private PlayerController playerController;
    private LineRenderer lineRenderer;

    private GrapplePoint currentTargetPoint; 
    private Vector3 grapplePointPosition;   
    private Vector3 velocity;
    private bool isGrappling;
    private float allowGrappleAt = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        if (ropeMaterial != null) lineRenderer.material = ropeMaterial;
        if (cam == null) cam = Camera.main;
    }

    void OnEnable()
    {
        if (grappleAction != null)
        {
            grappleAction.action.Enable();
            grappleAction.action.performed += OnGrapplePressed;
        }
    }

    void OnDisable()
    {
        if (grappleAction != null)
        {
            grappleAction.action.Disable();
            grappleAction.action.performed -= OnGrapplePressed;
        }
        ClearCurrentPoint();
    }

    void Update()
    {
        totalPointsOnLevel = GrapplePoint.AllPoints.Count;

        if (isGrappling)
        {
            ProcessGrappleMovement();
        }
        else
        {
            DetectBestPoint();
        }
    }

    void LateUpdate()
    {
        if (isGrappling) DrawRope();
    }

    // --- ЛОГИКА ПОИСКА (без изменений) ---
    void DetectBestPoint()
    {
        if (Time.time < allowGrappleAt)
        {
            ClearCurrentPoint();
            return;
        }

        GrapplePoint bestPoint = null;
        float bestAngle = detectionAngle; 

        Vector3 camPos = cam.transform.position;
        Vector3 camFwd = cam.transform.forward;

        foreach (GrapplePoint point in GrapplePoint.AllPoints)
        {
            if (point == null) continue;

            Vector3 dirToPoint = point.transform.position - camPos;
            float distance = dirToPoint.magnitude;

            if (distance < point.minDistance || distance > point.maxDistance) continue;
            if (transform.position.y > (point.transform.position.y - point.minHeightDifference)) continue;

            float angle = Vector3.Angle(camFwd, dirToPoint.normalized);
            if (angle > detectionAngle) continue;

            float checkDistance = distance - 0.5f;
            if (checkDistance > 0 && Physics.Raycast(camPos, dirToPoint.normalized, checkDistance, obstacleLayers))
                continue;

            if (angle < bestAngle)
            {
                bestAngle = angle;
                bestPoint = point;
            }
        }

        if (bestPoint != currentTargetPoint)
        {
            ClearCurrentPoint();
            currentTargetPoint = bestPoint;

            if (currentTargetPoint != null)
            {
                currentTargetName = currentTargetPoint.name;
                if (currentTargetPoint.hintUI != null)
                    currentTargetPoint.hintUI.SetActive(true);
            }
            else
            {
                currentTargetName = "None";
            }
        }
    }

    void ClearCurrentPoint()
    {
        if (currentTargetPoint != null)
        {
            if (currentTargetPoint.hintUI != null)
                currentTargetPoint.hintUI.SetActive(false);
            currentTargetPoint = null;
            currentTargetName = "None";
        }
    }

    void OnGrapplePressed(InputAction.CallbackContext context)
    {
        if (isGrappling) return;
        if (Time.time < allowGrappleAt) return;

        if (currentTargetPoint != null)
        {
            StartGrapple(currentTargetPoint);
        }
    }

    public void StartGrapple(GrapplePoint target)
    {
        grapplePointPosition = target.transform.position;
        if (target.hintUI != null) target.hintUI.SetActive(false);

        isGrappling = true;
        // Начинаем с текущей скорости игрока или с нуля, по желанию.
        // Обычно лучше с нуля для резкого рывка, или добавить импульс к текущей.
        velocity = Vector3.zero; 

        lineRenderer.enabled = true;
        playerController.enabled = false;
    }

    // --- ФИЗИКА (Исправленная под ТЗ тимлида) ---
    void ProcessGrappleMovement()
    {
        // 1. Вектор строго к точке (прямая линия)
        Vector3 vectorToTarget = grapplePointPosition - transform.position;
        float distance = vectorToTarget.magnitude;
        Vector3 dir = vectorToTarget.normalized;

        // 2. Ускоряемся к точке
        velocity += dir * pullAcceleration * Time.deltaTime;
        
        // Ограничение скорости
        if (velocity.magnitude > maxPullSpeed)
        {
            velocity = velocity.normalized * maxPullSpeed;
        }

        // Двигаем контроллер
        controller.Move(velocity * Time.deltaTime);

        // 3. Условие выхода: достигли дистанции остановки
        if (distance <= stopDistance)
        {
            FinishGrapple();
        }
    }

    void FinishGrapple()
    {
        isGrappling = false;
        lineRenderer.enabled = false;
        allowGrappleAt = Time.time + postGrappleCooldown;
        ClearCurrentPoint();

        playerController.enabled = true;

        // --- НОВАЯ ЛОГИКА ФИНАЛА ---
        // Берем вектор скорости, который был в момент отцепления.
        // Он направлен точно в сторону движения (к точке).
        // Если летели снизу -> вектор смотрит вверх-вперед -> мы летим вверх-вперед по параболе (гравитация сделает свое дело).
        // Если летели прямо -> вектор смотрит вперед -> летим вперед.
        
        Vector3 exitVelocity = velocity * exitMomentumMultiplier;

        playerController.SetVelocity(exitVelocity);
    }

    void DrawRope()
    {
        Vector3 startPos = handPosition != null ? handPosition.position : transform.position;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, grapplePointPosition);
    }
}