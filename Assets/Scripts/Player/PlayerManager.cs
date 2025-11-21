using UnityEngine;
using System.Threading.Tasks;

// universal Singleton template object
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static bool quittingApp = false;

    public static T Instance
    {
        get // getter C# property
        {
            if (quittingApp) return null;
            if (instance != null) return instance;

            // Try to find one in the scene first
            instance = FindFirstObjectByType<T>();

            if (instance == null)
            {
                GameObject singletonObject = new GameObject(typeof(T).Name);
                instance = singletonObject.AddComponent<T>();
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            // create the new instance
            instance = this as T;
        } else if (instance != this)
            {
                // remove other instances
                Destroy(gameObject);
            }
    }

    // when the application quits perform the following
    void OnApplicationQuit()
    {
        quittingApp = true;
    }
}

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("Player Components")]
    [SerializeField] Animator playerAnimator;

    [Header("Player Stats")]
    public float moveSpeed = 4, jumpForce = 3, gravityMultiplier = 2;
    public int attackDamage = 10;
    [SerializeField] int health = 100;

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            isDead = true;
        }
    }

    [Header("Player States")]
    public bool grounded;
    public bool huggingWall;
    public bool gamePaused;
    public bool falling = false;
    bool isDead = false;

    public bool IsDead() => isDead;

    [Header("Raycast Layers")]
    public LayerMask groundMask;
    public LayerMask wallLayer;
    public LayerMask obstacleLayer;
    public LayerMask enemyLayer;
}