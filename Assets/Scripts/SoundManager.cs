using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Footstep,
    Sneeze,
    Music
}

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSourceFootstep = null;
    private AudioSource audioSourceSneeze = null;
    private AudioSource audioSourceMusic = null;
    public AudioClip footstepSound = null;
    public AudioClip sneezeSound = null;
    public AudioClip music = null;
    private float timeWaitedBetweenFootstepSound;

    public void Start()
    {
        audioSourceFootstep = gameObject.AddComponent<AudioSource>();
        audioSourceSneeze   = gameObject.AddComponent<AudioSource>();
        audioSourceMusic    = gameObject.AddComponent<AudioSource>();

        timeWaitedBetweenFootstepSound = Random.Range(-0.1f, 1.5f);
    }

    public void PlaySound(SoundType soundType, float volume)
    {
        switch (soundType)
        {
            case SoundType.Footstep:
                audioSourceFootstep.volume = volume;
                audioSourceFootstep.PlayOneShot(footstepSound);
                break;
            case SoundType.Sneeze:
                audioSourceSneeze.volume = volume;
                audioSourceSneeze.PlayOneShot(sneezeSound);
                break;
            case SoundType.Music:
                audioSourceMusic.volume = volume;
                audioSourceMusic.loop = true;
                audioSourceMusic.clip = music;
                audioSourceMusic.Play();
                break;
        }
    }

    public void PlayFootstepSound(float timeToWaitBetweenFootstepSound = 2.0f, float volume = 0.3f)
    {
        timeWaitedBetweenFootstepSound += Time.fixedDeltaTime;

        if (timeWaitedBetweenFootstepSound >= timeToWaitBetweenFootstepSound)
        {
            timeWaitedBetweenFootstepSound = 0.0f;
            PlaySound(SoundType.Footstep, volume);
        }
    }
}
