using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Footstep,
    Sneeze
}

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource = null;
    public AudioClip footstepSound = null;
    public AudioClip sneezeSound = null;
    private float timeWaitedBetweenFootstepSound;
    public bool madeSound = false;
    protected float timer;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        timeWaitedBetweenFootstepSound = Random.Range(-0.1f, 1.5f);
    }

    public void PlaySound(SoundType soundType, float volume)
    {
        switch (soundType)
        {
            case SoundType.Footstep:
                audioSource.volume = volume;
                audioSource.PlayOneShot(footstepSound);
                break;
            case SoundType.Sneeze:
                audioSource.volume = volume;
                audioSource.PlayOneShot(sneezeSound);
                madeSound = true;
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

    private void Update()
    {
        if (madeSound)
        {
            timer += Time.deltaTime;

            if (timer >= 1f)
            {
                madeSound = false;
                timer = 0f;
            }
        }
    }
}
