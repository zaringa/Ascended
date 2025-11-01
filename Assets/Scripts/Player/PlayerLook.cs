using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Отвечает за поворот игрока по горизонтали и камеры по вертикали.
/// Прикрепляется к дочернему объекту Player (например, "CameraPivot").
/// </summary>
public class PlayerLook : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference useAction;


    [Header("Settings")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private bool invertY = false;
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;
    [SerializeField] private float maxInteractionLength = 2.6f;


    [Header("References")]
     public Transform playerBody;

    private float verticalRotation = 0f;
    private bool isFirstFrame = true;
    [HideInInspector] public GameObject lookedAtActor;
    private Vector2 lookAxis;

    void Start()
    {
        if (playerBody == null)
        {
            playerBody = transform.parent;
            if (playerBody == null)
            {
                Debug.LogError($"Не удалось найти родительский объект (playerBody) у {name}");
                return;
            }
        }

        transform.localRotation = Quaternion.identity;
        verticalRotation = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        isFirstFrame = true;
    }

    void OnEnable()
    {
        lookAction?.action.Enable();
        useAction?.action.Enable();
        useAction.action.performed += UseAction;
    }

    void OnDisable()
    {
        lookAction?.action.Disable();
    }
    void UseAction(InputAction.CallbackContext context)
    {
        if(lookedAtActor !=null)
        {
            lookedAtActor.BroadcastMessage("Execute");
        }
    }

    void Update()
    {
        if (lookAction == null || playerBody == null) return;

        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        // Игнорируем первый кадр после включения/запуска, чтобы избежать дёргания
        if (isFirstFrame)
        {
            isFirstFrame = false;
            return;
        }

        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;


        // Поворот игрока по горизонтали (Y оси)
        playerBody.Rotate(Vector3.up * mouseX);

        // Поворот камеры по вертикали (X оси)
        if (invertY) 
            mouseY = -mouseY;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward*maxInteractionLength);
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxInteractionLength, 3))
        {
            Debug.Log("Hited something");
            lookedAtActor = hit.transform.gameObject;
        }
        else
        {
            lookedAtActor = null;
        }
    }
}