using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class FloorSpikes : MonoBehaviour
{
    BoxCollider2D collider;
    Animator anim;
    bool canDamage = false;
    [SerializeField] int damage = 10;
    
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        transform.position += Vector3.up * 0.5f;

        Destroy(gameObject, 10);
    }

    void Update()
    {
        if (canDamage) CheckHitbox();
    }

    void CheckHitbox()
    {
        Collider2D[] targets = Physics2D.OverlapBoxAll(transform.position, collider.size, 0);
        foreach (var target in targets)
        {
            if (target.GetComponent<PlayerManager>() != null)
            {
                PlayerManager.Instance.TakeDamage(damage);
                canDamage = false;
            }
        }
    }

    // animation target
    public void AllowDamage() { canDamage = true; }
    public void DisableDamage() { canDamage = false; }
}//EndScript