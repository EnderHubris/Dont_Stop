using Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Healing")]
public class Healing : Ability
{
    [SerializeField] [Range(0f,1f)] float healAmount = 0.35f;

    public override void Perform()
    {
        if (EnoughAura())
        {
            PlayerManager.Instance.ConsumeAura(cost);
            new RunAfter(delay, GiveHealth);
        }
    }
    void GiveHealth()
    {
        PlayerManager.Instance.GainHealth(healAmount);
    }
}