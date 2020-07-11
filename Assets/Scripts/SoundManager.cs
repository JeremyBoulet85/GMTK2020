using UnityEngine;

public enum SoundType
{
    KeyCollect,
    Ambiance,
    Footstep,
    Hungry,
    Sneeze,
    Music,
    Fart
}

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSourceCollectKey = null;
    private AudioSource audioSourceFootstep = null;
    private AudioSource audioSourceAmbiance = null;
    private AudioSource audioSourceSneeze = null;
    private AudioSource audioSourceHungry = null;
    private AudioSource audioSourceMusic = null;
    private AudioSource audioSourceFart = null;
    private AudioSource audioSourceBell = null;
    public AudioClip footstepSound = null;
    public AudioClip sneezeSound = null;
    public AudioClip keyCollect = null;
    public AudioClip ambiance = null;
    public AudioClip music = null;
    public AudioClip bell1 = null;
    public AudioClip bell2 = null;
    private bool playingAmbiance = false;
    private float timeWaitedBetweenFootstepSound;
    private float timeWaitedBetweenBellSound = 12.0f;

    public void Start()
    {
        audioSourceCollectKey = gameObject.AddComponent<AudioSource>();
        audioSourceAmbiance   = gameObject.AddComponent<AudioSource>();
        audioSourceFootstep   = gameObject.AddComponent<AudioSource>();
        audioSourceSneeze     = gameObject.AddComponent<AudioSource>();
        audioSourceHungry     = gameObject.AddComponent<AudioSource>();
        audioSourceMusic      = gameObject.AddComponent<AudioSource>();
        audioSourceFart       = gameObject.AddComponent<AudioSource>();
        audioSourceBell       = gameObject.AddComponent<AudioSource>();

        timeWaitedBetweenFootstepSound = Random.Range(-0.1f, 1.5f);
        audioSourceBell.volume = 0.06f;
    }

    public void Update()
    {
        if (playingAmbiance)
        {
            timeWaitedBetweenBellSound += Time.deltaTime;

            if (timeWaitedBetweenBellSound >= 18.0f)
            {
                timeWaitedBetweenBellSound = Random.Range(-5.0f, 5.0f);

                if (Random.Range(0.0f, 1.0f) > 0.5f)
                    audioSourceBell.clip = bell1;
                else
                    audioSourceBell.clip = bell2;

                audioSourceBell.time = 7.0f + Random.Range(-1.0f, 1.0f);
                audioSourceBell.Play();
            }
        }
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
            case SoundType.Hungry:
                audioSourceHungry.volume = volume;
                audioSourceHungry.PlayOneShot(sneezeSound);
                break;
            case SoundType.Fart:
                audioSourceFart.volume = volume;
                audioSourceFart.PlayOneShot(sneezeSound);
                break;
            case SoundType.KeyCollect:
                audioSourceCollectKey.volume = volume;
                audioSourceCollectKey.PlayOneShot(keyCollect);
                break;
            case SoundType.Music:
                audioSourceMusic.volume = volume;
                audioSourceMusic.loop = true;
                audioSourceMusic.clip = music;
                audioSourceMusic.Play();
                break;
            case SoundType.Ambiance:
                audioSourceAmbiance.volume = volume;
                audioSourceAmbiance.loop = true;
                audioSourceAmbiance.clip = ambiance;
                audioSourceAmbiance.Play();
                playingAmbiance = true;
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
