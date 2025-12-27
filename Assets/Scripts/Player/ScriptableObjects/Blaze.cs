using Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Blaze")]
public class Blaze : Ability
{
    [SerializeField] float duration = 2f;

    public override void Perform()
    {
        if (EnoughAura())
        {
            PlayerManager.Instance.ConsumeAura(cost);
            new RunAfter(delay, BlazeBlast);
        }
    }
    Collider2D[] FindTargets()
    {
        Vector3 playerPosition = PlayerManager.Instance.transform.position;

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            playerPosition,
            PlayerManager.Instance.targetRange,
            PlayerManager.Instance.enemyLayer
        );

        return targets;
    }
    void BlazeBlast()
    {
        Collider2D[] targets = FindTargets();
        foreach (Collider2D target in targets)
        {
            IEnemy enemy = target.GetComponent<IEnemy>();
            if (enemy != null)
            {
                if (!enemy.IsDead())
                    enemy.Ignite(duration);
            }
        }
    }
}