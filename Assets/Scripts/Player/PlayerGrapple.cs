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
    [Tooltip("Слои стен/пола. НЕ ВКЛЮЧАЙ сюда слой самих точек, если у них есть коллайдеры!")]
    [SerializeField] private LayerMask obstacleLayers; 
    [Tooltip("Угол обзора (половина конуса).")]
    [SerializeField] private float detectionAngle = 15f; 

    [Header("Visuals")]
    [SerializeField] private float ropeWidth = 0.05f;
    [SerializeField] private Material ropeMaterial;
    [SerializeField] private Transform handPosition;

    [Header("Pull Physics")]
    public float pullAcceleration = 90f;
    public float maxPullSpeed = 40f;
    public float stopDistance = 2.0f;

    [Header("Vault Settings")]
    public float targetHeightOffset = 2.0f; 

    [Header("Finish Momentum")]
    public float horizontalMomentumPreserve = 1.0f;
    public float upwardBoost = 10f;
    public float postGrappleCooldown = 0.35f;

    // --- DEBUG INFO (чтобы видеть в инспекторе, сколько точек найдено) ---
    [Header("Debug Info")]
    [SerializeField] private int totalPointsOnLevel; // Показывает, сколько всего точек в списке
    [SerializeField] private string currentTargetName; // Имя текущей цели

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
        // Для дебага обновляем кол-во точек в инспекторе
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

        // Перебор статического списка
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
                currentTargetName = currentTargetPoint.name; // Для дебага
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
        velocity = Vector3.zero;

        lineRenderer.enabled = true;
        playerController.enabled = false;
    }

    void ProcessGrappleMovement()
    {
        Vector3 aimPosition = grapplePointPosition + Vector3.up * targetHeightOffset;
        Vector3 vectorToAim = aimPosition - transform.position;
        float distanceToAim = vectorToAim.magnitude;
        Vector3 dir = vectorToAim.normalized;

        velocity += dir * pullAcceleration * Time.deltaTime;
        if (velocity.magnitude > maxPullSpeed) velocity = velocity.normalized * maxPullSpeed;

        controller.Move(velocity * Time.deltaTime);

        if (distanceToAim <= stopDistance)
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

        Vector3 exitVelocity = velocity;
        exitVelocity.x *= horizontalMomentumPreserve;
        exitVelocity.z *= horizontalMomentumPreserve;
        exitVelocity.y = upwardBoost;

        playerController.SetVelocity(exitVelocity);
    }

    void DrawRope()
    {
        Vector3 startPos = handPosition != null ? handPosition.position : transform.position;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, grapplePointPosition);
    }
}