using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Animator))]
public class Flame : MonoBehaviour
{
    [SerializeField] Gradient flameColor;
    [SerializeField] Light2D flameLight;
    [SerializeField] int flameType = 0;
    [SerializeField] float colorShiftSpeed = 2f;

    Animator anim;
    float gIndex;

    void Start()
    {
        anim = GetComponent<Animator>();
        ChangeGradientIndex();
    }

    void ChangeGradientIndex()
    {
        gIndex = Random.Range(0f, 1f);
        Invoke("ChangeGradientIndex", 3f);
    }

    void Update()
    {
        anim.SetFloat("flameType", flameType);
        UpdateFlameColor();
    }

    void UpdateFlameColor()
    {
        flameLight.color = Color.Lerp(
            flameLight.color,
            flameColor.Evaluate(0.5f),
            colorShiftSpeed * Time.deltaTime
        );
    }
}//EndScript