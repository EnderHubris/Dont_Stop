using UnityEngine;

public class EnergyBlast : MonoBehaviour
{
    [SerializeField] BoxCollider2D collider;
    [SerializeField] Animator anim;
    [SerializeField] int damage = 5;
    bool canDamage = false;
    
    void Start()
    {
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
    public void CameraShake() { PlayerManager.Instance.CameraShake(); }
    public void AllowDamage() { canDamage = true; }
    public void DisableDamage() { canDamage = false; }
}//EndScript