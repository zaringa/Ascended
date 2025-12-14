using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerGrapple : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private float ropeWidth = 0.05f;
    [SerializeField] private Material ropeMaterial;
    [SerializeField] private Transform handPosition;

    [Header("Pull Physics")]
    public float pullAcceleration = 90f; 
    public float maxPullSpeed = 40f;     
    public float stopDistance = 2.0f;

    [Header("Vault Settings")]
    [Tooltip("Насколько выше реальной точки мы целимся, чтобы перелететь край (в метрах)")]
    public float targetHeightOffset = 2.0f; // <-- ВАЖНЫЙ ПАРАМЕТР

    [Header("Finish Momentum")]
    public float horizontalMomentumPreserve = 1.0f; 
    public float upwardBoost = 8f; 
    public float postGrappleCooldown = 0.35f;

    private CharacterController controller;
    private PlayerController playerController;
    private LineRenderer lineRenderer;

    private Transform currentTarget;
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
        
        if(ropeMaterial != null) lineRenderer.material = ropeMaterial;
    }

    void LateUpdate()
    {
        if (isGrappling) DrawRope();
    }

    void Update()
    {
        if (isGrappling) ProcessGrappleMovement();
    }

    public bool CanGrapple()
    {
        return !isGrappling && Time.time >= allowGrappleAt;
    }

    public void StartGrapple(Transform targetTransform)
    {
        currentTarget = targetTransform;
        grapplePointPosition = targetTransform.position;
        isGrappling = true;
        velocity = Vector3.zero; 
        
        lineRenderer.enabled = true;
        playerController.enabled = false; 
    }

    void DrawRope()
    {
        Vector3 startPos = handPosition != null ? handPosition.position : transform.position;
        lineRenderer.SetPosition(0, startPos);
        
        // Визуально веревка всё ещё идет в САМУ точку (чтобы выглядело красиво)
        Vector3 endPos = currentTarget != null ? currentTarget.position : grapplePointPosition;
        lineRenderer.SetPosition(1, endPos);
    }

    void ProcessGrappleMovement()
    {
        if (currentTarget != null) grapplePointPosition = currentTarget.position;

        // --- ЛОГИКА СМЕЩЕНИЯ ---
        // Мы физически тянемся не к самому объекту, а в точку НАД ним.
        // Это позволяет CharacterController'у не врезаться в край платформы.
        Vector3 aimPosition = grapplePointPosition + Vector3.up * targetHeightOffset;

        Vector3 vectorToAim = aimPosition - transform.position;
        float distanceToAim = vectorToAim.magnitude;
        Vector3 dir = vectorToAim.normalized;

        // Физика тяги
        velocity += dir * pullAcceleration * Time.deltaTime;
        if(velocity.magnitude > maxPullSpeed)
        {
            velocity = velocity.normalized * maxPullSpeed;
        }

        controller.Move(velocity * Time.deltaTime);

        // Проверяем дистанцию до ТОЧКИ ПРИЦЕЛИВАНИЯ (которая висит в воздухе над платформой)
        // Если мы подлетели близко к этой воображаемой точке, значит пора отцепляться
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

        playerController.enabled = true;

        // Расчет финальной инерции
        Vector3 exitVelocity = velocity;
        exitVelocity.x *= horizontalMomentumPreserve;
        exitVelocity.z *= horizontalMomentumPreserve;
        
        // Гарантированный подброс вверх для красивой дуги приземления
        exitVelocity.y = upwardBoost;

        // Передаем скорость и сбрасываем прыжки
        playerController.SetVelocity(exitVelocity);
        
        currentTarget = null;
    }
}