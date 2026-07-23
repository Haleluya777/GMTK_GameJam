using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
    private Vector2 moveInput;
    private const float MOVE_SPEED = 5f;
    private const float JUMP_POWER = 10f;

    public void Movement(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            Vector2 value = context.ReadValue<Vector2>();
            rigid.linearVelocityX = value.x * MOVE_SPEED;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rigid.AddForceY(JUMP_POWER, ForceMode2D.Impulse);
        }
    }
}
