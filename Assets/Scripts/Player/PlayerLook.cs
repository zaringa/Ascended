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

    [Header("Settings")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private bool invertY = false;
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;

    [Header("References")]
     public Transform playerBody;

    private float verticalRotation = 0f;
    private bool isFirstFrame = true;
    public Vector2 lookAxis;

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
        if (lookAction?.action != null)
        {
            lookAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (lookAction?.action != null)
        {
            lookAction.action.Disable();
        }
    }

    void Update()
    {
        if (playerBody == null) return;

        Vector2 lookInput = Vector2.zero;

        // Try to get input from InputActionReference
        if (lookAction?.action != null)
        {
            lookInput = lookAction.action.ReadValue<Vector2>();
        }
        else
        {
            // Fallback: Direct mouse input if InputActionReference is not set
            if (Mouse.current != null)
            {
                // Mouse.delta returns pixels, so we need to scale it down
                lookInput = Mouse.current.delta.ReadValue() * 0.1f;
            }
        }

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
    }
}