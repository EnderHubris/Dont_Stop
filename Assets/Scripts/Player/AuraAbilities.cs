using Utilities;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public abstract class Ability : ScriptableObject
{
    // shared variables between derived classes
    [SerializeField] protected int cost;
    [SerializeField] protected float delay;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected string AbilityName;
    [SerializeField] Color baseColor;

    // shared method between derived classes
    protected bool EnoughAura()
    {
        int currentAura = PlayerManager.Instance.GetAuraAmount();
        return currentAura >= cost;
    }

    public Color GetBaseColor() => baseColor;
    public Sprite GetIconSprite() => icon;
    public string GetName() => AbilityName;

    // derived classes must implement this function
    public abstract void Perform();
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class AuraAbilities : MonoBehaviour
{
    [SerializeField] Ability[] abilities;
    Animator abilityVfx;
    SpriteRenderer vfxRenderer;

    [SerializeField] Image iconImage;
    [SerializeField] int selected = 0;

    void Start()
    {
        abilityVfx = GetComponent<Animator>();
        vfxRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (abilities.Length == 0) return;
        if (abilityVfx == null) return;

        if (PlayerInput.PressedPrevSkill())
        {
            Debug.Log("Pressed L-DPAD");
            selected = (selected - 1 + abilities.Length) % abilities.Length;
        } else if (PlayerInput.PressedNextSkill())
            {
                Debug.Log("Pressed R-DPAD");
                selected = ++selected % abilities.Length;
            }

        UpdateIcon();

        if (PlayerInput.PressedSkillButton())
        {
            Ability activeAbility = abilities[selected];
            
            UpdateColor(activeAbility);
            abilityVfx.SetFloat("effectType", selected);
            
            if (activeAbility != null)
            {
                abilityVfx.Play("activate");
                activeAbility.Perform();
            }
        }
    }

    void UpdateIcon()
    {
        Ability activeAbility = abilities[selected];
        
        // Debug.Log($"Active Ability -> {activeAbility.GetName()}");

        if (iconImage != null && activeAbility != null)
            iconImage.sprite = activeAbility.GetIconSprite();
    }

    void UpdateColor(Ability ability)
    {
        if (ability != null && vfxRenderer != null)
        {
            vfxRenderer.color = ability.GetBaseColor();
        }
    }
}//EndScript