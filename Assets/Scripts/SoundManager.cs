using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip sneeze = null;

    [SerializeField]
    private AudioClip footstep = null;

    private AudioSource audioSource = null;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySneezeSound()
    {
        audioSource.volume = 0.8f;
        audioSource.PlayOneShot(sneeze);
    }

    public void PlayFootstepSound()
    {
        audioSource.volume = 0.4f;
        audioSource.PlayOneShot(footstep);
    }
}
