using UnityEngine;

/// <summary> Отвечает за управление аватаром игрока </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private PlayerInputHandler playerInputHandler;

	[Header("Movement Speed")]
	[SerializeField] private float runSpeed = 5;

	/// <summary> Нормализированный вектор ввода передвижения в контексте аватара игрока </summary>
	private Vector3 InputDirection => new(playerInputHandler.MovementInput.x, 0, playerInputHandler.MovementInput.y);
	private Rigidbody rb;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		Run();
	}

	/// <summary> Перемещает аватар игрока в соответствии с вводом через Action Input Asset со скоростью RunSpeed каждый FixedUpdate </summary>
	private void Run()
	{
		rb.MovePosition(rb.position + Time.fixedDeltaTime * runSpeed * InputDirection);
	}
}