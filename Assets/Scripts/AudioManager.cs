using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public Sound[] sounds;
    public AudioMixer mixer;

    bool playingAmbiance = false;
    float timeWaitedBetweenBellSound = 12.0f;
    float timeWaitedBetweenFootstepSound;

    List<string> playerSounds = new List<string> { "Sneeze", "Hungry", "Fart", "Dash"};


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            switch (s.mixerGroup)
            {
                case "Music":
                    s.source.outputAudioMixerGroup = mixer.FindMatchingGroups("Master/Music")[0];
                    break;
                case "Ambiant":
                    s.source.outputAudioMixerGroup = mixer.FindMatchingGroups("Master/Ambiant")[0];
                    break;
                case "SoundEffect":
                    s.source.outputAudioMixerGroup = mixer.FindMatchingGroups("Master/SoundEffect")[0];
                    break;
                default:
                    s.source.outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[0];
                    break;

            }

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

    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat("volume", volume);
    }

    public void SetAmbiantVolume(float volume)
    {
        mixer.SetFloat("Ambiant", volume);
    }

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("Music", volume);
    }

    public void SetSoundEffectVolume(float volume)
    {
        mixer.SetFloat("SoundEffect", volume);
    }

    public void Play(string name)
    {
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
        Sound s = Array.Find(sounds, sound => sound.name == type);

        s.source.time = 6.5f + UnityEngine.Random.Range(-1.0f, 1.0f);
        s.source.Play();
    }
}
