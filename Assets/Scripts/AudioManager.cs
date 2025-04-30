using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource menuMusic;
    public AudioSource gameMusic;


    private const string MusicPref = "MusicOn";

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Lire l'état enregistré
        bool musicOn = PlayerPrefs.GetInt(MusicPref, 1) == 1;
        SetMusicState(musicOn);
    }

    public void SetMusicState(bool isOn)
    {
        if (menuMusic != null) menuMusic.mute = !isOn;
        if (gameMusic != null) gameMusic.mute = !isOn;

        PlayerPrefs.SetInt(MusicPref, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsMusicOn()
    {
        return PlayerPrefs.GetInt(MusicPref, 1) == 1;
    }
}
