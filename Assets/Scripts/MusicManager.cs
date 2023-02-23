using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSIC_VOLUME = "musicVolume";

    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;
    private float volume = 0.3f;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();

        volume = PlayerPrefs.GetFloat( PLAYER_PREFS_MUSIC_VOLUME , 0.3f );
        audioSource.volume = volume;
    }

    public void ChangeVolume()
    {
        volume += 0.1f;
        if ( volume > 1 )
            volume = 0;
        audioSource.volume = volume;

        PlayerPrefs.SetFloat( PLAYER_PREFS_MUSIC_VOLUME , volume );
        PlayerPrefs.Save();
    }

    public float GetVolume() => volume;
}
