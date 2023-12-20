using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    public UnityEvent onBeginMoving;
    private float horizontal;
    private bool isFacingRight = true;

    private bool isDashing;
    private bool isWallSliding;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingCounter;
    private float wallJumpingTime = .2f;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Layers")]
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;

    [Header("Base Movement")]
    [SerializeField] private float speed = 50f;
    [SerializeField] private float jumpPower = 14f;
    [SerializeField] private float coyoteTime = .2f;
    [SerializeField] private float jumpBufferTime = .2f;
    
    [Header("Dashing")]
    [SerializeField] private bool canDash = true;
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = .2f;
    [SerializeField] private float dashingCooldown = 1f;

    [Header("Wall Slide")]
    [SerializeField] private float wallSlidingSpeed = 2f;

    [Header("Wall Jump")]
    [SerializeField] private bool canWallJump = true;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 16f);
    [SerializeField] private float wallJumpingDuration = .2f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource jumpSFX;
    [SerializeField] private AudioSource dashSFX;

    [Header("Visual")]
    [SerializeField] private TrailRenderer TrailRenderer;

    private enum MovementState { idle, running, jumping, falling, wallSliding };

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetPlayerTransform()
    {
        return gameObject.transform.position;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (PlayerDeath.Instance.IsDead || isDashing) return;

        horizontal = Input.GetAxisRaw("Horizontal");

        if (IsGrounded()) {
            coyoteTimeCounter = coyoteTime;
        } else coyoteTimeCounter -= Time.deltaTime;

        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferTime;
        } else jumpBufferCounter -= Time.deltaTime;

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f) {
            jumpSFX.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);

            jumpBufferCounter = 0f;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f) {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);

            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) {
            StartCoroutine(Dash());
        }

        if (canWallJump) {
            WallSlide();
        }
        WallJump();
        if (!isWallJumping) {
            UpdateAnimationState();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;
        // Turn on horizontal movement if not Walled (so Wall Jumping) AND not "dead"
        if (!isWallJumping && rb.bodyType != RigidbodyType2D.Static) {
            rb.velocity = new Vector2(horizontal * speed * Time.deltaTime, rb.velocity.y);
        }   
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .2f, terrainLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, .2f, terrainLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f) {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        } else {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding) {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        } else {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f) {
            jumpSFX.Play();
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection) {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private IEnumerator Dash()
    {
        // Starts Dashing
        dashSFX.Play();
        canDash = false;
        isDashing = true;
        // Store initial gravity
        float originalGravity = rb.gravityScale;
        // Turn off gravity to dash straight
        rb.gravityScale = 0f;
        // Push player in the facing direction
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        // Turn on the TrailRenderer
        TrailRenderer.emitting = true;
        // Stops dashing after waiting some time
        yield return new WaitForSeconds(dashingTime);
        TrailRenderer.emitting = false;
        // Sets gravity back to original
        rb.gravityScale = originalGravity;
        isDashing = false;
        // Post dash cooldown
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        // Move left/right
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f) {
            onBeginMoving?.Invoke();
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        // Animation based on left or right
        if (horizontal > 0f || horizontal < 0f && !isWallSliding) {
            state = MovementState.running;
        } else {
            state = MovementState.idle;
        }

        // Jumping
        if (rb.velocity.y > .1f) {
            state = MovementState.jumping;
        }

        // Falling
        else if (rb.velocity.y < -.1f) {
            state = MovementState.falling;
        }

        // Wall Slide
        if (isWallSliding) {
            state = MovementState.wallSliding;
        }

        anim.SetInteger("state", (int)state);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
