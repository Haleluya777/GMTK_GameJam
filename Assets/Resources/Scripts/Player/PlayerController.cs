using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : Units
{
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform rayPos;

    private const float MOVE_SPEED = 5f;
    private const float JUMP_POWER = 10f;

    private Rigidbody2D.SlideResults slideResults;
    private Rigidbody2D.SlideMovement slideMovement;

    private float horizontalInput;
    private int groundCount;
    private float rigidVelocity = 0;
    public bool jumpRequest;
    public bool isGround;

    private void Start()
    {
        if (rigid == null) rigid = GetComponent<Rigidbody2D>();
        slideMovement = new Rigidbody2D.SlideMovement();
        slideMovement.surfaceAnchor = Vector2.zero;
        slideMovement.surfaceSlideAngle = 90f;
        slideMovement.gravitySlipAngle = 90f;
        slideMovement.useLayerMask = true;
        slideMovement.layerMask = 1 << 6;
    }

    public override void Dead()
    {
        anim.CrossFade("Dead", 0f);
        DOVirtual.DelayedCall(1.5f, () => this.gameObject.SetActive(false));
    }


    public void Movement(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            horizontalInput = context.ReadValue<Vector2>().x;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGround)
        {
            jumpRequest = true;
        }
    }

    public void UnderJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGround)
        {
            var hitCollider = slideResults.surfaceHit.collider;
            if (hitCollider != null)
            {
                Debug.Log("뭔냄새야.");
                var platform = hitCollider.GetComponent<PlatForm>();
                if (platform != null && platform.canThrough)
                {
                    Debug.Log("뭔냄새야222.");
                    platform.Through();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (isGround)
        {
            if (horizontalInput == 0f)
            {
                anim.CrossFade("Idle", 0f);
            }
            else
            {
                anim.CrossFade("Walking", 0f);
            }
        }
        else
        {
            anim.CrossFade("Jump", 0f);
        }
    }

    private void FixedUpdate()
    {
        float targetGravity = isGround ? 0f : -rigidVelocity;
        if (jumpRequest)
        {
            rigidVelocity = JUMP_POWER;
            isGround = false;
            jumpRequest = false;
        }
        else if (!isGround)
        {
            rigidVelocity -= 15 * Time.fixedDeltaTime;
        }

        slideMovement.gravity = new Vector2(0f, rigidVelocity);
        slideResults = rigid.Slide(Vector2.right * horizontalInput * MOVE_SPEED, Time.deltaTime, slideMovement);
        isGround = slideResults.surfaceHit.collider != null;
    }
}
