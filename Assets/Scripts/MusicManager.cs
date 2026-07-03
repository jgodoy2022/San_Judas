using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioSource audioSource;

    // Ahora las variables se llaman exactamente como tus archivos
    public AudioClip Menu;
    public AudioClip Game;
    public AudioClip Creditos;

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        // Aquí usamos los nombres de tus escenas y tus archivos
        if (sceneName == "00_MainMenu") 
            clipToPlay = Menu;
        else if (sceneName == "01_Orfanato" || sceneName == "03_GameOver") 
            clipToPlay = Game;
        else if (sceneName == "04_Credits") 
            clipToPlay = Creditos;

        if (clipToPlay != null && audioSource.clip != clipToPlay)
        {
            audioSource.clip = clipToPlay;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}