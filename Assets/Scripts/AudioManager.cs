using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Patrón Singleton
    public static AudioManager Instance { get; private set; }

    private AudioSource audioSource;

    [Header("Sonidos del Jugador")]
    public AudioClip playerWalk;
    public AudioClip playerJump;
    public AudioClip playerAttackPunch;
    public AudioClip playerAttackSword;

    [Header("Sonidos de Enemigos")]
    public AudioClip demonAttack;

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
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayWalkSound() => PlaySound(playerWalk);
    public void PlayJumpSound() => PlaySound(playerJump);
    public void PlayPunchAttack() => PlaySound(playerAttackPunch);
    public void PlaySwordAttack() => PlaySound(playerAttackSword);
    public void PlayDemonAttack() => PlaySound(demonAttack);

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
}
