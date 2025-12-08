using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleDetector : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference grappleAction;

    [Header("References")]
    public Camera cam;
    public PlayerGrapple grapple;

    private GrapplePoint currentPoint;

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
    }

    void Update()
    {
        DetectPoint();
    }

    void DetectPoint()
    {
        // Если Grapple временно запрещён (cooldown), не ищем точки вообще
        if (grapple != null && !grapple.CanGrapple())
        {
            if (currentPoint != null)
            {
                currentPoint.hintUI.SetActive(false);
                currentPoint = null;
            }
            return;
        }

        if (currentPoint != null)
            currentPoint.hintUI.SetActive(false);

        currentPoint = null;

        if (cam == null) return;

        if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 80f))
            return;

        GrapplePoint point = hit.collider.GetComponent<GrapplePoint>();
        if (point == null)
            return;

        Vector3 toPoint = point.transform.position - transform.position;
        float distance = toPoint.magnitude;

        if (distance < point.minDistance || distance > point.maxDistance)
            return;

        if (transform.position.y > point.transform.position.y - point.minHeightDifference)
            return;

        float angle = Vector3.Angle(cam.transform.forward, toPoint.normalized);
        if (angle > point.maxViewAngle)
            return;

        currentPoint = point;
        if (currentPoint.hintUI != null)
            currentPoint.hintUI.SetActive(true);
    }

    void OnGrapplePressed(InputAction.CallbackContext context)
    {
        if (currentPoint != null && grapple != null && grapple.CanGrapple())
        {
            grapple.StartGrapple(currentPoint.transform);
        }
    }
}
