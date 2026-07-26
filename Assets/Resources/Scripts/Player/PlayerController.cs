using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : Units
{
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform rayPos;

    private float MOVE_SPEED = 5f;
    private const float JUMP_POWER = 7f;

    private Rigidbody2D.SlideResults slideResults;
    private Rigidbody2D.SlideMovement slideMovement;

    private float horizontalInput;
    private int groundCount;
    private float rigidVelocity = 0;
    public bool jumpRequest;
    public bool isGround;
    private float windForce;

    private CapsuleCollider2D capsule;
    private ContactFilter2D enemyFilter;
    private readonly Collider2D[] overlapResults = new Collider2D[8];

    private void Start()
    {
        if (rigid == null) rigid = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        slideMovement = new Rigidbody2D.SlideMovement();
        slideMovement.surfaceAnchor = Vector2.zero;
        slideMovement.surfaceSlideAngle = 90f;
        slideMovement.gravitySlipAngle = 90f;
        slideMovement.useLayerMask = true;
        slideMovement.layerMask = 1 << 6;

        enemyFilter = new ContactFilter2D();
        enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
        enemyFilter.useTriggers = true;
    }

    public override void Dead()
    {
        isDead = true;
        anim.CrossFade("Dead", 0f);
        LocalGameManager.instance.DisableInput();
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
                var platform = hitCollider.GetComponent<PlatForm>();
                if (platform != null && platform.canThrough)
                {
                    platform.Through();
                }
            }
        }
    }

    public void WindGimmick(float value)
    {
        windForce = value;
    }

    private void LateUpdate()
    {
        if (isDead) return;
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

        bool nearEnemy = CheckNearbyEnemy();
        float speed = nearEnemy ? MOVE_SPEED * 0.25f : MOVE_SPEED;

        slideMovement.gravity = new Vector2(0f, rigidVelocity);
        slideResults = rigid.Slide((Vector2.right * horizontalInput * speed) + (Vector2.right * windForce), Time.deltaTime, slideMovement);
        this.gameObject.transform.localScale = new Vector2(horizontalInput < 0 ? 1 : -1, 1);
        isGround = slideResults.surfaceHit.collider != null;
    }

    private bool CheckNearbyEnemy()
    {
        int count = Physics2D.OverlapCapsule(
            (Vector2)transform.position + capsule.offset,
            capsule.size,
            capsule.direction,
            0f,
            enemyFilter,
            overlapResults
        );

        for (int i = 0; i < count; i++)
        {
            if (overlapResults[i].GetComponent<EnemyController>() != null)
                return true;
        }
        return false;
    }
}
