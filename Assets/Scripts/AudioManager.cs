using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public bool madeSound = false;

    bool playingAmbiance = false;
    float timeWaitedBetweenBellSound = 12.0f;
    float timeWaitedBetweenFootstepSound;

    List<string> playerSounds = new List<string> { "Sneeze", "Hungry", "Fart" };


    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }

        timeWaitedBetweenFootstepSound = UnityEngine.Random.Range(-0.1f, 1.5f);
    }

    public void Update()
    {
        if (playingAmbiance)
        {
            timeWaitedBetweenBellSound += Time.deltaTime;

            if (timeWaitedBetweenBellSound >= 18.0f)
            {
                timeWaitedBetweenBellSound = UnityEngine.Random.Range(-5.0f, 5.0f);

                if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f)
                    PlayBell("Bell1");
                else
                    PlayBell("Bell2");
            }
        }
    }

    public void Play(string name)
    {
        if (playerSounds.Contains(name))
            madeSound = true;

        Sound s = Array.Find(sounds, sound => sound.name == name);

        s.source.Play();
    }

    public void PlayFootstepSound(bool isEnemy, float timeToWaitBetweenFootstepSound = 2.0f)
    {
        timeWaitedBetweenFootstepSound += Time.fixedDeltaTime;

        if (timeWaitedBetweenFootstepSound >= timeToWaitBetweenFootstepSound)
        {
            timeWaitedBetweenFootstepSound = 0.0f;

            string sound = isEnemy ? "FootstepEnemy" : "Footstep";

            Play(sound);
        }
    }

    public void PlayAmbiance()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "Ambiance");

        s.source.loop = true;
        s.source.Play();

        playingAmbiance = true;
    }

    public void PlayMusic()
    {
        Sound s = Array.Find(sounds, sound => sound.name == "Music");

        s.source.loop = true;
        s.source.Play();
    }

    void PlayBell(string type)
    {

        Debug.Log(type);
        Sound s = Array.Find(sounds, sound => sound.name == type);

        s.source.time = 7.0f + UnityEngine.Random.Range(-1.0f, 1.0f);
        s.source.Play();
    }
}
