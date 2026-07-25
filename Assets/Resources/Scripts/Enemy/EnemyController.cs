using UnityEngine;
using DG.Tweening;

public class EnemyController : Units
{
    [Header("References")]
    public Rigidbody2D rigid;
    public Animator anim;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float gravityAccel = 20f;
    public Rigidbody2D.SlideMovement slideSettings = new Rigidbody2D.SlideMovement();

    [HideInInspector] public float verticalVelocity;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public Vector2 direction;

    private void Awake()
    {
        if (rigid == null) rigid = GetComponent<Rigidbody2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.freezeRotation = true;

        slideSettings.surfaceUp = Vector2.up;
        slideSettings.surfaceSlideAngle = 90f;
        slideSettings.gravitySlipAngle = 0f;
        slideSettings.surfaceAnchor = Vector2.zero;
        slideSettings.useLayerMask = true;
        slideSettings.layerMask = 1 << 6;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        Move();
    }

    public override void Dead()
    {
        Debug.Log("뒤짐");
        anim.CrossFade("Dead", 0f);
        DOVirtual.DelayedCall(1.5f, () => this.gameObject.SetActive(false));
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
