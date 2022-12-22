using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public SoundGroup[] soundGroups;

    private List<Sound> allSounds = new List<Sound>();

    private List<Sound> globalSounds = new List<Sound>();

    public static AudioManager instance;

    [Serializable]
    public struct SoundGroup
    {
        public string name;

        public Sound[] sounds;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        //Add sounds to all sounds
        foreach (SoundGroup soundGroup in soundGroups)
        {
            allSounds.AddRange(soundGroup.sounds);
        }

        foreach (Sound s in allSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.maxDistance = s.maxDistance;
        }
    }

    private void Update()
    {
        List<Sound> sounds = globalSounds.ToList();
        foreach(Sound s in sounds)
        {
            if (s.source == null || !s.source.isPlaying)
            {
                s.soundEndEvent.Invoke();
                Destroy(s.source);
                globalSounds.Remove(s);
            }
        }
    }

    public Sound Play(string soundName)
    {
        Sound s = allSounds.Find(sound => sound.name == soundName);

        try
        {
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audio.clip = s.clip;
            audio.volume = s.volume;
            audio.pitch = s.pitch;
            audio.loop = s.loop;
            audio.spatialBlend = 1;
            audio.rolloffMode = AudioRolloffMode.Linear;
            audio.maxDistance = s.maxDistance;
            audio.Play();
            Sound sound = s.Copy();
            sound.source = audio;
            globalSounds.Add(sound);
            return sound;
        }
        catch (Exception)
        {
            Debug.Log("No sound with this name!");
        }

        return null;
    }

    public Sound Play(Sound soundToPlay)
    {
        try
        {
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audio.clip = soundToPlay.clip;
            audio.volume = soundToPlay.volume;
            audio.pitch = soundToPlay.pitch;
            audio.loop = soundToPlay.loop;
            audio.spatialBlend = 1;
            audio.rolloffMode = AudioRolloffMode.Linear;
            audio.maxDistance = soundToPlay.maxDistance;
            audio.Play();
            Sound sound = soundToPlay.Copy();
            sound.source = audio;
            globalSounds.Add(sound);
            return sound;
        }
        catch (Exception)
        {
            Debug.Log("No sound with this name!");
        }

        return null;
    }

    public Sound Play(Sound soundToPlay, Transform transformToSpawn)
    {
        try
        {
            AudioSource audio = transformToSpawn.gameObject.AddComponent<AudioSource>();
            audio.clip = soundToPlay.clip;
            audio.volume = soundToPlay.volume;
            audio.pitch = soundToPlay.pitch;
            audio.loop = soundToPlay.loop;
            audio.spatialBlend = 1;
            audio.rolloffMode = AudioRolloffMode.Linear;
            audio.maxDistance = soundToPlay.maxDistance;
            audio.Play();
            Sound sound = soundToPlay.Copy();
            sound.source = audio;
            globalSounds.Add(sound);
            return sound;
        }
        catch (Exception)
        {
            Debug.Log("No sound with this name!");
        }

        return null;
    }

    public Sound Play(string soundName, Transform transformToSpawn)
    {
        Sound s = allSounds.Find(sound => sound.name == soundName);

        try
        {
            AudioSource audio = transformToSpawn.gameObject.AddComponent<AudioSource>();
            audio.clip = s.clip;
            audio.volume = s.volume;
            audio.pitch = s.pitch;
            audio.loop = s.loop;
            audio.spatialBlend = 1;
            audio.rolloffMode = AudioRolloffMode.Linear;
            audio.maxDistance = s.maxDistance;
            audio.Play();
            Sound sound = s.Copy();
            sound.source = audio;
            globalSounds.Add(sound);
            return sound;
        }
        catch (Exception)
        {
            Debug.Log("No sound with this name!");
        }

        return null;
    }

    public Sound PlayOneShot(string soundName, float speed)
    {
        Sound s = allSounds.Find(sound => sound.name == soundName);

        try
        {
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audio.clip = s.clip;
            audio.volume = s.volume;
            audio.pitch = s.pitch;
            audio.loop = s.loop;
            audio.spatialBlend = 1;
            audio.rolloffMode = AudioRolloffMode.Linear;
            audio.maxDistance = s.maxDistance;
            audio.PlayOneShot(s.clip, speed);
            Sound sound = s.Copy();
            sound.source = audio;
            globalSounds.Add(sound);
            return sound;
        }
        catch (Exception)
        {
            Debug.Log("No sound with this name!");
        }

        return null;
    }

    public void PlaySoundOnThempTransform(string soundName, Vector2 pos)
    {
        GameObject themp = new GameObject("ThempSoundHolder");
        themp.transform.position = pos;
        Sound normalGrandeSound = instance.Play(soundName, themp.transform);
        normalGrandeSound.soundEndEvent.AddListener(() => Destroy(themp));
    }

    public Sound GetSoundFromList(string soundName)
    {
        try
        {
            Sound sound = allSounds.Find(sound => sound.name == soundName);

            return sound;
        }
        catch (Exception)
        {
            Debug.Log("No sound with this name!");
        }

        return null;
    }

    public Sound GetPlayingSound(string soundName)
    {
        try
        {
            Sound sound = allSounds.Find(sound => sound.name == soundName);

            Sound audioSource = globalSounds.Find(audio => audio.clip == sound.clip);

            return audioSource;
        }
        catch (Exception)
        {
            Debug.Log("No sound with this name!");
        }

        return null;
    }

    public AudioSource GetAudioSourceFromTransform(string soundName, Transform transformToGetSoundFrom)
    {
        try
        {
            Sound sound = allSounds.Find(sound => sound.name == soundName);

            AudioSource audioSource = Array.Find(transformToGetSoundFrom.GetComponents<AudioSource>(),
                audio => audio.clip == sound.clip);

            return audioSource;
        }
        catch (Exception)
        {
            Debug.Log("No sound with this name!");
        }

        return transformToGetSoundFrom.GetComponent<AudioSource>();
    }
}

[Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    public float volume = 1;
    public float pitch = 1;

    public float maxDistance = 16;

    public bool loop;

    public UnityEvent soundEndEvent;

    [HideInInspector]
    public AudioSource source;

    public Sound Copy()
    {
        Sound copySound = new Sound();
        copySound.name = name;
        copySound.clip = clip;
        copySound.volume = volume;
        copySound.pitch = pitch;
        copySound.maxDistance = maxDistance;
        copySound.loop = loop;
        copySound.soundEndEvent = soundEndEvent;
        copySound.source = source;
        return copySound;
    }
}
