using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource audioSource;
    private AudioSource musicSource;

    [Header("Sonidos del Jugador")]
    public AudioClip playerWalk;
    public AudioClip playerJump;
    public AudioClip playerAttackPunch;
    public AudioClip playerAttackSword;

    [Header("Sonidos de Enemigos")]
    public AudioClip demonAttack;

    [Header("Música de Fondo")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)]
    public float musicVolume = 0.1f;

    [Header("Configuración")]
    [Range(0f, 1f)]
    public float volume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSource()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic() => musicSource.Stop();

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip, volume);
    }

    // Solo reproduce si el enemigo está visible en cámara
    public void PlayDemonAttack(GameObject demon)
    {
        if (demon == null) return;

        // Verificar si el renderer del demonio está visible en cámara
        Renderer demonRenderer = demon.GetComponent<Renderer>();

        if (demonRenderer != null && demonRenderer.isVisible)
        {
            PlaySound(demonAttack);
            Debug.Log($"[AudioManager] Sonido de ataque - {demon.name} visible en cámara");
        }
        else
        {
            Debug.Log($"[AudioManager] {demon.name} fuera de cámara - sonido omitido");
        }
    }

    public void PlayWalkSound() => PlaySound(playerWalk);
    public void PlayJumpSound() => PlaySound(playerJump);
    public void PlayPunchAttack() => PlaySound(playerAttackPunch);
    public void PlaySwordAttack() => PlaySound(playerAttackSword);

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
}