using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundPack", menuName = "ScriptableObjects/SoundPack", order = 1)]
public class SoundPack : ScriptableObject
{
    public SoundType SoundType;
    public List<AudioClip> Sounds;

    public AudioClip GetRandomSound()
    {
        return Sounds[Random.Range(0, Sounds.Count)];
    }
}