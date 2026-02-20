using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioClip jumpClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    public Slider bgmSlider;
    public Slider sfxSlider;

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string BGM_TOGGLE_KEY = "BGMToggle";

    void Start()
    {
        if (bgmSlider != null)
            bgmSlider.value = GetBGMVolume();

        if (sfxSlider != null)
            sfxSlider.value = GetSFXVolume();
    }

    public float GetBGMVolume()
    {
        return bgmSource.volume;
    }

    public float GetSFXVolume()
    {
        return sfxSource.volume;
    }

    void Awake()
    {
        instance = this;
        LoadSettings();
        SetupBGM();
    }

    void SetupBGM()
    {
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;

        bool isOn = PlayerPrefs.GetInt(BGM_TOGGLE_KEY, 1) == 1;

        if (isOn)
            bgmSource.Play();
        else
            bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void ToggleBGM(bool isOn)
    {
        PlayerPrefs.SetInt(BGM_TOGGLE_KEY, isOn ? 1 : 0);
        PlayerPrefs.Save();

        if (isOn)
        {
            if (!bgmSource.isPlaying)
                bgmSource.Play();
        }
        else
        {
            bgmSource.Stop();
        }
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    void LoadSettings()
    {
        float bgmVol = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        bgmSource.volume = bgmVol;
        sfxSource.volume = sfxVol;
    }
}