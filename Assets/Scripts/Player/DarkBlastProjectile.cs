using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DarkBlastProjectile : MonoBehaviour
{
    Rigidbody2D rb2d;
    PolygonCollider2D collider;
    [SerializeField] int damage = 2;
    [SerializeField] float speed = 3f;
    [SerializeField] bool moveLeft = false;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();

        if (moveLeft)
        {
            rb2d.linearVelocity = Vector2.left * speed;
        } else
            {
                rb2d.linearVelocity = Vector2.left * speed * -1;
            }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerManager.Instance.CameraShake();
        // AudioManager.Instance.PlaySlashSfx();
        
        if (collision.gameObject.GetComponent<IEnemy>() != null)
        {
            collision.gameObject.GetComponent<IEnemy>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}//EndScript