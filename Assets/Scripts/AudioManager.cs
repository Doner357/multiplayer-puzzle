using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Settings")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource2D;
    [SerializeField] private float minDistance = 2.0f;
    [SerializeField] private float maxDistance = 100.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource2D.PlayOneShot(clip, volume);
    }

    public AudioSource Play3DAt(AudioClip clip, Vector3 position, float volume = 1f, float spatialBlend = 1f)
    {
        if (clip == null) return null;

        GameObject tempAudio = new GameObject("TempAudio");
        tempAudio.transform.position = position;

        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = spatialBlend;
        
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        source.Play();

        Destroy(tempAudio, clip.length);

        return source;
    }
}
