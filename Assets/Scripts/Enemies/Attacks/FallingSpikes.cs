using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class FallingSpikes : MonoBehaviour
{
    Rigidbody2D rb2d;
    BoxCollider2D collider;
    Animator anim;
    SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem ps;
    [SerializeField] int damage = 5;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Destroy(gameObject, 20);
    }

    // animation target
    public void Fall()
    {
        rb2d.simulated = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Spike Landed");
        PlayerManager.Instance.CameraShake();

        if (!spriteRenderer.enabled) return;
        
        if (collision.gameObject.GetComponent<PlayerManager>() != null)
        {
            PlayerManager.Instance.TakeDamage(damage);
        }

        spriteRenderer.enabled = false;
        rb2d.simulated = false;
        collider.isTrigger = true;
        ps.Play();
    }
}//EndScript