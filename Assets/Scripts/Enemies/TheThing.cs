using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Utilities;

using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class TheThing : MonoBehaviour, IEnemy, IBoss
{
    Rigidbody2D rb2d;
    Animator anim;
    SpriteRenderer spriteRenderer;
    bool isDead = false;
    bool alerted = false;
    bool attacking = false;
    bool jumped = false;
    bool useRunVel = true;
    bool active = false;

    [SerializeField] Vector2 groundCheckSize, hurtBoxSize;
    [SerializeField] Transform foot, leftHitCenter, rightHitCenter;
    int maxHealth = 50;
    [SerializeField] int health = 50, damage = 10, auraGain = 5, pointCost = 10, timeGain = 10;

    [SerializeField] float moveSpeed = 4f, gravityMultiplier = 1.5f, eyeSight = 4f, jumpForce = 4f, stoppingDistance = 1f;
    [SerializeField] float jumpChance = 0.3f, attackChance = 0.3f;

    [SerializeField] LayerMask playerLayer;
    [SerializeField] Animator resurrectedVfx, effectAnim;
    [SerializeField] ParticleSystem scream;
    [SerializeField] GameObject FallingSpike, FloorSpike;

    Vector2 vel;
    Vector2 playerPos;
    Vector2 lookDir;

    void Start()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("Cannot locate Player Manager!");
            return;
        }

        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        maxHealth = health;

        SetLookDir();
    }

    void Update()
    {
        if (!active) return;

        if (IsDead())
        {
            rb2d.linearVelocityX = 0;
            return;
        }

        if (PlayerManager.Instance != null)
        {
            playerPos = new Vector2(
                PlayerManager.Instance.transform.position.x,
                PlayerManager.Instance.transform.position.y
            );
        }

        Move();
        Attack();
    }

    public void StartFight()
    {
        anim.SetTrigger("fight");
        active = true;
        alerted = true;
    }

    bool isGrounded()
    {
        var groundCollider = Physics2D.OverlapBox(
            foot.position,
            groundCheckSize,
            0,
            PlayerManager.Instance.groundMask
        );
        return groundCollider != null;
    }

    RunAfter jumpCoolDown;
    void Move()
    {
        anim.SetBool("grounded", isGrounded());

        if (isGrounded())
        {
            // initially stand still
            // when player is detected or an attack only injures chase the player
            if (alerted)
            {
                bool moveRight = transform.position.x < playerPos.x;

                if (!useRunVel)
                {
                    useRunVel = true;
                    // anim.Play("landing");
                    jumpCoolDown = new RunAfter(2f, ResetJump);
                }
                
                if (useRunVel)
                {
                    float dist2Player = Vector2.Distance(new Vector2(transform.position.x, playerPos.y), playerPos);
                    if (dist2Player > stoppingDistance && !attacking) rb2d.linearVelocityX = (moveRight) ? moveSpeed : -moveSpeed;
                    else rb2d.linearVelocityX = 0;
                }

                anim.SetFloat("speed", Mathf.Clamp01(Mathf.Abs(rb2d.linearVelocityX)));
                
                spriteRenderer.flipX = !moveRight;

                // randomly decide to jump
                Jump();

                // when close to the player randomly decide to attack
                Attack();
            }

            // apply const force incase enemy runs off a ledge/edge
            if (rb2d.linearVelocityY < 0)
            {
                rb2d.linearVelocityY = -2;
            }
        } else
            {
                // apply falling velocity
                rb2d.linearVelocityY += -10f * gravityMultiplier * Time.deltaTime;
            }
    }

    void ResetJump() { jumped = false; }
    void Jump()
    {
        if (jumped) return;

        Vector2 a = new Vector2(0, playerPos.y);
        Vector2 b = new Vector2(0, transform.position.y);

        float heightDiff = Vector2.Distance(a, b);

        bool playerInAir = heightDiff > 2f && !PlayerManager.Instance.grounded;
        bool playerOnHigherGround = heightDiff > 2f && PlayerManager.Instance.grounded;
        
        if (playerInAir || playerOnHigherGround)
        {
            float randValue = Random.Range(0f, 1f);
            if (randValue < jumpChance)
            {
                // anim.Play("jump");
                jumped = true;
                useRunVel = false;

                // give jump random direction
                float velX = rb2d.linearVelocityX;
                if (velX < 0) velX -= Random.Range(0, 4);
                else if (velX > 0) velX += Random.Range(0, 4);
                else velX += Random.Range(-4, 4);

                rb2d.linearVelocityX = velX;
                rb2d.linearVelocityY = jumpForce + Random.Range(0, 4);
            }
        }
    }

    Sleep attackDelay = null;
    Sleep specialDelay = null;
    void AttackFinished()
    {
        attacking = false;
        attackDelay = new Sleep(3f);
    }

    float t = 0;
    void Attack()
    {
        if (spriteRenderer == null || !alerted) return;
        if (attacking) return;

        float dist2Player = Vector2.Distance(transform.position, playerPos);
        if (dist2Player < stoppingDistance + 0.15f)
        {
            if (isGrounded())
            {
                t = 0;
                float randValue = Random.Range(0f, 1f);
                if ((attackDelay == null || attackDelay.Finished()) && randValue < attackChance)
                {
                    attacking = true;
                    
                    // skeleton will attack in the right direction
                    anim.SetInteger("attackType", Random.Range(0, 2));
                    anim.Play("pre-attack");

                    AnimationClip attackClip = anim.GetCurrentAnimatorClipInfo(0)[0].clip;
                    RunAfter evt = new RunAfter(attackClip.length * 0.85f, AttackFinished);

                }
            }
        } else
            {
                t += Time.deltaTime;
                if (t > 10)
                {
                    SpecialAttack();
                    t = 0;
                }
            }
    }
    public void CheckHurtBox() // Animation Event
    {
        Vector2 center = (!spriteRenderer.flipX) ? (Vector2)rightHitCenter.position : (Vector2)leftHitCenter.position;

        var hitObject = Physics2D.OverlapBox(
            center,
            hurtBoxSize,
            0,
            playerLayer
        );

        if (hitObject != null && hitObject.transform != null &&
            hitObject.transform.GetComponent<PlayerManager>() != null)
        {
            PlayerManager.Instance.TakeDamage(damage);
        }
    }

    void SpecialAttack()
    {
        // reduce spamming
        if (specialDelay != null && !specialDelay.Finished()) return;
        specialDelay = new Sleep(3f);

        scream.Play();
        PlayerManager.Instance.CameraShake();

        if (Random.Range(0,100) < 50)
        {
            new RunAfter(1, FallingSpikes);
        } else
            {
                new RunAfter(1, FloorSpikes);
            }
    }

    void FallingSpikes()
    {
        // spawn spikes above the player
        StartCoroutine(SpawnFallingSpikes());
    }
    IEnumerator SpawnFallingSpikes()
    {
        for (int i = 0; i < 10; ++i)
        {
            Vector3 pos = PlayerManager.Instance.transform.position + new Vector3(0, 10, 0);
            Instantiate(FallingSpike, pos, Quaternion.identity);
            yield return new WaitForSeconds(1);
        }
    }

    void FloorSpikes()
    {
        // spawn spikes on the player
        StartCoroutine(SpawnFloorSpikes());
    }
    IEnumerator SpawnFloorSpikes()
    {
        for (int i = 0; i < 10; ++i)
        {
            RaycastHit2D hit2d = Physics2D.Raycast(PlayerManager.Instance.transform.position, Vector2.down, 50, PlayerManager.Instance.groundMask);
            Instantiate(FloorSpike, hit2d.point, Quaternion.identity);
            yield return new WaitForSeconds(1);
        }
    }

    void Resurrect()
    {
        PlayerHUD.Instance.AuraFarmDetected();

        anim.Play("revive");
        resurrectedVfx.Play("activate");

        health = maxHealth * 2;
        damage *= 2;
        auraGain = 0;
        pointCost = (int)Mathf.Ceil((float)pointCost * 1.5f);

        new RunAfter(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length * 0.85f, ReviveFinished);
    }
    void ReviveFinished() { isDead = false; }

    bool effectActive = false;
    RunAfter effectAfter;
    public void Ignite(float duration)
    {
        if (isDead) return;
        if (effectActive) return;
        if (effectAnim != null) effectAnim.Play("ignite");

        effectActive = true;
        _ = DamageOverTime(1000);
        effectAfter = new RunAfter(duration, EndEffect);
    }
    async Task DamageOverTime(int delay)
    {
        while (effectActive)
        {
            if (isDead) return;
            TakeDamage(3, false);
            await Task.Delay(delay);
        }
    }
    void EndEffect()
    {
        effectActive = false;
        if (effectAnim != null) effectAnim.Play("no_effect");
    }

    int afterDeathHits = 0;
    bool enraged = false;
    public void TakeDamage(int amount, bool getAura)
    {
        PlayerManager.Instance.GainAura(auraGain);

        if (isDead)
        {
            ++afterDeathHits;
            if (afterDeathHits == 5)
            {
                Resurrect();
                enraged = true;
            }
            return;
        }

        health -= amount;
        alerted = true;
        
        if (health <= 0)
        {
            PlayerManager.Instance.GainPoints(pointCost);
            
            if (effectAfter != null) effectAfter.Stop();
            EndEffect();

            PlayerHUD.Instance.GainTime((!enraged) ? timeGain : timeGain * 3);

            health = 0;
            anim.Play("death");
            resurrectedVfx.Play("idle");
            isDead = true;
        } else
            {
                anim.Play("hurt");
            }
    }
    public bool IsDead() => isDead;

    void SetLookDir()
    {
        bool lookRight = Random.Range(0,100) % 3 == 0;
        lookDir = (lookRight) ? transform.right : -transform.right;

        if (lookRight)
        {
            spriteRenderer.flipX = true;
        }
    }

    void OnDrawGizmos()
    {
        if (foot == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(foot.position, groundCheckSize);

        if (leftHitCenter == null || rightHitCenter == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(leftHitCenter.position, hurtBoxSize);
        Gizmos.DrawWireCube(rightHitCenter.position, hurtBoxSize);

        // not always accurate, this is a reference to adjust eyeSight
        if (lookDir != Vector2.zero) Debug.DrawRay(transform.position, lookDir * eyeSight, Color.red);
    }
}//EndScript