using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class Jumppad : MonoBehaviour
{
    [SerializeField] float jumpBoost = 2.5f;
    
    Animator anim;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        PlayerManager.Instance.launchPadMultiplier = 1;
    }

    void OnTriggerEnter2D(Collider2D collider2d)
    {
        if (collider2d.GetComponent<PlayerManager>() != null)
        {
            PlayerManager.Instance.launchPadMultiplier = jumpBoost;
        }
    }
    void OnTriggerStay2D(Collider2D collider2d)
    {
        if (collider2d.GetComponent<PlayerManager>() != null)
        {
            if (PlayerInput.PressedJump())
            {
                anim.Play("activate", 0, 0f);
            }
        }
    }
    void OnTriggerExit2D(Collider2D collider2d)
    {
        if (collider2d.GetComponent<PlayerManager>() != null)
        {
            PlayerManager.Instance.launchPadMultiplier = 1;
        }
    }
}//EndScript