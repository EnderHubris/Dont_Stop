using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : Singleton<PlayerHUD>
{
    [SerializeField] Image healthBar, auraBar, levelBar;
    [SerializeField] Gradient healthGradient, auraGradient, levelGradient;
    [SerializeField] Text timerDisplay, levelDisplay;
    [SerializeField] float remainingTime = 90f;
    [SerializeField] Animator popup_text;

    void Start()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            _ = UpdateTimeDisplay();
        } else
            {
                // WebGL doesnt work well with async tasks like an executable would
                StartCoroutine(UpdateTimeDisplayWeb());
            }
    }

    void Update()
    {
        UpdateHealthBar();
        UpdateAuraBar();
        UpdateLevelBar();
    }

    void UpdateHealthBar()
    {
        float x = PlayerManager.Instance.GetHealthPercent();
        Vector3 nScale = new Vector3(x, 1, 1);

        healthBar.color = Color.Lerp(healthBar.color, healthGradient.Evaluate(x), 10 * Time.deltaTime);

        float dist = Vector3.Distance(healthBar.transform.localScale, nScale);
        
        if (dist > 0.1f)
            healthBar.transform.localScale = Vector3.Lerp(healthBar.transform.localScale, nScale, 10 * Time.deltaTime);
        else
            healthBar.transform.localScale = nScale;
    }

    void UpdateAuraBar()
    {
        float x = PlayerManager.Instance.GetAuraPercent();
        Vector3 nScale = new Vector3(x, 1, 1);

        auraBar.color = Color.Lerp(auraBar.color, auraGradient.Evaluate(x), 10 * Time.deltaTime);

        float dist = Vector3.Distance(auraBar.transform.localScale, nScale);
        
        if (dist > 0.1f)
            auraBar.transform.localScale = Vector3.Lerp(auraBar.transform.localScale, nScale, 10 * Time.deltaTime);
        else
            auraBar.transform.localScale = nScale;
    }
    
    void UpdateLevelBar()
    {
        float x = PlayerManager.Instance.GetLevelProgress();
        Vector3 nScale = new Vector3(x, 1, 1);

        levelBar.color = Color.Lerp(levelBar.color, levelGradient.Evaluate(x), 10 * Time.deltaTime);

        float dist = Vector3.Distance(levelBar.transform.localScale, nScale);
        
        if (dist > 0.1f)
            levelBar.transform.localScale = Vector3.Lerp(levelBar.transform.localScale, nScale, 10 * Time.deltaTime);
        else
            levelBar.transform.localScale = nScale;
    }
    public void UpdateLevelDisplay()
    {
        levelDisplay.text = PlayerManager.Instance.GetLevel().ToString();
    }

    async Task UpdateTimeDisplay()
    {
        while (remainingTime > 0)
        {
            // do not consume time when in a boss fight
            if (!PlayerManager.Instance.inBossFight)
                --remainingTime;
                
            ShowRemainingTime();
            await Task.Delay(1000); // wait 1 second
        }

        // player died from running out of time
        PlayerManager.Instance.InstantKill();
        remainingTime = 0;
    }
    IEnumerator UpdateTimeDisplayWeb()
    {
        while (remainingTime > 0)
        {
            // do not consume time when in a boss fight
            if (!PlayerManager.Instance.inBossFight)
                --remainingTime;
                
            ShowRemainingTime();
            yield return new WaitForSeconds(1f);
        }

        // player died from running out of time
        PlayerManager.Instance.InstantKill();
        remainingTime = 0;
    }

    void ShowRemainingTime()
    {
        int totalSeconds = Mathf.FloorToInt(remainingTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string timerString = $"{minutes:00}:{seconds:00}";
        timerDisplay.text = timerString;
    }
    public void GainTime(int amount)
    {
        remainingTime += amount;
        ShowRemainingTime(); // update time display
    }

    bool warningShown = false;
    public void AuraFarmDetected()
    {
        if (warningShown) return;
        
        warningShown = true;
        popup_text.Play("aura_farm_pop_up");
    }
}//EndScript