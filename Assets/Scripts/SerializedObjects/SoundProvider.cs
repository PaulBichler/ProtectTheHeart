using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundProvider", menuName = "ScriptableObjects/SoundProvider", order = 1)]
public class SoundProvider : ScriptableObject
{
    public List<SoundPack> SoundPacks;

    public void PlayRandomSoundFromPack(SoundType soundType, float volume = 1f)
    {
        if (Camera.main is { })
            AudioSource.PlayClipAtPoint(SoundPacks.First(o => o.SoundType == soundType).GetRandomSound(),
                Camera.main.transform.position, volume);
    }

    public List<AudioClip> GetSoundPackContents(SoundType soundType)
    {
        return SoundPacks.First(o => o.SoundType == soundType).Sounds;
    }
}