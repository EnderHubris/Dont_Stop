using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class FurySlash : MonoBehaviour
{
    Rigidbody2D rb2d;
    PolygonCollider2D collider;
    [SerializeField] int damage = 2;
    [SerializeField] float speed = 3f;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();

        SetRotation();
        SetTrajectory();
    }

    void SetRotation()
    {
        Vector3 target = PlayerManager.Instance.transform.position;
        Vector3 start = transform.position;
        target.z = start.z = 0;

        Vector2 dir = (target - start).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void SetTrajectory()
    {
        Vector3 target = PlayerManager.Instance.transform.position;
        Vector3 start = transform.position;
        target.z = start.z = 0;

        Vector3 dir = (target - start).normalized;
        rb2d.linearVelocity = (Vector2)dir * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Slice Landed");
        PlayerManager.Instance.CameraShake();
        AudioManager.Instance.PlaySlashSfx();
        
        if (collision.gameObject.GetComponent<PlayerManager>() != null)
        {
            PlayerManager.Instance.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}//EndScript