using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip sneeze = null;

    public void PlaySneezeSound()
    {
        GameObject sound = new GameObject("Sound");
        AudioSource audioSource = sound.AddComponent<AudioSource>();
        audioSource.volume = 0.3f;
        audioSource.PlayOneShot(sneeze);
    }
}
