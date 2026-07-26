using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class EnemyController : Units
{
    [Header("References")]
    public Rigidbody2D rigid;
    public Animator anim;
    public AIController aiController;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float gravityAccel = 20f;
    public Rigidbody2D.SlideMovement slideSettings = new Rigidbody2D.SlideMovement();

    [HideInInspector] public float verticalVelocity;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public Vector2 direction;

    private Tween deadTween;
    private bool needTeleport;
    private Vector2 teleportTarget;

    private void Awake()
    {
        if (rigid == null) rigid = GetComponent<Rigidbody2D>();

        isDead = false;

        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.freezeRotation = true;

        slideSettings.surfaceUp = Vector2.up;
        slideSettings.surfaceSlideAngle = 90f;
        slideSettings.gravitySlipAngle = 0f;
        slideSettings.surfaceAnchor = Vector2.zero;
        slideSettings.useLayerMask = true;
        slideSettings.layerMask = 1 << 6;
    }

    void OnEnable()
    {
        deadTween?.Kill();
        isDead = false;
        verticalVelocity = 0f;
        isGrounded = false;
        direction = Vector2.zero;
        moveSpeed = Random.Range(3f, 4f);
    }

    void FixedUpdate()
    {
        if (needTeleport)
        {
            rigid.position = teleportTarget;
            rigid.linearVelocity = Vector2.zero;
            verticalVelocity = 0f;
            needTeleport = false;
        }
        ApplyGravity();
        Move();
    }

    public void TeleportTo(Vector2 pos)
    {
        teleportTarget = pos;
        needTeleport = true;
    }

    public override void Dead()
    {
        isDead = true;
        aiController.curState = AIController.UnitState.Dead;
        anim.CrossFade("Dead", 0f);
        deadTween = DOVirtual.DelayedCall(1.5f, () =>
        {
            if (Pool != null) Pool.Release(gameObject);
            else gameObject.SetActive(false);
        });
    }

    public void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravityAccel * Time.fixedDeltaTime;
        }
        slideSettings.gravity = new Vector2(0f, verticalVelocity);
    }

    public void Move()
    {
        var result = rigid.Slide(direction * moveSpeed, Time.fixedDeltaTime, slideSettings);
        isGrounded = result.surfaceHit.collider != null;
    }
}
