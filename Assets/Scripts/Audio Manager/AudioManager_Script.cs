using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager_Script : MonoBehaviour
{

    // Delegate type for functions that play sounds
    // public delegate void PlaySoundDelegate();

    // Dictionary to map enum values to delegate functions
    // private Dictionary<SoundName, PlaySoundDelegate> soundDelegates = new Dictionary<SoundName, PlaySoundDelegate>();
    
    // Dictionary to map sound names to sound objects
    private Dictionary<SoundName, sound> soundDictionary = new Dictionary<SoundName, sound>();

    public sound[] sounds; // sound is a class

    public static AudioManager_Script Instance; // to access the AudioManager_Script from other scripts
    public bool isMuted;

    // Awake is called before the Start function.

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        foreach (sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;

            // Set the AudioSource to not play on awake
            s.source.playOnAwake = false;
            // Add sound to the dictionary using its name
            soundDictionary.Add(s.name, s);
        }

        // Initialize the dictionary with enum values and corresponding functions
        // soundDelegates.Add(SoundName.Eat, PlayEatSound);
        // soundDelegates.Add(SoundName.Theme, PlayThemeSound);

        isMuted = false;
    }


    void Start(){
        // Play the theme sound at the start
        PlayThemeSound();
    }


/// <summary>
/// ///////////////////////////////  Functions to play sounds by type  ///////////////////////////////
/// </summary>
    // Function to play the "Eat" sound
    public void PlayConversionSound()
    {
        Play(SoundName.Conversion);
    }
    // Function to play the "Theme" sound
    private void PlayThemeSound()
    {
        Play(SoundName.Theme);
    }
    // Function to play the "Theme" sound
    public void PlayClockSound()
    {
        Play(SoundName.Clock);
    }
    public void StopClockSound()
    {
        if (soundDictionary.ContainsKey(SoundName.Clock))
            soundDictionary[SoundName.Clock].source.Stop();
    }
  // Function to play a sound by name using the dictionary
    public void Play(SoundName name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            soundDictionary[name].source.Play();
        }
        else
        {
            Debug.LogWarning("Sound with name " + name + " not found.");
        }
    }



    // Call this function from outside the class, passing the desired enum value
    // public void PlaySoundByType(SoundName soundName)
    // {
    //     // Check if the enum value is in the dictionary, then invoke the corresponding delegate
    //     if (soundDelegates.ContainsKey(soundName))
    //     {
    //         soundDelegates[soundName]?.Invoke();
    //     }
    // }

  

    // Rest of your code...

    // Function to mute music
    public void MuteMusic()
    {
            if (soundDictionary.ContainsKey(SoundName.Theme))
                soundDictionary[SoundName.Theme].source.Stop();
    }
    public void UnMuteMusic()
    {
            if (soundDictionary.ContainsKey(SoundName.Theme))
                soundDictionary[SoundName.Theme].source.Play();
    }


    /// <summary>
    /// Stops a sound after a specified delay
    /// </summary>
    /// <param name="soundName">The type of sound to stop</param>
    /// <param name="delay">Delay in seconds before stopping the sound</param>
    public void StopSoundAfterDelay(SoundName soundName, float delay)
    {
        StartCoroutine(StopSoundCoroutine(soundName, delay));
    }

    private System.Collections.IEnumerator StopSoundCoroutine(SoundName soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (soundDictionary.ContainsKey(soundName))
        {
            soundDictionary[soundName].source.Stop();
        }
        else
        {
            Debug.LogWarning("Sound with name " + soundName + " not found.");
        }
    }

}
