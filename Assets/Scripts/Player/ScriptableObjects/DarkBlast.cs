using Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dark Blast")]
public class DarkBlast : Ability
{
    [SerializeField] GameObject DarkBlastLeft, DarkBlastRight;
    [SerializeField] AnimationClip chargingAnimation;

    public override void Perform()
    {
        if (EnoughAura())
        {
            PlayerManager.Instance.ConsumeAura(cost);
            new RunAfter(delay, ChargeBlast);
        }
    }

    void ChargeBlast()
    {
        // spawn in dark blast prefab
        new RunAfter(chargingAnimation.length, ShootBlast);
    }

    void ShootBlast()
    {
        Instantiate(
            PlayerManager.Instance.lookingLeft ? DarkBlastLeft : DarkBlastRight,
            PlayerManager.Instance.transform.position, 
            Quaternion.identity
        );
    }
}