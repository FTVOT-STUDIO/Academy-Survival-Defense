using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Inst;

    [Header("#BGM ====")]
    public AudioClip[] bgmClips;
    public float bgmVolume;
    AudioSource bgmPlayer;
    int bgmIndex;
    [Space]
    [Header("#Environment Sound ====")]
    public AudioClip[] envClips;
    public float envVolume;
    AudioSource envPlayer;
    int envIndex;
    [Space]
    [Header("#SFX ====")]
    public AudioClip[] sfxClip;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayer;
    int channelIndex;
    [Space]
    public int maxSfxCount = 3;
    [Space]
    [Header("#Filter ====")]



    [Header("#Audio Effect")]
    AudioLowPassFilter audioDivingEffect;
    AudioReverbFilter audioCaveFilter;
    AudioHighPassFilter audioHighPassFilter;
    AudioLowPassFilter audioLowPassFilter;

    public enum Bgm
    {
        Normal, LobbyBgm, Hard, Result, Metatron
    }

    public enum Env
    {
        Rain
    }

    public enum Sfx
    {
        ButtonClick1, ButtonClick2, SpawnClick, Attack, EnemyHit, Explo1, Explo2, Gravity, vibration,
        AbiltyOn, AbiltyGage, LevelUpPanel, SpawnUnit, Attack1, SelectCard, Boomb, TryInstDeath, OnWaveChanged, Merge,
        Reward, Select1, Select2, Select3, LevelUp, Upgrade, Sword, SpawnEnemy, EliteEnemy, damaged, EnemyDead, UnitDead, UniiDead,
        Noise, ShotGun, ShotGun2, GameWin, GameLose, Attack2, DotDamage, DingDong, ClearBuff, Laugh0, Illusion, UnitDeActive,
        Alert, Bells, DownGrade, DJ, ScreenChange0, ScreenChange1, FocusOn, ScreenChange2, Boy, Girl, KO, Star, Void,
        select4, lux, select5, BryceEffect, Laugh1, Trans, StarLightOn0, StarLightOn1, StarLightOn2, Laser,
        ScalarUXIntro0, ScalarUXIntro1, ScalarUXIntro2, ScalarUXSkill0, ScalarUXSkill1, ScalarUXSkill2, ScalarUXSkill3, ScalarUXSkill4, ScalarUXSkill5,
        SnowWhiteTrans0, SnowWhiteTrans1, SnowWhiteTrans2, SnowWhiteAttack0, SnowWhiteAttack1, SnowWhiteAttack2, SnowWhiteAttack3, SnowWhiteAttack4,
        select6, select7, spin0, spin1, aura, boxOpen, Special, aura1, FocusOn1, GetItem, BuyItem, FocusOn2, select8, punch, airPlane, bomb, bombCount, skillGas,
        GevurahGun, Reward1, Sword1, Wings, Siren, cheers, CyclopsIntro0, CyclopsIntro1, CyclopsSkill0, CyclopsSkill1, CyclopsSkill2, CyclopsSkill3, CyclopsSkill4, CyclopsSkill5,
        QuestClaer, DataLoad, BatHit, HomeRun, KohakuAblity, Sword2, Fire, GlassBreak, FingerSnap, RoboPunch1, Title, ClockTicking, Spotlight, MonsterLaugh, Danso,
        JapenIntro, PaperOpen, CartoonRunning, Children, Padding, Sword3, Screem, SlowMotion, Crash, Trumpet,
        HeartBeat, Success0, Success1, Success2, Fail, BaseBallIntro, Drills, Hammer, RummagingBag, Piyoong, Idea, cheering, Crash1, EpicaSkill0, Idea2, Fall, Explo3, EpicaSkill1, MalkuthTrans, ZoomojiTransFail, Stress, ShotGun3, Explo4, Explo5, Stress2, Alert2, Shotdown, CoinSell, ShockBeatSkill, GameResult, Explo6, EarthQuake, Explo7, Electronic, Epic, Glitch, Alert3, Chord, Explo8, InputBeep, OutputBeep, MultiBelling, EagleScream, MalkuthAbilityOff, Explo9, MalkuthStun, TimeStop, MalkuthAbilityGet, malkuthAttackSpeed, Heal, Slience, Silence1, ButtonClick3, Hologram, ButtonClick4, Laser2, HodAbility0, HodAbility1, HodKill, HodScream, HodSummon0, HodSummon1, ChokmahManaRegen, ChokmahTrans, Keyboard, Gas, ZetaAbility, GamchicothAppear0, GamchicothAppear1, GamchicothAppear2, GamchicothSkill0, GamchicothSkill1, GamchicothSkill2, GamchicothAppear3, GamchicothSkill3, InGameUpgrade, SupOn, SupOff, VoidClusterSkill, VoidClusterStun,
        BinahCheatCode, ChesedCheatCode, ChokmahCheatCode, GevurahCheatCode, HodCheatCode, KehterCheatCode, MalkuthCheatCode, NetzachCheatCode, TipherethCheatCode, YesodCheatCode, KetherAbility1, KetherAbility2, TiferetSkill0, TiferetSkill1, TiferetAbility0, TiferetAbility1, TiferetAbility2, TiferetAbility3, TiferetAbility4, TiferetAbility5,
        NemesisSkillEffect0, NemesisSkillEffect1, NemesisSkillEffect2, NemesisTerrifiedEffect0, NemesisTerrifiedEffect1, NemesisSkillEffect3, NetzachAbility0, NetzachAbility1, NetzachAbility2, NetzachAbility3, NetzachAbility4, NetzachAttack, NetzachSameTarget, NetzachCurHp, NetzachInstDeath, NetzachLostHp, NetzachAbility5, IrisApear0, IrisApear1, IrisApear2, IrisSkill, IrisSpin, IrisSkill1, IrisSkill2, Siren1, IgnisSkill0, IgnisSkill1, IgnisSuperAttack, IgnisTransOff, IgnisTransOn0, IgnisTransOn1, IgnisSkill2, ScalarUiSkill, speedToggleOn, speedToggleOff, AbilityReady1, AbilityReady2

    }

    void Awake()
    {
        Inst = this;
        Init();
    }

    void OnDestroy()
    {
        bgmPlayer = null;
        audioDivingEffect = null;
        audioCaveFilter = null;

        for (int i = 0; i < sfxPlayer.Length; i++)
        {
            sfxPlayer[i] = null;
        }
    }

    void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;

        GameObject envObject = new GameObject("EnvPlayer");
        envObject.transform.parent = transform;
        envPlayer = envObject.AddComponent<AudioSource>();
        envPlayer.playOnAwake = false;
        envPlayer.loop = true;
        envPlayer.volume = envVolume;

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayer = new AudioSource[channels];

        for (int index = 0; index < sfxPlayer.Length; index++)
        {
            sfxPlayer[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayer[index].playOnAwake = false;
            sfxPlayer[index].volume = sfxVolume;
        }

        audioDivingEffect = Camera.main.GetComponent<AudioLowPassFilter>();
        audioCaveFilter = Camera.main.GetComponent<AudioReverbFilter>();
        audioHighPassFilter = Camera.main.GetComponent<AudioHighPassFilter>();
        audioLowPassFilter = Camera.main.GetComponent<AudioLowPassFilter>();
    }

    public void PlayBgm(Bgm bgm)
    {
        int index = (int)bgm;
        if (index < 0 || index >= bgmClips.Length) return;

        bgmIndex = index;

        bgmPlayer.clip = bgmClips[bgmIndex];

        bgmPlayer.Play();
    }

    public void PlayEnv(Env env)
    {
        int index = (int)env;
        if (index < 0 || index >= envClips.Length) return;

        envIndex = index;
        envPlayer.clip = envClips[envIndex];
        envPlayer.Play();
    }

    public void PlaySfx(Sfx sfx)
    {

        int playingCount = 0;
        AudioClip targetClip = sfxClip[(int)sfx];
        foreach (var sfxSource in sfxPlayer)
        {
            if (sfxSource.isPlaying && sfxSource.clip == targetClip)
            {
                playingCount++;
                if (playingCount >= maxSfxCount)
                    return;
            }
        }

        for (int index = 0; index < sfxPlayer.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayer.Length;

            if (sfxPlayer[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayer[loopIndex].clip = targetClip;
            sfxPlayer[loopIndex].Play();
            break;
        }
    }
    public void StopBgm()
    {
        bgmPlayer.Stop();
    }

    public void StopEnv()
    {
        if (envPlayer.isPlaying)
        {
            envPlayer.Stop();
        }
    }

    public void StopSfx(Sfx sfx)
    {
        foreach (AudioSource sfxSource in sfxPlayer)
        {
            if (sfxSource.isPlaying && sfxSource.clip == sfxClip[(int)sfx])
            {
                sfxSource.Stop();
                break;
            }
        }
    }

    public void StopAllSfx()
    {
        foreach (AudioSource sfxSource in sfxPlayer)
        {
            if (sfxSource.isPlaying)
            {
                sfxSource.Stop();
            }
        }
    }

    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp(volume, 0f, 1f);
        bgmPlayer.volume = bgmVolume;
    }

    public float GetBgmVolume()
    {
        return bgmPlayer.volume;
    }

    public void SetEnvVolume(float volume)
    {
        envVolume = Mathf.Clamp(volume, 0f, 1f);
        envPlayer.volume = envVolume;
    }

    public float GetEnvVolume()
    {
        return envPlayer.volume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp(volume, 0f, 1f);
        foreach (AudioSource sfxSource in sfxPlayer)
        {
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
        }
    }

    public float GetSfxVolume()
    {
        return sfxVolume;
    }

    public void AudioLowPassFilter(bool isPlay)
    {
        if (audioLowPassFilter == null)
            return;
        audioLowPassFilter.enabled = isPlay;
    }

    public void GameStopEffect(bool isPlay)
    {
        if (audioHighPassFilter == null)
            return;

        audioHighPassFilter.enabled = isPlay;
    }
}
