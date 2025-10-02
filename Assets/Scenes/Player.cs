using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [HideInInspector] public float f_JumpPower = 120.0f;
    public InputActionReference jump;
    protected CharacterController moveController;
    protected Camera mainCam;
    private bool isGrounded_b;
    private Vector3 moveDir = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveController = GetComponent<CharacterController>();
        mainCam = GetComponentInChildren<Camera>();

    }
    void Jump(InputAction.CallbackContext obj)
    {
        Debug.Log("Jumped");
        if (moveController.isGrounded)
        {
            moveDir.y = f_JumpPower;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moveController.isGrounded)
        {
            //bla bla bla
        }
        moveDir.y -= 9.8f * Time.deltaTime;
        moveController.Move(moveDir * Time.deltaTime);
        
    }
    void OnEnable()
    {
        jump.action.started += Jump;
    }
    void OnDisable()
    {
        jump.action.started -= Jump;
    }
}
