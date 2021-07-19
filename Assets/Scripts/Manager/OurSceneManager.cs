using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OurSceneManager : MonoBehaviour
{
    private static LevelData _levelData;
    private Image _image;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _image = GetComponentInChildren<Image>();
        _levelData = Resources.Load<LevelData>("LevelData");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    public static void LoadGame()
    {
        SoundManager.PlayMusic("Game");
        SceneManager.LoadSceneAsync("LevelBase", LoadSceneMode.Single);
        SceneManager.LoadSceneAsync(_levelData.LevelNames[Random.Range(0, _levelData.LevelNames.Length)],
            LoadSceneMode.Additive);
    }
    
    private void SceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        _image.DOFade(1, 0.0001f).OnComplete(() => _image.raycastTarget = true);
        StartCoroutine(UnFade());
    }

    private IEnumerator UnFade()
    {
        yield return new WaitForSeconds(2);
        _image.DOFade(0, 0.3f).OnComplete(() => _image.raycastTarget = false);
    }


    public static void LoadLobby()
    {
        SoundManager.PlayMusic("Lobby");
        SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
    }

    public static void LoadEndScene()
    {
        SoundManager.PlayMusic("Game");
        SceneManager.LoadSceneAsync("LevelBase", LoadSceneMode.Single);
        SceneManager.LoadSceneAsync("EndScene", LoadSceneMode.Additive);
    }

    public static void LoadMenu()
    {
        SoundManager.PlayMusic("Menu");
        SceneManager.LoadSceneAsync("StartMenu", LoadSceneMode.Single);
    }
}