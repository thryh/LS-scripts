using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Componenti
    private Rigidbody2D rigidbody;
    private BoxCollider2D boxcollider;
    private Animator animator;
    private float horizontalInput;
    private float verticalInput;
    const string PLAYER_RUN = "Player_Run";
    const string PLAYER_IDLE = "Player_Idle";
    const string PLAYER_CLIMBING = "Player_Climbing";
    string currentState;
    bool spacePressed = false;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    private bool facingRight = true;
    [SerializeField] private LayerMask jumpableTerrain;

    // Movimento
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float velocityPower;
    [SerializeField] private float friction;
    [SerializeField] private float jumpCutMultiplier;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float fallGravityMultiplier;
    [SerializeField] private float jumpCoyoteTime;
    private float gravityscale;

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;

    // Salto
    private bool isJumping = false;
    private bool jumpInputReleased;
    private float lastJumpTime;
    private float lastOnGroundTime;

    [SerializeField] private LayerMask ladderLayer;
    [SerializeField] private float distance;
    private bool isClimbing = false;

    // Start is called before the first frame updates
    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        boxcollider = gameObject.GetComponent<BoxCollider2D>();
        gravityscale = rigidbody.gravityScale;
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        //rigidbody.velocity = new Vector2(horizontalInput * moveSpeed, rigidbody.velocity.y);

        /*if (horizontalInput != 0)
        {
            rigidbody.velocity = new Vector2(horizontalInput * moveSpeed, rigidbody.velocity.y);

            if (horizontalInput < 0)
            {
                if (facingRight) Flip();
                ChangeAnimationState(PLAYER_RUN);
            }
            else if (horizontalInput > 0)
            {
                if (!facingRight) Flip();
                ChangeAnimationState(PLAYER_RUN);
            }
        }
        else if (horizontalInput == 0 && isOnGround())
        {
            rigidbody.velocity = new Vector2(0f, 0f);
            ChangeAnimationState(PLAYER_IDLE);
        }*/

        /*if (rigidbody.velocity.y > .1f)
        {
            if (horizontalInput != 0)
            {
                rigidbody.velocity = new Vector2(horizontalInput * moveSpeed, rigidbody.velocity.y);
            } else
            {
                ChangeAnimationState(PLAYER_IDLE);
                rigidbody.velocity = new Vector2(0f, rigidbody.velocity.y);
            }
        }

        if (Input.GetKeyDown("space") && isOnGround())
        {
            spacePressed = true;
            //rigidbody.velocity = new Vector2(rigidbody.velocity.x, 14f);
        }*/

        if (Input.GetKey("space")) lastJumpTime = jumpBufferTime;

        if (Input.GetKeyUp("space")) OnJumpUp();

        if (isOnGround())
        {
            lastOnGroundTime = jumpCoyoteTime;
        }

        // Salto
        if (lastOnGroundTime>0.0f && lastJumpTime > 0 && !isJumping)
        {
            Jump();
        }

        if (rigidbody.velocity.y < 0)
        {
            isJumping = false;
        }

        if (horizontalInput != 0)
        {
            ChangeAnimationState(PLAYER_RUN);
            if (horizontalInput > 0)
            {
                if (!facingRight) Flip();
            }
            else
            {
                if (facingRight) Flip();
            }
        } 
        else
        {
            ChangeAnimationState(PLAYER_IDLE);
        }

        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.up, distance, ladderLayer);

        if (hitInfo.collider != null)
        {
            if (verticalInput !=0 && horizontalInput == 0)
            {
                animator.Play(PLAYER_CLIMBING);
                rigidbody.velocity = new Vector2(0f, verticalInput * moveSpeed);
                //rigidbody.gravityScale = 0;
            }
            else
            {
                //rigidbody.gravityScale = gravityscale;
                isClimbing = false;
            }
        }

            /*if (isClimbing == true)
            {
                ChangeAnimationState(PLAYER_CLIMBING);
                rigidbody.velocity = new Vector2(rigidbody.position.x, verticalInput * moveSpeed);
                //rigidbody.gravityScale = 0;
            }/*
            else
            {
                rigidbody.gravityScale = gravityscale;
            }*/

            // Timer
            lastOnGroundTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        #region Run

        float targetSpeed = horizontalInput*moveSpeed;
        float speedDifference = targetSpeed - rigidbody.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, velocityPower) * Mathf.Sign(speedDifference);

        rigidbody.AddForce(movement * Vector2.right);

        #endregion

        /*if (isClimbing == true)
        {
            ChangeAnimationState(PLAYER_CLIMBING);
            rigidbody.AddForce(movement * Vector2.up);
            //rigidbody.gravityScale = 0;
        }*/

        // Friction
        if (lastOnGroundTime>0 && horizontalInput == 0)
        {
            float amount = Mathf.Min(Mathf.Abs(rigidbody.velocity.x), Mathf.Abs(friction));
            amount *= Mathf.Sign(rigidbody.velocity.x);
            rigidbody.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }


        /*if (spacePressed)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
            spacePressed = false;
        }*/

        if (rigidbody.velocity.y < 0)
        {
            rigidbody.gravityScale = gravityscale * fallGravityMultiplier;
        }
        else
        {
            rigidbody.gravityScale = gravityscale;
        }
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);

        currentState = newState;
    }

    private void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }

    private bool isOnGround()
    {
        return Physics2D.BoxCast(boxcollider.bounds.center, boxcollider.bounds.size, 0f, Vector2.down, .1f, jumpableTerrain);
    }

    private void Jump()
    {
        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isJumping = true;
        jumpInputReleased = false;
        lastJumpTime = 0.0f;
        lastOnGroundTime = 0.0f;
    }

    private void OnJump()
    {
        lastJumpTime = jumpBufferTime;
    }

    private void OnJumpUp()
    { 
        if (rigidbody.velocity.y > 0 && isJumping)
        {
            rigidbody.AddForce(Vector2.down * rigidbody.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }

        jumpInputReleased = true;
        lastJumpTime = 0;
    }
}
