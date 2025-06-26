using UnityEngine;
using System;

    public enum SoundName
    {
        Theme,
        Clock,
        LevelCompleted,
        LevelFailed,
        Conversion,
        CollectProduct,
        TrainWhistle,
        TrainRunning,
    }
[System.Serializable]
public class sound
{

    public SoundName name; // Enum representing the sound name
    public AudioClip clip;
    [Range(0, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    [Range(0f, 1f)]
    public float spatialBlend; // 0 = 2D, 1 = 3D
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
