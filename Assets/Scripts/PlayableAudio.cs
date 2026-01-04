using System;
using UnityEngine;

[Serializable]
public struct PlayableAudio
{
    public AudioClip clip;
    [UnityEngine.Range(0f, 1f)]
    public float volume;
}
