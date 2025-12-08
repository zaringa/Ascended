using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerGrapple : MonoBehaviour
{
    [Header("Pull")]
    public float pullAcceleration = 40f;
    public float maxPullSpeed = 35f;

    [Header("Finish")]
    public float overshootDistance = 2.0f;
    public float exitSpeedMultiplier = 1.15f;
    public float postGrappleCooldown = 0.35f;

    private CharacterController controller;
    private PlayerController playerController;

    private Transform target;
    private Vector3 velocity;
    private bool isGrappling;
    private float allowGrappleAt = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isGrappling)
            ProcessGrapple();
    }

    public bool CanGrapple()
    {
        return !isGrappling && Time.time >= allowGrappleAt;
    }

    public void StartGrapple(Transform point)
    {
        target = point;
        isGrappling = true;
        velocity = Vector3.zero;

        playerController.enabled = false;
    }

    void ProcessGrapple()
    {
        Vector3 dir = (target.position - transform.position).normalized;

        velocity += dir * pullAcceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxPullSpeed);

        controller.Move(velocity * Time.deltaTime);

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist < 1.3f)
        {
            StartCoroutine(FinishGrapple(dir));
        }
    }

    IEnumerator FinishGrapple(Vector3 directionToTarget)
    {
        isGrappling = false;
        allowGrappleAt = Time.time + postGrappleCooldown;

        // 1) Overshoot — проталкиваем игрока ЧЕРЕЗ точку
        Vector3 forward = transform.forward;
        Vector3 overshoot = forward * overshootDistance;

        controller.Move(overshoot);

        // Ждём 1 кадр чтобы CharacterController "признал" новое положение
        yield return null;

        // 2) Добавляем внешний импульс — ТОЛЬКО после overshoot
        float exitSpeed = velocity.magnitude * exitSpeedMultiplier;
        if (exitSpeed < 10f) exitSpeed = 10f;

        playerController.AddExternalImpulse(forward * exitSpeed);

        // 3) Теперь можно вернуть управление
        playerController.enabled = true;

        target = null;
    }
}
