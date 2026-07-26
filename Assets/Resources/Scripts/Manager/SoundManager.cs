using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource countDownAudioSource;
    [SerializeField] private AudioSource bgmDownAudioSource;
    [SerializeField] private AudioSource windAudioSource;

    public AudioClip laserSound;
    public List<AudioClip> countDownSound;
    public List<AudioClip> countDownSoundError;

    public void PlaySound(AudioClip clip)
    {
        countDownAudioSource.PlayOneShot(clip);
    }

    public void PlayWindSound()
    {
        windAudioSource.Play();
    }
}
