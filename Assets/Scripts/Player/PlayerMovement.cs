using UnityEngine;

public static class PlayerInput
{
    public static bool PressedJump() => Input.GetKeyDown(KeyCode. Space);
    public static bool HoldingJump() => Input.GetKey(KeyCode. Space);
    public static bool MovingLeft() => Input.GetKey(KeyCode. A);
    public static bool MovingRight() => Input.GetKey(KeyCode. D);
    public static bool PressedAttack() => Input.GetButtonDown("Fire1");
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb2d;
    Animator anim;

    [SerializeField] Vector2 groundCheckSize;
    [SerializeField] Transform playerFoot;

    void Start()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("Cannot locate Player Manager!");
            return;
        }

        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (PlayerManager.Instance.IsDead()) return;

        Move();
    }

    bool isGrounded()
    {
        var groundCollider = Physics2D.OverlapBox(
            playerFoot.position,
            groundCheckSize,
            0,
            PlayerManager.Instance.groundMask
        );
        return groundCollider != null;
    }

    void Move()
    {
        float velx = 0;
        if (PlayerInput.MovingRight()) velx = PlayerManager.Instance.moveSpeed;
        else if (PlayerInput.MovingLeft()) velx = -PlayerManager.Instance.moveSpeed;

        rb2d.linearVelocity = new Vector2(velx, rb2d.linearVelocityY);
        
        PlayerManager.Instance.grounded = isGrounded();

        if (isGrounded())
        {
            PlayerManager.Instance.falling = false;
            if (PlayerInput.PressedJump())
            {
                rb2d.linearVelocity = new Vector2(rb2d.linearVelocityX, PlayerManager.Instance.jumpForce);
            }
        } else
            {
                if (!PlayerManager.Instance.falling)
                {
                    if (rb2d.linearVelocityY <= 0) PlayerManager.Instance.falling = true;

                    // mid-jump
                    if (!PlayerInput.HoldingJump() && rb2d.linearVelocityY > 0)
                    {
                        rb2d.linearVelocity = new Vector2(rb2d.linearVelocityX, 0);
                        PlayerManager.Instance.falling = true;
                    }
                } else
                    {
                        // falling
                        rb2d.linearVelocityY += -10 * PlayerManager.Instance.gravityMultiplier * Time.deltaTime;
                    }
            }
    }

    void OnDrawGizmos()
    {
        if (playerFoot == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(playerFoot.position, groundCheckSize);
    }
}//EndScript