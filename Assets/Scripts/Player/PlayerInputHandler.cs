using UnityEngine;
using UnityEngine.InputSystem;

// Прикрепляется к отдельному от игрока Gameobject'у
/// <summary> Обрабатывает поступающий от пользователя ввод </summary>
public class PlayerInputHandler : MonoBehaviour
{
	[Header("Unity Input Asset")]
	[SerializeField] private InputActionAsset inputActions;

	[Header("Action Map Name Reference")]
	[SerializeField] private string actionMapName = "Player";

	[Header("Action Name References")]
	[SerializeField] private string movement = "Run";

	private InputAction moveAction;

	/// <summary> Нормализированный вектор ввода передвижения </summary>
	[HideInInspector] public Vector2 MovementInput;

	void OnEnable()
	{
		inputActions.FindActionMap(actionMapName).Enable();
	}

	void OnDisable()
	{
		inputActions.FindActionMap(actionMapName).Disable();
	}

	void Awake()
	{
		InputActionMap playerInputMap = inputActions.FindActionMap(actionMapName);

		moveAction = playerInputMap.FindAction(movement);

		SubscribeActionValuesToInputEvents();
	}

	void SubscribeActionValuesToInputEvents()
	{
		moveAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
		moveAction.canceled += inputInfo => MovementInput = Vector2.zero;
	}
}