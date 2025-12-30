using Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Lightning")]
public class Lightning : Ability
{
    [SerializeField] int damage = 10;

    public override void Perform()
    {
        if (EnoughAura())
        {
            PlayerManager.Instance.ConsumeAura(cost);
            new RunAfter(delay, LightningBlast);
        }
    }

    Collider2D[] FindTargets()
    {
        Vector3 playerPosition = PlayerManager.Instance.transform.position;

        float range = PlayerManager.Instance.targetRange;
        range *= 0.75f;

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            playerPosition,
            range,
            PlayerManager.Instance.enemyLayer
        );

        return targets;
    }
    void LightningBlast()
    {
        bool playSound = true;
        Collider2D[] targets = FindTargets();
        foreach (Collider2D target in targets)
        {
            if (playSound)
            {
                // AudioManager.Instance.PlayBlazeSfx();
                playSound = false;
            }

            IEnemy enemy = target.GetComponent<IEnemy>();
            if (enemy != null)
            {
                if (!enemy.IsDead()) enemy.Shock(damage);
            }
        }
    }
}