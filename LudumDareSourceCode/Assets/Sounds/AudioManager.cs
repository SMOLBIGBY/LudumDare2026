using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    public AudioClip menuMusic;
    public AudioClip levelMusic;
    public AudioClip ghostMovement;
    public AudioClip deathSound;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private bool isReady = false;

    void Start()
    {
        float musicVol = PlayerPrefs.GetFloat("musicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("sfxVolume", 1f);

        musicSource.volume = musicVol;
        sfxSource.volume = sfxVol;

        // prevent triggering events while setting initial values
        musicSlider.SetValueWithoutNotify(musicVol);
        sfxSlider.SetValueWithoutNotify(sfxVol);

        isReady = true;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }
    void Update()
    {
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }

    public void SetMusicVolume(float volume)
    {
        Debug.Log("Music slider: " + volume);

        if (!isReady) return;

        musicSource.volume = volume;
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    public void SetSFXVolume(float volume)
    {
        Debug.Log("SFX slider: " + volume);

        if (!isReady) return;

        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
}