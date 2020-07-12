using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetVolume(float volume)
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
}
