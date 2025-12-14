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
    public float pullAcceleration = 60f;
    public float maxPullSpeed = 40f;
    public float stopDistance = 1.5f; // Дистанция остановки от стены

    [Header("Cooldown")]
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
        if (isGrappling)
        {
            DrawRope();
        }
    }

    void Update()
    {
        if (isGrappling)
            ProcessGrappleMovement();
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
        
        if (currentTarget != null) grapplePointPosition = currentTarget.position;
        lineRenderer.SetPosition(1, grapplePointPosition);
    }

    void ProcessGrappleMovement()
    {
        Vector3 vectorToTarget = grapplePointPosition - transform.position;
        float distance = vectorToTarget.magnitude;
        Vector3 dir = vectorToTarget.normalized;

        // Ускоряемся к цели
        velocity += dir * pullAcceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxPullSpeed);

        controller.Move(velocity * Time.deltaTime);

        // Если мы достаточно близко -> ОСТАНОВКА
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

        // Полная остановка
        velocity = Vector3.zero;

        playerController.enabled = true;
        playerController.ResetVelocity();
        
        // Маленький импульс вперед (2 единицы), чтобы "прилипнуть" к стене
        // Это поможет сразу начать Wall Slide / Wall Run, если игрок зажмет клавиши
        playerController.AddExternalImpulse(transform.forward * 2f);

        currentTarget = null;
    }
}