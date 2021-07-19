using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundProvider Provider;

    private static SoundManager _instance;
    private static List<AudioSource> _source = new List<AudioSource>();
    private MaterialDataContainer materialData;
    
    private static int mainSource;

    private static int count = 2;

    private void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
            materialData = Resources.Load<MaterialDataContainer>("MaterialDataContainer");
            var musicboy = gameObject.GetComponent<AudioSource>();
            musicboy.enabled = true;
            _source.Add(musicboy);
            mainSource = 0;
            Provider ??= Resources.Load<SoundProvider>("SoundProvider");
            return;
        }
        
        Destroy(gameObject);
    }

    private float prevIntensity = 1;
    private float intensity = 1;
    private bool change;

    private float maxIntensity = 1.2f;
    
    private static float timer = 0f;
    private float cooldown = 0.5f;

    public void OnBeat()
    {
        if (intensity == 1)
        {
            change = true;
            intensity = maxIntensity;
            return;
        }
        
        intensity = 1;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= cooldown)
        {
            OnBeat();
            timer = 0;
        }
        
        if (!change) return;
        
        foreach (Material material in materialData.MaterialData)
        {
            material.SetFloat("Vector1_cbe13c6ac63d4673b36fe2219d3cffe2", intensity);
        }
    }

    public static void PlayMusic(string where)
    {
        int index = -1;

        switch (where)
        {
            case "Menu":
                index = 0;
                break;
            case "Lobby":
                index = 1;
                break;
            case "Game":
                index = count;
                List<AudioClip> clips = Provider.GetSoundPackContents(SoundType.Music);
                if (count < clips.Count - 1) count++;
                break;
        }

        AudioClip audioClip = Provider.GetSoundPackContents(SoundType.Music)[count];

        float timeLeft = _source[mainSource].clip.length - _source[mainSource].time;
        _instance.StartCoroutine(_instance.StopAudio(_source[mainSource], timeLeft));

        if (index == mainSource) return;

        //Make new Source
        if (_source.Count <= index)
        {
            AudioSource audio = _instance.gameObject.AddComponent<AudioSource>();
            _source.Add(audio);
            audio.loop = true;
            audio.clip = audioClip;
            audio.priority = 0;
            audio.volume = 0.8f;
            audio.PlayDelayed(timeLeft);
        }
        else
        {
            _source[index].PlayDelayed(timeLeft);
        }

        mainSource = index;
    }

    public IEnumerator StopAudio(AudioSource audioSource, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        audioSource.Stop();
    }
    

}