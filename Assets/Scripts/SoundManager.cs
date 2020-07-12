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
    private AudioSource audioSourceMusic = null;
    public AudioClip keyCollect = null;
    public AudioClip music = null;

    public void Start()
    {
        audioSourceCollectKey = gameObject.AddComponent<AudioSource>();
        audioSourceMusic      = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySound(SoundType soundType, float volume)
    {
        switch (soundType)
        {
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
        }
    }
}
