using Utilities;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Threading.Tasks;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] float shiftSpeed = 5;
    [SerializeField] AudioSource BossIntro, BossMain, GameMain;
    bool GameMusicPresent() => BossIntro != null && BossMain != null && GameMain != null;

    [SerializeField] AudioSource playerHit, jumpSfx, itemPickup;
    [SerializeField] AudioSource enemyHit, bossHit, screamSfx;
    [SerializeField] AudioSource floorSpikeSfx, fallingSpikeSfx, slashImpactSfx, energyBlastSfx;

    public void PlayPlayerHit() { if (playerHit != null) playerHit.Play(); }
    public void PlayJumpSfx() { if (jumpSfx != null) jumpSfx.Play(); }
    public void PlayPickupSfx() { if (itemPickup != null) itemPickup.Play(); }
    
    public void PlayEnemyHit() { if (enemyHit != null) enemyHit.Play(); }
    public void PlayBossHit() { if (bossHit != null) bossHit.Play(); }
    public void PlayScream() { if (screamSfx != null) screamSfx.Play(); }
    
    public void PlayFloorSpikeSfx() { if (floorSpikeSfx != null) floorSpikeSfx.Play(); }
    public void PlaySpikeImpactSfx() { if (fallingSpikeSfx != null) fallingSpikeSfx.Play(); }
    public void PlaySlashSfx() { if (slashImpactSfx != null) slashImpactSfx.Play(); }
    public void PlayBlastSfx() { if (energyBlastSfx != null) energyBlastSfx.Play(); }
    
    void Start()
    {
        IntroAudio();
    }

    public void PlayBossIntro()
    {
        if (!GameMusicPresent()) return;

        StopCoroutine(QuietBossMusic());
        StopCoroutine(QuietBossIntroMusic());
        StopCoroutine(RunMainMusic());

        StartCoroutine(QuietMainMusic());
        StartCoroutine(RunBossIntroMusic());
    }
    
    IEnumerator QuietMainMusic()
    {
        while (GameMain.volume > 0.1f)
        {
            GameMain.volume = Mathf.Lerp(GameMain.volume, 0, shiftSpeed * Time.deltaTime);
            yield return null;
        }
        GameMain.volume = 0;
    }
    
    IEnumerator RunMainMusic()
    {
        GameMain.Play();

        while (GameMain.volume < 0.7f)
        {
            GameMain.volume = Mathf.Lerp(GameMain.volume, 0.8f, shiftSpeed * Time.deltaTime);
            yield return null;
        }
        GameMain.volume = 0.8f;
    }

    public void BossEnded()
    {
        if (!GameMusicPresent()) return;

        StartCoroutine(QuietBossMusic());
        StartCoroutine(QuietBossIntroMusic());
        StartCoroutine(RunMainMusic());
    }
    
    IEnumerator QuietBossMusic()
    {
        while (BossMain.volume > 0.1f)
        {
            BossMain.volume = Mathf.Lerp(BossMain.volume, 0, shiftSpeed * Time.deltaTime);
            yield return null;
        }
        BossMain.volume = 0;
        BossMain.Stop();
    }
    
    IEnumerator QuietBossIntroMusic()
    {
        while (BossIntro.volume > 0.1f)
        {
            BossIntro.volume = Mathf.Lerp(BossIntro.volume, 0, shiftSpeed * Time.deltaTime);
            yield return null;
        }
        BossIntro.volume = 0;
    }
    
    IEnumerator RunBossIntroMusic()
    {
        BossIntro.Play();

        double startTime = AudioSettings.dspTime + 0.1;
        BossMain.PlayScheduled(startTime + BossIntro.clip.length);

        while (BossIntro.volume < 0.7f)
        {
            BossIntro.volume = Mathf.Lerp(BossIntro.volume, 0.8f, shiftSpeed * Time.deltaTime);
            yield return null;
        }
        BossIntro.volume = 0.8f;
    }

//=======================================================================

    // hard set all sources to 0 volume
    void MuteAll()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (var src in sources)
        {
            src.volume = 0;
        }
    }

    void AmpAll()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (var src in sources)
        {
            if (src.transform.tag != "sfx") src.volume = 0.8f;
            else src.volume = 0.4f;
        }
    }

    // all audio sources start at zero and slowly
    // amplify to target audio volume
    void IntroAudio()
    {
        MuteAll();

        StopCoroutine(TurnDown());
        StartCoroutine(TurnUp());
    }

    IEnumerator TurnUp()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        bool loop = true;

        while (loop)
        {
            foreach (var src in sources)
            {
                if (src.transform.tag != "sfx") src.volume = Mathf.Lerp(src.volume, 0.8f, shiftSpeed * Time.deltaTime);
                else src.volume = Mathf.Lerp(src.volume, 0.4f, shiftSpeed * Time.deltaTime);
                
                if (src.transform.tag != "sfx" && src.volume > 0.7f)
                {
                    loop = false;
                    break;
                }
            }
            yield return null;
        }

        AmpAll();
    }

    // trigger on scene transition
    public void Transitioning() { QuietAll(); }

    // deamplify to zero volume
    void QuietAll()
    {
        StopCoroutine(TurnUp());
        StartCoroutine(TurnDown());
    }

    IEnumerator TurnDown()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        bool loop = true;

        while (loop)
        {
            foreach (var src in sources)
            {
                src.volume = Mathf.Lerp(src.volume, 0, shiftSpeed * Time.deltaTime);
                if (src.transform.tag != "sfx" && src.volume < 0.1f)
                {
                    loop = false;
                    break;
                }
            }
            yield return null;
        }

        MuteAll();
    }
}//EndScript