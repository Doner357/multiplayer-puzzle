using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Playables;

public class SoundPlayer3D : MonoBehaviour
{
    public List<PlayableAudio> audioClips;

    private Dictionary<int, AudioSource> activeSounds = new Dictionary<int, AudioSource>();

    public void PlaySound(int index)
    {
        if (index < 0 || index >= audioClips.Count)
        {
            Debug.LogError("Index out of range");
            return;
        }

        StopSound(index);

        PlayableAudio playableAudio = audioClips[index];

        AudioSource source = AudioManager.Instance.Play3DAt(
            playableAudio.clip, 
            transform.position, 
            playableAudio.volume
        );

        if (source != null)
        {
            activeSounds[index] = source;
        }
    }

    public void PlayAll()
    {
        for (int i = 0; i < audioClips.Count; i++)
        {
            PlaySound(i);
        }
    }

    public void StopSound(int index)
    {
        if (activeSounds.ContainsKey(index))
        {
            AudioSource source = activeSounds[index];

            if (source != null)
            {
                source.Stop();
                Destroy(source.gameObject);
            }

            activeSounds.Remove(index);
        }
    }

    public void StopAllSounds()
    {
        foreach (var kvp in activeSounds)
        {
            AudioSource source = kvp.Value;
            if (source != null)
            {
                source.Stop();
                Destroy(source.gameObject);
            }
        }
        
        activeSounds.Clear();
    }

    private void OnDestroy()
    {
        StopAllSounds();
    }
}
